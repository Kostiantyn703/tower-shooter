using Godot;
using System;

public partial class Sandbox : Node2D
{
    float BackgroundLowBound = 300;
    float HeightCounter = 0;
    float LowestBorder = 0;

    Camera2D Camera = null;
    Sprite2D Background = null;

    CharacterBody2D Player = null;

    [Signal]
    public delegate void TeleportEventHandler();

    private void OnTeleportBodyEntered(Node2D body)
    {
        Vector2 teleportPos = GetNode<BlocksChunk>("BlocksChunk").PlayerStart;
        Player.Position = teleportPos;
        GD.Print("Enter teleport");
    }

    public override void _Ready()
	{
		Player = GetNode<CharacterBody2D>("MainCharacter");
        Vector2 StartPosition = GetNode<Node2D>("InitialPlayerPosition").Position;

		Player.Position = StartPosition;
		Camera = Player.GetNode<Camera2D>("Camera");
        Background = GetNode<Sprite2D>("Background");

        GetNode<Area2D>("Teleport").BodyEntered += OnTeleportBodyEntered;
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
