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
using UnityEngine; // Unity Engine library to use in MonoBehaviour classes

// Class to move the drone from the keyboard
public class MoveDroneKeyboard : MonoBehaviour
{
    // -----------------------------------------------------------------------------------------------------
    // Public variables that appear in the Inspector:

    // Target movement parameters
    [Header("Target settings")]
    public string targetName = "prefabDrone1Lidar"; // Name of the target object
    public float axisSpeed = 1f; // Speed of axis change
    public float returnSpeed = 10f; // Speed of axis return to zero

    [Header("Control values used")]
    [Range(0f, 1f)] public float throttle;
    [Range(-1f, 1f)] public float yaw;
    [Range(-1f, 1f)] public float pitch;
    [Range(-1f, 1f)] public float roll;

    // -----------------------------------------------------------------------------------------------------
    // Private variables of this class:

    // Reference to the target object
    private GameObject targetObject;

    // Reference to the CarDynamics script of the target object
    private DroneDynamics droneDynamics;

    // Reference to the Car PIDController scripts of the target object
    private DronePIDController dronePIDController;

    // -----------------------------------------------------------------------------------------------------
    // Start is called before the first frame update:

    void Start()
    {
        // Find the target object by name
        targetObject = GameObject.Find(targetName);
        
        // Get the car scripts from the target object
        if (targetObject != null)
        {
            droneDynamics = targetObject.GetComponent<DroneDynamics>();
            dronePIDController = targetObject.GetComponent<DronePIDController>();
        }

        // Disable the CarPIDController script to allow manual control
        if(dronePIDController != null) dronePIDController.enabled = false;

        throttle = 0.5f;
    }

    // -----------------------------------------------------------------------------------------------------
    // Update is called once per frame:

    void Update()
    {
        // Throttle (U / J)
        if (Input.GetKey(KeyCode.U))
            throttle += axisSpeed * Time.deltaTime;
        else if (Input.GetKey(KeyCode.J))
            throttle -= axisSpeed * Time.deltaTime;
        else
            throttle = Mathf.MoveTowards(throttle, 0.5f, returnSpeed * Time.deltaTime);

        // Yaw (H / K)
        if (Input.GetKey(KeyCode.H))
            yaw = -1;
        else if (Input.GetKey(KeyCode.K))
            yaw = 1;
        else
            yaw = Mathf.MoveTowards(yaw, 0f, returnSpeed * Time.deltaTime);

        // Pitch (↑ / ↓)
        if (Input.GetKey(KeyCode.UpArrow))
            pitch += axisSpeed * Time.deltaTime;
        else if (Input.GetKey(KeyCode.DownArrow))
            pitch -= axisSpeed * Time.deltaTime;
        else
            pitch = Mathf.MoveTowards(pitch, 0f, returnSpeed * Time.deltaTime);

        // Roll (← / →)
        if (Input.GetKey(KeyCode.LeftArrow))
            roll -= axisSpeed * Time.deltaTime;
        else if (Input.GetKey(KeyCode.RightArrow))
            roll += axisSpeed * Time.deltaTime;
        else
            roll = Mathf.MoveTowards(roll, 0f, returnSpeed * Time.deltaTime);

        // Clamp values between -1 and 1
        throttle = Mathf.Clamp(throttle, 0f, 1f);
        yaw = Mathf.Clamp(yaw, -1f, 1f);
        pitch = Mathf.Clamp(pitch, -1f, 1f);
        roll = Mathf.Clamp(roll, -1f, 1f);

        // Update the target position in the DroneDynamics script using keyboard input
        droneDynamics.throttle = throttle;
        droneDynamics.yaw      = yaw;
        droneDynamics.pitch    = pitch;
        droneDynamics.roll     = roll;
    }
    
}
