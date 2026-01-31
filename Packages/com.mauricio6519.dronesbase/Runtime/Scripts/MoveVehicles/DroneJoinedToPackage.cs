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

// Enum to define the package states
public enum PackageState
{
    Idle,       // Package is not attached or delivered
    Attached,   // Package is attached to the drone
    Delivered   // Package has been delivered
}

// Class to join the package object with the drone object 
public class DroneJoinedToPackage : MonoBehaviour
{

    // -----------------------------------------------------------------------------------------------------
    // Public variables that appear in the Inspector:

    // Package object to be attached to the drone
    public Transform packageObject;

    // Package state to control attachment and delivery
    public PackageState packageState = PackageState.Idle;

    // -----------------------------------------------------------------------------------------------------
    // Private variables of this class:

    // Variables to join the package object with the drone object
    private FixedJoint packageJoint; 
    private Rigidbody packageRigidbody;

    // -----------------------------------------------------------------------------------------------------
    // Update is called once per frame:

    void Update()
    {
        
        // If the package object is not null
        if (packageObject != null)
        {
            
            // Get the Rigidbody component of the package object
            packageRigidbody = packageObject.GetComponent<Rigidbody>();

            // Handle package behavior based on its state
            switch (packageState)
            {
                case PackageState.Attached:
                    AttachPackage();
                    break;

                case PackageState.Delivered:
                    ReleasePackage();
                    break;

                case PackageState.Idle:
                default:
                    // Do nothing in Idle state
                    break;
            }

        }

    }

    // -----------------------------------------------------------------------------------------------------
    // Method to attach the package to the drone
    void AttachPackage()
    {
        // Create a FixedJoint if not already created and Rigidbody exists
        if (packageJoint == null && packageRigidbody != null)
        {
            packageJoint = gameObject.AddComponent<FixedJoint>();
            packageJoint.connectedBody = packageRigidbody;  
        }

        // After attaching, return state to Idle to prevent repeated attachment
        packageState = PackageState.Idle;
    }

    // -----------------------------------------------------------------------------------------------------
    // Method to release the package from the drone
    void ReleasePackage()
    {
        // Destroy the joint if it exists
        if (packageJoint != null)
        {
            Destroy(packageJoint);
            packageRigidbody.WakeUp();
        }

        // After releasing, return state to Idle
        packageState = PackageState.Idle;
    }

}
