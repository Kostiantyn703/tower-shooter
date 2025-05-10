using Godot;
using System;

class CharacterState
{
	public virtual string GetName() { return "none"; }
}

class IdleState : CharacterState
{
	public override string GetName() { return "idle"; }
}

class MoveState : CharacterState
{
	public override string GetName() { return "move"; }
}

class AirState : CharacterState
{
	public override string GetName() { return "air"; }
}

class CharacterStateMachine
{
	public void HandleState(Vector2 velocity, Vector2 direction)
	{
		if (velocity.Length() == 0 && direction.Length() == 0)
		{
			State = new IdleState();
		}
		else if (velocity.X != 0 && direction.X != 0 && velocity.Y == 0)
		{
			State = new MoveState();
		}
		else if (velocity.Y != 0 )
		{
			State = new AirState();
		}
	}
	
	public void LogCurrentState()
	{
		GD.Print(State.GetName());
	}
	
	CharacterState State = null;
}

class AnimationHandler
{

	
	public void HandleAnimation(ref AnimatedSprite2D move, ref AnimatedSprite2D idle)
	{
		
	}
	
}
//class HoldState : CharacterState
//{
	//
//}

public partial class MainCharacter : CharacterBody2D
{
	[Export]
	public int Speed { get; set; } = 350;
	[Export]
	public int JumpVelocity { get; set; } = -450;
	
	private bool IsHold = false;
	private Vector2 InputDirection = Vector2.Zero;
	
	CharacterStateMachine StateMachine = null;
	AnimationHandler AnimHandler = null;
	
	private enum AimDirection
	{
		AD_UP,
		AD_DOWN,
		AD_LEFT,
		AD_LEFT_45,		// left up
		AD_LEFT_135,	// left down
		AD_RIGHT,
		AD_RIGHT_45,	// right up
		AD_RIGHT_135,	// right down
		AD_NONE
	}
	
	private AimDirection AimDir = AimDirection.AD_NONE;
	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	
	public override void _Ready()
	{
		AimDir = AimDirection.AD_RIGHT;
		StateMachine = new CharacterStateMachine();
		AnimHandler = new AnimationHandler();
	}
	
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		
		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity.Y += gravity * (float)delta;
		}
		
		ProcessInput(ref velocity);
		Velocity = velocity;
		StateMachine.HandleState(Velocity, InputDirection);
		StateMachine.LogCurrentState();
		
		MoveAndSlide();
		GetAndHandleAnimation();
		
	}
	
	private void ProcessInput(ref Vector2 velocity)
	{
		InputDirection = Input.GetVector("move_left", "move_right", "up", "down");
		
		//GD.Print(InputDirection.X + " " + InputDirection.Y);
		
		if (InputDirection.X != 0)
		{
			velocity.X = InputDirection.X * Speed; 
		}
		else if (InputDirection.X == 0)
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		}
		
		if (Input.IsActionJustPressed("jump"))
		{
			if (IsOnFloor())
			{
				velocity.Y = JumpVelocity;
			}
		}
		
		//if (IsOnFloor())
		//{
			//if (InputDirection.Y == 1) {
				//if (!IsHold)	State = CharacterState.CS_LAY_DOWN;
				//else State = CharacterState.CS_AIM_DOWN;
			//}
			//else if (InputDirection.Y == -1)
			//{
				//State = CharacterState.CS_AIM_UP;
			//}
		//}
		//if (Input.IsActionPressed("hold")) {
			//IsHold = true;
		//}
		//else if (Input.IsActionJustReleased("hold"))
		//{
			//IsHold = false;
		//} 
		//
		//
		//if (InputDirection == Vector2.Zero && IsOnFloor() || (IsHold && InputDirection.X != 0)) {
			//State = CharacterState.CS_IDLE;
		//}
		//
		//if (InputDirection.X != 0 && !IsHold)
		//{
			//velocity.X = InputDirection.X * Speed;
			//if (IsOnFloor())
			//{
				//State = CharacterState.CS_RUN;
			//}
			//else
			//{
				//State = CharacterState.CS_AIR;
			//} 
		//}
		//else if (InputDirection.X == 0)
		//{
			//velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		//}
		//
		//if (IsOnFloor())
		//{
			//if (InputDirection.Y == 1) {
				//if (!IsHold)	State = CharacterState.CS_LAY_DOWN;
				//else State = CharacterState.CS_AIM_DOWN;
			//}
			//else if (InputDirection.Y == -1)
			//{
				//State = CharacterState.CS_AIM_UP;
			//}
		//}
		//
		
		//GD.Print(Velocity.X + " " + Velocity.Y);
	}
	
	private void GetAndHandleAnimation()
	{
		AnimatedSprite2D moveAnim = GetNode<AnimatedSprite2D>("MovementAnim");
		AnimatedSprite2D idleAnim = GetNode<AnimatedSprite2D>("IdleAnim");
		AnimHandler.HandleAnimation(ref moveAnim, ref idleAnim);
		
		if (Velocity.Length() > 0) {
			moveAnim.Play();
		}
		else
		{
			moveAnim.Stop();
		}
		
		
		
		//if (IsIdleState())
		//{
			//moveAnim.Visible = false;
			//idleAnim.Visible = true;
		//}
		//else
		//{
			//moveAnim.Visible = true;
			//idleAnim.Visible = false;
		//}
		
		//switch (State) {
			//case CharacterState.CS_IDLE:
				//idleAnim.Animation = "idle";
				//break;
			//case CharacterState.CS_AIM_UP:
				//idleAnim.Animation = "look_up";
				//break;
			//case CharacterState.CS_AIM_DOWN:
				//idleAnim.Animation = "aim_down";
				//break;
			//case CharacterState.CS_LAY_DOWN:
				//moveAnim.Animation = "lay_down";
				//break;
			//case CharacterState.CS_RUN:
				//moveAnim.Animation = "run";
				//break;
			//case CharacterState.CS_AIR:
				//moveAnim.Animation = "air";
				//break;
		//}
		//
		//if (InputDirection.X != 0)
		//{
			//moveAnim.FlipH = InputDirection.X < 0;
			//idleAnim.FlipH = InputDirection.X < 0;
		//}
		
	}
}
