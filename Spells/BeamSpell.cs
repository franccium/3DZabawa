using Godot;
using System;

public partial class BeamSpell : Spell
{
    [Export] public int BeamCount;
    [Export] public float BeamRange;
    [Export] public float BeamRadius;
    [Export] public float BeamImpactForce;
    [Export] public float DamageUpdateFrequency;
    [Export] public float Recoil;
    [Export] public float BeamSpread;
    [Export] public float DamageFalloff;
    [Export] public bool IsOneShot;
    [Export] public bool ShouldSpawnAoe;
    public Vector3 BeamDirection { get; set; }
}
