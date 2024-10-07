using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Key Mission", menuName = "Missions/Key Mission")]
public class Mission_KeyFind : Mission
{
    [SerializeField] private GameObject key;
    private bool keyFound;

    public override bool MissionCompleted()
    {
        return keyFound;
    }

    public override void StartMission()
    {
        MissionObject_Key.OnKeyPickedUp += PickUpKey;

        UI.instance.inGameUI.UpdateMissionInfo("Find a key-holder. Retrive the key");

        //找到随机的enemy
        Enemy enemy = LevelGenerator.instance.GetRandomEnemy();
        //然后把key给随机的enemy
        enemy.GetComponent<Enemy_DropController>()?.GiveKey(key);
        //然后随机的enemy升级
        enemy.MakeEnemyVIP();

    }

    private void PickUpKey()
    {
        keyFound = true;
        MissionObject_Key.OnKeyPickedUp -= PickUpKey;

        UI.instance.inGameUI.UpdateMissionInfo("You've get the key! \n Get to the evacuation point");
    }

}
