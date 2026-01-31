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

// Class to visualize LIDAR data
public class LidarVisualizer : MonoBehaviour
{
    
    // -----------------------------------------------------------------------------------------------------
    // Public variables that appear in the Inspector:

    // LIDAR settings
    // 4 → intervals of 90°
    // 8 → intervals of 45°
    // 12 → intervals of 30°
    // 36 → intervals of 10°
    [Header("LIDAR Settings")]
    public int numberIntervalGroupings = 8; // Number of intervals for grouping LIDAR points
    public bool getReduceScan = true; // Whether to get reduced scan data

    // Visualization settings
    [Header("Visualization")]
    public bool showDrawScan = false;
    public Color fullScanColor = Color.red;
    public Color reducedScanColor = Color.green;

    // LIDAR data
    [Header("LIDAR Data")]
    public List<LidarPoint> lidarData;

    // -----------------------------------------------------------------------------------------------------
    // Private variables of this class:

    // Reference to the SimpleLidarSensor component
    private SimpleLidarSensor lidarSensor;

    // LIDAR scan data storage
    private List<LidarPoint> fullScan;
    private List<LidarPoint> reducedScan;

    // -----------------------------------------------------------------------------------------------------
    // Start is called before the first frame update:

    void Start()
    {
        
        // Get the SimpleLidarSensor component
        lidarSensor = GetComponent<SimpleLidarSensor>();

        // Check if the component exists, if not, log an error and return
        if (lidarSensor == null)
        {
            Debug.LogError("SimpleLidarSensor component not found on this GameObject.");
            return;
        }

        // Initialize the scan data lists
        fullScan = new List<LidarPoint>();
        reducedScan = new List<LidarPoint>();

        // Initialize the lidarData list
        lidarData = new List<LidarPoint>();

    }

    // -----------------------------------------------------------------------------------------------------
    // FixedUpdate is called at fixed time intervals, ideal for physical changes in objects:

    void FixedUpdate()
    {
        
        // Check if the LIDAR sensor component is available
        if (lidarSensor == null) return;

        // Get the full LIDAR scan data
        fullScan = lidarSensor.Scan();

        // Process the scan data based on the settings
        if (getReduceScan)
        {
            
            // Get the reduced LIDAR scan data
            reducedScan = lidarSensor.GetReducedScanGrouped(fullScan, numberIntervalGroupings);

            // Update the lidarData variable
            lidarData = reducedScan;

        }
        else
        {
            
            // Update the lidarData variable
            lidarData = fullScan;

        }

    }

    // -----------------------------------------------------------------------------------------------------
    // Update is called once per frame:

    void Update()
    {

        // Draw the scan data if enabled
        if (getReduceScan)
        {
            if (showDrawScan) DrawScan(reducedScan, reducedScanColor);
        }
        else
        {
            if (showDrawScan) DrawScan(fullScan, fullScanColor);
        } 

    }

    // -----------------------------------------------------------------------------------------------------
    // Method to draw the LIDAR scan data in the Scene view:

    void DrawScan(List<LidarPoint> scanData, Color color)
    {
        
        // Draw rays for each point in the scan data
        Vector3 origin = lidarSensor.transform.position + lidarSensor.transform.TransformDirection(lidarSensor.offset);

        // Iterate through the scan data
        foreach (var point in scanData)
        {
            
            // Calculate the direction of the ray in world coordinates
            float angleRad = point.angle * Mathf.Deg2Rad;
            Vector3 localDirection = new Vector3(Mathf.Cos(angleRad), 0, Mathf.Sin(angleRad));
            Vector3 worldDirection = lidarSensor.transform.TransformDirection(localDirection);

            // Draw the ray in the Scene view
            if (showDrawScan) Debug.DrawRay(origin, worldDirection * point.distance, color);
        }

    }

}
