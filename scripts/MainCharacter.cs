using Godot;
using System;

public enum AimDirection
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

class AnimationHandler
{
	public AnimatedSprite2D CurrentSprite;
	
	public void Init(ref AnimatedSprite2D sprite)
	{
		CurrentSprite = sprite;
	}
	
	public void OnStateChange(Node owner, ref CharacterState state, AimDirection direction)
	{
		state.PrepareAnimation(owner, ref CurrentSprite, direction);
	}
	
	public void HandleAnimation(ref CharacterState state, AimDirection direction)
	{
		state.ProcessAnimation(ref CurrentSprite, direction);
	}
}

class CharacterStateMachine
{
	public CharacterState State = null;
	public AimDirection AimDir = AimDirection.AD_RIGHT;
	
	public void SetState(Vector2 velocity, Vector2 direction)
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
	
	public void SetAimDirection(Vector2 direction)
	{
		if (direction.X > 0 && direction.Y == 0)
		{
			AimDir = AimDirection.AD_RIGHT; 
		}
		if (direction.X < 0 && direction.Y == 0)
		{
			AimDir = AimDirection.AD_LEFT;
		}
		if (direction.X > 0 && direction.Y < 0)
		{
			AimDir = AimDirection.AD_RIGHT_45;
		}
		if (direction.X < 0 && direction.Y < 0)
		{
			AimDir = AimDirection.AD_LEFT_45;
		}
		if (direction.X > 0 && direction.Y > 0)
		{
			AimDir = AimDirection.AD_RIGHT_135;
		}
		if (direction.X < 0 && direction.Y > 0)
		{
			AimDir = AimDirection.AD_LEFT_135;
		}
	}
	
	public void LogCurrentState()
	{
		GD.Print(State.GetName());
	}
}

abstract class CharacterState
{
	protected bool IsAimRight(AimDirection direction)
	{
		return direction == AimDirection.AD_RIGHT;
	}
	
	protected bool IsAimRight45(AimDirection direction)
	{
		return direction == AimDirection.AD_RIGHT_45;
	}
	
	protected bool IsAimRight135(AimDirection direction)
	{
		return direction == AimDirection.AD_RIGHT_135;
	}
	
	private bool IsAimAllRight(AimDirection direction)
	{
		return IsAimRight(direction) || IsAimRight45(direction) || IsAimRight135(direction);
	}
	
	protected bool IsAimLeft(AimDirection direction)
	{
		return direction == AimDirection.AD_LEFT;
	}
	
	protected bool IsAimLeft45(AimDirection direction)
	{
		return direction == AimDirection.AD_LEFT_45;
	}
	
	protected bool IsAimLeft135(AimDirection direction)
	{
		return direction == AimDirection.AD_LEFT_135;
	}
	
	private bool IsAimAllLeft(AimDirection direction)
	{
		return IsAimLeft(direction) || IsAimLeft45(direction) || IsAimLeft135(direction);
	}
	
	public abstract string GetName();
	
	public virtual void PrepareAnimation(Node owner, ref AnimatedSprite2D sprite, AimDirection direction)
	{
		if (IsAimAllRight(direction))
		{
			sprite.FlipH = false;
		}
		else if (IsAimAllLeft(direction))
		{
			sprite.FlipH = true;
		}
	}
	
	public abstract void ProcessAnimation(ref AnimatedSprite2D sprite, AimDirection direction);
	
	protected AnimatedSprite2D SwitchSprite(Node owner, string spriteNameToHide, string spriteNameToShow)
	{
		owner.GetNode<AnimatedSprite2D>(spriteNameToHide).Visible = false;
		AnimatedSprite2D resultSprite = owner.GetNode<AnimatedSprite2D>(spriteNameToShow);
		resultSprite.Visible = true;
		return resultSprite;
	}
}

class IdleState : CharacterState
{
	public override string GetName() { return "idle"; }
	
	public override void PrepareAnimation(Node owner, ref AnimatedSprite2D sprite, AimDirection direction)
	{
		base.PrepareAnimation(owner, ref sprite, direction);
		sprite = SwitchSprite(owner, "MovementAnim", "IdleAnim");
	}
	
	public override void ProcessAnimation(ref AnimatedSprite2D sprite, AimDirection direction)
	{
		sprite.Animation = "idle";
	}
}

class MoveState : CharacterState
{
	public override string GetName() { return "move"; }
	
	public override void PrepareAnimation(Node owner, ref AnimatedSprite2D sprite, AimDirection direction)
	{
		base.PrepareAnimation(owner, ref sprite, direction);
		sprite = SwitchSprite(owner, "IdleAnim", "MovementAnim");
	}
	
	public override void ProcessAnimation(ref AnimatedSprite2D sprite, AimDirection direction)
	{
		if (IsAimRight45(direction) || IsAimLeft45(direction))
		{
			sprite.Animation = "run_aim_45";
		}
		else if (IsAimRight135(direction) || IsAimLeft135(direction))
		{
			sprite.Animation = "run_aim_135";
		}
		else
		{
			sprite.Animation = "run";
		}
		
		sprite.Play();
	}
}

class AirState : CharacterState
{
	public override string GetName() { return "air"; }
	
	public override void PrepareAnimation(Node owner, ref AnimatedSprite2D sprite, AimDirection direction)
	{
		base.PrepareAnimation(owner, ref sprite, direction);
		sprite = SwitchSprite(owner, "IdleAnim", "MovementAnim");;
	}
	
	public override void ProcessAnimation(ref AnimatedSprite2D sprite, AimDirection direction)
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
	
	public override void _Ready()
	{
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
		StateMachine.SetState(Velocity, InputDirection);
		StateMachine.SetAimDirection(InputDirection);
		StateMachine.LogCurrentState();
		AnimHandler.OnStateChange(this, ref StateMachine.State, StateMachine.AimDir);
		AnimHandler.HandleAnimation(ref StateMachine.State, StateMachine.AimDir);
		
		MoveAndSlide();
	}
	
	private void ProcessInput(ref Vector2 velocity)
	{
		InputDirection = Input.GetVector("move_left", "move_right", "up", "down");
		
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
