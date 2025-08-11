using Godot;
using Godot.Collections;

namespace Stardust.Godot.UI
{
	public partial class PlayerSlot : Control
	{
		public bool PlayerReady { get; private set; }
		
		[Export] private bool IsLocal;
		[Export] private TextureRect portrait;
		[Export] private Label nameplate;
		[Export] private ShaderMaterial greyscaleShader;
		[Export] private Control characterSelectDrawer;
		[Export] private ColorRect fadeRect;
		[Export] private ColorRect curtainRect;
		[Export] private GpuParticles2D particles;
		[Export] private AudioStreamPlayer audioStreamPlayer;
		[Export] private AudioStreamPlayer slotClickAudio;
		[Export] private Control pdaGraphic;
		[Export] private TextureRect readyLight;

		[Export] private Dictionary<string, Texture2D> portraits;

		private string[] selectableChars;
		private string selectedChar = "Open";
		private int randIndex = 0;
		private float curtainOriginalSize;
		private Tween randCycleTween;
		private Tween charDrawerTween;
		private Tween onSelectTween;
		private Tween fadeTween;
		private Tween moveTween;

		public override void _Ready()
		{
			PawnType[] pawns =
			{
				PawnType.Concorde,
				PawnType.Aurora,
				PawnType.Zambuko,
				PawnType.Rosetta,
				PawnType.Wolfram
			};
			GenerateSelectableChars(pawns);
			ChangeImage(selectedChar);

			if (!IsLocal)
			{
				characterSelectDrawer.QueueFree();
				portrait.GetParent<Control>().MouseFilter = MouseFilterEnum.Ignore;
				MouseFilter = MouseFilterEnum.Ignore;
			}
			else
			{
				ZIndex += 1;
                MouseEntered += OnMouseEntered;
                MouseExited += OnMouseExited;
                LobbyScreen.OnReady += SetReady;
			}

			particles.Emitting = false;
			curtainOriginalSize = curtainRect.Size.Y;
			VisibilityChanged += OnVisibilityChanged;
		}

        public override void _Process(double delta)
		{
			if (Input.IsKeyPressed(Key.H))
			{
				ChangePortrait("Open");
			}
			
			particles.GlobalPosition = curtainRect.GlobalPosition + Vector2.Right * curtainRect.Size / 2;
        }

        public void SetReady(bool ready)
        {
	        PlayerReady = ready;
	        PlayReadyAnimation(ready);
        }

        private void OnMouseEntered()
        {
	        if (PlayerReady) return;
	        
			charDrawerTween?.Kill();

			charDrawerTween = CreateTween();
			charDrawerTween.SetTrans(Tween.TransitionType.Quart).SetEase(Tween.EaseType.Out);
			charDrawerTween.TweenProperty(characterSelectDrawer, "position", Vector2.Right * 75, .25f);
        }

        private void OnMouseExited()
        {
            charDrawerTween?.Kill();

            charDrawerTween = CreateTween();
            charDrawerTween.SetTrans(Tween.TransitionType.Quart).SetEase(Tween.EaseType.In);
            charDrawerTween.TweenProperty(characterSelectDrawer, "position", Vector2.Zero, .25f);
        }

        private void OnVisibilityChanged()
        {
	        if (!Visible) return;
	        ChangeImage(IsLocal ? "Random" : "Open");
        }

        private void GenerateSelectableChars(PawnType[] chars)
		{
			selectableChars = new string[chars.Length + 1];
			selectableChars[0] = "Random";
			for (int i = 0; i < chars.Length; i++)
			{
				selectableChars[i + 1] = chars[i].ToString();
			}
		}

		private void ChangePortrait(string newChar)
		{
			if (Visible)
			{
				audioStreamPlayer.Play();
			}
			PlayCharSelectAnimation();
			selectedChar = newChar;
			ChangeImage(newChar);
            portrait.Modulate = Colors.White;
        }

		private void ChangeImage(string newChar)
		{
			nameplate.Text = newChar.ToUpper();

			if (newChar == "Random")
			{
				portrait.Show();
				portrait.Material = greyscaleShader;
				PlayRandomCycle();
				return;
			}
			portrait.Material = null;
			randCycleTween?.Kill();

			portrait.Visible = newChar != "Open";
			
			if (portrait.Visible)
				portrait.Texture = portraits[newChar];
		}

		private void PlayReadyAnimation(bool state)
		{
			Color lightColor = state ? Colors.Green : Colors.Red;
			PlayFadeAnimation();
			moveTween?.Kill();
			moveTween = CreateTween();
			moveTween.SetTrans(Tween.TransitionType.Quart);
			moveTween.SetEase(Tween.EaseType.Out);
			
			if (!state) slotClickAudio.Play();

			moveTween.TweenProperty(pdaGraphic, "position", Vector2.Down * 25, .25f);

			moveTween.TweenInterval(.25f).Finished += () =>
			{
				readyLight.SelfModulate = lightColor;
				if (state) slotClickAudio.Play();
			};
			
			if (state)
				moveTween.TweenProperty(pdaGraphic, "position", Vector2.Down * 30, .25f);
			else
				moveTween.TweenProperty(pdaGraphic, "position", Vector2.Zero, .25f);
		}

		private void PlayCharSelectAnimation()
		{
			particles.Emitting = true;
			PlayFadeAnimation();
			curtainRect.Show();
			onSelectTween?.Kill();
			onSelectTween = CreateTween();
			onSelectTween.SetTrans(Tween.TransitionType.Quart);
			onSelectTween.SetEase(Tween.EaseType.Out);
			onSelectTween.TweenProperty(curtainRect, "offset_top", curtainOriginalSize, 1f).From(0f);
			onSelectTween.Parallel().TweenInterval(0.5f).Finished +=() => particles.Emitting = false;
		}

		private void PlayFadeAnimation()
		{
			fadeTween?.Kill();
			fadeTween = fadeRect.CreateTween();
			fadeTween.SetTrans(Tween.TransitionType.Quart);
			fadeTween.SetEase(Tween.EaseType.Out);
			fadeTween.TweenProperty(fadeRect, "color", Colors.Transparent, 1f).From(Colors.White);
		}

		private void PlayRandomCycle()
		{
			randCycleTween?.Kill();
			portrait.Modulate = new Color(1,1,1, 0f);
            randIndex++;
            if (randIndex >= selectableChars.Length) randIndex = 1;
            portrait.Texture = portraits[selectableChars[randIndex]];

            randCycleTween = portrait.CreateTween();
			randCycleTween.TweenProperty(portrait, "modulate", new Color(1, 1, 1, 1f), 2f).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Circ);
			randCycleTween.TweenProperty(portrait, "modulate", new Color(1, 1, 1, 0f), 2f).SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Circ);
			randCycleTween.Finished += PlayRandomCycle;
		}
	} 
}
