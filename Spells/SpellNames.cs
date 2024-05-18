using Godot;
using System;

static public class SpellNames
{
    public static StringName Cancel { get; } = new StringName("alt");
    public static StringName Fire { get; } = new StringName("f");
    public static StringName Water { get; } = new StringName("v");
    public static StringName Lightning { get; } = new StringName("r");
    public static StringName Holy { get; } = new StringName("h");
    public static StringName Dark { get; } = new StringName("c");
    public static StringName Projectile { get; } = new StringName("q");
    public static StringName Aoe { get; } = new StringName("e");
    public static StringName Beam { get; } = new StringName("x");
    public static StringName Buff { get; } = new StringName("tab");
    // array of inputs and check if all inputs are okay?
    //combinations stored in an enum
    // Elements.Fire, Types.Aoe, Shapes.Ball
}