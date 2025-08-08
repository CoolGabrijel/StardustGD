using Godot;
using Godot.Collections;

namespace Stardust.Godot.UI
{
	public partial class PlayerSlot : Control
	{
		[Export] bool IsLocal;
		[Export] TextureRect portrait;
		[Export] Label nameplate;
		[Export] ShaderMaterial greyscaleShader;
		[Export] Control characterSelectDrawer;
		[Export] private ColorRect fadeRect;
		[Export] private ColorRect curtainRect;
		[Export] private GpuParticles2D particles;

		[Export] Dictionary<string, Texture2D> portraits;

		private string[] selectableChars;
		private string selectedChar = "Random";
		private int randIndex = 0;
		private float curtainOriginalSize;
		private Tween randCycleTween;
		private Tween charDrawerTween;
		private Tween onSelectTween;

		public override void _Ready()
		{
			PawnType[] pawns = new PawnType[]
			{
				PawnType.Concorde,
				PawnType.Aurora,
				PawnType.Zambuko,
				PawnType.Rosetta,
				PawnType.Wolfram
			};
			GenerateSelectableChars(pawns);
			ChangePortrait(selectedChar);

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
			}

			particles.Emitting = false;
			curtainOriginalSize = curtainRect.Size.Y;
			VisibilityChanged += OnVisibilityChanged;
		}

        public override void _Process(double delta)
		{
			if (Input.IsKeyPressed(Key.H))
			{
				ChangePortrait("Zambuko");
			}
			
			particles.GlobalPosition = curtainRect.GlobalPosition + Vector2.Right * curtainRect.Size / 2;
        }

        private void OnMouseEntered()
        {
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
	        ChangePortrait("Random");
        }

        public void GenerateSelectableChars(PawnType[] chars)
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
			selectedChar = newChar;
            nameplate.Text = newChar.ToUpper();

            if (newChar == "Random")
            {
				portrait.Material = greyscaleShader;
				PlayRandomCycle();
				PlayCharSelectAnimation();
				return;
            }
            
            portrait.Material = null;
            portrait.Texture = portraits[newChar];
            randCycleTween?.Kill();
            portrait.Modulate = Colors.White;
            PlayCharSelectAnimation();
        }

		private void PlayCharSelectAnimation()
		{
			particles.Emitting = true;
			curtainRect.Show();
			onSelectTween?.Kill();
			onSelectTween = CreateTween();
			onSelectTween.SetTrans(Tween.TransitionType.Quart);
			onSelectTween.SetEase(Tween.EaseType.Out);
			onSelectTween.TweenProperty(fadeRect, "color", Colors.Transparent, 1f).From(Colors.White);
			onSelectTween.Parallel().TweenProperty(curtainRect, "offset_top", curtainOriginalSize, 1f).From(0f);
			onSelectTween.Finished += () => particles.Emitting = false;
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
