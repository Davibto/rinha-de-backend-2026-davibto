using System.Runtime.CompilerServices;

namespace RinhaBackend.Models;

[InlineArray(16)]
public struct Vector16
{
    public sbyte Element0;
}

public struct TransactionRecord
{
    public Vector16 Vector;
    public bool IsFraud { get; set; }
}