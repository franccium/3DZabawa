using Godot;
using System;

public partial class GunslingerEnemy : ShootingEnemy
{
	public override void _Ready()
	{
        base._Ready();
	}

	public override void _Process(double delta)
	{
        base._Process(delta);
	}

	protected override void InitAttack()
	{
		base.InitAttack();
	}
}
