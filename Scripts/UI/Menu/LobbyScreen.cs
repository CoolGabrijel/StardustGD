using Godot;
using System;

namespace Stardust.Godot.UI
{
	public partial class LobbyScreen : Control
	{
		public static event Action<bool> OnReady;
		
		[Export] private Button backBtn;
		[Export] private Button readyBtn;
		[Export] private Button[] lobbyBtns;
		
		private Tween moveTween;

		private void OnReadyClicked()
		{
			//GetTree().ChangeSceneToFile("res://Scenes/Game2D.tscn");
			if (readyBtn.ButtonPressed)
				MusicController.Instance.OnPlayerReady();
			
			OnReady?.Invoke(readyBtn.ButtonPressed);
		}
		
		private void OnSettingsClicked()
		{
			moveTween?.Kill();

			moveTween = CreateTween();
			moveTween.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Expo);
			moveTween.TweenProperty(this, "position", new Vector2(0, -450), .25f);

			foreach (Button btn in lobbyBtns)
			{
				btn.Hide();
			}
			backBtn.Show();
		}

		private void OnBackClicked()
		{
			moveTween?.Kill();

			moveTween = CreateTween();
			moveTween.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Expo);
			moveTween.TweenProperty(this, "position", Vector2.Zero, .25f);

			foreach (Button btn in lobbyBtns)
			{
				btn.Show();
			}
			backBtn.Hide();
		}

		private void OnLeaveClicked()
		{
			MainMenuScreen.Instance.ShowMainMenu();
		}
	}
}
