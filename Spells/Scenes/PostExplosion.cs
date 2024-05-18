using Godot;
using System;

public partial class PostExplosion : GpuParticles3D
{
    private void _on_rigid_body_3d_body_entered()
    {
        this.Emitting = true;
        GD.Print("asdasdsa");
        //EmitFlags.Velocity = true;

    }
}
