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

// Class to show the target position of the drone
public class ShowPIDTarget : MonoBehaviour
{
    // -----------------------------------------------------------------------------------------------------
    // Public variables that appear in the Inspector:

    // Target visibility parameters
    [Header("Target visibility settings")]
    public bool isVisible = true; // Whether the target is visible or not
    public string targetName = "prefabDrone1Lidar"; // Name of the target object

    // -----------------------------------------------------------------------------------------------------
    // Private variables of this class:

    // Reference to the target object
    private GameObject targetObject;

    // Reference to the Drone or Car PIDController scripts of the target object
    private DronePIDController dronePIDController;
    private CarPIDController carPIDController;

    // -----------------------------------------------------------------------------------------------------
    // Start is called before the first frame update:

    void Start()
    {
        // Find the target object by name
        targetObject = GameObject.Find(targetName);

        // Get the DronePIDController component from the target object
        if (targetObject != null)
        {
            dronePIDController = targetObject.GetComponent<DronePIDController>();
            carPIDController = targetObject.GetComponent<CarPIDController>();
        }
    }

    // -----------------------------------------------------------------------------------------------------
    // FixedUpdate is called at fixed time intervals, ideal for physical changes in objects:

    void FixedUpdate()
    {
        // Set the visibility of the target object
        SetVisible(isVisible);
        
        // Update the position and rotation of this object to match the target data from the drone
        if (dronePIDController != null)
        {
            transform.position = dronePIDController.targetData.position;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, dronePIDController.targetData.yRotation, transform.eulerAngles.z);
        }

        // Update the position and rotation of this object to match the target data from the car
        if (carPIDController != null)
        {
            transform.position = carPIDController.targetPosition;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0f, transform.eulerAngles.z);
        }
    }

    // -----------------------------------------------------------------------------------------------------
    // Method to set the visibility of the target object:

    public void SetVisible(bool visible)
    {
        // Get all Renderer components in this object and its children
        var renderers = GetComponentsInChildren<Renderer>();

        // Enable or disable each renderer based on the visibility parameter
        foreach (Renderer r in renderers) r.enabled = visible;
    }

}
