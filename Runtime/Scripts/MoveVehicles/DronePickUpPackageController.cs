using UnityEngine;

public enum ArmState
{
    Off,
    MoveForward,
    Return
}

public class DronePickUpPackageController : MonoBehaviour
{
    
    [Header("Package End Support Settings")]
    [SerializeField] private Transform[] packageEndSupport;

    [Header("Package Sub-Support Settings")]
    [SerializeField] private Transform[] packageSupportFrontLeft;
    [SerializeField] private Transform[] packageSupportFrontRight;
    [SerializeField] private Transform[] packageSupportBackLeft;
    [SerializeField] private Transform[] packageSupportBackRight;
    

    [Header("State")]
    public ArmState currentState = ArmState.Off;

    [Header("Movement Settings")]
    [Tooltip("Distance along the arm's local X axis")]
    public float moveDistanceEndSupport = 0.013f;
    public float moveDistanceSupportFrontLeftRightBack = 0.00012f;

    [Tooltip("Time in seconds to complete the forward or return movement")]
    public float moveDuration = 2.5f;

    // Internal storage
    private Vector3[] startPositionsEndSupport;
    private Vector3[] forwardTargetsEndSupport;

    private Vector3[] startPositionsSupportFrontLeft;
    private Vector3[] forwardTargetsSupportFrontLeft;

    private Vector3[] startPositionsSupportFrontRight;
    private Vector3[] forwardTargetsSupportFrontRight;

    private Vector3[] startPositionsSupportBackLeft;
    private Vector3[] forwardTargetsSupportBackLeft;

    private Vector3[] startPositionsSupportBackRight;
    private Vector3[] forwardTargetsSupportBackRight;

    




    private float progress; // 0 to 1

    private int finalStepDistanceEndSupport = 7;
    private int finalStepDistanceSupportFrontLeftRightBack = 7;

    private int directionMultiplier = 1;

    private void Awake()
    {
        
        if (packageEndSupport == null || packageEndSupport.Length != 4 ||
            packageSupportFrontLeft == null || packageSupportFrontLeft.Length != 7 ||
            packageSupportFrontRight == null || packageSupportFrontRight.Length != 7 ||
            packageSupportBackLeft == null || packageSupportBackLeft.Length != 7 || 
            packageSupportBackRight == null || packageSupportBackRight.Length != 7) return;

        startPositionsEndSupport = new Vector3[4];
        forwardTargetsEndSupport = new Vector3[4];

        for (int i = 0; i < 4; i++)
        {
            startPositionsEndSupport[i] = packageEndSupport[i].localPosition;

            if ((i == 0) || (i == 2))
                directionMultiplier = 1;
            else if ((i == 1) || (i == 3))
                directionMultiplier = -1;

            forwardTargetsEndSupport[i] = startPositionsEndSupport[i] + packageEndSupport[i].localRotation * (Vector3.right * directionMultiplier * moveDistanceEndSupport * finalStepDistanceEndSupport);
        }


        startPositionsSupportFrontLeft = new Vector3[7];
        forwardTargetsSupportFrontLeft = new Vector3[7];

        for (int i = 0; i < 7; i++)
        {
            startPositionsSupportFrontLeft[i] = packageSupportFrontLeft[i].localPosition;

            directionMultiplier = 1;
                
            forwardTargetsSupportFrontLeft[i] = startPositionsSupportFrontLeft[i] + packageSupportFrontLeft[i].localRotation * (Vector3.right * directionMultiplier * moveDistanceSupportFrontLeftRightBack * finalStepDistanceSupportFrontLeftRightBack);

            finalStepDistanceSupportFrontLeftRightBack--;
        }

        finalStepDistanceSupportFrontLeftRightBack = 7;

        startPositionsSupportFrontRight = new Vector3[7];
        forwardTargetsSupportFrontRight = new Vector3[7];

        for (int i = 0; i < 7; i++)
        {
            startPositionsSupportFrontRight[i] = packageSupportFrontRight[i].localPosition;

            directionMultiplier = -1;

            forwardTargetsSupportFrontRight[i] = startPositionsSupportFrontRight[i] + packageSupportFrontRight[i].localRotation * (Vector3.right * directionMultiplier * moveDistanceSupportFrontLeftRightBack * finalStepDistanceSupportFrontLeftRightBack);

            finalStepDistanceSupportFrontLeftRightBack--;
        }

        finalStepDistanceSupportFrontLeftRightBack = 7;

        startPositionsSupportBackLeft = new Vector3[7];
        forwardTargetsSupportBackLeft = new Vector3[7];

        for (int i = 0; i < 7; i++)
        {
            startPositionsSupportBackLeft[i] = packageSupportBackLeft[i].localPosition;

            directionMultiplier = 1;

            forwardTargetsSupportBackLeft[i] = startPositionsSupportBackLeft[i] + packageSupportBackLeft[i].localRotation * (Vector3.right * directionMultiplier * moveDistanceSupportFrontLeftRightBack * finalStepDistanceSupportFrontLeftRightBack);

            finalStepDistanceSupportFrontLeftRightBack--;
        }

        finalStepDistanceSupportFrontLeftRightBack = 7;

        startPositionsSupportBackRight = new Vector3[7];
        forwardTargetsSupportBackRight = new Vector3[7];

        for (int i = 0; i < 7; i++)
        {
            startPositionsSupportBackRight[i] = packageSupportBackRight[i].localPosition;

            directionMultiplier = -1;

            forwardTargetsSupportBackRight[i] = startPositionsSupportBackRight[i] + packageSupportBackRight[i].localRotation * (Vector3.right * directionMultiplier * moveDistanceSupportFrontLeftRightBack * finalStepDistanceSupportFrontLeftRightBack);

            finalStepDistanceSupportFrontLeftRightBack--;
        }

    }

