using Godot;
using System;

public partial class Barbarian : MeleeEnemy
{
    private AnimationPlayer _animationPlayer;

    public override void _Ready()
    {
        base._Ready();
        
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        GD.Print("state: " + CurrentState.ToString());
        if(CurrentState == EnemyStates.Chase)
        {
           // _animationPlayer.Play("Walking_A");
        }
    }

    protected override void Wait()
    {
        base.Wait();

        //_animationPlayer.Play("Sit_Floor_Idle");
    }

    protected override void InitAttack()
    {
        //_animationPlayer.Play("1H_Melee_Attack_Chop");
    }

    //todo attack aniomations
    //todo idle state and other states
}
