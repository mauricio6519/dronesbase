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
using System.Collections.Generic; // Library to use Lists
using UnityEngine; // Unity Engine library to use in MonoBehaviour classes

// Class to create a simple LIDAR sensor using raycasting:
public class SimpleLidarSensor : MonoBehaviour
{
    
    // -----------------------------------------------------------------------------------------------------
    // Public variables that appear in the Inspector:

    public int rayCount = 360; // Number of rays in the LIDAR scan
    public float maxRange = 50f; // Maximum range of the LIDAR in meters
    public Vector3 offset = new Vector3(0, 0.385f, 0); // Offset of the LIDAR from the drone's center

    // -----------------------------------------------------------------------------------------------------
    // Method to perform a LIDAR scan and return the distances:

    public List<LidarPoint> Scan()
    {
        
        // List to store the scan data
        List<LidarPoint> scanData = new List<LidarPoint>();
        
        // Calculate the origin of the LIDAR rays
        Vector3 origin = transform.position + transform.TransformDirection(offset);

        // Perform raycasting for each angle
        for (int i = 0; i < rayCount; i++)
        {
            
            // Calculate the angle in degrees and radians
            float angleDeg = i;
            float angleRad = i * Mathf.Deg2Rad;

            // Calculate the direction of the ray in world coordinates
            Vector3 localDirection = new Vector3(Mathf.Cos(angleRad), 0, Mathf.Sin(angleRad));
            Vector3 worldDirection = transform.TransformDirection(localDirection);

            // Perform the raycast and get the distance
            float distance = maxRange;
            if (Physics.Raycast(origin, worldDirection, out RaycastHit hit, maxRange))
                distance = hit.distance;

            // Store the angle and distance in the scan data
            scanData.Add(new LidarPoint(angleDeg, distance));

        }

        // Return the complete scan data
        return scanData;

    }

    // -----------------------------------------------------------------------------------------------------
    // Method to reduce the LIDAR scan data by grouping points into intervals:

    public List<LidarPoint> GetReducedScanGrouped(List<LidarPoint> fullScan, int groupCount)
    {
        
        // Size of each group in degrees
        float groupSize = 360f / groupCount;

        // Final list with all reduced results
        List<LidarPoint> finalReduced = new List<LidarPoint>();

        // Process each group
        for (int g = 0; g < groupCount; g++)
        {
            
            // Define the angle range for this group
            float startAngle = g * groupSize;
            float endAngle = startAngle + groupSize;

            // Extract the points that fall within this group
            List<LidarPoint> groupPoints = fullScan.FindAll(p =>
                p.angle >= startAngle && p.angle < endAngle
            );

            // Variables for clustering within the group
            List<LidarPoint> reducedGroup = new List<LidarPoint>();
            bool insideObject = false;
            LidarPoint closestPoint = null;

            // Iterate through the points in the group
            foreach (var point in groupPoints)
            {
                if (point.distance < maxRange) // valid detection
                {
                    if (!insideObject)
                    {
                        insideObject = true;
                        closestPoint = point;
                    }
                    else
                    {
                        if (point.distance < closestPoint.distance)
                            closestPoint = point;
                    }
                }
                else
                {
                    if (insideObject)
                    {
                        reducedGroup.Add(closestPoint);
                        insideObject = false;
                    }
                }
            }

            // If the last point in the group was part of an object
            if (insideObject)
                reducedGroup.Add(closestPoint);

            // Add group results to the final list
            finalReduced.AddRange(reducedGroup);

        }

        // Return the reduced scan data
        return finalReduced;

    }
    
}
