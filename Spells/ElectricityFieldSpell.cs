using Godot;
using System;

public partial class ElectricityFieldSpell : AoeCircleSpell
{
    private GpuParticles3D _waterParticles;
    private GpuParticles3D _electricityParticles;
    private double maxDuration;
    private Timer _explosionDelay;
    private Area3D _areaOfEffect;

    public override void _Ready()
    {
        _explosionDelay = GetNode<Timer>("ElectricityFieldAoeSpellEffect/ExplosionDelay");
        _waterParticles = GetNode<GpuParticles3D>("ElectricityFieldAoeSpellEffect/WaterParticles");
        _electricityParticles = GetNode<GpuParticles3D>("ElectricityFieldAoeSpellEffect/ElectricityParticles");
        maxDuration = _waterParticles.Lifetime;
        _waterParticles.Emitting = true;
        _explosionDelay.Start();
    }

    private void _on_explosion_delay_timeout()
    {
        _electricityParticles.Emitting = true;

        SceneTreeTimer effectTimer = GetTree().CreateTimer(maxDuration);
        effectTimer.Connect("timeout", new Callable(this, MethodName.QueueFree));

    }

    private void _on_aoe_body_entered(Node3D body)
    {
        if (body is CharacterBody3D)
        {
            GD.Print("electricity dealt " + InnerDamage + " dmg to " + body);
        }
    }
}
