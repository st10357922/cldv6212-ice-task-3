using System;
using System.IO;
using System.Text;
using Azure.Storage.Queues; 
using Azure.Storage.Files.Shares;
using Azure.Storage.Queues.Models;
using Azure;

class Program
{
    static void Main(string[] args)
    {
        string[] messages = RetrieveMessagesFromQueue();

        string fileName = "Atlegang-Mogatusi.txt";
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);

        File.WriteAllLines(filePath, messages);
        Console.WriteLine($"Fibonacci sequence written to {fileName}");

        UploadFileToAzureFileStorage(filePath, fileName);
    }

    static string[] RetrieveMessagesFromQueue()
    {
        string connectionString = "DefaultEndpointsProtocol=https;AccountName=cldvicetask2;AccountKey=sgNtHprm/MWGAUxX+diMlaZb/UBlZeYsqxi+xjlX1ApydF3Ydp/RQ/7ADHUH4Zyia09IpH/fM4o4+AStq/HBVA==;EndpointSuffix=core.windows.net";

        string queueName = "fibonacci-queue";

        QueueClient queueClient = new QueueClient(connectionString, queueName);

        if (queueClient.Exists())
        {
            Console.WriteLine("Retrieving messages from the queue...");
            QueueMessage[] retrievedMessages = queueClient.ReceiveMessages(maxMessages: 32); 
            string[] messageContents = new string[retrievedMessages.Length];

            for (int i = 0; i < retrievedMessages.Length; i++)
            {
                messageContents[i] = retrievedMessages[i].MessageText;
                queueClient.DeleteMessage(retrievedMessages[i].MessageId, retrievedMessages[i].PopReceipt); 
                Console.WriteLine($"Processed message: {retrievedMessages[i].MessageText}");
            }

            return messageContents;
        }
        else
        {
            Console.WriteLine("Queue does not exist.");
            return new string[] { };
        }
    }

    static void UploadFileToAzureFileStorage(string filePath, string fileName)
    {
        string connectionString = "DefaultEndpointsProtocol=https;AccountName=cldvicetask2;AccountKey=sgNtHprm/MWGAUxX+diMlaZb/UBlZeYsqxi+xjlX1ApydF3Ydp/RQ/7ADHUH4Zyia09IpH/fM4o4+AStq/HBVA==;EndpointSuffix=core.windows.net";

        string shareName = "fibonacci-share";
        string directoryName = "fibonacci-files";

        ShareClient shareClient = new ShareClient(connectionString, shareName);
        shareClient.CreateIfNotExists();

        ShareDirectoryClient directoryClient = shareClient.GetDirectoryClient(directoryName);
        directoryClient.CreateIfNotExists();

        ShareFileClient fileClient = directoryClient.GetFileClient(fileName);

        using (FileStream stream = File.OpenRead(filePath))
        {
            fileClient.Create(stream.Length); // Create the file
            fileClient.UploadRange(new HttpRange(0, stream.Length), stream); // Upload the content
            Console.WriteLine($"File {fileName} uploaded to Azure File Storage.");
        }
    }
}
