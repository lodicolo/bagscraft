using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace BagsCraft.Scripts;

public partial class Chunk : Node3D
{
    [Export(PropertyHint.NodeType, nameof(MeshInstance3D))]
    private MeshInstance3D? _meshRenderer;

    public override void _Ready()
    {
        base._Ready();

        _meshRenderer ??= GetNode<MeshInstance3D>("MeshRenderer");

        if (_meshRenderer == default)
        {
            throw new InvalidOperationException();
        }

        var vertexIndex = 0;
        List<Vector3> vertices = [];
        List<int> triangles = [];
        List<Vector2> uvs = [];

        for (var p = 0; p < VoxelData.VoxelTris.GetLength(0); ++p)
        for (var i = 0; i < 6; ++i)
        {
            var triangleIndex = VoxelData.VoxelTris[p, i];
            vertices.Add(VoxelData.VoxelVerts[triangleIndex]);
            triangles.Add(vertexIndex);
            uvs.Add(VoxelData.VoxelUVs[i]);
            ++vertexIndex;
        }

        SurfaceTool surfaceTool = new();
        surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
        for (var vi = 0; vi < vertices.Count; ++vi)
        {
            surfaceTool.SetUV(uvs[vi]);
            surfaceTool.AddVertex(vertices[vi]);
            surfaceTool.AddIndex(triangles[vi]);
        }

        var material = GD.Load<StandardMaterial3D>("res://Assets/Materials/Voxels.tres");
        surfaceTool.SetMaterial(material);
        surfaceTool.GenerateNormals();

        var mesh = surfaceTool.Commit();
        _meshRenderer.Mesh = mesh;

        Debug.WriteLine("Generated mesh");
    }
}