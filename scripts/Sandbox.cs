using Godot;
using System;

public partial class Sandbox : Node
{
	Camera2D Camera = null;
	Sprite2D Background = null;

	float BackgroundLowBound = 300;
    public override void _Ready()
	{
		Vector2 StartPosition = GetNode<Node2D>("InitialPlayerPosition").Position;
		
		CharacterBody2D player = GetNode<CharacterBody2D>("MainCharacter");
		player.Position = StartPosition;
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
    }
}
