using System;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    private CinemachineCamera camera;
    private CinemachinePositionComposer cameraComposer;

    private float targetCameraDistance;
    private float distanceChangeRate;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            print("No more Instance");
            Destroy(gameObject);
        }

        camera = GetComponentInChildren<CinemachineCamera>();
        cameraComposer = GetComponentInChildren<CinemachinePositionComposer>();
    }

    private void Update()
    {
        UpdateCameraDistance();
    }

    private void UpdateCameraDistance()
    {
        float currentDistance = cameraComposer.CameraDistance;

        if (Mathf.Abs(targetCameraDistance - currentDistance) < .01f) return;

        cameraComposer.CameraDistance =
            Mathf.Lerp(cameraComposer.CameraDistance, targetCameraDistance, distanceChangeRate * Time.deltaTime);
    }

    public void ChangeCameraDistance(float distance) => targetCameraDistance = distance;
}
