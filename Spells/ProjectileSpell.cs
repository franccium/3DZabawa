using Godot;
using System;

public partial class ProjectileSpell : Spell
{
    [Export] public float ProjectileDamage;
    [Export] public int ProjectileCount;
    [Export] public float ProjectileRange;
    [Export] public float ProjectileSpeed { get; set; }
    [Export] public float ProjectileImpactForce;
    [Export] public float FireRate;
    [Export] public float Recoil;
    [Export] public float DamageFalloff;
    [Export] public bool ShouldSpawnAoe;
    public Vector3 ProjectileDirection { get; set; }
}
