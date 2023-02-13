using System.Diagnostics;
using System.IO.Pipes;
using System.Net.Sockets;

namespace BullyAlgorithm
{
    public static class Coordinator
    {
        public static int Id { get; set; }
        public static int chunkSize { get; set; }
        public static int[] largeArray { get; set; }

        // List to store the responses from the processes
        private static List<int> responses = new List<int>();
        public static int numberOfProcesses { get; set; }

        
       
        //public static void DistributedComputing(List<Process> processes, Dictionary<int, string> sharedMemory)
        //{
        //    // Checking if there is an active coordinator.
        //    if (IsCoordinatorAlive(sharedMemory))
        //    {
        //        // number of processes in shared memory, excluding coordinator
        //        int numberOfProcesses = sharedMemory.Count - 1;
        //        if (numberOfProcesses != 0)
        //        {
        //            // Creating the large array in the Coordinator class.
        //            CreateLargeArray();
        //            // Dividing the array into chunks in the Coordinator class.
        //            int[][] chunks = DivideArray(numberOfProcesses);
        //            int chunkIndex = 0;
        //            // Sending the chunks to each process.
        //            foreach (var process in processes)
        //            {
        //                // Skipping the coordinator process.
        //                if (process.ProcessId == Id)
        //                {
        //                    continue;
        //                }
        //                process.SendChunk(chunks[chunkIndex]);
        //                chunkIndex++;

        //            }
        //            // Finding the minimum value in all the responses.
        //            int minValue = FindMinimum();
        //            Console.WriteLine("The minimum value in all the responses is: " + minValue);
        //        }
        //        else
        //            Console.WriteLine("There is no process for sharing the task with a coordinator. ");
        //    }
        //    else
        //        Console.WriteLine("There is no coordinator yet; start the election first.");
        //}

        ////ReceiveResult is used to add the result received from each process to the responses list.
        //public static void ReceiveResult(int processId, int result)
        //{
        //    responses.Add(result);
        //}
        //#region Private Methods
        ////CreateLargeArray initializes the "largeArray" property with random integers within the range of 1 to 10000.
        //private static void CreateLargeArray()
        //{
        //    int Seed = (int)DateTime.Now.Ticks;
        //    Random randomListCell = new Random(Seed);
        //    largeArray = new int[100];
        //    for (int i = 0; i < largeArray.Length; i++)
        //    {
        //        largeArray[i] = randomListCell.Next(1, 10000);
        //    }
        //    responses = new List<int>();
        //}

        ////DivideArray is used to divide the largeArray into smaller chunks of equal sizes.
        //private static int[][] DivideArray(int numOfProcesses)
        //{
        //    numberOfProcesses = numOfProcesses;
        //    try
        //    {
        //        chunkSize = largeArray.Length / numberOfProcesses;

        //    }
        //    catch (DivideByZeroException e) //if the number of processes is 0, the code will throw a DivideByZeroException.
        //    {
        //        Console.WriteLine(e.Message);

        //    }
        //    int startIndex = 0;
        //    int endIndex = chunkSize;
        //    int[][] chunks = new int[numberOfProcesses][];

        //    for (int i = 0; i < numberOfProcesses; i++)
        //    {
        //        chunks[i] = new int[chunkSize];
        //        Array.Copy(largeArray, startIndex, chunks[i], 0, chunkSize);
        //        startIndex += chunkSize;
        //        endIndex += chunkSize;
        //    }
        //    return chunks;
        //}
        //private static int FindMinimum()
        //{
        //    int minimumValue = int.MaxValue;
        //    for (int i = 0; i < numberOfProcesses; i++)
        //    {
        //        if (responses[i] < minimumValue)
        //        {
        //            minimumValue = responses[i];
        //        }
        //    }
        //    return minimumValue;
        //}

       // #endregion
    }
}
