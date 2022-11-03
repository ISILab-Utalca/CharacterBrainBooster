using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Curves
{
    public static float CuadratricaInvertida(float value, float e = 1f, float dx = 0f, float dy = 0f)
    {
        return (1f / (Mathf.Pow(value, e) + dx)) + dy;
    }

    public static float Cuadratica(float value, float e = 2f, float dx = 0f, float dy = 0f)
    {
        return Mathf.Pow(value + dx, e) + dy;
    }

    public static float Escalonada(float value, float n = 0.5f, float max = 1f, float min = 0.1f)
    {
        return value >= n ? max : min;
    }

    public static float Sigmoide(float value,float n, float dx = 2.7f, float dy = 0f)
    {
        return (1 / (1 + Mathf.Pow(dx,-value + n))) + dy;
    }
}
