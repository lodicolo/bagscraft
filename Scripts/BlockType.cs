using System;
using Godot;

namespace BagsCraft.Scripts;

[GlobalClass]
[Tool]
public partial class BlockType : Resource
{
    [Export] public string BlockName { get; set; }

    [Export] public bool IsSolid { get; set; }

    [Export] public int BackFaceTexture { get; set; }

    [Export] public int FrontFaceTexture { get; set; }

    [Export] public int TopFaceTexture { get; set; }

    [Export] public int BottomFaceTexture { get; set; }

    [Export] public int LeftFaceTexture { get; set; }

    [Export] public int RightFaceTexture { get; set; }

    public int GetTextureId(int faceIndex)
    {
        return faceIndex switch
        {
            0 => BackFaceTexture,
            1 => FrontFaceTexture,
            2 => TopFaceTexture,
            3 => BottomFaceTexture,
            4 => LeftFaceTexture,
            5 => RightFaceTexture,
            _ => throw new ArgumentOutOfRangeException(
                nameof(faceIndex),
                faceIndex,
                $"{nameof(faceIndex)} is expected to be in the range [0, 5]."
            ),
        };
    }
}