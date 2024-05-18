using Godot;
using System;

public partial class GenericEffectHandler : Node3D
{
    public override void _Ready()
    {
        double maxDuration = 0;
        foreach(var child in GetChildren())
        {
            if(child is GpuParticles3D gpuParticle)
            {
                maxDuration = Mathf.Max(maxDuration, gpuParticle.Lifetime);
                gpuParticle.Emitting = true;
                continue;
            }
            if(child is CpuParticles3D cpuParticle)
            {
                maxDuration = Mathf.Max(maxDuration, cpuParticle.Lifetime);
                cpuParticle.Emitting = true;
                continue;
            } 
        }

        SceneTreeTimer timer = GetTree().CreateTimer(maxDuration);
        timer.Connect("timeout", new Callable(this, MethodName.QueueFree));
    }
}
