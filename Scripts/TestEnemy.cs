using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class TestEnemy : CharacterBody3D
{
	public enum States
	{
		Patrol,
		Chase,
		Hunt,
		Wait
	}

	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	public const float BaseSpeed = 5.0f;
	public const float BaseJumpVelocity = 4.5f;

	public States CurrentState;

	[Export] private float _patrolSpeed = 2f;
	private Timer _patrolTimer;
	[Export] private float _chaseSpeed = 6f;

	public NavigationAgent3D NavigationAgent;
	private List<Marker3D> _waypoints = new List<Marker3D>();
	private int _waypointIndex;

	private Vector3 _targetPosition = new Vector3();
	private Vector3 _lastSightDirection;

	private Player _player;
	private Camera3D _playerCamera;

	public override void _Ready()
	{
		NavigationAgent = GetNode<NavigationAgent3D>("NavigationAgent3D");
		CurrentState = States.Patrol;
		_patrolTimer = GetNode<Timer>("PatrolTimer");
		_player = GetTree().GetNodesInGroup("Player")[0] as Player;
		_playerCamera = _player.GetNode<Camera3D>("Camera3D");

		_waypoints = GetTree().GetNodesInGroup("EnemyWaypoint").Select(saar => saar as Marker3D).ToList();
		NavigationAgent.TargetPosition = _waypoints[0].GlobalPosition;
	}

	public override void _Process(double delta)
	{
		switch (CurrentState)
		{
			case States.Patrol:
				if(NavigationAgent.IsNavigationFinished())
				{
					_patrolTimer.Start();
					CurrentState = States.Wait;
					return;
				}

				MoveToTargetPosition(delta, _patrolSpeed);
				CheckForPlayer();

				break;

			case States.Chase:
				if (NavigationAgent.IsNavigationFinished())
				{
					//GD.Print("attacking");
					CurrentState = States.Wait;
					_patrolTimer.Start();
					return;
				}

				NavigationAgent.TargetPosition = _player.GlobalPosition;
				MoveToTargetPosition(delta, _chaseSpeed);

				break;

			case States.Hunt:
				if(NavigationAgent.IsNavigationFinished())
				{
					CurrentState = States.Wait;
					_patrolTimer.Start();
					return;
				}

				MoveToTargetPosition(delta, _patrolSpeed);
				
				break;

			case States.Wait:
				break;
			default:
				break;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		switch (CurrentState)
		{
			case States.Patrol:
				
				break;
			case States.Chase:
				break;
			case States.Hunt:
				break;
			case States.Wait:
				break;
			default:
				break;
		}

		if (!IsOnFloor())
		{
			velocity.Y -= gravity * (float)delta;
		}
	}

	private void MoveToTargetPosition(double delta, float speed)
	{
		Vector3 targetPos = NavigationAgent.GetNextPathPosition();
		Vector3 direction = GlobalPosition.DirectionTo(targetPos);
		Vector3 sightDirection = _lastSightDirection.Lerp(targetPos, 0.2f);
		LookAt(new Vector3(sightDirection.X, GlobalPosition.Y, sightDirection.Z), Vector3.Up);
		_lastSightDirection = sightDirection;
		Velocity = direction * speed;
		MoveAndSlide();

		if (_playerInHearingFar)
			CheckForPlayer();
	}

	private void CheckForPlayer()
	{
		PhysicsDirectSpaceState3D spaceState3D = GetWorld3D().DirectSpaceState;
		var result = spaceState3D.IntersectRay(new PhysicsRayQueryParameters3D() 
		{
			From=GetNode<Marker3D>("Head").GlobalPosition, 
			To = _playerCamera.GlobalPosition, 
			Exclude = new Godot.Collections.Array<Godot.Rid> { this.GetRid() } 
		});

		if (result.Keys.Count > 0)
		{
			Node3D node = (Node3D)result["collider"];
			if (node is Player)
			{
				Player p = node as Player;

				if(_playerInHearingClose)
				{
					if(!p.IsCrouched)
					{
						CurrentState = States.Chase;
					}
					//GD.Print("Player in close hearing");
				}
				if (_playerInHearingFar)
				{
					if (!p.IsCrouched)
					{
						CurrentState = States.Hunt;
						NavigationAgent.TargetPosition = p.GlobalPosition;
					}
					//GD.Print("Player in far hearing");
				}
				if (_playerInSightClose)
				{
					CurrentState = States.Chase;
					//GD.Print("Player in close sight");
				}
				if (_playerInSightFar)
				{
					CurrentState = States.Hunt;
					NavigationAgent.TargetPosition = p.GlobalPosition;
					//GD.Print("Player in far sight");
				}
			}
		}
	}


	#region SIGNALS

	private void _on_patrol_timer_timeout()
	{
		_waypointIndex += 1;
		if (_waypointIndex > _waypoints.Count - 1)
			_waypointIndex = 0;

		NavigationAgent.TargetPosition = _waypoints[_waypointIndex].GlobalPosition;
		CurrentState = States.Patrol;
	}

	// PLAYER DETECTION
	private bool _playerInHearingClose;
	private bool _playerInHearingFar;
	private bool _playerInSightClose;
	private bool _playerInSightFar;

	private void _on_close_hearing_range_body_entered(Node3D obj)
	{
		if (obj is Player)
			_playerInHearingClose = true;
	}
	private void _on_close_hearing_range_body_exited(Node3D obj)
	{
		if (obj is Player)
			_playerInHearingClose = false;
	}
	private void _on_far_hearing_range_body_entered(Node3D obj)
	{
		if (obj is Player)
			_playerInHearingFar = true;
	}
	private void _on_far_hearing_range_body_exited(Node3D obj)
	{
		if (obj is Player)
			_playerInHearingFar = false;
	}
	private void _on_close_sight_body_entered(Node3D obj)
	{
		if (obj is Player)
			_playerInSightClose = true;
	}
	private void _on_close_sight_body_exited(Node3D obj)
	{
		if (obj is Player)
			_playerInSightClose = false;
	}
	private void _on_far_sight_body_entered(Node3D obj)
	{
		if (obj is Player)
			_playerInSightFar = true;
	}
	private void _on_far_sight_body_exited(Node3D obj)
	{
		if (obj is Player)
			_playerInSightFar = false;
	}

	#endregion
}
