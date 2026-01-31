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

// Class to create a Vector PID controller to control the drone's position in 3D space:
public class VectorPID
{
    
    // -----------------------------------------------------------------------------------------------------
    // Private variables of this class:

    // PID controllers for each axis:
    private PID pidX = new PID();
    private PID pidY = new PID();
    private PID pidZ = new PID();

    // -----------------------------------------------------------------------------------------------------
    // Method to compute the PID controller for each axis independently:

    public Vector3 Compute(Vector3 error, float Kp, float Ki, float Kd)
    {
        
        // Compute PID for each axis:
        float outputX = pidX.Compute(error.x, Kp, Ki, Kd);
        float outputY = pidY.Compute(error.y, Kp, Ki, Kd);
        float outputZ = pidZ.Compute(error.z, Kp, Ki, Kd);

        // Return the PID outputs as a Vector3:
        return new Vector3(outputX, outputY, outputZ);

    }

    // -----------------------------------------------------------------------------------------------------
    // Method to reset the integral value of the controller:

    public void Reset()
    {
        pidX.Reset();
        pidY.Reset();
        pidZ.Reset();
    }
    
}
