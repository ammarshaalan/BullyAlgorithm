using System.Net.Sockets;

namespace BullyAlgorithm
{
    public class BullyAlgorithmProgram
    {
        static void Main(string[] args)
        {
            // Prompt the user to enter the process ID
            Console.WriteLine("Enter id of process");

            // Try to parse the process ID as an integer
            if (!int.TryParse(Console.ReadLine(), out int processId))
            {
                // If the input is not a valid integer, display an error message and return
                Console.WriteLine("Invalid process ID.");
                return;
            }

            // Create a TcpClient instance to connect to the server
            TcpClient client = null;

            try
            {
                // Connect to the server on localhost, port 8080
                client = new TcpClient("LocalHost", 8080);
                Console.WriteLine("Connecting to server...");
                NetworkStream stream = client.GetStream();

                // Set the write and read timeouts to infinite (-1)
                stream.WriteTimeout = -1;
                stream.ReadTimeout = -1;

                // Send the process ID to the server
                MessageManager.SendMessageToServer(stream, processId.ToString());

                // Receive the first message from the server
                var firstMessage = MessageManager.ReceiveMessageFromServer(stream);

                // If the first message is "Close", the process is already connected
                if (firstMessage == "Close")
                {
                    Console.WriteLine("Process with ID " + processId + " already connected.");
                    client.Close();

                }
                // Print the first message from the server
                MessageManager.PrintMessageFromMessage(firstMessage);

                // Send a message to the server indicating that the coordinator is alive
                MessageManager.SendMessageToServer(stream, MessageType.COORDINATOR_Alive.ToString());

                var process = new Process(processId, 100, client);// will use it later when make tasks with coordinator

                var messageFromServer = MessageManager.ReceiveMessageFromServer(stream);
                MessageManager.PrintMessageFromMessage(messageFromServer);

                // If the coordinator is alive and the process ID is greater than the coordinator ID
                if (messageFromServer == "Coordinator is alive.")
                {
                    // Send a message to the server requesting the coordinator ID
                    MessageManager.SendMessageToServer(stream, MessageType.CoordinatorId.ToString());
                    var coordinator = MessageManager.ReceiveMessageFromServer(stream);
                    int coordinatorId = int.Parse(coordinator);

                    // If the process ID is greater than the coordinator ID
                    if (processId > coordinatorId)
                    {
                        // Send a message to the server to initiate the election
                        Console.WriteLine("Iam higher than Coordinator. I will initializing election.");
                        MessageManager.SendMessageToServer(stream, MessageType.INIT_ELECTION.ToString());
                        messageFromServer = MessageManager.ReceiveMessageFromServer(stream);
                        MessageManager.PrintMessageFromMessage(messageFromServer);
                    }
                    else
                    {
                        // Send Heartbeat signal to the server
                        MessageManager.SendMessageToServer(stream, MessageType.HeartbeatSignal.ToString());
                        messageFromServer = MessageManager.ReceiveMessageFromServer(stream);
                        MessageManager.PrintMessageFromMessage(messageFromServer);

                    }

                }
                else
                {
                    MessageManager.SendMessageToServer(stream, MessageType.INIT_ELECTION.ToString());
                    messageFromServer = MessageManager.ReceiveMessageFromServer(stream);
                    MessageManager.PrintMessageFromMessage(messageFromServer);
                }

                while (true)
                {
                    Console.WriteLine("Enter 1 to manually terminate the coordinator");
                    Console.WriteLine("Enter 2 to listen to the server.");
                    Console.WriteLine("Enter 0 to exit the Process.");

                    string input = Console.ReadLine();
                    if (int.TryParse(input, out int numberInput))
                    {
                        switch (numberInput)
                        {
                            case 1:

                                // If the user is the coordinator, don't allow to terminate
                                if (messageFromServer == MessageType.NewCoordinator.ToString())
                                {
                                    Console.WriteLine("You are not allowed to terminate the coordinator as you are the coordinator.");
                                    break;
                                }
                                else
                                {
                                    SendProcessIdToServer(processId);
                                    // Send message to check if the coordinator is alive
                                    MessageManager.SendMessageToServer(stream, MessageType.COORDINATOR_Alive.ToString());
                                    messageFromServer = MessageManager.ReceiveMessageFromServer(stream);
                                    MessageManager.PrintMessageFromMessage(messageFromServer);

                                    // If the coordinator is alive
                                    if (messageFromServer == "Coordinator is alive.")
                                    {
                                        // Terminate the coordinator
                                        MessageManager.SendMessageToServer(stream, MessageType.TERMINATE_COORDINATOR.ToString());
                                        MessageManager.ReceiveMessageFromServer(stream);
                                        // Initiate election
                                        MessageManager.SendMessageToServer(stream, MessageType.Election_Message.ToString());
                                        messageFromServer = MessageManager.ReceiveMessageFromServer(stream);
                                        MessageManager.PrintMessageFromMessage(messageFromServer);
                                        break;
                                    }
                                    else
                                    {
                                        Console.WriteLine("There is currently no coordinator.");
                                    }
                                    goto case 2;
                                }
                            case 2:
                                // If the client is connected to the server
                                while (client.Connected)
                                {
                                    // Receive message from the server
                                    var message = MessageManager.ReceiveMessageFromServer(stream);
                                    if (message == "")
                                    {
                                        client.Close();
                                        return;
                                    }
                                    MessageManager.PrintMessageFromMessage(message);
                                }
                                break;
                            case 0:
                                MessageManager.SendMessageToServer(stream, MessageType.Exit.ToString());
                                client.Close();
                                return;
                            default:
                                Console.WriteLine("Invalid input, try again.");
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input, try again.");
                    }
                }
            }
            catch (Exception)
            {
            }


            finally
            {
                client.Close();
            }
        }
        // The method `SendProcessIdToServer` is used to send the `processId` of a process to the server.
        // A new TcpClient is created with `LocalHost` and port `8080` to communicate with the server.
        // The method returns a NetworkStream object which is used to communicate with the server.
        public static NetworkStream SendProcessIdToServer(int processId)
        {
            TcpClient tempClient = new TcpClient("LocalHost", 8080);
            NetworkStream tempStream = tempClient.GetStream();
            MessageManager.SendMessageToServer(tempStream, "*" + processId.ToString());
            return tempStream;
        }


    }
}
