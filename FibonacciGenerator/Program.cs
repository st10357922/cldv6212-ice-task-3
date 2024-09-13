using System;
using Azure.Storage.Queues; // Namespace for Azure Queue Storage

class Program
{
    static void Main(string[] args)
    {
        // Generate the Fibonacci sequence up to 233
        int a = 0, b = 1, next = 0;

        Console.WriteLine("Fibonacci Sequence up to 233:");

        while (next <= 233)
        {
            Console.WriteLine(next);
            // Send the number to Azure Queue Storage
            SendMessageToQueue(next.ToString());

            // Generate the next Fibonacci number
            a = b;
            b = next;
            next = a + b;
        }
    }

    static void SendMessageToQueue(string message)
    {
        // Connection string to your Azure Storage account
        string connectionString = "DefaultEndpointsProtocol=https;AccountName=cldvicetask2;AccountKey=sgNtHprm/MWGAUxX+diMlaZb/UBlZeYsqxi+xjlX1ApydF3Ydp/RQ/7ADHUH4Zyia09IpH/fM4o4+AStq/HBVA==;EndpointSuffix=core.windows.net";

        // Name of the queue
        string queueName = "fibonacci-queue";

        // Create a QueueClient
        QueueClient queueClient = new QueueClient(connectionString, queueName);

        // Create the queue if it doesn't exist
        queueClient.CreateIfNotExists();

        if (queueClient.Exists())
        {
            // Send a message to the queue
            queueClient.SendMessage(message);
            Console.WriteLine($"Sent message: {message}");
        }
    }
}
