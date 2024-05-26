using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class EnemyController : Node
{
    private EnemySpawner _enemySpawner;

    private List<Marker3D> _spawnPoints = new List<Marker3D>();

    public override void _Ready()
    {
        _enemySpawner = GetNode<Node>("EnemySpawner") as EnemySpawner;
        _spawnPoints = GetTree().GetNodesInGroup("EnemySpawnpoints").Select(saar => saar as Marker3D).ToList();

        CallDeferred("SpawnEnemiesDebug");
    }

    public override void _Process(double delta)
    {
    }

    #region ENEMY MOVEMENT



    #endregion


    #region SPAWNING

    public void SpawnEnemy(EnemySpawner.EnemyTypes type, Vector3 position)
    {
        _enemySpawner.SpawnEnemy(type, position);
    }

    public void SpawnEnemy(EnemySpawner.EnemyTypes type, Marker3D spawnPoint)
    {
        _enemySpawner.SpawnEnemy(type, spawnPoint);
    }

    public void SpawnEnemiesDebug()
    {
        //SpawnEnemy(EnemySpawner.EnemyTypes.MPISeven, _spawnPoints[0]);
        //SpawnEnemy(EnemySpawner.EnemyTypes.Barbarian, _spawnPoints[1]);
        SpawnEnemy(EnemySpawner.EnemyTypes.Rogue, _spawnPoints[1]);
        //SpawnEnemy(EnemySpawner.EnemyTypes.Knight, _spawnPoints[0]);
    }

    #endregion
}
