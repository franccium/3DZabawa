using Godot;
using System;

public partial class FirePillarSpell : AoeCircleSpell
{
    private float _explosionStrength = 15f;

    private void _on_aoe_body_entered(Node3D body)
    {
        if (body is CharacterBody3D)
        {
            GD.Print("fire pillar dealt " + InnerDamage + " dmg to " + body);
        }
        // throw up rigid bodies caught in the explosion
        if (body is RigidBody3D rigidbody)
        {
            GD.Print("got a rigid body " + rigidbody + " in aoe");
            rigidbody.ApplyImpulse(new Vector3(
                GlobalPosition.X - rigidbody.GlobalPosition.X, 
                _explosionStrength, 
                GlobalPosition.Z - rigidbody.GlobalPosition.Z
                ));
        }
    }
}
