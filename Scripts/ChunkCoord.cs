namespace BagsCraft.Scripts;

public record struct ChunkCoord(int X, int Z)
{
    public override string ToString() => $"({X}, {Z})";
}