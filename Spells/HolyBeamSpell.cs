using Godot;
using System;

public partial class HolyBeamSpell : BeamSpell
{
    private RayCast3D _rayCast;
    private GpuParticles3D _particles;

    public override void _Ready()
    {
        _rayCast = GetNode<RayCast3D>("RayCast3D");
        _particles = GetNode<GpuParticles3D>("RayCast3D/BeamSpellEffect/GPUParticles3D");
        GetNode<Timer>("Timer").Start();
    }

    private void _on_timer_timeout()
    {
        QueueFree();
    }
}
