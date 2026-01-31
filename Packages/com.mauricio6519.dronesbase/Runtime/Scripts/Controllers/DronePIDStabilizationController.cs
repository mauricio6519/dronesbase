/*******************************************************************************
* Copyright 2025 INTRIG
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*     http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
******************************************************************************/

// Libraries
using System; // Library to use MathF class
using UnityEngine; // Unity Engine library to use in MonoBehaviour classes

// Main Class to control the drone's movements using PID control:
public class DronePIDStabilizationController : MonoBehaviour
{
    // -----------------------------------------------------------------------------------------------------
    // Public or SerializeField variables that appear in the Inspector:

    // Margin of error to consider the target reached
    [Header("Target Error Margin")]
    public float targetErrorMargin = 0.1f;

    // Set the adjusted variable from velocity correction, may need to be optimized
    [Header("PID Velocity Variable")]
    public float PIDAdjustedVariableFromVelocity = 20.0f;

    [Header("Stabilization PID")]
    public PIDSettingsMinMax stabilizationSettings = new PIDSettingsMinMax { KpMin = 3.0f, KpMax = 3.0f, KiMin = 0.0f, KiMax = 0.0f, KdMin = 0.2f, KdMax = 0.4f };

    // -----------------------------------------------------------------------------------------------------
    // Private variables of this class:

    // Vector PID control object for stabilization
    private VectorPID stabilizationPID;

    // Rigidbody component of the drone
    private Rigidbody rb;

    // Variables to deal with the drone's rotation
    private Vector3 currentRotation;
    private Vector3 targetRotation;
    private Vector3 stabilizationError;

    // Variables for the PID controller to stabilize the drone
    private Vector3 stabilizationPIDOutput;
    private Vector3 correctionTorque;

    // Variables for a PID controller to stabilize the drone
    private float KpS;
    private float KiS;
    private float KdS;

    // -----------------------------------------------------------------------------------------------------
    // Start is called before the first frame update:

    void Start()
    {
        // Get the Rigidbody component of the drone
        rb = GetComponent<Rigidbody>();

        // Initialize the Vector PID control object for stabilization
        stabilizationPID = new VectorPID();
    }
    
    // -----------------------------------------------------------------------------------------------------
    // FixedUpdate is called at fixed time intervals, ideal for physical changes in objects:

    void FixedUpdate()
    {
        // Apply the PID controller to stabilize the rotation of the drone
        ApplyPIDStabilization();
    }

    // -----------------------------------------------------------------------------------------------------
    // Method to apply the PID controller to stabilize the drone: 

    void ApplyPIDStabilization()
    {
        // Get the current rotation of the drone (Euler angles)
        currentRotation = transform.localEulerAngles;

        // Define target orientation (level pitch & roll)
        targetRotation = new Vector3(0f, currentRotation.y, 0f);

        // Compute error between target and current rotation
        stabilizationError = targetRotation - currentRotation;

        // Normalize errors to [-180, 180]
        if (stabilizationError.x > 180) stabilizationError.x -= 360;
        if (stabilizationError.x < -180) stabilizationError.x += 360;
        if (stabilizationError.y > 180) stabilizationError.y -= 360;
        if (stabilizationError.y < -180) stabilizationError.y += 360;
        if (stabilizationError.z > 180) stabilizationError.z -= 360;
        if (stabilizationError.z < -180) stabilizationError.z += 360;

        // Set PID parameters for stabilization
        KpS = Mathf.Lerp(stabilizationSettings.KpMin, stabilizationSettings.KpMax, Mathf.Clamp01(stabilizationError.magnitude / PIDAdjustedVariableFromVelocity));
        KiS = Mathf.Lerp(stabilizationSettings.KiMin, stabilizationSettings.KiMax, Mathf.Clamp01(stabilizationError.magnitude / PIDAdjustedVariableFromVelocity));
        KdS = Mathf.Lerp(stabilizationSettings.KdMin, stabilizationSettings.KdMax, Mathf.Clamp01(stabilizationError.magnitude / PIDAdjustedVariableFromVelocity));

        // Apply PID control for stabilization
        stabilizationPIDOutput = stabilizationPID.Compute(stabilizationError, KpS, KiS, KdS);

        // Reset integral if nearly stable
        if (stabilizationError.magnitude < targetErrorMargin) stabilizationPID.Reset();

        // Apply torque correction (pitch=X, yaw=Y, roll=Z)
        correctionTorque = new Vector3(stabilizationPIDOutput.x, stabilizationPIDOutput.y, stabilizationPIDOutput.z);

        // Apply torque to the rigidbody
        rb.AddRelativeTorque(correctionTorque);
    }

}
