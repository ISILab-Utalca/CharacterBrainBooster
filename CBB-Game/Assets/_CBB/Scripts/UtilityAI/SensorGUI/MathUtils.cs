using UnityEngine;

public static class MathUtils
{
    public static Vector3[] GetCircleVertices(Vector3 center, float radius, int numVertices, Vector3 normalVector)
    {
        Vector3[] vertices = new Vector3[numVertices];
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, normalVector);

        for (int i = 0; i < numVertices; i++)
        {
            float angle = i * (360f / numVertices);
            Vector3 vertexPosition = center + rotation * (Quaternion.Euler(0f, angle, 0f) * (Vector3.forward * radius));
            vertices[i] = vertexPosition;
        }

        return vertices;
    }
}
