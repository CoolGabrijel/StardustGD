using Godot;
using System;

namespace Stardust.Godot.UI;

public partial class SettingNode : Control
{
	public enum VolumeType {Main, Music, Sfx}
	
	[Export] private VolumeType volumeType;
	[Export] private Slider slider;
	[Export] private LineEdit lineEdit;
	[Export] private TextureRect grabber;
	[Export] private Control fillContainer;
	[Export] private ColorRect fillRect;
	[Export] private Color normalFillColor;
	[Export] private Color highlightedFillColor;
	[Export] private Vector2 grabberOffset;

	public override void _Process(double delta)
	{
		UpdateGrabber();
		UpdateFill();
	}

	private void UpdateGrabber()
	{
		float xGrabberPos = slider.GlobalPosition.X + slider.Size.X / (100 / (float)slider.Value);
		Vector2 pos = new Vector2(xGrabberPos, grabber.GlobalPosition.Y) + grabberOffset;
		grabber.GlobalPosition = pos;
	}

	private void UpdateFill()
	{
		float xFillSize = slider.Size.X / (100 / (float)slider.Value);
		Vector2 size = new Vector2(xFillSize, slider.Size.Y);
		fillRect.Size = size;
	}

	private void OnLineEditValueSubmitted(string newText)
	{
		if (!int.TryParse(newText, out int result)) return;
		
		slider.Value = result;
		
		SetVolume(result);
	}

	private void OnSliderValueChanged(float value)
	{
		lineEdit.Text = value.ToString();
		
		SetVolume(value);
	}

	private void SetVolume(float volume)
	{
		float dbValue = Mathf.LinearToDb(volume * 0.01f);

		int busIndex = volumeType switch
		{
			VolumeType.Main => AudioServer.GetBusIndex("Master"),
			VolumeType.Music => AudioServer.GetBusIndex("Music"),
			VolumeType.Sfx => AudioServer.GetBusIndex("Sfx"),
			_ => AudioServer.GetBusIndex("Master")
		};

		AudioServer.SetBusVolumeDb(busIndex, dbValue);
	}

	private void OnMouseEntered()
	{
		fillContainer.Modulate = highlightedFillColor;
	}

	private void OnMouseExited()
	{
		fillContainer.Modulate = normalFillColor;
	}
}
