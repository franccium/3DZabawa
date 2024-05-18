using Godot;
using System;

public partial class AttributeComponent : Node
{
	[Export] public float MaxHp; // maximum hp 
	[Export] public float CurrHp;
	[Export] public float MaxStamina; // maximum stamina, stamina depletes with actions lice running, rolls and dashes
	[Export] public float CurrStamina;
	[Export] public float Defense; // general defense, applies to damage calculation for every damage type, however some abilities have a higher penetration for this stat
	[Export] public float PhysicalRes; // percentage reduction of damage from physical sources
	[Export] public float MagicRes; // percentage reduction of damage from magical element
	[Export] public float FireRes; // percentage reduction of damage from lightning element
	[Export] public float WaterRes; // percentage reduction of damage from lightning element
	[Export] public float LightningRes; // percentage reduction of damage from lightning element
	[Export] public float DarkRes; // percentage reduction of damage from dark element
	[Export] public float HolyRes; // percentage reduction of damage from holy element
	[Export] public float EvasionChance; // percentage chance of avoiding physical damage

	[Export] public float BaseSpeed { get; set; } // basic movement speed, without modifiers


	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}

	public void CalculateDamageInflicted(float damage_inflicted)
	{
		float damage = damage_inflicted;
		GD.Print("AttributeComponent calculated " + damage + " inflicted");
	}
}
