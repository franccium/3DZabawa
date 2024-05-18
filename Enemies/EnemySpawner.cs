using Godot;
using System;
using System.Collections.Generic;

public partial class EnemySpawner : Node
{
    public enum EnemyTypes
    {
        MPISeven,
        Barbarian,
    }

    private Dictionary<EnemyTypes, PackedScene> _enemyScenes = new Dictionary<EnemyTypes, PackedScene>();

	public override void _Ready()
	{
        _enemyScenes.Add(EnemyTypes.MPISeven, GD.Load<PackedScene>("res://Enemies/Shooters/Scenes/m_pi_se7en.tscn"));
        _enemyScenes.Add(EnemyTypes.Barbarian, GD.Load<PackedScene>("res://TestCharacters/barbarian.tscn"));
	}

	public override void _Process(double delta)
	{
	}

    public void SpawnEnemy(EnemyTypes type, Vector3 position)
    {
        Enemy enemy = _enemyScenes[type].Instantiate() as Enemy;
        GetParent().AddChild(enemy);
        enemy.GlobalPosition = position;
    }

    public void SpawnEnemy(EnemyTypes type, Marker3D spawnPoint)
    {
        SpawnEnemy(type, spawnPoint.GlobalPosition);
    }
}
