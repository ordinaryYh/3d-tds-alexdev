using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//abstract代表是一个抽象类
//抽象类的内部，函数只有声明，没有实现
public abstract class Mission : ScriptableObject
{
    public string missionName;
    [TextArea]
    public string missionDescription;

    public abstract void StartMission();
    public abstract bool MissionCompleted();

    public virtual void UpdateMission()
    {

    }
}
