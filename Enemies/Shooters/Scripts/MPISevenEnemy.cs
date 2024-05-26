using Godot;
using System;

/*
biega za graczem, skacze, robi uniki
strzela szybko


*/

public partial class MPISevenEnemy : GunslingerEnemy
{
	private Node3D _gun;
	private SMGEffectController _shootingController;

	public override void _Ready()
	{
        base._Ready();
		_gun = GetNode<Node3D>("Gun");
		_shootingController = GetNode<SMGEffectController>("SMGEffectsController");
	}

	public override void _Process(double delta)
	{
        base._Process(delta);
	}

    protected override void InitAttack()
	{
		// show weapon 
		// rotate the weapon in the players direction
		// shoot
		_gun.Show();
		_gun.LookAt(player.GlobalPosition);
		//_shootingController.Fire();
	}

    protected override void StopAttack()
	{
		_gun.Hide();
	}
}
