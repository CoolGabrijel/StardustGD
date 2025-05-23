using Godot;
using System;

namespace Stardust
{
	public class StardustGameConfig
	{
		public enum GameDifficulty { Easy, Normal, Hard }

		public static StardustGameConfig CurrentConfig { get; set; }


		public bool FirstStepsEnabled { get; set; } = false;
		public GameDifficulty Difficulty { get; set; } = GameDifficulty.Easy;
        public bool DamageDisabled { get; set; } = false;
		public bool FailStateDisabled { get; set; } = false;
    } 
}
