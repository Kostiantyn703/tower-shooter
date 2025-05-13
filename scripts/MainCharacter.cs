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
	public AnimatedSprite2D CurrentSprite = null;
	
	public void Init(AnimatedSprite2D sprite)
	{
		CurrentSprite = sprite;
	}
	
	public void OnStateChange(Node owner, CharacterState state, AimDirection direction)
	{
		state.PrepareAnimation(owner, ref CurrentSprite, direction);
		state.FlipAnimation(ref CurrentSprite, direction);
	}
	
	public void OnAimDirectionChange(CharacterState state, AimDirection direction)
	{
		state.FlipAnimation(ref CurrentSprite, direction);
	}
	
	public void HandleAnimation(CharacterState state, AimDirection direction)
	{
		state.ProcessAnimation(CurrentSprite, direction);
	}
}

class CharacterStateMachine
{
	public CharacterState CurrentState = null;
	public AimDirection AimDir = AimDirection.AD_RIGHT;
	
	public void Init()
	{
		CurrentState = new IdleState();
	}
	
	public bool ChangeState(Vector2 velocity, Vector2 direction, bool isHold, bool isOnFloor)
	{
		CharacterState newState = null;
		if (isHold && isOnFloor)
		{
            newState = new HoldState();
        }
		else if (isOnFloor && velocity.Length() == 0 && direction.Length() == 0)
		{
			newState = new IdleState();
		}
		else if (velocity.X != 0 && direction.X != 0 && velocity.Y == 0)
		{
			newState = new MoveState();
		}
		else if (!isOnFloor && velocity.Y != 0 )
		{
			newState = new AirState();
		}

		if (newState != null && CurrentState.GetName() != newState.GetName()) {
			CurrentState = newState;
			LogCurrentState();
			return true;
		}
		return false;
	}
	
	public bool ChangeAimDirection(Vector2 direction)
	{
		AimDirection newAimDir = AimDirection.AD_NONE;
		
		if (direction.X > 0 && direction.Y == 0)
		{
			newAimDir = AimDirection.AD_RIGHT; 
		}
		if (direction.X < 0 && direction.Y == 0)
		{
			newAimDir = AimDirection.AD_LEFT;
		}
		if (direction.X > 0 && direction.Y < 0)
		{
			newAimDir = AimDirection.AD_RIGHT_45;
		}
		if (direction.X < 0 && direction.Y < 0)
		{
			newAimDir = AimDirection.AD_LEFT_45;
		}
		if (direction.X > 0 && direction.Y > 0)
		{
			newAimDir = AimDirection.AD_RIGHT_135;
		}
		if (direction.X < 0 && direction.Y > 0)
		{
			newAimDir = AimDirection.AD_LEFT_135;
		}
		if (direction.X == 0 && direction.Y < 0)
		{
			newAimDir = AimDirection.AD_UP;
		}
		if (direction.X == 0 && direction.Y > 0)
		{
			newAimDir = AimDirection.AD_DOWN;
		}
		
		if (AimDir != newAimDir)
		{
			AimDir = newAimDir;
			return true;
		}
		return false;
	}
	
	public void LogCurrentState()
	{
		GD.Print(CurrentState.GetName());
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
	
	protected bool IsAimUp(AimDirection direction)
	{
		return direction == AimDirection.AD_UP;
	}
	
	protected bool IsAimDown(AimDirection direction)
	{
		return direction == AimDirection.AD_DOWN;
	}
	
	public abstract string GetName();
	
	public void FlipAnimation(ref AnimatedSprite2D sprite, AimDirection direction)
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
	
	public abstract void PrepareAnimation(Node owner, ref AnimatedSprite2D sprite, AimDirection direction);
	
	public abstract void ProcessAnimation(AnimatedSprite2D sprite, AimDirection direction);
	
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
		sprite = SwitchSprite(owner, "MovementAnim", "IdleAnim");
	}
	
