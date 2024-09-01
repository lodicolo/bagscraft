using Godot;
using Godot.Collections;

namespace BagsCraft.Scripts;

[Tool]
public partial class World : Node3D
{
    [Export] public Material? Material { get; set; }

    [Export(PropertyHint.ArrayType, "BlockType")]
    // [Export]
    public Array<BlockType> BlockTypes { get; set; } = [];
}