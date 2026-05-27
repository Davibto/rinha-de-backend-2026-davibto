namespace RinhaBackend.Models
{
    public readonly record struct VpTreeNode(
        int LeftChildId,
        int RightChildId,
        float Threshold
    );
}
