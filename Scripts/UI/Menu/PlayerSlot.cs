using Godot;
using Godot.Collections;

namespace Stardust.Godot.UI
{
	public partial class PlayerSlot : Control
	{
		[Export] TextureRect portrait;
		[Export] Label nameplate;

		[Export] Dictionary<string, Texture2D> portraits;

		private string[] selectableChars;
		private string selectedChar = "Random";
		private int randIndex = 0;
		private Tween randCycleTween;

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
		}

		public override void _Process(double delta)
		{

			if (Input.IsKeyPressed(Key.H))
			{
				ChangePortrait("Zambuko");
			}
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
				PlayRandomCycle();
				return;
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
