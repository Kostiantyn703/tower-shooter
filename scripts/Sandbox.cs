using Godot;
using System;

public class PlatformSpawner
{
	public float HighestPlatform = 0;
	float SpawnOffset = 160;
	float ApproachDistance = 300;
	Node PlatformLayer = null;

	public void Init(Node platformLayer)
	{
		PlatformLayer = platformLayer;
        HighestPlatform = PlatformLayer.GetNode<Node2D>("Platform5").Position.Y;
	}
	public void SpawnPlatform(Vector2 position)
	{
		PackedScene platformScene = GD.Load<PackedScene>("res://scenes//platform_big.tscn");
		Node2D platformInstance = (Node2D)platformScene.Instantiate();
		platformInstance.Position = position;
		PlatformLayer.AddChild(platformInstance);

		HighestPlatform = position.Y;
	}

	public Vector2 CalculateSpawnPosition(float playerX)
	{
		return new Vector2(playerX, HighestPlatform - SpawnOffset);
	}

	public bool ShouldSpawnPlatform(float playerY)
	{
		bool result = false;
		if (playerY < HighestPlatform + ApproachDistance)
		{
			result = true;
		}
		return result;
	}

}

public partial class Sandbox : Node
{
    float BackgroundLowBound = 300;
    float HeightCounter = 0;
	float LowestBorder = 0;

    Camera2D Camera = null;
	Sprite2D Background = null;

	CharacterBody2D Player = null;
	PlatformSpawner Platforms = null;
    public override void _Ready()
	{
		Player = GetNode<CharacterBody2D>("MainCharacter");
        Vector2 StartPosition = GetNode<Node2D>("InitialPlayerPosition").Position;

		Player.Position = StartPosition;
		Player.GetNode<Label>("HeightLabel").Text = Convert.ToString(HeightCounter);
		Camera = Player.GetNode<Camera2D>("Camera");
        Background = GetNode<Sprite2D>("Background");

        LowestBorder = GetNode<Node>("Boundaries").GetNode<StaticBody2D>("Ground").Position.Y;

        Platforms = new PlatformSpawner();
		Platforms.Init(GetNode<Node>("Platforms"));
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
		if (Platforms.ShouldSpawnPlatform(Player.Position.Y))
		{
			Vector2 spawnPosition = Platforms.CalculateSpawnPosition(Player.Position.X);
			Platforms.SpawnPlatform(spawnPosition);
        }
    }

	private void UpdateHeightCounter()
	{
		HeightCounter = (LowestBorder - Player.Position.Y);

		string heightText = String.Format("{0:N2}", HeightCounter / 20);
		Player.GetNode<Label>("HeightLabel").Text = heightText;
    }
}
