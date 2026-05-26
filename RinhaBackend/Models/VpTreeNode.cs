namespace RinhaBackend.Models
{
    public readonly record struct VpTreeNode(
        int Id,
        int LeftChildId,
        int RightChildId,
        float Threshold
    );
}
