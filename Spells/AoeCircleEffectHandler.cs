using Godot;
using System;

public partial class AoeCircleEffectHandler : Node3D
{
    [Export] public Light3D OptionalLight { get; set; }
    [Export] public float LightDuration { get; set; }

    public override void _Ready()
    {
        double maxDuration = 0;
        foreach (var child in GetChildren())
        {
            if (child is GpuParticles3D gpuParticle)
            {
                maxDuration = Mathf.Max(maxDuration, gpuParticle.Lifetime);
                gpuParticle.Emitting = true;
                continue;
            }
            if (child is CpuParticles3D cpuParticle)
            {
                maxDuration = Mathf.Max(maxDuration, cpuParticle.Lifetime);
                cpuParticle.Emitting = true;
                continue;
            }
        }

        if (OptionalLight != null)
        {
            //Tween newTween = CreateTween();
            //newTween.TweenProperty(OptionalLight, "light_energy", 0, LightDuration);
            //newTween.TweenProperty(OptionalLight, "omni_energy", 0, LightDuration);
        }

        SceneTreeTimer timer = GetTree().CreateTimer(maxDuration);
        timer.Connect("timeout", new Callable(this, MethodName.QueueFree));
    }
}
