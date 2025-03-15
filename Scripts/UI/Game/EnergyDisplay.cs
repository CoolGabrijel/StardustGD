using Godot;
using System.Collections.Generic;
using System.Linq;

namespace Stardust.Godot.UI
{
	public partial class EnergyDisplay : Control
	{
		[Export] Label energyLabel;
		[Export] Control actionCardContainer;

		Pawn previousPawn;
		Dictionary<EnergyCard, Panel> cardToPanel = new();

		public override void _Ready()
		{
			previousPawn = GameStart.LocalPlayer;
			InstantiateCards(GameStart.LocalPlayer);
        }

		public override void _Process(double delta)
		{
			if (previousPawn != GameStart.LocalPlayer)
			{
				AssignCards(GameStart.LocalPlayer);
				previousPawn = GameStart.LocalPlayer;
			}

            energyLabel.Text = GameLogic.EnergyExpended.ToString();

			foreach (KeyValuePair<EnergyCard, Panel> pair in cardToPanel)
			{
				if (pair.Key.Exhausted)
				{
					pair.Value.Modulate = Colors.White / 2;
					continue;
				}
				else
				{
					pair.Value.Modulate = Colors.White;
                }
			}

            if (GameLogic.TurnQueue.CurrentPawn == GameStart.LocalPlayer)
            {
                // Card that will be exhausted
                EnergyCard card = GameStart.LocalPlayer.EnergyCards.Where((x) => !x.Exhausted).Where((x) => x.Energy >= GameLogic.EnergyExpended).FirstOrDefault();
				if (card == null) return; // When every card is depleted, card will be null

				// Visualize it
                cardToPanel[card].Modulate = new Color(2, 2, 2);
            }
        }

		private void AssignCards(Pawn pawn)
		{
			cardToPanel.Clear();

            for (int i = 0; i < pawn.EnergyCards.Length; i++)
            {
                cardToPanel.Add(pawn.EnergyCards[i], actionCardContainer.GetChild<Panel>(i));
            }
        }

		private void InstantiateCards(Pawn pawn)
		{
			Node cardGraphicPrefab = actionCardContainer.GetChild(0).Duplicate();

			actionCardContainer.FreeChildren();

			foreach (EnergyCard card in pawn.EnergyCards)
			{
				Node instance = cardGraphicPrefab.Duplicate();
				instance.GetNode<Label>("Label").Text = card.Energy.ToString();
				actionCardContainer.AddChild(instance);
				cardToPanel.Add(card, instance as Panel);
			}
		}
	} 
}
