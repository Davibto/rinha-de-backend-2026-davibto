namespace RinhaBackend.Services;

using System;
using System.IO;
using RinhaBackend.Models;

public class DataLoaderService
{
    public TransactionRecord[] Dataset { get; private set; } = Array.Empty<TransactionRecord>();

    public void LoadBinFile(string filePath)
    {
        const int totalRecords = 3000000;
        TransactionRecord[] tempDataset = new TransactionRecord[totalRecords];

        using var stream = File.OpenRead(filePath);
        using var reader = new BinaryReader(stream);

        int currentIndex = 0;

        while (stream.Position < stream.Length)
        {
            TransactionRecord record = new TransactionRecord();

            for (int i = 0; i < 14; i++)
            {
                record.Vector[i] = reader.ReadSByte();
            }

            byte flag = reader.ReadByte();
            record.IsFraud = (flag == 1);

            tempDataset[currentIndex] = record;
            currentIndex++;
        }

        Dataset = tempDataset;
    }
}