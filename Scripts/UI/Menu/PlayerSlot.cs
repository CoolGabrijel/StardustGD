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

		[Export] Dictionary<string, Texture2D> portraits;

		private string[] selectableChars;
		private string selectedChar = "Random";
		private int randIndex = 0;
		private Tween randCycleTween;
		private Tween charDrawerTween;

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
		}

        public override void _Process(double delta)
		{

			if (Input.IsKeyPressed(Key.H))
			{
				ChangePortrait("Zambuko");
			}
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
				return;
            }
			else
			{
				portrait.Material = null;
			}

            portrait.Texture = portraits[newChar];
            randCycleTween?.Kill();
            portrait.Modulate = Colors.White;
        }

		private void PlayRandomCycle()
		{
			randCycleTween?.Kill();
			//portrait.Modulate = new Color(1,1,1, 0.6f);
			portrait.Modulate = new Color(1,1,1, 0f);
            randIndex++;
            if (randIndex >= selectableChars.Length) randIndex = 1;
            portrait.Texture = portraits[selectableChars[randIndex]];

            randCycleTween = portrait.CreateTween();
			//randCycleTween.TweenProperty(portrait, "modulate", new Color(1, 1, 1, 0.1f), 2f).SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Circ);
			randCycleTween.TweenProperty(portrait, "modulate", new Color(1, 1, 1, 1f), 2f).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Circ);
			randCycleTween.TweenProperty(portrait, "modulate", new Color(1, 1, 1, 0f), 2f).SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Circ);
			randCycleTween.Finished += PlayRandomCycle;
		}
	} 
}
