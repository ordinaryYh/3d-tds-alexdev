using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Mathematics;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    private CinemachineVirtualCamera virtualCamera;
    private CinemachineFramingTransposer transposer;

    [Header("Cmaera Distance")]
    [SerializeField] private bool canChangeCameraDistance;
    [SerializeField] private float distanceChangeRate;
    private float targetCameraDistance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Debug.Log("you have more than one manager");
            Destroy(gameObject);
        }

        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    private void Update()
    {
        UpdateCameraDistance();
    }

    private void UpdateCameraDistance()
    {
        if (canChangeCameraDistance == false)
            return;

        float currentDistance = transposer.m_CameraDistance;

        if (Mathf.Abs(targetCameraDistance - currentDistance) > 0.01f)
        {
            transposer.m_CameraDistance = Mathf.Lerp(currentDistance, targetCameraDistance, distanceChangeRate * Time.deltaTime);
        }
    }

    public void ChangeCameraDistance(float distance) => targetCameraDistance = distance;


}