	public override void ProcessAnimation(AnimatedSprite2D sprite, AimDirection direction)
	{
		if (IsAimUp(direction))
		{
			sprite.Animation = "aim_up";
		}
		else if (IsAimDown(direction))
		{
			sprite.Animation = "lay_down";
		}
		else
		{
			sprite.Animation = "idle";
		}
	}
}

class HoldState : CharacterState
{
	public override string GetName() { return "hold"; }

    public override void PrepareAnimation(Node owner, ref AnimatedSprite2D sprite, AimDirection direction)
    {
        sprite = SwitchSprite(owner, "MovementAnim", "IdleAnim");
    }

    public override void ProcessAnimation(AnimatedSprite2D sprite, AimDirection direction)
    {
        if (IsAimUp(direction))
        {
            sprite.Animation = "aim_up";
        }
        else if (IsAimDown(direction))
        {
            sprite.Animation = "aim_down";
        }
        else if (IsAimRight45(direction) || IsAimLeft45(direction))
        {
            sprite.Animation = "aim_45";
        }
        else if (IsAimRight135(direction) || IsAimLeft135(direction))
        {
            sprite.Animation = "aim_135";
        }
        else
        {
            sprite.Animation = "idle";
        }

        sprite.Play();
    }
}

class MoveState : CharacterState
{
	public override string GetName() { return "move"; }
	
	public override void PrepareAnimation(Node owner, ref AnimatedSprite2D sprite, AimDirection direction)
	{
		sprite = SwitchSprite(owner, "IdleAnim", "MovementAnim");
	}
	
	public override void ProcessAnimation(AnimatedSprite2D sprite, AimDirection direction)
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
		sprite = SwitchSprite(owner, "IdleAnim", "MovementAnim");;
	}
	
	public override void ProcessAnimation(AnimatedSprite2D sprite, AimDirection direction)
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
		StateMachine.Init();
		AnimHandler = new AnimationHandler();
		AnimatedSprite2D sprite = GetNode<AnimatedSprite2D>("MovementAnim");
		AnimHandler.Init(sprite);
		PlatformOnLeave = PlatformOnLeaveEnum.DoNothing;
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
		if (StateMachine.ChangeState(Velocity, InputDirection, IsHold, IsOnFloor()))
		{
			AnimHandler.OnStateChange(this, StateMachine.CurrentState, StateMachine.AimDir);
			AnimHandler.HandleAnimation(StateMachine.CurrentState, StateMachine.AimDir);
		}
		if (StateMachine.ChangeAimDirection(InputDirection))
		{
			AnimHandler.OnAimDirectionChange(StateMachine.CurrentState, StateMachine.AimDir);
			AnimHandler.HandleAnimation(StateMachine.CurrentState, StateMachine.AimDir);
		}
		MoveAndSlide();
    }
	
	private void ProcessInput(ref Vector2 velocity)
	{
		InputDirection = Input.GetVector("move_left", "move_right", "up", "down");
        if (Input.IsActionJustPressed("hold"))
        {
            IsHold = true;

        }
        if (Input.IsActionJustReleased("hold"))
        {
            IsHold = false;
        }

		if (!IsHold)
		{
            if (InputDirection.X != 0)
            {
				FixInputDirection();
                velocity.X = InputDirection.X * Speed;
			}
            else if (InputDirection.X == 0)
            {
                velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
            }
        }
		if (IsHold && IsOnFloor())
		{
			velocity.X = 0;
		}
		
		if (Input.IsActionJustPressed("jump"))
		{
			if (IsOnFloor())
			{
				velocity.Y = JumpVelocity;
			}
		}
	}

	private void FixInputDirection()
	{
		if (IsOnWall()) return;

        if (InputDirection.X > 0 && InputDirection.X != 1)
        {
            InputDirection.X = 1;
        }
        else if (InputDirection.X < 0 && InputDirection.X != -1)
        {
            InputDirection.X = -1;
        }
    }
}
