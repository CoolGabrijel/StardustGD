using Godot;
using System;

namespace Stardust.Godot;

public partial class NetworkManager : Node
{
	public override void _Ready()
	{
		PIOMP.MultiplayerUtils.CreateMessageHandlers(System.Reflection.Assembly.GetAssembly(typeof(ClientSend)));
	}
	
	public override void _PhysicsProcess(double delta)
	{
		PIOMP.MultiplayerUtils.Tick();
	}
}