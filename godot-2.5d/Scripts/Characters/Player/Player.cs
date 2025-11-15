using Godot;
using System;

public partial class Player : CharacterBody3D
{
	[Export] private AnimationPlayer animPlayerNode;

	private Vector2 direction = new();
	
	public override void _Ready()
	{
		animPlayerNode.Play(GConstants.ANIM_IDLE);
	}

	public override void _PhysicsProcess(double delta)
	{
		Velocity = new(direction.X, 0, direction.Y) ;
		Velocity *= 5;


		MoveAndSlide();
		Flip();
	}

	public override void _Input(InputEvent @event)
	{
		
		direction = Input.GetVector(GConstants.MOVE_LEFT, GConstants.MOVE_RIGHT, GConstants.MOVE_FORWARD, GConstants.MOVE_BACKWARD);
		if (direction == Vector2.Zero)
		{
			animPlayerNode.Play(GConstants.ANIM_IDLE);
		}
		else
		{
			animPlayerNode.Play(GConstants.ANIM_WALK);
		}
	}

	private void Flip()
	{
		if (direction == Vector2.Zero)
			{
				return;
			}
		Vector3 lookDir = new(direction.X,0,direction.Y);

		float targetAngle = Mathf.Atan2(lookDir.X, lookDir.Z);

		Rotation = new Vector3(Rotation.X, targetAngle, Rotation.Z);
	}
	
}
