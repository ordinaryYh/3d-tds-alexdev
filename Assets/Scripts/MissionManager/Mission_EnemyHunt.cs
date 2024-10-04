using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

[CreateAssetMenu(fileName = "New Key Mission", menuName = "Missions/Hunt Mission")]
public class Mission_EnemyHunt : Mission
{
    public int amountToKill = 12;
    public EnemyType enemyType;

    private int killsToGo;

    public override bool MissionCompleted()
    {
        return killsToGo <= 0;
    }

    public override void StartMission()
    {
        killsToGo = amountToKill;
        MissionObject_HuntTarget.OnTargetKilled += ElimilateTarget;

        List<Enemy> validEnemies = new List<Enemy>();

        if (enemyType == EnemyType.Random)
            validEnemies = LevelGenerator.instance.GetEnemies();
        else
        {
            foreach (var enemy in LevelGenerator.instance.GetEnemies())
            {
                if (enemy.enemyType == enemyType)
                    validEnemies.Add(enemy);
            }
        }

        for (int i = 0; i < amountToKill; i++)
        {
            if (validEnemies.Count <= 0)
                return;

            int randomIndex = Random.Range(0, validEnemies.Count);
            validEnemies[randomIndex].gameObject.AddComponent<MissionObject_HuntTarget>();
            validEnemies.RemoveAt(randomIndex);
        }
    }

    private void ElimilateTarget()
    {
        killsToGo--;

        if (killsToGo <= 0)
            MissionObject_HuntTarget.OnTargetKilled -= ElimilateTarget;
    }
}
