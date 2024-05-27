using Godot;
using Stardust.Actions;
using System;

namespace Stardust.Godot
{
	public partial class RoomPartsDisplay : Button
	{
		[Export] private Room2D roomGraphic;
        [Export] Label partsLabel;

        public override void _Ready()
        {
            Pressed += OnClick;
        }

        public override void _Process(double delta)
        {
            if (roomGraphic.Room.Parts > 0)
            {
                Show();
                partsLabel.Text = $"Parts: {roomGraphic.Room.Parts}";
                Disabled = false;
            }
            else
            {
                Hide();
                Disabled = true;
            }
        }

        private void OnClick()
        {
            PickUpPart PickupAction = new(GameStart.LocalPlayer, roomGraphic.Room, roomGraphic.Room.GetItem(ItemType.Part));
            PickupAction.Do();
            ActionLibrary.AddAction(PickupAction);
        }

        public override void _ExitTree()
        {
            Pressed -= OnClick;
        }
    } 
}
