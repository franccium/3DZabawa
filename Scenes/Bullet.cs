using Godot;
using System;

public partial class Bullet : RigidBody3D
{
    private void _on_body_entered(Node3D body)
    {
        QueueFree();
    }
}
