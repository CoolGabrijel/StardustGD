using Godot;
using System;

namespace Stardust.Godot
{
    [Tool]
    public partial class RoomSelection2D : Node2D
    {
        public static RoomSelection2D Instance { get; private set; }

        [Export] int spriteSize = 410;
        [Export] int sheenThickness = 20;

        Tween moveTween;

        public override void _Ready()
        {
            Instance = this;
        }

        public void SetPos(Vector2 pos)
        {
            if (moveTween != null) moveTween.Kill();

            moveTween = CreateTween();
            moveTween.TweenProperty(this, "global_position", pos, .1f);
        }
    } 
}
