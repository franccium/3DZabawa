using Godot;
using System;

public partial class AoeSpell : Spell
{
    [Export] public float DamageUpdateFrequency;
    [Export] public float ActiveTime;
    [Export] public int InnerDamage;
    [Export] public int OuterDamage;
}
