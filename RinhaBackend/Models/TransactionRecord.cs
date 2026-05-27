using System.Runtime.CompilerServices;

namespace RinhaBackend.Models;

[InlineArray(14)]
public struct Vector14
{
    public sbyte Element0;
}

public struct TransactionRecord
{
    public Vector14 Vector;
    public bool IsFraud { get; set; }
}