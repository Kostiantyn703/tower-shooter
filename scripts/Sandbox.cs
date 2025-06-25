using Godot;
using System;

public partial class Sandbox : Node2D
{
    float LowestBorder = 300;

    Camera2D Camera = null;
    Sprite2D Background = null;

    CharacterBody2D Player = null;

    public override void _Ready()
	{
		Player = GetNode<CharacterBody2D>("MainCharacter");
        Vector2 StartPosition = GetNode<Node2D>("InitialPlayerPosition").Position;

		Player.Position = StartPosition;
		Camera = Player.GetNode<Camera2D>("Camera");
        Background = GetNode<Sprite2D>("Background");
    }

    public override void _Process(double delta)
    {
		Background.GlobalPosition = Camera.GlobalPosition;
        if (Camera.GlobalPosition.Y > LowestBorder)
        {
            Vector2 lowest = new Vector2(Camera.GlobalPosition.X, LowestBorder);
            Camera.GlobalPosition = lowest;
        }
    }
}
