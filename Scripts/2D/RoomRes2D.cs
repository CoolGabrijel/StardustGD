using Godot;
using System;

namespace Stardust.Godot
{
    [GlobalClass] [Tool]
    public partial class RoomRes2D : Resource
    {
        [Export] public Texture2D Texture;
        [Export] public RoomType Type;
        [Export] public Texture2D Tex_Activation;
    } 
}
