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
public class DronePIDController : MonoBehaviour
{
    // -----------------------------------------------------------------------------------------------------
    // Public or SerializeField variables that appear in the Inspector:

    // Target object variables
    [Header("Target Position and Rotation")]
    public TargetData targetData; 

    // Margin of error to consider the target reached
    [Header("Target Error Margin")]
    public float targetErrorMargin = 0.1f;

    // Set the adjusted variable from velocity correction, may need to be optimized
    [Header("PID Velocity Variable")]
    public float PIDAdjustedVariableFromVelocity = 20.0f;

    // PID settings for each control axis, may need to be optimized
    [Header("Throttle PID")]
    public PIDSettingsMinMax throttleSettings = new PIDSettingsMinMax { KpMin = 1.5f, KpMax = 2.0f, KiMin = 0.0f, KiMax = 0.1f, KdMin = 1.5f, KdMax = 3.5f };

    [Header("Pitch PID")]
    public PIDSettingsMinMax pitchSettings = new PIDSettingsMinMax { KpMin = 1.0f, KpMax = 4.0f, KiMin = 0.0f, KiMax = 0.1f, KdMin = 1.0f, KdMax = 2.0f };

    [Header("Roll PID")]
    public PIDSettingsMinMax rollSettings = new PIDSettingsMinMax { KpMin = 1.0f, KpMax = 4.0f, KiMin = 0.0f, KiMax = 0.1f, KdMin = 1.0f, KdMax = 2.0f};

    [Header("Yaw PID")]
    public PIDSettingsMinMax yawSettings = new PIDSettingsMinMax { KpMin = 3.0f, KpMax = 3.0f, KiMin = 0.0f, KiMax = 0.0f, KdMin = 0.2f, KdMax = 2.0f };

    // -----------------------------------------------------------------------------------------------------
    // Private variables of this class:

    // DroneDynamics component of the drone
    private DroneDynamics droneDynamics;

    // Target orientation of the drone
    private float targetOrientation;

    // PID control variables
    private float KpThrottle, KiThrottle, KdThrottle;    
    private float KpPitch, KiPitch, KdPitch;
    private float KpRoll, KiRoll, KdRoll;
    private float KpYaw, KiYaw, KdYaw;

    // PID control objects
    private PID throttlePID;
    private PID pitchPID;
    private PID rollPID;
    private PID yawPID;

    // Rigidbody component of the drone
    private Rigidbody rb;

    // Target position
    private Vector3 targetPosition;

    // Variables to calculate the error in the PID control
    private float currentY;
    private float yError;
    private float currentZ;
    private float currentX;
    private float zError;
    private float xError;
    private float currentYaw;
    private float yawError;

    // -----------------------------------------------------------------------------------------------------
    // Start is called before the first frame update:

    void Start()
    {
        // Get the Rigidbody component of the drone
        rb = GetComponent<Rigidbody>();

        // Get the DroneDynamics component
        droneDynamics = GetComponent<DroneDynamics>();

        // Initialize the PID control objects
        throttlePID = new PID();
        pitchPID = new PID();
        rollPID = new PID();
        yawPID = new PID();

        // Initialize the target data
        targetData.position = transform.position;
        targetData.yRotation = transform.localEulerAngles.y;
    }
    
    // -----------------------------------------------------------------------------------------------------
    // FixedUpdate is called at fixed time intervals, ideal for physical changes in objects:

    void FixedUpdate()
    {
        // If the droneDynamics is not assigned, exit
        if(droneDynamics == null) return;
        
        // Get the target position and orientation
        targetPosition = targetData.position;
        targetOrientation = targetData.yRotation;
        
        // Apply the Yaw, Throttle, Pitch, and Roll PID controllers
        ApplyYawPIDControl();
        ApplyThrottlePIDControl();
        ApplyPitchRollPIDControl();
    }

    // -----------------------------------------------------------------------------------------------------
    // Method to apply the throttle PID control:

