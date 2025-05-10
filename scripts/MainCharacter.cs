using Godot;
using System;

class CharacterState
{
	public virtual string GetName() { return "none"; }
	public virtual void ProcessAnimation(ref AnimatedSprite2D sprite) {}
}

class IdleState : CharacterState
{
	public override string GetName() { return "idle"; }
	public override void ProcessAnimation(ref AnimatedSprite2D sprite)
	{
		sprite.Animation = "air";
		sprite.Stop();
	}
}

class MoveState : CharacterState
{
	public override string GetName() { return "move"; }
	public override void ProcessAnimation(ref AnimatedSprite2D sprite)
	{
		sprite.Animation = "run";
		sprite.Play();
	}
}

class AirState : CharacterState
{
	public override string GetName() { return "air"; }
	public override void ProcessAnimation(ref AnimatedSprite2D sprite)
	{
		sprite.Animation = "air";
		sprite.Play();
	}
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
	
	public CharacterState State = null;
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
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
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
	
	
	class AnimationHandler
	{
		AnimatedSprite2D CurrentSprite;
		
		public void Init(ref AnimatedSprite2D sprite)
		{
			CurrentSprite = sprite;
			//IdleAnim = GetNode<AnimatedSprite2D>("IdleAnim");
		}
		
		public void OnStateChange(ref CharacterState state)
		{
			
			
			
		}
		
		public void HandleAnimation(ref CharacterState state)
		{
			state.ProcessAnimation(ref CurrentSprite);
		}
		
	}
	
	public override void _Ready()
	{
		AimDir = AimDirection.AD_RIGHT;
		StateMachine = new CharacterStateMachine();
		AnimHandler = new AnimationHandler();
		AnimatedSprite2D sprite = GetNode<AnimatedSprite2D>("MovementAnim");
		AnimHandler.Init(ref sprite);
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
		AnimHandler.OnStateChange(ref StateMachine.State);
		AnimHandler.HandleAnimation(ref StateMachine.State);
		
		MoveAndSlide();
		//GetAndHandleAnimation();
		
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
	}
	
	private void GetAndHandleAnimation()
	{
		
		//if (InputDirection.X != 0)
		//{
			//moveAnim.FlipH = InputDirection.X < 0;
			//idleAnim.FlipH = InputDirection.X < 0;
		//}
		//
	
		
		
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
		
		
	}
}
