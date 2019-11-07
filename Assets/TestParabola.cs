using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestParabola : MonoBehaviour
{
    public Transform someObject; //object that moves along parabola.
    float objectT = 0; //timer for that object

    public Transform Ta, Tb; // начало и конец
    public float h; // желаемая высота параболы
    public int quality; // Качество линии, сколько "полигонов".

    public Transform Test;

    [Range(0, 360)]
    public int angle; // Поворот кривой линии.

    Vector3 a, b;

    void Update()
    {
        a = Ta.position;
        b = Tb.position;

        //Shows how to animate something following a parabola
        objectT = Time.time % 1; //completes the parabola trip in one second
        someObject.position = SampleParabola(a, b, h, objectT);
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(Vector3.zero, b - a);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(b, a);

        Gizmos.color = Color.red;

        Vector3 lastP = SampleParabola(a, b, h, 0);
        for (float i = 0; i < quality + 1; i++)
        {
            Vector3 p = SampleParabola(a, b, h, i / quality);
            //Gizmos.color = i % 2 == 0 ? Color.blue : Color.green;
            Gizmos.DrawLine(lastP, p);
            lastP = p;
        }
    }

    public Vector3 to;

    Vector3 SampleParabola(Vector3 start, Vector3 end, float height, float t)
    {
        float parabolicT = t * 2 - 1;
        //Когда Y старт и Y конец находятся практически на одном уровне, то упрощаем работу, иначе приходится ложнее.
        if (Mathf.Abs(start.y - end.y) < 0.1f)
        {
            // начало и конец примерно на одном уровне, представьте, что они более простое решение с меньшим количеством шагов
            Vector3 travelDirection = end - start;
            Vector3 result = start + t * travelDirection;
            result += Quaternion.AngleAxis(angle, travelDirection) *  new Vector3(0, (-parabolicT * parabolicT + 1) * height, 0);
            return result;

        }
        else
        {
            // начало и конец не уровень, становится все сложнее
            Vector3 travelDirection = end - start;
            Vector3 levelDirecteion = end - new Vector3(start.x, end.y, start.z);
            Vector3 right = Vector3.Cross(travelDirection, levelDirecteion);
            Vector3 up = Quaternion.AngleAxis(angle, travelDirection) * Vector3.Cross(right, travelDirection);
            if (end.y > start.y) up = -up;
            Vector3 result = start + t * travelDirection;
            result += ((-parabolicT * parabolicT + 1) * height) * up.normalized;
            return result;
        }
    }
}
