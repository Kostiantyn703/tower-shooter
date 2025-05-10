using Godot;
using System;

class CharacterState
{
	public virtual string GetName() { return "none"; }
	public virtual void PrepareAnimation(Node owner, ref AnimatedSprite2D sprite) {}
	public virtual void ProcessAnimation(ref AnimatedSprite2D sprite) {}
}

class IdleState : CharacterState
{
	public override string GetName() { return "idle"; }
	
	public override void PrepareAnimation(Node owner, ref AnimatedSprite2D sprite)
	{
		owner.GetNode<AnimatedSprite2D>("MovementAnim").Visible = false;
		AnimatedSprite2D idleAnim = owner.GetNode<AnimatedSprite2D>("IdleAnim");
		idleAnim.Visible = true;
		sprite = idleAnim;
	}
	
	public override void ProcessAnimation(ref AnimatedSprite2D sprite)
	{
		sprite.Animation = "idle";
		sprite.Stop();
	}
}

class MoveState : CharacterState
{
	public override string GetName() { return "move"; }
	
	public override void PrepareAnimation(Node owner, ref AnimatedSprite2D sprite)
	{
		owner.GetNode<AnimatedSprite2D>("IdleAnim").Visible = false;
		AnimatedSprite2D moveAnim = owner.GetNode<AnimatedSprite2D>("MovementAnim");
		moveAnim.Visible = true;
		sprite = moveAnim;
	}
	
	public override void ProcessAnimation(ref AnimatedSprite2D sprite)
	{
		sprite.Animation = "run";
		sprite.Play();
	}
}

class AirState : CharacterState
{
	public override string GetName() { return "air"; }
	
	public override void PrepareAnimation(Node owner, ref AnimatedSprite2D sprite)
	{
		owner.GetNode<AnimatedSprite2D>("IdleAnim").Visible = false;
		AnimatedSprite2D moveAnim = owner.GetNode<AnimatedSprite2D>("MovementAnim");
		moveAnim.Visible = true;
		sprite = moveAnim;
	}
	
	public override void ProcessAnimation(ref AnimatedSprite2D sprite)
	{
		sprite.Animation = "air";
		sprite.Play();
	}
}

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
		public AnimatedSprite2D CurrentSprite;
		
		public void Init(ref AnimatedSprite2D sprite)
		{
			CurrentSprite = sprite;
			//IdleAnim = GetNode<AnimatedSprite2D>("IdleAnim");
		}
		
		public void OnStateChange(Node owner, ref CharacterState state)
		{
			state.PrepareAnimation(owner, ref CurrentSprite);
		}
		
		public void HandleAnimation(ref CharacterState state)
		{
			state.ProcessAnimation(ref CurrentSprite);
		}
		
	}

	class CharacterStateMachine
	{
		public CharacterState State = null;
		
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
	}
//class HoldState : CharacterState
//{
	//
//}
	
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
		AnimHandler.OnStateChange(this, ref StateMachine.State);
		AnimHandler.HandleAnimation(ref StateMachine.State);
		
		MoveAndSlide();
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
}