    private void Update()
    {
        if (packageEndSupport == null || packageEndSupport.Length != 4 ||
            packageSupportFrontLeft == null || packageSupportFrontLeft.Length != 7 ||
            packageSupportFrontRight == null || packageSupportFrontRight.Length != 7 ||
            packageSupportBackLeft == null || packageSupportBackLeft.Length != 7 || 
            packageSupportBackRight == null || packageSupportBackRight.Length != 7) return;

        if (currentState == ArmState.Off) return;

        float delta = Time.deltaTime / moveDuration;

        if (currentState == ArmState.MoveForward)
        {
            progress += delta;
            progress = Mathf.Clamp01(progress);

            for (int i = 0; i < packageEndSupport.Length; i++)
            {
                if (packageEndSupport[i] == null) continue;

                // Move from start to forward target (calculated once)
                packageEndSupport[i].localPosition = Vector3.Lerp(startPositionsEndSupport[i], forwardTargetsEndSupport[i], progress);
            }


            for (int i = 0; i < packageSupportFrontLeft.Length; i++)
            {
                if (packageSupportFrontLeft[i] == null) continue;

                packageSupportFrontLeft[i].localPosition = Vector3.Lerp(startPositionsSupportFrontLeft[i], forwardTargetsSupportFrontLeft[i], progress);
            }


            for (int i = 0; i < packageSupportFrontRight.Length; i++)
            {
                if (packageSupportFrontRight[i] == null) continue;

                packageSupportFrontRight[i].localPosition = Vector3.Lerp(startPositionsSupportFrontRight[i], forwardTargetsSupportFrontRight[i], progress);
            }

            for (int i = 0; i < packageSupportBackLeft.Length; i++)
            {
                if (packageSupportBackLeft[i] == null) continue;

                packageSupportBackLeft[i].localPosition = Vector3.Lerp(startPositionsSupportBackLeft[i], forwardTargetsSupportBackLeft[i], progress);
            }

            for (int i = 0; i < packageSupportBackRight.Length; i++)
            {
                if (packageSupportBackRight[i] == null) continue;

                packageSupportBackRight[i].localPosition = Vector3.Lerp(startPositionsSupportBackRight[i], forwardTargetsSupportBackRight[i], progress);
            }


        }
        else if (currentState == ArmState.Return)
        {
            progress -= delta;
            progress = Mathf.Clamp01(progress);

            for (int i = 0; i < packageEndSupport.Length; i++)
            {
                if (packageEndSupport[i] == null) continue;

                packageEndSupport[i].localPosition = Vector3.Lerp(startPositionsEndSupport[i], forwardTargetsEndSupport[i], progress);
            }


            for (int i = 0; i < packageSupportFrontLeft.Length; i++)
            {
                if (packageSupportFrontLeft[i] == null) continue;

                packageSupportFrontLeft[i].localPosition = Vector3.Lerp(startPositionsSupportFrontLeft[i], forwardTargetsSupportFrontLeft[i], progress);
            }

            for (int i = 0; i < packageSupportFrontRight.Length; i++)
            {
                if (packageSupportFrontRight[i] == null) continue;

                packageSupportFrontRight[i].localPosition = Vector3.Lerp(startPositionsSupportFrontRight[i], forwardTargetsSupportFrontRight[i], progress);
            }

            for (int i = 0; i < packageSupportBackLeft.Length; i++)
            {
                if (packageSupportBackLeft[i] == null) continue;

                packageSupportBackLeft[i].localPosition = Vector3.Lerp(startPositionsSupportBackLeft[i], forwardTargetsSupportBackLeft[i], progress);
            }

            for (int i = 0; i < packageSupportBackRight.Length; i++)
            {
                if (packageSupportBackRight[i] == null) continue;

                packageSupportBackRight[i].localPosition = Vector3.Lerp(startPositionsSupportBackRight[i], forwardTargetsSupportBackRight[i], progress);
            }


        }
    }

    // Optional helper methods to trigger state changes
    public void StartMoveForward()
    {
        currentState = ArmState.MoveForward;
    }

    public void StartReturn()
    {
        currentState = ArmState.Return;
    }

    public void Stop()
    {
        currentState = ArmState.Off;
    }
}
