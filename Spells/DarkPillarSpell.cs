using Godot;
using System;

public partial class DarkPillarSpell : AoeCircleSpell
{
    private void _on_aoe_body_entered(Node3D body)
    {
        if (body is CharacterBody3D)
        {
            GD.Print("dark pillar dealt " + InnerDamage + " dmg to " + body);
        }
    }
}
