using Godot;

namespace Stardust.Godot
{
    [Tool]
    public partial class RoomSelection2D : Node2D
    {
        public static RoomSelection2D Instance { get; private set; }

        [Export] int spriteSize = 410;
        [Export] int sheenThickness = 20;
        [Export] AudioStreamPlayer audioPlayer;

        RandomNumberGenerator rng = new();
        Tween moveTween;

        public override void _Ready()
        {
            Instance = this;
        }

        public void SetPos(Vector2 pos)
        {
            moveTween?.Kill();

            moveTween = CreateTween();
            moveTween.TweenProperty(this, "global_position", pos, .1f);

            if (pos != GlobalPosition)
            {
                audioPlayer.PitchScale = rng.RandfRange(0.9f, 1.1f);
                audioPlayer.Play();
            }
        }
    } 
}
