using Godot;
using System;

namespace Stardust.Godot.UI;

public partial class SettingsMenu : Control
{
	private void Back()
	{
		MainMenuScreen.Instance.ShowMainMenu();
	}
}
