using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace BullyAlgorithm
{
    public enum MessageType
    {
        ConnectedWithServer,
        COORDINATOR_Alive,
        INIT_ELECTION,
        CoordinatorId,
        HeartbeatSignal,
        TERMINATE_COORDINATOR,
        Election_Message,
        STOP_PROCESS,
        NewCoordinator,
        Exit

    }
    public static class MessageManager
    {
        public static void SendMessage(int senderId, string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[{DateTime.Now}] Process {senderId} sent message: {message}");
            Console.ResetColor();
        }

        public static void PrintMessageFromMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[" + DateTime.Now + "] Received a message [ " + message + " ] from Server");
            Console.ResetColor();
        }

        public static void WinningMessage(int receiverId)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"[{DateTime.Now}] [Process {receiverId}] received a message: You won in the elections.");
            Console.ResetColor();
        }
        public static void NoMessageReceived(int processId)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[{DateTime.Now}] Process {processId} didn't receive a message from Coordinator.");
            Console.ResetColor();
        }

        public static string ReceiveMessageFromServer(NetworkStream networkStream)
        {
            byte[] buffer = new byte[256];
            int bytesRead = networkStream.Read(buffer, 0, 256);
            return Encoding.UTF8.GetString(buffer, 0, bytesRead);
        }

        public static void SendMessageToServer(NetworkStream networkStream, string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            networkStream.Write(buffer, 0, buffer.Length);
        }

    }

}