    void ApplyThrottlePIDControl()
    {
        // Get the current position of the drone
        currentY = transform.position.y;

        // Calculate the error between the target position and the current position
        yError = targetPosition.y - currentY;

        // Set the PID control parameters for throttle
        KpThrottle = Mathf.Lerp(throttleSettings.KpMin, throttleSettings.KpMax, Mathf.Clamp01( MathF.Abs(rb.velocity.y) / PIDAdjustedVariableFromVelocity ));
        KiThrottle = Mathf.Lerp(throttleSettings.KiMin, throttleSettings.KiMax, Mathf.Clamp01( MathF.Abs(yError) / PIDAdjustedVariableFromVelocity ));
        KdThrottle = Mathf.Lerp(throttleSettings.KdMin, throttleSettings.KdMax, Mathf.Clamp01( MathF.Abs(rb.velocity.y) / PIDAdjustedVariableFromVelocity ));

        // Apply the PID control to the throttle
        droneDynamics.throttle = throttlePID.Compute(yError, KpThrottle, KiThrottle, KdThrottle);

        // Reset the PID control to the throttle
        if(Mathf.Abs(yError) < targetErrorMargin) throttlePID.Reset();
    }

    // -----------------------------------------------------------------------------------------------------
    // Method to apply the pitch and roll PID control:

    void ApplyPitchRollPIDControl()
    {
        // Get the current position of the drone in the z and x axis
        currentZ = transform.position.z;
        currentX = transform.position.x;
        
        // Calculate the error between the target position and the current position
        zError = targetPosition.z - currentZ;
        xError = targetPosition.x - currentX;

        // Set the PID control parameters for pitch
        KpPitch = Mathf.Lerp(pitchSettings.KpMin, pitchSettings.KpMax, Mathf.Clamp01( MathF.Abs(rb.velocity.z) / PIDAdjustedVariableFromVelocity ));
        KiPitch = Mathf.Lerp(pitchSettings.KiMin, pitchSettings.KiMax, Mathf.Clamp01( MathF.Abs(zError) / PIDAdjustedVariableFromVelocity ));
        KdPitch = Mathf.Lerp(pitchSettings.KdMin, pitchSettings.KdMax, Mathf.Clamp01( MathF.Abs(rb.velocity.z) / PIDAdjustedVariableFromVelocity ));

        // Set the PID control parameters for roll
        KpRoll = Mathf.Lerp(rollSettings.KpMin, rollSettings.KpMax, Mathf.Clamp01( MathF.Abs(rb.velocity.x) / PIDAdjustedVariableFromVelocity ));
        KiRoll = Mathf.Lerp(rollSettings.KiMin, rollSettings.KiMax, Mathf.Clamp01( MathF.Abs(xError) / PIDAdjustedVariableFromVelocity ));
        KdRoll = Mathf.Lerp(rollSettings.KdMin, rollSettings.KdMax, Mathf.Clamp01( MathF.Abs(rb.velocity.x) / PIDAdjustedVariableFromVelocity ));
        
        // Apply the PID control to the pitch
        droneDynamics.pitch = pitchPID.Compute(zError, KpPitch, KiPitch, KdPitch);

        // Apply the PID control to the roll
        droneDynamics.roll = rollPID.Compute(xError, KpRoll, KiRoll, KdRoll);

        // Reset the PID control to the pitch
        if(Mathf.Abs(zError) < targetErrorMargin) pitchPID.Reset();

        // Reset the PID control to the roll
        if(Mathf.Abs(xError) < targetErrorMargin) rollPID.Reset();
    }

    // -----------------------------------------------------------------------------------------------------
    // Method to apply the yaw PID control:

    void ApplyYawPIDControl()
    {
        // Get the current yaw of the drone
        currentYaw = transform.localEulerAngles.y;

        // Calculate the error between the target orientation and the current yaw
        yawError = targetOrientation - currentYaw;

        // Normalize the yaw error
        if (yawError > 180) yawError -= 360;
        if (yawError < -180) yawError += 360;

        // Set the PID control parameters for yaw
        KpYaw = Mathf.Lerp(yawSettings.KpMin, yawSettings.KpMax, Mathf.Clamp01(MathF.Abs(yawError) / PIDAdjustedVariableFromVelocity));
        KiYaw = Mathf.Lerp(yawSettings.KiMin, yawSettings.KiMax, Mathf.Clamp01(MathF.Abs(yawError) / PIDAdjustedVariableFromVelocity));
        KdYaw = Mathf.Lerp(yawSettings.KdMin, yawSettings.KdMax, Mathf.Clamp01(MathF.Abs(yawError) / PIDAdjustedVariableFromVelocity));

        // Apply the PID control to the yaw
        droneDynamics.yaw = yawPID.Compute(yawError, KpYaw, KiYaw, KdYaw);

        // Reset the PID control to the yaw
        if (Mathf.Abs(yawError) < targetErrorMargin) yawPID.Reset();
    }

}
