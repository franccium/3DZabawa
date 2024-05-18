using System;
using System.Reflection.Metadata;
using Godot;

public partial class Player : CharacterBody3D
{
	enum States
	{
		Stand,
		Walk,
		Sprint,
		Crouch,
		Sneak,
		Jump,
		InAir,
		Aim,
		Cast
	}

	private States _currentState = States.Stand;

	[Export] public AnimationTree AnimationTree { get; set; }
	[Export] public string IdleAnimationName { get; set; }
	[Export] public string AimingAnimationName { get; set; }
	[Export] public string FireAnimationName { get; set; }
	[Export] public string AimingFireAnimationName { get; set; }
	[Export] public string ReloadAnimationName { get; set; }
	[Export] private WeaponEffectsController _gunEffects { get; set; }
	[Export] private SpellEffectsController _spellEffects { get; set; }
	[Export] public Node3D PlayerCamera { get; set; }

	private Node3D _arms;

	public const float BaseSpeed = 15.0f;
	public const float BaseJumpVelocity = 8f;
	public float CurrSpeed { get; set; } = 15f;

	private Camera3D _camera;
	public const float CameraSensitivity = 2.5f;

	private AnimationPlayer _animationPlayer;

	// public for smth like stealth later?
	public bool IsCrouched;
	public bool IsAiming;
	public bool IsCasting;
	private const float _crouchSpeed = 10f;
	private const float _aimSpeed = 12f;
	private const float _sprintSpeed = 30f;

	public bool IsFlashlightOn;

	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	private short _maxJumps = 5;
	private short _remainingJumps;

	public override void _Ready()
	{
		_camera = GetNode<Camera3D>("Camera3D");
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

		// Hides the cursor
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

    public override void _Process(double delta)
    {
        HandleAnimation();
    }

    public override void _PhysicsProcess(double delta)
	{
		Vector3 direction = GetInput();
		HandleMovement(direction, delta);
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		// Rotates the player model and the camera based on mouse movement
		if (@event is InputEventMouseMotion)
		{
			InputEventMouseMotion motion = @event as InputEventMouseMotion;
			Rotation = new Vector3(Rotation.X, Rotation.Y - motion.Relative.X * 0.001f * CameraSensitivity, Rotation.Z);
			// clamp to make it impossible to turn upside down
			_camera.Rotation = new Vector3(Mathf.Clamp(_camera.Rotation.X - motion.Relative.Y * 0.001f * CameraSensitivity, -2, 2), _camera.Rotation.Y, _camera.Rotation.Z);
		}
	}

	private void ToggleMouseMode()
	{
		if (Input.MouseMode == Input.MouseModeEnum.Captured)
			Input.MouseMode = Input.MouseModeEnum.Visible;
		else
			Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	/// <summary>
	/// gets input from the player and changes his state accordingly
	/// </summary>
	private Vector3 GetInput()
	{
		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_backwards");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

		if (Input.IsActionJustPressed("flashlight"))
			ToggleFlashlight();

		if (Input.IsActionPressed("crouch"))
			_currentState = States.Crouch;

        if (direction != Vector3.Zero)
        {
            if (Input.IsActionPressed("crouch"))
                _currentState = States.Sneak;
            if (Input.IsActionPressed("sprint"))
                _currentState = States.Sprint;
            else
                _currentState = States.Walk;
        }
        else
        {
            _currentState = States.Stand;
        }

        if (Input.IsActionJustPressed("jump") && _remainingJumps > 0)
		{
			_currentState = States.Jump;
			_remainingJumps--;
		}

        if (IsOnFloor())
        {
            _remainingJumps = _maxJumps;
        }
        else
        {
            _currentState = States.InAir;
            if (Input.IsActionJustPressed("jump") && _remainingJumps > 0)
            {
                _currentState = States.Jump;
                _remainingJumps--;
            }
        }

        if (Input.IsActionPressed("mouse_right"))
		{
			_currentState = States.Cast;
			IsCasting = true;
			//CastSpell();
		}
        else
        {
            IsCasting = false;
        }
        if (Input.IsActionPressed("mouse_left"))
        {
            FireWeapon();
		}

		if (Input.IsActionJustPressed("escape"))
			ToggleMouseMode();

		return direction;
	}

	private void FireWeapon()
	{
		if (!_gunEffects.HasAmmo)
		{
			IsAiming = false;
			ReloadWeapon();
			return;
		}
		_gunEffects.Fire();
	}

	private void CastSpell()
	{
		//_spellEffects.Cast();
	}

	private void ReloadWeapon()
	{
		_gunEffects.Reload();
	}

	private void ToggleFlashlight()
	{
		if (IsFlashlightOn)
			_animationPlayer.Play("hide_flashlight");
		else
			_animationPlayer.Play("show_flashlight");

		IsFlashlightOn = !IsFlashlightOn;
	}

	private void HandleMovement(Vector3 direction, double delta)
	{
		Vector3 velocity = Velocity;
		CurrSpeed = BaseSpeed;
		//GD.Print(CurrSpeed);

		if (_currentState == States.Sprint)
			CurrSpeed = _sprintSpeed;

		if (_currentState == States.Sneak)
		{
			CurrSpeed = _crouchSpeed;
			GD.Print("crouching");
		}

		//if (_currentState == States.Aim)
		//currSpeed = _aimSpeed;

		if (_currentState == States.Jump)
			velocity.Y = BaseJumpVelocity;

		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * CurrSpeed;
			velocity.Z = direction.Z * CurrSpeed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, CurrSpeed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, CurrSpeed);
		}

		if (_currentState == States.InAir)
			velocity.Y -= gravity * (float)delta;

		Velocity = velocity;
		MoveAndSlide();
	}

	private void HandleAnimation()
	{
		if (Input.IsActionPressed("crouch"))
		{
			if (!IsCrouched)
			{
				_animationPlayer.Play("crouch_animation");
				IsCrouched = true;
				GD.Print("coruched");
			}
		}
		else if (Input.IsActionJustReleased("crouch") && IsCrouched)
		{
			GD.Print("uncr");
			PhysicsDirectSpaceState3D spaceState = GetWorld3D().DirectSpaceState;
			var result = spaceState.IntersectRay(
				new PhysicsRayQueryParameters3D()
				{
					From = Position,
					To = new Vector3(Position.X, Position.Y + 2, Position.Z),
					Exclude = new Godot.Collections.Array<Godot.Rid> { this.GetRid() }
				});
			if (result.Count <= 0)
			{
				_animationPlayer.PlayBackwards("crouch_animation");
				_currentState = States.Walk;
				IsCrouched = false;
			}
		}
	}
}
