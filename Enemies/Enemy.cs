using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Enemy : CharacterBody3D
{
    [Export] public AttributeComponent Attributes; // A set of attributes that apply to every character in game
    [Export] public Area3D DetectionArea; // Area3D which sends a signal whenever Player enters it
    [Export] public CollisionShape3D DetectionAreaCollisionShape; // the shape of the detection area, with a modifiable radius
    [Export] public float DetectionRadius = 20f; // determines the size of the DetectionArea
    [Export] public NavigationAgent3D NavigationAgent;
    [Export] public Timer WaitTimer; // the timer that runs after changing state from attacking to wait to delay returning to patrol, as well as between walking to the next patrol point
    [Export] public ProgressBar HealthBar;
    [Export] public float AttackRange = 4f;
    private CollisionShape3D _hitbox;
    private Area3D _hitboxArea;

    private const float gravity = 9.8f;
    private Vector3 _targetPosition = new Vector3();
    private Vector3 _lastSightDirection;
    public Player player;
    private List<Marker3D> _waypoints = new List<Marker3D>();
    private int _waypointIndex;

    public enum EnemyStates
    {
        Patrol, // until player gets in range of sight, keep walking around
        Wait, // when the player gets out of range, wait for a moment and look at last direction, then switch to patrol
        Chase,
        Attack // when the player gets in range, initialise an attack (which varies depending on EnemyType)
    }

    public EnemyStates CurrentState;

    public override void _Ready()
    {
        GatherRequirements();
    }

    /// <summary>
    /// gets every node needed for the enemy to function, such as his detection area, the player and the navigation
    /// </summary>
    protected virtual void GatherRequirements()
    {
        Attributes = GetNode<Node>("AttributeComponent") as AttributeComponent;
        DetectionArea = GetNode<Area3D>("DetectionArea");
        DetectionAreaCollisionShape = GetNode<CollisionShape3D>("DetectionArea/CollisionShape3D");
        SphereShape3D detectionShape = (SphereShape3D)DetectionAreaCollisionShape.Shape;
        detectionShape.Radius = DetectionRadius;
        player = GetTree().GetNodesInGroup("Player")[0] as Player;

        WaitTimer = GetNode<Timer>("WaitTimer");
        WaitTimer.WaitTime = 2f;
        WaitTimer.OneShot = true;

        _waypoints = GetTree().GetNodesInGroup("EnemyWaypoint").Select(saar => saar as Marker3D).ToList();
        NavigationAgent = GetNode<NavigationAgent3D>("NavigationAgent3D");
        NavigationAgent.TargetPosition = _waypoints[0].GlobalPosition;
        GD.Print("initial target position: " + NavigationAgent.TargetPosition);

        HealthBar = GetNode<ProgressBar>("HealthBarViewport/HealthBar");
        HealthBar.MaxValue = Attributes.MaxHp;
        HealthBar.Value = Attributes.MaxHp;

        _hitboxArea = GetNode<Area3D>("HitboxArea");
        _hitbox = _hitboxArea.GetNode<CollisionShape3D>("HitboxShape");


        DetectionArea.BodyEntered += OnDetectionAreaBodyEntered;
        DetectionArea.BodyExited += OnDetectionAreaBodyExited;

        /*
		SphereShape3D detectionShape = (SphereShape3D)DetectionAreaCollisionShape.Shape;
		detectionShape.Radius = DetectionRadius;
		player = GetTree().GetNodesInGroup("Player")[0] as Player;

		_waypoints = GetTree().GetNodesInGroup("EnemyWaypoint").Select(saar => saar as Marker3D).ToList();
		NavigationAgent.TargetPosition = _waypoints[0].GlobalPosition;

		HealthBar.MaxValue = Attributes.MaxHp;
		HealthBar.Value = Attributes.MaxHp;
        */
    }

    public override void _Process(double delta)
    {
        //CheckForPlayer();
        //UpdateStates();
        //UpdatePosition((float)delta);
        //MoveToTargetPosition((float)delta);
        UpdateStates();

        GD.Print("state: " + CurrentState.ToString());
    }

    public override void _PhysicsProcess(double delta)
    {
        UpdatePosition((float)delta);
    }

    //public void UpdatePosition(float delta)
    //{
    //Vector3 velocity = Velocity;
    //velocity += MoveDirection * delta;
    //velocity.Y -= gravity * delta;

    //Velocity = velocity;
    //MoveAndSlide();
    //}

    /// <summary>
    /// not yet sure if needed 
    /// </summary>
    /// <param name="delta"></param> <summary>
    protected virtual void UpdatePosition(float delta)
    {
        MoveToTargetPosition(delta);
    }

    /// <summary>
    /// sets velocity in a direction specified in the navigation agent, which is also updated when spotting the player
    /// </summary>
    /// <param name="delta"></param> <summary>
    protected virtual void MoveToTargetPosition(float delta)
    {
        Vector3 velocity = Velocity;

        Vector2 targetPos = new Vector2(NavigationAgent.GetNextPathPosition().X, NavigationAgent.GetNextPathPosition().Z);
        Vector3 targetPoint = new Vector3(targetPos.X, GlobalPosition.Y, targetPos.Y);
        Vector3 direction = GlobalPosition.DirectionTo(targetPoint);
        Vector3 sightDirection = _lastSightDirection.Lerp(targetPoint, 0.9f);

        /* NAVIGATION DEBUG PRINTS
        GD.Print("waypoints left: " + _waypoints.Count);
        GD.Print("next path position: " + NavigationAgent.GetNextPathPosition());
        GD.Print("current position: " + GlobalPosition);
        GD.Print("sight direction: " + sightDirection);
        GD.Print("targetPos: " + targetPos);
        GD.Print("targetPoint: " + targetPoint);
        */

        //LookAt(sightDirection, Vector3.Up);
        LookAt(new Vector3(player.GlobalPosition.X, GlobalPosition.Y, player.GlobalPosition.Z), Vector3.Up);
        _lastSightDirection = sightDirection;

        UpdateGravity(ref velocity, delta);
        velocity.X = direction.X * Attributes.BaseSpeed;
        velocity.Z = direction.Z * Attributes.BaseSpeed;
        Velocity = velocity;
        MoveAndSlide();
    }

    protected virtual void UpdateGravity(ref Vector3 velocity, float delta)
    {
        if (!IsOnFloor())
        {
            velocity.Y -= gravity * (float)delta;
        }
    }

    /// <summary>
    /// calls functions based on the current state
    /// </summary>
    protected virtual void UpdateStates()
    {
        switch (CurrentState)
        {
            case EnemyStates.Patrol:
                Patrol();
                break;

            case EnemyStates.Wait:
                Wait();
                break;

            case EnemyStates.Chase:
                ChasePlayer();
                break;

            case EnemyStates.Attack:
                InitAttack();
                CheckForRemainingInAttackRange();
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// entered after each wait state, travelling between points specified in the scenes' navigation
    /// </summary>
    protected virtual void Patrol()
    {
        // maybe i dont want to update pos in process but here, but maybe not
        if (NavigationAgent.IsNavigationFinished())
        {
            WaitTimer.Start();
            CurrentState = EnemyStates.Wait;
            return;
        }
    }

    /// <summary>
    /// entered when players gets out of detection area and on reaching the point in 
    /// patrol state, exited on WaitTimer timeout / on player walking into detection area
    /// </summary> <summary>
    protected virtual void Wait()
    {
        if (WaitTimer.IsStopped())
            WaitTimer.Start();
        //todo some looking around in place
    }

    /// <summary>
    /// a function that can be handled by each enemy differently, called in attack state
    /// </summary>
    protected virtual void InitAttack() { }

    /// <summary>
    /// a function that can be handled by each enemy differently, called when exiting attack state
    /// </summary>
    protected virtual void StopAttack() { }

    protected virtual void CheckForRemainingInAttackRange()
    {
        if (GlobalPosition.DistanceTo(player.GlobalPosition) > AttackRange)
        {
            CurrentState = EnemyStates.Chase;
        }
    }

    /// <summary>
    /// called in attack state, updates NavigationAgent target position to be the current player position 
    /// </summary>
    protected virtual void ChasePlayer()
    {
        NavigationAgent.TargetPosition = player.GlobalPosition;

        GD.Print("distance to player: " + GlobalPosition.DistanceTo(player.GlobalPosition));

        if (GlobalPosition.DistanceTo(player.GlobalPosition) < AttackRange)
        {
            CurrentState = EnemyStates.Attack;
        }
    }


    #region SIGNALS
    // requires an Area3D node called DetectionArea
    private void OnDetectionAreaBodyEntered(Node3D body)
    {
        GD.Print(this + " detection_radius_body_entered by : " + body);
        if (body is Player player)
        {
            CurrentState = EnemyStates.Chase;
            NavigationAgent.TargetPosition = player.GlobalPosition;
        }
    }

    private void OnDetectionAreaBodyExited(Node3D body)
    {
        GD.Print(this + " detection_radius_body_exited by : " + body);
        if (body is Player player)
        {
            StopAttack();
            CurrentState = EnemyStates.Wait;
        }
    }

    /// <summary>
    /// on the WaitTimer timeout enter patrol state and update the waypoint to the next one in the list, 
    /// else if its the last one, return to the first waypoint; then set the navigation agent target to the waypoint
    /// </summary>
    private void _on_wait_timer_timeout()
    {
        //if (CurrentState != EnemyStates.Patrol)
        CurrentState = EnemyStates.Patrol;

        _waypointIndex += 1;
        if (_waypointIndex > _waypoints.Count - 1)
            _waypointIndex = 0;

        NavigationAgent.TargetPosition = _waypoints[_waypointIndex].GlobalPosition;
    }

    private void _on_hitbox_area_body_entered(Node3D body)
    {
        if (body is ProjectileSpell spell)
        {
            Attributes.CalculateDamageInflicted(spell.ProjectileDamage);
            HealthBar.Value = Attributes.CurrHp;
        }
        else if (body is RigidBody3D)
        {
            Attributes.CalculateDamageInflicted(5);
            HealthBar.Value = Attributes.CurrHp;
        }
    }

    #endregion
}
