using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Key Mission", menuName = "Missions/Car delivery Mission")]
public class Mission_CarDelivery : Mission
{

    private bool carWasDelivered;

    public override void StartMission()
    {
        FindObjectOfType<MissionObject_CarDeliveryZone>(true).gameObject.SetActive(true);

        string missionText = "Find a functional vehicle";
        string missionDetails = "Deliver it to the evacuation point";
        UI.instance.inGameUI.UpdateMissionInfo(missionText, missionDetails);

        carWasDelivered = false;
        MissionObject_CarToDeliver.OnCarDelivery += CarDeliveryCompleted;

        Car[] cars = FindObjectsOfType<Car>();

        foreach (Car car in cars)
        {
            car.gameObject.AddComponent<MissionObject_CarToDeliver>();
        }
    }

    public override bool MissionCompleted()
    {
        return carWasDelivered;
    }

    private void CarDeliveryCompleted()
    {
        carWasDelivered = true;
        Debug.Log("小车任务完成");
        MissionObject_CarToDeliver.OnCarDelivery -= CarDeliveryCompleted;

        UI.instance.inGameUI.UpdateMissionInfo("Get to the evacuation point");
    }



}
