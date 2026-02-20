using Godot;
using Stardust.Actions;
using System.Linq;

namespace Stardust.Godot
{
	public partial class RoomObjDisplay : Button
	{
        [Export] private Room2D roomGraphic;
        [Export] Label objLabel;

        Tween colorTween;

        public override void _Ready()
        {
            Pressed += OnClick;
            MouseEntered += OnMouseEntered;
            MouseExited += OnMouseExited;
        }

        public override void _Process(double delta)
        {
            if (ObjectiveHandler.CurrentObjective == null) return;
            //if (ObjectiveHandler.CurrentObjective.Tasks[0].Tag != "Base") return;
            if (!ObjectiveHandler.CurrentObjective.Tasks[0].CanUseActionToComplete)
            {
                Hide();
                Disabled = true;
                return;
            }

            Objective obj = ObjectiveHandler.CurrentObjective;

            int taskCount = 0;

            foreach (Task task in obj.Tasks)
            {
                if (task.Completed) continue;

                if (task.Room.RoomType == roomGraphic.Room.RoomType)
                {
                    taskCount++;
                    //GD.Print(task.Description);
                }
            }

            if (taskCount > 0)
            {
                if (taskCount > 1) objLabel.Text = $"x{taskCount}";
                else objLabel.Text = "";

                Show();
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
            bool isPlayerInRoom = GameStart.LocalPlayer.Room == roomGraphic.Room;
            bool doesPlayerHaveEnergy = GameStart.LocalPlayer.EnergyCards.Where((e) => !e.Exhausted).Max((e) => e.Energy) > GameLogic.EnergyExpended;
            bool isRoomOverCapacity = roomGraphic.Room.Pawns.Count > roomGraphic.Room.Capacity;

            if (!isPlayerInRoom || !doesPlayerHaveEnergy || isRoomOverCapacity)
            {
                GetViewport().GuiReleaseFocus();
                return;
            }

            IUndoableAction action = null;
            if (ObjectiveHandler.CurrentObjective.Tasks[0].Tag == "FirstSteps")
            {
                action = new CompleteMarsTask(GameStart.LocalPlayer);
            }
            else
            {
                action = new CompleteBaseTask(roomGraphic.Room.RoomType);
            }

            action.Do();
            ActionLibrary.AddAction(action);
            
            if (PIOMP.Room.IsHost) ServerSend.CompleteTask(GameStart.PlayerId);
            else if (PIOMP.Room.IsInRoom) ClientSend.ReqCompleteTask();

            ObjectiveHandler.CheckAllObjectivesCompleted();
            GetViewport().GuiReleaseFocus();
        }

        private void OnMouseEntered()
        {
            colorTween?.Kill();

            colorTween = CreateTween();
            colorTween.TweenProperty(this, "modulate", new Color(2, 2, 2), .2f).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
        }

        private void OnMouseExited()
        {
            colorTween?.Kill();

            colorTween = CreateTween();
            colorTween.TweenProperty(this, "modulate", Colors.White, .2f).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
        }

        public override void _ExitTree()
        {
            Pressed -= OnClick;
        }
    } 
}
