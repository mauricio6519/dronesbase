using UnityEngine;

public class Lidar360Visualizer : MonoBehaviour
{
    [Header("Lidar Settings")]
    public int numRays = 360;           // resolución del LiDAR
    public float maxDistance = 10f;     // distancia máxima que puede detectar
    public float updateRate = 0.1f;     // frecuencia de actualización en segundos

    [Header("Visualization Settings")]
    public Color pointColor = Color.red;
    public float pointSize = 0.05f;     // tamaño visual del punto

    private float[] distances;           // arreglo de distancias medidas
    private GameObject[] points;         // objetos visuales para cada rayo
    private float timer = 0f;

    void Start()
    {
        distances = new float[numRays];
        points = new GameObject[numRays];

        // Crear un pequeño punto para cada rayo
        for (int i = 0; i < numRays; i++)
        {
            GameObject p = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            p.transform.localScale = Vector3.one * pointSize;
            p.GetComponent<Renderer>().material.color = pointColor;
            Destroy(p.GetComponent<Collider>()); // no queremos colisiones
            points[i] = p;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= updateRate)
        {
            Scan();
            timer = 0f;
        }
    }

    void Scan()
    {
        float angleIncrement = 360f / numRays;

        for (int i = 0; i < numRays; i++)
        {
            float angle = i * angleIncrement;
            Vector3 dir = Quaternion.Euler(0, angle, 0) * transform.forward;

            // Raycast
            if (Physics.Raycast(transform.position, dir, out RaycastHit hit, maxDistance))
            {
                distances[i] = hit.distance;
            }
            else
            {
                distances[i] = maxDistance;
            }

            // Actualizar posición del punto visual
            Vector3 pointPos = transform.position + dir * distances[i];
            points[i].transform.position = pointPos;

            // Debug opcional
            Debug.DrawRay(transform.position, dir * distances[i], Color.green, updateRate);
        }
    }
}
