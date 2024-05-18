using Godot;
using System;

public partial class GenericProjectileSpellEffectHandler : Node3D
{
    private GpuParticles3D _particles;
    private double maxDuration;

    public override void _Ready()
    {
        //_particles = GetNode<GpuParticles3D>("PostExplosion");
        //maxDuration = _particles.Lifetime;
        //_particles.Emitting = true;
        // _particles.Rotate()
        //_particleShape = GetNode<RigidBody3D>("RigidBody3D");
        //_particleShape.ApplyForce(new Vector3(50, 120, 70));
        //_particles.Emitting = true;

        //SceneTreeTimer effectTimer = GetTree().CreateTimer(maxDuration);
        //effectTimer.Connect("timeout", new Callable(this, MethodName.QueueFree));
    }
}
