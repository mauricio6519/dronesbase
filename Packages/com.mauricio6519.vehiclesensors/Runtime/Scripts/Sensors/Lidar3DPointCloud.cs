using UnityEngine;

public class Lidar3DPointCloud : MonoBehaviour
{
    [Header("LiDAR Settings")]
    public int horizontalRays = 360;     // rayos por capa horizontal
    public int verticalLayers = 64;      // número de capas verticales
    public float verticalFOV = 30f;      // ángulo vertical total (+/-15°)
    public float maxDistance = 50f;      // distancia máxima
    public float scanRate = 0.1f;        // frecuencia de escaneo en segundos

    [Header("Optional Visualization")]
    public bool drawGizmos = false;       // dibujar puntos en editor
    public float gizmoSize = 0.02f;
    public Color gizmoColor = Color.green;

    private float timer = 0f;
    private Vector3[] pointCloud;        // almacena todos los puntos 3D
    private int totalRays;

    void Start()
    {
        totalRays = horizontalRays * verticalLayers;
        pointCloud = new Vector3[totalRays];
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= scanRate)
        {
            Scan();
            timer = 0f;
        }
    }

    void Scan()
    {
        Vector3 origin = transform.position;
        int index = 0;

        for (int v = 0; v < verticalLayers; v++)
        {
            // Ángulo vertical de esta capa
            float verticalAngle = Mathf.Lerp(-verticalFOV / 2f, verticalFOV / 2f, v / (float)(verticalLayers - 1));

            for (int h = 0; h < horizontalRays; h++)
            {
                float horizontalAngle = h * 360f / horizontalRays;

                // Dirección del rayo
                Quaternion rotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0f);
                Vector3 dir = rotation * transform.forward;

                // Raycast
                if (Physics.Raycast(origin, dir, out RaycastHit hit, maxDistance))
                    pointCloud[index] = hit.point;
                else
                    pointCloud[index] = origin + dir * maxDistance;

                index++;
            }
        }
    }

    // Devuelve la nube de puntos como Vector3[]
    public Vector3[] GetPointCloud()
    {
        return pointCloud;
    }

    // Dibujar puntos en Scene View con Gizmos (solo editor)
    void OnDrawGizmos()
    {
        if (!drawGizmos || pointCloud == null)
            return;

        Gizmos.color = gizmoColor;
        foreach (var p in pointCloud)
        {
            Gizmos.DrawSphere(p, gizmoSize);
        }
    }
}
