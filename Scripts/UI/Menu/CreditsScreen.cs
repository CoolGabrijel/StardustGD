using Godot;
using System;

namespace Stardust.Godot.UI;

public partial class CreditsScreen : Control
{
	public void OpenLink(string url)
	{
		OS.ShellOpen(url);
	}
	
	public void CloseMenu()
	{
		MainMenuScreen.Instance.ShowMainMenu();
	}
}
