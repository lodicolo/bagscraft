using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace BagsCraft.Scripts;

[Tool]
public partial class Chunk : Node3D
{
    private readonly MeshInstance3D _meshRenderer;

    private int _vertexIndex;
    private readonly List<Vector3> _vertices = [];
    private readonly List<int> _triangles = [];
    private readonly List<Vector2> _uvs = [];

    private readonly byte[,,] _voxelMap = new byte[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];

    private string DebugName => $"{NativeInstance:x16} (\"{Name}\")";

    [Export] private World? _world;

    public Chunk()
    {
        Debug.WriteLine($"[Chunk {DebugName}] Initializing...");

        _meshRenderer = new MeshInstance3D { Name = "MeshRenderer" };
        AddChild(_meshRenderer);
    }

    public override void _Ready()
    {
        if (_meshRenderer == default)
        {
            throw new InvalidOperationException("Failed to create mesh renderer");
        }

        if (_world == default)
        {
            throw new InvalidOperationException($"Missing reference to a {nameof(World)} node.");
        }

        base._Ready();

        PopulateVoxelMap();
        CreateMeshData();
        CreateMesh();
    }

    private void PopulateVoxelMap()
    {
        Debug.WriteLine($"[Chunk {DebugName}] Populating voxel data...");

        for (int y = 0; y < VoxelData.ChunkHeight; ++y)
        for (int x = 0; x < VoxelData.ChunkWidth; ++x)
        for (int z = 0; z < VoxelData.ChunkWidth; ++z)
        {
            _voxelMap[x, y, z] = y switch
            {
                < 1 => 0,
                VoxelData.ChunkHeight - 1 => 3,
                _ => 1,
            };
        }

        Debug.WriteLine($"[Chunk {DebugName}] Done populating voxel data");
    }

    private bool CheckVoxel(int x, int y, int z)
    {
        if (x < 0 || y < 0 || z < 0)
        {
            return false;
        }

        if (x >= VoxelData.ChunkWidth || y >= VoxelData.ChunkHeight || z >= VoxelData.ChunkWidth)
        {
            return false;
        }

        var blockTypeId = _voxelMap[x, y, z];
        var blockType = _world?.BlockTypes[blockTypeId];
        return blockType?.IsSolid ?? false;
    }

    private bool CheckVoxel(Vector3I positionInChunk) =>
        CheckVoxel(positionInChunk.X, positionInChunk.Y, positionInChunk.Z);

    private bool CheckVoxel(Vector3 pos) => CheckVoxel(
        Mathf.FloorToInt(pos.X),
        Mathf.FloorToInt(pos.Y),
        Mathf.FloorToInt(pos.Z)
    );

    private void CreateMeshData()
    {
        Debug.WriteLine($"[Chunk {DebugName}] Generating mesh data...");

        for (int y = 0; y < VoxelData.ChunkHeight; ++y)
        for (int x = 0; x < VoxelData.ChunkWidth; ++x)
        for (int z = 0; z < VoxelData.ChunkWidth; ++z)
        {
            AddVoxelDataToChunk(x, y, z);
        }

        Debug.WriteLine($"[Chunk {DebugName}] Done generating mesh data ({_vertices.Count} vertices, {_triangles.Count / 3} tris)");
    }

    private void AddVoxelDataToChunk(int x, int y, int z)
    {
        if (!CheckVoxel(x, y, z))
        {
            return;
        }

        Vector3I positionInChunk = new(x, y, z);
        for (var p = 0; p < VoxelData.VoxelTris.GetLength(0); ++p)
        {
            var faceVoxelPosition = positionInChunk + VoxelData.FaceChecks[p];
            if (CheckVoxel(faceVoxelPosition))
            {
                // Skip faces that are covered
                continue;
            }

            var blockTypeId = _voxelMap[x, y, z];
            var blockType = _world?.BlockTypes[blockTypeId];
            var textureId = blockType?.GetTextureId(p) ?? -1;

            for (var vi = 0; vi < 4; ++vi)
            {
                _vertices.Add(positionInChunk + VoxelData.VoxelVerts[VoxelData.VoxelTris[p, vi]]);
            }

            if (textureId >= 0)
            {
                AddTexture(textureId);
            }
            else
            {
                GD.PushWarning("Invalid texture ID/missing block type");
            }

            _triangles.Add(_vertexIndex + 0);
            _triangles.Add(_vertexIndex + 1);
            _triangles.Add(_vertexIndex + 2);
            _triangles.Add(_vertexIndex + 2);
            _triangles.Add(_vertexIndex + 3);
            _triangles.Add(_vertexIndex + 0);

            _vertexIndex += 4;
        }
    }

    private void AddTexture(int textureId)
    {
        var x = textureId % VoxelData.TextureAtlasSizeInBlocks;
        var y = textureId / VoxelData.TextureAtlasSizeInBlocks;

        Vector2 uv = new(x, y);
        uv *= VoxelData.NormalizedBlockTextureSize;
        // uv.Y = 1 - uv.Y - VoxelData.NormalizedBlockTextureSize;

        _uvs.Add(uv + new Vector2(0, VoxelData.NormalizedBlockTextureSize));
        _uvs.Add(uv);
        _uvs.Add(uv + new Vector2(VoxelData.NormalizedBlockTextureSize, 0));
        _uvs.Add(uv + Vector2.One * VoxelData.NormalizedBlockTextureSize);
    }

    private void CreateMesh()
    {
        Debug.WriteLine($"[Chunk {DebugName}] Generating mesh...");

        SurfaceTool surfaceTool = new();
        surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
        for (var vi = 0; vi < _vertices.Count; ++vi)
        {
            surfaceTool.SetUV(_uvs[vi]);
            surfaceTool.AddVertex(_vertices[vi]);
        }

        foreach (var index in _triangles)
        {
            surfaceTool.AddIndex(index);
        }

        var material = GD.Load<StandardMaterial3D>("res://Assets/Materials/Voxels.tres");
        surfaceTool.SetMaterial(_world?.Material ?? material);
        surfaceTool.GenerateNormals();

        var mesh = surfaceTool.Commit();
        _meshRenderer.Mesh = mesh;

        Debug.WriteLine($"[Chunk {DebugName}] Done generating mesh");
    }
}