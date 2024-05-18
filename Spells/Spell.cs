using Godot;
using System;

public partial class Spell : Node3D
{
    [Export] public Timer CooldownTimer;
    [Export] public float Cooldown;
    [Export] public float CastTime;
    [Export] public int ManaCost;
    public PackedScene scene;
}
