using UnityEngine;

public class DronePropellerController : MonoBehaviour
{
    [Header("References")]
    private DroneDynamics droneDynamics;

    [Header("Propellers")]
    [SerializeField] private Transform[] propellers;

    [Header("Rotation")]
    [Tooltip("Maximum rotation speed in degrees per second")]
    public float maxRotationSpeed = 2000f;

    private void Awake()
    {
        // Get reference to DroneDynamics component on the same GameObject
        droneDynamics = GetComponent<DroneDynamics>();
    }

    private void Update()
    {
        if (droneDynamics == null) return;

        if(propellers.Length == 0) return;

        // Read throttle value from DroneDynamics (expected range 0–1)
        float throttle = droneDynamics.throttle;

        float rotationSpeed = throttle * maxRotationSpeed;

        // Rotate each propeller
        foreach (Transform propeller in propellers)
        {
            if (propeller == null) continue;
            propeller.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
        }
    }
    
}
