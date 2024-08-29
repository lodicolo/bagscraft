using Godot;

namespace BagsCraft.Scripts;

public static class VoxelData
{
    public static readonly Vector3[] VoxelVerts =
    [
        new Vector3(0, 0, 0),
        new Vector3(1, 0, 0),
        new Vector3(1, 1, 0),
        new Vector3(0, 1, 0),
        new Vector3(0, 0, 1),
        new Vector3(1, 0, 1),
        new Vector3(1, 1, 1),
        new Vector3(0, 1, 1),
    ];

    // UVs and Tris are different from the original tutorial because in Godot +Z is out of and -Z is into the screen.
    // I imagine this is the opposite in Unity. I also have no idea why this is the case in Godot, it's weird.

    public static readonly int[,] VoxelTris =
    {
        { 1, 2, 3, 3, 0, 1 }, // Back Face
        { 4, 7, 6, 6, 5, 4 }, // Front Face
        { 7, 3, 2, 2, 6, 7 }, // Top Face
        { 5, 1, 0, 0, 4, 5 }, // Bottom Face
        { 0, 3, 7, 7, 4, 0 }, // Left Face
        { 5, 6, 2, 2, 1, 5 }, // Right Face
    };

    public static readonly Vector2[] VoxelUVs =
    [
        new Vector2(0, 1),
        new Vector2(0, 0),
        new Vector2(1, 0),
        new Vector2(1, 0),
        new Vector2(1, 1),
        new Vector2(0, 1),
    ];
}