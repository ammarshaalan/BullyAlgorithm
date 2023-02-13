using System.Net.Sockets;
using System.Text;

namespace BullyAlgorithm
{
    class Program
    {
        private static Dictionary<int, TcpClient> Processes = new Dictionary<int, TcpClient>();
        private static int coordinatorId = 0;
        private static int processId = 0;

        static void Main(string[] args)
        {

            TcpListener listener = new TcpListener(System.Net.IPAddress.Any, 8080);
            listener.Start();

            Console.WriteLine("Waiting for process connection...");
            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();

                var stream = client.GetStream();

                string processDetails = MessageManager.ReceiveMessageFromClient(stream);
                if (processDetails.StartsWith("*"))
                {
                    processId = int.Parse(processDetails.Substring(1));
                    stream = Processes[processId].GetStream();

                }
                else
                {
                    if (!int.TryParse(processDetails, out processId))
                    {
                        Console.WriteLine("Invalid client ID.");
                        MessageManager.SendMessageToClient(stream, "Error: Invalid client ID.");
                        client.Close();
                        stream.Close();
                        continue;
                    }

                    if (Processes.ContainsKey(processId))
                    {
                        Console.WriteLine("Process with ID " + processId + " already connected. Closing connection.");
                        MessageManager.SendMessageToClient(stream, MessageType.Exit.ToString());
                        client.Close();
                        continue;
                    }
                    MessageManager.SendMessageToClient(stream, "Connected with server");
                    MessageManager.PrintMessageFromProcess(processId, " connected.");
                    Processes.Add(processId, client);
                }

                ServerManager.CheckDisconnectedClients(ref Processes, processId, ref coordinatorId);
             
                while (client.Connected)
                {
                    Thread.Sleep(100);
                    string message =null;
                    // Read data from the client
                    if (stream.DataAvailable)
                    {
                        byte[] buffer = new byte[256];
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        if (bytesRead == 0)
                        {
                            break;
                        }
                        else
                        {
                            message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        }
                    }
                    else
                    {
                        break;
                    }

                    // Handle the message received from the client
                    switch ((MessageType)Enum.Parse(typeof(MessageType), message))
                    {
                        case MessageType.COORDINATOR_Alive:
                            if (coordinatorId != 0)
                                MessageManager.SendMessageToClient(stream, "Coordinator is alive.");
                            else
                                MessageManager.SendMessageToClient(stream, "Coordinator not alive.");
                            break;

                        case MessageType.INIT_ELECTION:
                            ServerManager.HandleInitElectionMessage(processId, Processes, ref coordinatorId);
                            break;

                        case MessageType.CoordinatorId:
                            MessageManager.SendMessageToClient(stream, coordinatorId.ToString());
                            break;
                        case MessageType.HeartbeatSignal:
                            MessageManager.SendMessageToClient(Processes[coordinatorId].GetStream(), "Process " + processId + " connected.");
                            MessageManager.SendMessageToClient(stream, "Coordinator " + coordinatorId + "  received the message");
                            break;

                        case MessageType.TERMINATE_COORDINATOR:
                            ServerManager.TerminateCoordinator(processId, ref Processes, ref coordinatorId);
                            break;
                        case MessageType.Election_Message:
                            coordinatorId = ServerManager.StartElection(processId, Processes, stream);
                            ServerManager.BroadcastCoordinator(Processes, coordinatorId);
                            break;

                        case MessageType.STOP_PROCESS:
                            ServerManager.StopProcess(Processes, processId);
                            break;

                        case MessageType.Exit:
                            Console.WriteLine("Exiting...");
                            //exit = true;
                            break;

                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            break;
                    }
                }
            }
        }
    }
}
