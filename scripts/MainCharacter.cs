using Godot;
using System;

public partial class MainCharacter : CharacterBody2D
{
	[Export]
	public int Speed { get; set; } = 350;
	[Export]
	public int JumpVelocity { get; set; } = -450;
	
	private bool IsHold = false;
	private Vector2 InputDirection = Vector2.Zero;
	
	private enum CharacterState 
	{
		CS_IDLE,
		CS_LAY_DOWN,
		CS_RUN,
		CS_AIR,
		CS_AIM_DOWN,
		CS_AIM_UP,
		CS_AIM_45,
		CS_AIM_135
	}
	
	CharacterState State = CharacterState.CS_IDLE;
	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	
	public override void _Ready()
	{
	}
	
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		
		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity.Y += gravity * (float)delta;
			State = CharacterState.CS_AIR;
		}
		
		ProcessInput(velocity);
		
		
		MoveAndSlide();
		HandleAnimation();
		
	}
	
	private void ProcessInput(Vector2 velocity)
	{
		InputDirection = Input.GetVector("move_left", "move_right", "up", "down");
		
		if (Input.IsActionPressed("hold")) {
			IsHold = true;
		}
		else if (Input.IsActionJustReleased("hold"))
		{
			IsHold = false;
		} 
		
		
		if (InputDirection == Vector2.Zero && IsOnFloor() || (IsHold && InputDirection.X != 0)) {
			State = CharacterState.CS_IDLE;
		}
		
		if (InputDirection.X != 0 && !IsHold)
		{
			velocity.X = InputDirection.X * Speed;
			if (IsOnFloor())
			{
				State = CharacterState.CS_RUN;
			}
			else
			{
				State = CharacterState.CS_AIR;
			} 
		}
		else if (InputDirection.X == 0)
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		}
		
		if (IsOnFloor())
		{
			if (InputDirection.Y == 1) {
				if (!IsHold)	State = CharacterState.CS_LAY_DOWN;
				else State = CharacterState.CS_AIM_DOWN;
			}
			else if (InputDirection.Y == -1)
			{
				State = CharacterState.CS_AIM_UP;
			}
		}
		
		if (Input.IsActionJustPressed("jump"))
		{
			velocity.Y = JumpVelocity;
		}
		
		Velocity = velocity;
	}
	
	private void HandleAnimation()
	{
		AnimatedSprite2D moveAnim = GetNode<AnimatedSprite2D>("MovementAnim");
		if (Velocity.Length() > 0) {
			moveAnim.Play();
		}
		else
		{
			moveAnim.Stop();
		}
		
		AnimatedSprite2D idleAnim = GetNode<AnimatedSprite2D>("IdleAnim");
		
		if (IsIdleState())
		{
			moveAnim.Visible = false;
			idleAnim.Visible = true;
		}
		else
		{
			moveAnim.Visible = true;
			idleAnim.Visible = false;
		}
		
		switch (State) {
			case CharacterState.CS_IDLE:
				idleAnim.Animation = "idle";
				break;
			case CharacterState.CS_AIM_UP:
				idleAnim.Animation = "look_up";
				break;
			case CharacterState.CS_AIM_DOWN:
				idleAnim.Animation = "aim_down";
				break;
			case CharacterState.CS_LAY_DOWN:
				moveAnim.Animation = "lay_down";
				break;
			case CharacterState.CS_RUN:
				moveAnim.Animation = "run";
				break;
			case CharacterState.CS_AIR:
				moveAnim.Animation = "air";
				break;
		}
		
		if (InputDirection.X != 0)
		{
			moveAnim.FlipH = InputDirection.X < 0;
			idleAnim.FlipH = InputDirection.X < 0;
		}
		
	}
	
	private bool IsIdleState()
	{
		return State == CharacterState.CS_IDLE ||
				State == CharacterState.CS_AIM_DOWN ||
					State == CharacterState.CS_AIM_UP;
	}
	
}
