using System.IO.Pipes;
using System.Net.Sockets;
using Timer = System.Timers.Timer;

namespace BullyAlgorithm
{
    public class Process
    {
        private int processId;
        private int interval;
        private TcpClient client;
        private bool isCoordinator = false;
        private int coordinatorId;


        private int[] chunk;

        public Process(int processId, int interval, TcpClient client)
        {
            this.processId = processId;
            this.interval = interval;
            this.client = client;
        }

        public int ProcessId { get { return processId; } }
        
        //public void SendChunk(int[] chunk)
        //{
        //    // Assign the received chunk to the Chunk property
        //    this.chunk = chunk;
        //    // Find the minimum value in the chunk
        //    FindMinimumInChunk();
        //}
        //private void FindMinimumInChunk()
        //{
        //    // Initialize minimumValue with the maximum possible integer value
        //    int minimumValue = int.MaxValue;
        //    for (int i = 0; i < chunk.Length; i++)
        //    {
        //        // If the current value is less than the minimumValue, update minimumValue
        //        if (chunk[i] < minimumValue)
        //        {
        //            minimumValue = chunk[i];
        //        }
        //    }
        //    // Call the ReceiveResult method of the Coordinator class and pass the process Id and the minimum value
        //    Coordinator.ReceiveResult(processId, minimumValue);
        //}
    }
}
