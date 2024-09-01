using Godot;

namespace BagsCraft.Scripts;

public static class VoxelData
{
    public const int ChunkWidth = 5;
    public const int ChunkHeight = 15;

    public const int TextureAtlasSizeInBlocks = 4;

    public const float NormalizedBlockTextureSize = 1f / TextureAtlasSizeInBlocks;

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
        { 1, 2, 3, 0 }, // Back Face
        { 4, 7, 6, 5 }, // Front Face
        { 7, 3, 2, 6 }, // Top Face
        { 5, 1, 0, 4 }, // Bottom Face
        { 0, 3, 7, 4 }, // Left Face
        { 5, 6, 2, 1 }, // Right Face
    };

    public static readonly Vector2[] VoxelUVs =
    [
        new Vector2(0, 1),
        new Vector2(0, 0),
        new Vector2(1, 0),
        // new Vector2(1, 0),
        new Vector2(1, 1),
        // new Vector2(0, 1),
    ];

    public static readonly Vector3I[] FaceChecks =
    [
        new Vector3I(+0, +0, -1),
        new Vector3I(+0, +0, +1),
        new Vector3I(+0, +1, +0),
        new Vector3I(+0, -1, +0),
        new Vector3I(-1, +0, +0),
        new Vector3I(+1, +0, +0),
    ];
}