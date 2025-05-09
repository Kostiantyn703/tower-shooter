using Godot;
using System;

public partial class Sandbox : Node
{
	
	public override void _Ready()
	{
		Vector2 StartPosition = GetNode<Node2D>("InitialPlayerPosition").Position;
		
		CharacterBody2D player = GetNode<CharacterBody2D>("MainCharacter");
		player.Position = StartPosition;
	}
}
