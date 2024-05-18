using Godot;
using System;

public partial class FirePillarAoeSpellEffect : Node3D
{
    //todo STANDARDIZE SETTING THIS IN SPELLEFFECT AND IN SPELL ITSELF, CAUSE ITS ANNOYING
    private GpuParticles3D _particles;
    private GpuParticles3D _particlesExplosion;
    private double maxDuration;
    private Timer _explosionDelay;

    public override void _Ready()
    {
        _explosionDelay = GetNode<Timer>("ExplosionDelay");
        _particles = GetNode<GpuParticles3D>("GPUParticles3D");
        _particlesExplosion = GetNode<GpuParticles3D>("GPUParticles3D2");
        maxDuration = _particles.Lifetime;
        _explosionDelay.Start();
    }

    private void _on_explosion_delay_timeout()
    {
        _particles.Emitting = true;
        _particlesExplosion.Emitting = true;

        SceneTreeTimer effectTimer = GetTree().CreateTimer(maxDuration);
        //todo LIKE HERE I HAVE TO GETPAREN() TO DESTROY THE WHOLE SCENE, WITH AOE AND THINGS
        effectTimer.Connect("timeout", new Callable(GetParent(), MethodName.QueueFree));
    }
}
