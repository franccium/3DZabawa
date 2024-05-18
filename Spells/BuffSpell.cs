using Godot;
using System;

public partial class BuffSpell : Spell
{
    [Export] public float Duration;
    [Export] public float Strength;
    public Node3D Target{ get; set; }
}
