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
using System; // Library to use Serializable attribute

// Class to represent a single LIDAR point
[Serializable] public class LidarPoint
{
    public float angle; // Angle in degrees
    public float distance; // Distance in meters

    // Constructor to initialize a LidarPoint with angle and distance
    public LidarPoint(float angle, float distance)
    {
        this.angle = angle;
        this.distance = distance;
    }
}
