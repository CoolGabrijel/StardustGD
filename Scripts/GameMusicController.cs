using Godot;

namespace Stardust.Godot;

public partial class GameMusicController : AudioStreamPlayer
{
	[Export] private AudioStream lowDamageIntro;
	[Export] private AudioStream lowDamageLoop;
	[Export] private AudioStream medDamageIntro;
	[Export] private AudioStream medDamageLoop;
	[Export] private AudioStream highDamageIntro;
	[Export] private AudioStream highDamageLoop;

	private AudioStream nextStream;

	public override void _Process(double delta)
	{
		int damage = 0;

		foreach (Room room in GameLogic.RoomManager.Rooms)
		{
			damage += room.DamageAmount;
		}

		if (damage < 3)
		{
			if (Stream != lowDamageLoop && Stream != lowDamageIntro)
			{
				Stream = lowDamageIntro;
				nextStream = lowDamageLoop;
				Finished += OnFinished;
				Play();
			}
		}
		else if (damage < 6)
		{
			if (Stream != medDamageLoop && Stream != medDamageIntro)
			{
				Stream = medDamageIntro;
				nextStream = medDamageLoop;
				Finished += OnFinished;
				Play();
			}
		}
		else if (damage < 9)
		{
			if (Stream != highDamageLoop && Stream != highDamageIntro)
			{
				Stream = highDamageIntro;
				nextStream = highDamageLoop;
				Finished += OnFinished;
				Play();
			}
		}
	}

	private void OnFinished()
	{
		Stream = nextStream;
		Play();
	}
}
