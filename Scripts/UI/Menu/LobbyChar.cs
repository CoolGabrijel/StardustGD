using Godot;
using System;

namespace Stardust.Godot.UI
{
	public partial class LobbyChar : Control
	{
        [Export] PawnType PawnType { get; set; }
        [Export] bool isRandom;

        [ExportGroup("References")]
        [Export] TextureButton btn;
        [Export] TextureRect btnBorder;
        [Export] TextureRect btnMask;
        [Export] Control portraits;

        public override void _Ready()
        {
            btn.MouseEntered += OnMouseEntered;
            btn.MouseExited += OnMouseExited;

            for (int i = 0; i < GetParent().GetChildCount(); i++)
            {
                if (GetParent().GetChild(i) == this)
                {
                    if (i % 2 != 0)
                    {
                        btnBorder.FlipV = true;
                        btnMask.FlipV = true;
                        btn.RotationDegrees = -90;
                    }
                }
            }

            for (int i = 0; i < portraits.GetChildCount(); i++)
            {
                portraits.GetChild<TextureRect>(i).Visible = (int)PawnType == i;
            }
        }

        private void OnMouseEntered()
        {
            btnBorder.Modulate = Colors.DarkCyan;
            btn.Modulate = Colors.DarkCyan;
        }

        private void OnMouseExited()
        {
            btnBorder.Modulate = Colors.White;
            btn.Modulate = Colors.White;
        }
    } 
}
