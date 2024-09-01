using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;
using Godot.Collections;

namespace BagsCraft.Scripts;

[Tool]
public partial class World : Node3D
{
    [Export] public Camera3D? Player { get; set; }

    [Export] public Vector3 Spawn { get; set; }

    [Export] public Material? Material { get; set; }

    [Export(PropertyHint.ArrayType, nameof(BlockType))]
    public Array<BlockType> BlockTypes { get; set; } = [];

    private readonly Chunk?[,] _chunks = new Chunk?[VoxelData.WorldSizeInChunks, VoxelData.WorldSizeInChunks];
    private readonly HashSet<ChunkCoord> _activeChunks = [];

    private ChunkCoord _playerLastChunkCoord;

    public override void _Ready()
    {
        base._Ready();

        GenerateWorld();
        _playerLastChunkCoord = GetChunkCoordFromVector3(Player?.GlobalPosition ?? default);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        var playerCurrentChunkCoord = GetChunkCoordFromVector3(Player?.GlobalPosition ?? default);
        if (playerCurrentChunkCoord != _playerLastChunkCoord)
        {
            CheckViewDistance(playerCurrentChunkCoord);
        }
    }

    public byte GetVoxel(int x, int y, int z)
    {
        if (!IsVoxelInWorld(x, y, z))
        {
            return 0;
        }

        return y switch
        {
            < 1 => 1,
            VoxelData.ChunkHeight - 1 => 3,
            _ => 2,
        };
    }

    public bool IsChunkInWorld(int x, int z)
    {
        return x is >= 0 and < VoxelData.WorldSizeInChunks &&
               z is >= 0 and < VoxelData.WorldSizeInChunks;
    }

    public Chunk? GetChunk(ChunkCoord chunkCoord) =>
        IsChunkInWorld(chunkCoord.X, chunkCoord.Z) ? _chunks[chunkCoord.X, chunkCoord.Z] : default;

    public bool IsVoxelInWorld(int x, int y, int z)
    {
        return x is >= 0 and < VoxelData.WorldSizeInBlocks &&
               y is >= 0 and < VoxelData.ChunkHeight &&
               z is >= 0 and < VoxelData.WorldSizeInBlocks;
    }

    private void GenerateWorld()
    {
        const int halfWorldSizeInChunks = VoxelData.WorldSizeInChunks >> 1;
        const int halfViewDistanceInChunks = VoxelData.ViewDistanceInChunks >> 1;
        const int minChunk = halfWorldSizeInChunks - halfViewDistanceInChunks;
        const int maxChunk = halfWorldSizeInChunks + halfViewDistanceInChunks;

        for (int x = minChunk; x < maxChunk; ++x)
        for (int z = minChunk; z < maxChunk; ++z)
        {
            CreateChunk(new ChunkCoord(x, z));
        }

        Spawn = new Vector3(
            VoxelData.WorldSizeInBlocks >> 1,
            VoxelData.ChunkHeight + 2,
            VoxelData.WorldSizeInBlocks >> 1
        );

        if (Player != default)
        {
            Player.GlobalPosition = Spawn;
        }
    }

    private void CheckViewDistance(ChunkCoord playerChunk)
    {
        const int halfViewDistanceInChunks = VoxelData.ViewDistanceInChunks >> 1;

        HashSet<ChunkCoord> previouslyActiveChunks = [.._activeChunks];

        for (int x = playerChunk.X - halfViewDistanceInChunks; x < playerChunk.X + halfViewDistanceInChunks; ++x)
        for (int z = playerChunk.Z - halfViewDistanceInChunks; z < playerChunk.Z + halfViewDistanceInChunks; ++z)
        {
            if (!IsChunkInWorld(x, z))
            {
                continue;
            }

            ChunkCoord thisChunk = new(x, z);

            Chunk? chunkAt = GetChunk(thisChunk);
            if (chunkAt == default)
            {
                CreateChunk(thisChunk);
            }
            else if (!chunkAt.IsActive)
            {
                chunkAt.IsActive = true;
                _activeChunks.Add(thisChunk);
            }

            previouslyActiveChunks.Remove(thisChunk);
        }

        foreach (var chunkCoord in previouslyActiveChunks)
        {
            _activeChunks.Remove(chunkCoord);
            var chunk = GetChunk(chunkCoord);
            if (chunk != default)
            {
                chunk.IsActive = false;
            }
        }
    }

    private void CreateChunk(ChunkCoord coord)
    {
        Chunk chunk = new(coord, this);
        _chunks[coord.X, coord.Z] = chunk;
        _activeChunks.Add(coord);
    }

    public static ChunkCoord GetChunkCoordFromVector3(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.X / VoxelData.ChunkWidth);
        int z = Mathf.FloorToInt(position.Z / VoxelData.ChunkWidth);
        return new ChunkCoord(x, z);
    }
}