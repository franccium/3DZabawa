using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class SpellEffectsController : Node
{
    [Export] private Node3D _spellOrigin;
    [Export] public RayCast3D SpellRayCast;

    // a dictionary of packed scenes for quick loading of spells
    private Dictionary<string, PackedScene> _spellScenes;
    // a dictionary that keeps track of cooldowns of each casted spell, so that they cant be instantiated when on cooldown
    private Dictionary<string, Timer> _spellCooldowns;

    private Spell _castedSpellEffect;

    private Player _player;

    public override void _Ready()
    {
        _spellScenes = new Dictionary<string, PackedScene>
        {
            { "Fireball", ResourceLoader.Load<PackedScene>("res://Spells/Scenes/fireball_projectile_spell.tscn")},
            { "FirePillar", ResourceLoader.Load<PackedScene>("res://Spells/Scenes/fire_pillar_aoe_spell.tscn") },
            {"DarkPillar", ResourceLoader.Load<PackedScene>("res://Spells/Scenes/dark_pillar_aoe_spell.tscn") },
            {"ElectricityField", ResourceLoader.Load<PackedScene>("res://Spells/Scenes/electricity_field_aoe_spell.tscn") },
            {"HolyBeam", ResourceLoader.Load<PackedScene>("res://Spells/Scenes/holy_beam_beam_spell.tscn") },
            {"SpeedBuff", ResourceLoader.Load<PackedScene>("res://Spells/Scenes/speed_buff_spell.tscn") }
        };

        _spellCooldowns = new Dictionary<string, Timer>
        {
            { "Fireball", null},
            { "FirePillar", null },
            {"DarkPillar", null },
            {"ElectricityField", null},
            {"HolyBeam", null },
            {"SpeedBuff", null }
        };

        _player = GetParent() as Player;
    }

    /// <summary>
    /// categorises spell and uses an appropriate way of casting it
    /// </summary>
    /// <param name="spell"></param> <summary>
    public void Cast(string spell)
    {
        if(_spellCooldowns[spell] != null && !_spellCooldowns[spell].IsStopped())
        {
            GD.Print(spell + " is on cooldown!");
            return;
        }

        _castedSpellEffect = _spellScenes[spell].Instantiate() as Spell;
        if (_castedSpellEffect is AoeSpell)
        {
            AoeCast();
        }
        else if (_castedSpellEffect is ProjectileSpell)
        {
            ProjectileCast();
        }
        else if (_castedSpellEffect is BeamSpell)
        {
            BeamCast();
        }
        else if (_castedSpellEffect is BuffSpell)
        {
            BeamCast();
        }

        _spellCooldowns[spell] = _castedSpellEffect.CooldownTimer;
        _castedSpellEffect.CooldownTimer.Start();
    }

    /// <summary>
    /// cast a spell with the characteristics of all aoe casts
    /// </summary> <summary>
    private void AoeCast()
    {
        AddChild(_castedSpellEffect);

        // cast the spell at the point of raycast collision
        if (SpellRayCast.IsColliding())
        {
            Vector3 hitLocation = SpellRayCast.GetCollisionPoint();
            _castedSpellEffect.GlobalPosition = hitLocation;
        }
    }

    /// <summary>
    /// cast a spell with the characteristics of all projectile casts
    /// </summary> <summary>
    private void ProjectileCast()
    {
        ProjectileSpell _projectile = _castedSpellEffect as ProjectileSpell;
        //apply a projectile direction based on the direction on spellRayCast
        _projectile.ProjectileDirection = SpellRayCast.GlobalTransform.Basis.Z;
        AddChild(_projectile);
        _projectile.GlobalPosition = _spellOrigin.GlobalPosition;

        if (SpellRayCast.IsColliding())
        {
            Vector3 hitLocation = SpellRayCast.GetCollisionPoint();
            Node3D impactTarget = SpellRayCast.GetCollider() as Node3D;
            _projectile.LookAt(-hitLocation);

            if (impactTarget is RigidBody3D rigidBody)
            {
                GD.Print("hit a " + rigidBody + " with a projectile");
                rigidBody.ApplyImpulse(SpellRayCast.GlobalTransform.Basis.Z * _projectile.ProjectileImpactForce, hitLocation - rigidBody.GlobalPosition);
            }
        }
    }

    /// <summary>
    /// cast a spell with the characteristics of all beam casts
    /// </summary> <summary>
    private void BeamCast()
    {
        BeamSpell _beamSpell = _castedSpellEffect as BeamSpell;
        _beamSpell.BeamDirection = SpellRayCast.GlobalTransform.Basis.Z;
        _spellOrigin.AddChild(_beamSpell); // add as a child of spell origin for the beam to stick to the caster
        _beamSpell.GlobalPosition = _spellOrigin.GlobalPosition;


        if (SpellRayCast.IsColliding())
        {
            Vector3 hitLocation = SpellRayCast.GetCollisionPoint();
            Node3D impactTarget = SpellRayCast.GetCollider() as Node3D;

            if (impactTarget is RigidBody3D rigidBody)
            {
                GD.Print("hit a " + rigidBody + " with a projectile");
                //rigidBody.ApplyImpulse(SpellRayCast.GlobalTransform.Basis.Z * _impactForce, hitLocation - rigidBody.GlobalPosition);
            }
        }
    }

    /// <summary>
    /// cast a spell with the characteristics of all buff casts
    /// </summary> <summary>
    private void BuffCast()
    {
        BuffSpell _buffSpell = _castedSpellEffect as BuffSpell;
        _player.CurrSpeed += 50f; // debugging placeholder
        _buffSpell.Target = _player; //todo doesnt work
        AddChild(_castedSpellEffect);


        GD.Print("used a buff");

    }

}
