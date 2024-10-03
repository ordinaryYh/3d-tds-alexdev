using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Timer Mission", menuName = "Missions/Timer Mission")]
public class Mission_Timer : Mission
{
    public float time;
    private float currentTime;

    public override bool MissionCompleted()
    {
        return currentTime > 0;
    }

    public override void UpdateMission()
    {
        currentTime -= Time.deltaTime;

        if (currentTime < 0)
        {
            Debug.Log("game over");
        }

        //这个是输出时间
        string timeText = System.TimeSpan.FromSeconds(currentTime).ToString("mm':'ss");

        Debug.Log(timeText);
    }

    public override void StartMission()
    {
        currentTime = time;

        Debug.Log("任务开始 + " + currentTime.ToString());
    }
}
