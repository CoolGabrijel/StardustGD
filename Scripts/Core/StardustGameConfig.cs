using Godot;
using System;

namespace Stardust
{
	public class StardustGameConfig
	{
		public static StardustGameConfig CurrentConfig { get; set; }

		public bool DamageDisabled { get; set; } = false;
		public bool FirstStepsEnabled { get; set; } = false;
	} 
}
