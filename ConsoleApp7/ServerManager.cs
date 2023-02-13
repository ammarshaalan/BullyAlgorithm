
using System.IO;
using System.IO.Pipes;
using System.Net.Sockets;

namespace BullyAlgorithm
{
    public static class ServerManager
    {
        public static void CheckDisconnectedClients(ref Dictionary<int, TcpClient> Processes, int processId, ref int coordinatorId)
        {
            var disconnectedClients = Processes.Where(kvp => (kvp.Value.Client.Poll(0, SelectMode.SelectRead) || !kvp.Value.Client.Connected) && kvp.Key != processId).ToList();
            foreach (var disconnectedClient in disconnectedClients)
            {
                if (coordinatorId == disconnectedClient.Key)
                {
                    coordinatorId = 0;
                    Console.WriteLine("The Coordinator " + disconnectedClient.Key + " has disconnected.");

                }
                else
                    Console.WriteLine("Process with ID " + disconnectedClient.Key + " has disconnected.");
                Processes.Remove(disconnectedClient.Key);

            }
        }

        public static void HandleInitElectionMessage(int processId, Dictionary<int, TcpClient> Processes, ref int coordinatorId)
        {
            //Console.WriteLine("Received init election message from process " + processId + ".");
            MessageManager.PrintMessageFromProcess(processId, MessageType.INIT_ELECTION.ToString());

            bool higherProcessExists = false;
            foreach (var item in Processes)
            {
                if (item.Key > processId)
                {
                    higherProcessExists = true;
                    break;
                }
            }

            if (!higherProcessExists)
            {
                Console.WriteLine("No higher process exists with id greater than " + processId);
                Console.WriteLine("the process number " + processId + " become the coordinator");
                MessageManager.SendMessageToClient(Processes[processId].GetStream(), MessageType.NewCoordinator.ToString());
                coordinatorId = processId;
                BroadcastCoordinator(Processes, coordinatorId);
            }
            else
            {
                Console.WriteLine("A higher process with id greater than " + processId + " exists");
            }
        }

        public static int StartElection(int processId, Dictionary<int, TcpClient> Processes, NetworkStream stream)
        {
            int coordinator = 0;
            while (coordinator == 0)
            {
                Console.WriteLine("\nElection message sent by process: " + processId);
                bool processFound = false;
                foreach (KeyValuePair<int, TcpClient> process in Processes)
                {
                    if (process.Key > processId)
                    {
                        Console.WriteLine("Process " + process.Key + " is higher than " + processId + " and has responded");
                        processId = process.Key;
                        processFound = true;
                        break;
                    }
                }
                if (!processFound)
                {
                    Console.WriteLine("No higher process found. " + processId + " becomes the coordinator");
                    coordinator = processId;
                }
            }
            Console.WriteLine("\nCoordinator: " + coordinator);
            MessageManager.SendMessageToClient(stream, "new coordinator is " + coordinator);
            return coordinator;
        }

        public static void BroadcastCoordinator(Dictionary<int, TcpClient> processes, int newCoordinatorId)
        {
            string message = $"New coordinator is {newCoordinatorId}";

            foreach (KeyValuePair<int, TcpClient> process in processes)
            {
                if (process.Key == newCoordinatorId)
                {
                    byte[] messageBytes = System.Text.Encoding.ASCII.GetBytes("The new coordinator is you.");
                    process.Value.GetStream().Write(messageBytes, 0, messageBytes.Length);
                    continue;
                }

                try
                {
                    TcpClient currentProcess = process.Value;

                    if (currentProcess.Connected)
                    {
                        NetworkStream stream = currentProcess.GetStream();
                        byte[] messageBytes = System.Text.Encoding.ASCII.GetBytes(message);
                        stream.Write(messageBytes, 0, messageBytes.Length);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending message to process {process.Key}: {ex.Message}");
                }
            }
        }

       

        public static void TerminateCoordinator(int processId, ref Dictionary<int, TcpClient> Processes, ref int coordinatorId)
        {
            Console.WriteLine("The coordinator is manually terminated by the process " + processId);
            RemoveFailedCoordinator(ref Processes, coordinatorId);
            coordinatorId = 0;
            MessageManager.SendMessageToClient(Processes[processId].GetStream(), "Coordinator has been terminated manually by process " + processId);
        }

        public static void StopProcess(Dictionary<int, TcpClient> Processes, int processId)
        {
            Console.WriteLine("Enter the id of the process you want to stop: ");
            processId = int.Parse(Console.ReadLine());
            if (Processes.ContainsKey(processId))
            {
                Console.WriteLine("Client with ID " + processId + " already connected. Closing connection.");
                MessageManager.SendMessageToClient(Processes[processId].GetStream(), "Close");
                Processes[processId].Close();
            }
            else
            {
                Console.WriteLine("Process with id " + processId + " not found");
            }
        }

        private static Dictionary<int, TcpClient> RemoveFailedCoordinator(ref Dictionary<int, TcpClient> processes, int failedCoordinatorId)
        {
            if (processes.ContainsKey(failedCoordinatorId))
            {
                TcpClient failedCoordinatorClient = processes[failedCoordinatorId];
                failedCoordinatorClient.Close();
                processes.Remove(failedCoordinatorId);
                Console.WriteLine("Process " + failedCoordinatorId + " removed.");
            }
            return processes;
        }


    }
}
