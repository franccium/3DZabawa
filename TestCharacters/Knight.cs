using Godot;
using System;

public partial class Knight : MeleeEnemy
{
    private AnimationPlayer _animationPlayer;
    private AnimationTree _animationTree;
    private AnimationNodeStateMachinePlayback _locomotionPlayback;
    private AnimationNodeStateMachinePlayback _upperBodyPlayback;

    public override void _Ready()
    {
        base._Ready();

        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _animationTree = GetNode<AnimationTree>("AnimationTree");

        _locomotionPlayback = (AnimationNodeStateMachinePlayback)_animationTree.Get("parameters/LocomotionStateMachine/playback");
        _upperBodyPlayback = (AnimationNodeStateMachinePlayback)_animationTree.Get("parameters/UpperBodyStateMachine/playback");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        GD.Print("state: " + CurrentState.ToString());
        if (CurrentState == EnemyStates.Chase)
        {
            // _animationPlayer.Play("Walking_A");
        }
    }

    protected override void Wait()
    {
        base.Wait();

        _upperBodyPlayback.Travel("Unarmed_Idle");
        //_animationPlayer.Play("Sit_Floor_Idle");
    }

    protected override void InitAttack()
    {

        //_animationPlayer.Play("1H_Melee_Attack_Chop");

        _upperBodyPlayback.Travel("1H_Melee_Attack_Slice_Horizontal");
    }

    protected override void ChasePlayer()
    {
        base.ChasePlayer();

        _upperBodyPlayback.Travel("Idle");
    }

    //todo attack aniomations
    //todo idle state and other states
}
