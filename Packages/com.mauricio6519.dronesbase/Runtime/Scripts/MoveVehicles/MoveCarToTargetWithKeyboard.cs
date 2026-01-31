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

// Class to move the car from the keyboard
public class MoveCarToTargetWithKeyboard : MonoBehaviour
{
    // -----------------------------------------------------------------------------------------------------
    // Public variables that appear in the Inspector:

    // Target movement parameters
    [Header("Target settings")]
    public string targetName = "prefabTruckBlue"; // Name of the target object

    // -----------------------------------------------------------------------------------------------------
    // Private variables of this class:

    // Reference to the target object
    private GameObject targetObject;

    // Reference to the CarDynamics script of the target object
    private CarDynamics carDynamics;

    // Reference to the Car PIDController scripts of the target object
    private CarPIDController carPIDController;

    // -----------------------------------------------------------------------------------------------------
    // Start is called before the first frame update:

    void Start()
    {
        // Find the target object by name
        targetObject = GameObject.Find(targetName);
        
        // Get the car scripts from the target object
        if (targetObject != null)
        {
            carDynamics = targetObject.GetComponent<CarDynamics>();
            carPIDController = targetObject.GetComponent<CarPIDController>();
        }

        // Disable the CarPIDController script to allow manual control
        if(carPIDController != null) carPIDController.enabled = false;
    }

    // -----------------------------------------------------------------------------------------------------
    // Update is called once per frame:

    void Update()
    {
        // Update the target position in the CarDynamics script using keyboard input
        carDynamics.throttle = Input.GetAxis("Vertical");
        carDynamics.steering = Input.GetAxis("Horizontal");
        carDynamics.isBraking = Input.GetKey(KeyCode.Space);
    }

}
