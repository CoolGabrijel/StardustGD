using Godot;

namespace Stardust.Godot
{
	[GlobalClass]
	public partial class RoomIcon : Resource
	{
		[Export] public Texture2D Icon;
        [Export] public RoomType RoomType;
	} 
}
