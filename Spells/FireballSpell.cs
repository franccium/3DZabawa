using Godot;
using System;

public partial class FireballSpell : ProjectileSpell
{
    private GpuParticles3D _particles;
    private RigidBody3D _rigidBody;
    private double maxDuration;

    public override void _Ready()
    {
        _rigidBody = GetNode<RigidBody3D>("ProjectileSpellEffect/RigidBody3D");
        _particles = GetNode<GpuParticles3D>("ProjectileSpellEffect/RigidBody3D/GPUParticles3D");
        maxDuration = _particles.Lifetime;
        _rigidBody.LinearVelocity = ProjectileDirection * ProjectileSpeed;
        _particles.Emitting = true;


        // emits a particle with a set velocity (which is in the second parameter)
        //_particles.EmitParticle(GlobalTransform, ProjectileDirection * ProjectileSpeed, Colors.Red, Colors.Red, 4);

        //SceneTreeTimer effectTimer = GetTree().CreateTimer(maxDuration);
        //effectTimer.Connect("timeout", new Callable(this, MethodName.QueueFree));
    }
}
