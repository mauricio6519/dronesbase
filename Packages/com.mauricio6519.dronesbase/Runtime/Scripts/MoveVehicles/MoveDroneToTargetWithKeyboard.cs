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
public class MoveDroneToTargetWithKeyboard : MonoBehaviour
{
    // -----------------------------------------------------------------------------------------------------
    // Public variables that appear in the Inspector:

    // Target movement parameters
    [Header("Speed of movement and rotation")]
    public float moveSpeed = 3f; // Speed of movement
    public float rotationSpeed = 100f; // Speed of rotation
    public bool isVisible = true; // Whether the target is visible or not
    public string targetName = "DroneTarget"; // Name of the target object

    // -----------------------------------------------------------------------------------------------------
    // Private variables of this class:

    // Reference to the target object
    private GameObject targetObject;

    // Reference to the DronePIDController script of the target object
    private DronePIDController dronePIDController;

    // -----------------------------------------------------------------------------------------------------
    // Start is called before the first frame update:

    void Start()
    {
        // Find the target object by name
        targetObject = GameObject.Find(targetName);
        
        // Get the DronePIDController script from the target object
        if (targetObject != null) dronePIDController = targetObject.GetComponent<DronePIDController>();
    }

    // -----------------------------------------------------------------------------------------------------
    // FixedUpdate is called at fixed time intervals, ideal for physical changes in objects:

    void FixedUpdate()
    {
        // Set the visibility of the target object
        SetVisible(isVisible);
        
        // Update the target position and rotation in the DronePIDController script using this object's transform
        if (dronePIDController != null)
        {
            dronePIDController.targetData.position = transform.position;
            dronePIDController.targetData.yRotation = transform.eulerAngles.y;
        }
    }

    // -----------------------------------------------------------------------------------------------------
    // Update is called once per frame:

    void Update()
    {
        // Movement relative to the object's orientation (local space)
        if (Input.GetKey(KeyCode.U)) // Move forward
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.J)) // Move backward
        {
            transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.H)) // Move left
        {
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.K)) // Move right
        {
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
        }

        // Vertical movement (global)
        if (Input.GetKey(KeyCode.UpArrow)) // Move up
        {
            transform.Translate(Vector3.up * moveSpeed * Time.deltaTime, Space.World);
        }
        if (Input.GetKey(KeyCode.DownArrow)) // Move down
        {
            transform.Translate(Vector3.down * moveSpeed * Time.deltaTime, Space.World);
        }

        // Rotation with left/right arrows
        if (Input.GetKey(KeyCode.LeftArrow)) // Rotate left
        {
            transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime, Space.World);
        }
        if (Input.GetKey(KeyCode.RightArrow)) // Rotate right
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
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
