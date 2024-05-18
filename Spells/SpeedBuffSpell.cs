using Godot;
using System;

public partial class SpeedBuffSpell : BuffSpell
{
    private Player _target;

    public override void _Ready()
    {
        _target = Target as Player;
        _target.CurrSpeed += 50f;
    }
}
