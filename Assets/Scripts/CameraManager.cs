using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    private CinemachinePositionComposer cameraComposer;

    private float _targetCameraDistance;

    [SerializeField] private bool canChangeCameraDistance;
    [SerializeField]private float distanceChangeRate;

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

        cameraComposer = GetComponentInChildren<CinemachinePositionComposer>();
    }

    private void Update()
    {
        // UpdateCameraDistance();
    }

    private void UpdateCameraDistance()
    {
        if (canChangeCameraDistance == false)
        {
            return;
        }
        
        float currentDistance = cameraComposer.CameraDistance;
        print(_targetCameraDistance - currentDistance);

        if (Mathf.Abs(_targetCameraDistance - currentDistance) < .01f) return;

        cameraComposer.CameraDistance =
            Mathf.Lerp(cameraComposer.CameraDistance, _targetCameraDistance, distanceChangeRate * Time.deltaTime);
    }

    public void ChangeCameraDistance(float distance) => _targetCameraDistance = distance;
}
