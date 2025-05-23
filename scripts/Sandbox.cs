using Godot;
using System;

public partial class Sandbox : Node
{
	Camera2D Camera = null;
	Sprite2D Background = null;

	float BackgroundLowBound = 300;
	float HeightCounter = 0;
    public override void _Ready()
	{
		Vector2 StartPosition = GetNode<Node2D>("InitialPlayerPosition").Position;
		
		CharacterBody2D player = GetNode<CharacterBody2D>("MainCharacter");
		player.Position = StartPosition;
		player.GetNode<Label>("HeightLabel").Text = Convert.ToString(HeightCounter);
		Camera = player.GetNode<Camera2D>("Camera");
        Background = GetNode<Sprite2D>("Background");

		
    }

    public override void _Process(double delta)
    {
		Vector2 backgroundGlobalPos = Background.GlobalPosition;
        
		if (Camera.GlobalPosition.Y > BackgroundLowBound)
		{
            backgroundGlobalPos.Y = BackgroundLowBound;
        }
		else
		{
            backgroundGlobalPos.Y = Camera.GlobalPosition.Y;
        }
		Background.GlobalPosition = backgroundGlobalPos;

		UpdateHeightCounter();
    }

	private void UpdateHeightCounter()
	{
        CharacterBody2D player = GetNode<CharacterBody2D>("MainCharacter");
		float lowestHeight = GetNode<Node>("Boundaries").GetNode<StaticBody2D>("Ground").Position.Y;


		HeightCounter = (lowestHeight - player.Position.Y) / 20;

		string heightText = String.Format("{0:N2}", HeightCounter);
		player.GetNode<Label>("HeightLabel").Text = heightText;
    }
}
