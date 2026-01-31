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
using System; // Library to use Serializable attribute

// Class to hold PID settings for tuning
[Serializable] public class PIDSettings
{
    public float Kp;
    public float Ki;
    public float Kd;
}

// Class to hold PID settings for tuning using min and max values
[Serializable] public class PIDSettingsMinMax
{
    public float KpMin;
    public float KpMax;
    public float KiMin;
    public float KiMax;
    public float KdMin;
    public float KdMax;
}
// Class to hold target position and rotation data
[Serializable] public class TargetData
{
    public Vector3 position;
    public float yRotation; 
}
