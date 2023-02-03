using Godot;
using System;

public class Player : KinematicBody2D
{
	[Export] private Vector2 MovementSpeed = Vector2.Zero;

	private AnimatedSprite _sprite;
	private Camera2D _camera;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_sprite = GetNode<AnimatedSprite>("AnimatedSprite");
		_camera = GetNode<Camera2D>("Camera2D");

		_camera.MakeCurrent();
	}

	public override void _PhysicsProcess(float deltaSeconds)
	{
		base._PhysicsProcess(deltaSeconds);

		float horizontal = Input.GetAxis("MoveLeft", "MoveRight");
		float vertical = Input.GetAxis("MoveUp", "MoveDown");

		Vector2 movement = new Vector2(horizontal, vertical) * MovementSpeed;
		Vector2 movementResult = MoveAndSlide(movement);
	}
}
