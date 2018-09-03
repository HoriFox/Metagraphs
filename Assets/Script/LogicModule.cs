using System;
using System.Collections.Generic;
using UnityEngine;
namespace nm
{
    public class LogicModule : MonoBehaviour
    {
        public double ConvertToRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }

        public Vector3 GetCartesian(float radius, float verticalAngle, float horisontalAngle)
        {
            double radiansV = ConvertToRadians(verticalAngle);
            double radiansH = ConvertToRadians(horisontalAngle);

            float x = radius * (float)Math.Sin(radiansV) * (float)Math.Cos(radiansH);
            float y = radius * (float)Math.Cos(radiansV);
            float z = radius * (float)Math.Sin(radiansV) * (float)Math.Sin(radiansH);
            return new Vector3(x, y, z);
        }

        Dictionary<int, GameObject> objectGame = new Dictionary<int, GameObject>();

        void Create()
        {
            parent = new GameObject("Parent");

            double theta;
            double phi;
            double x, y, z;

            double golden_angle = Math.PI * (5 - Math.Sqrt(5));

            double division = (start - end) / quantity;

            for (int i = 0; i < quantity; i++)
            {
                theta = golden_angle * i;
                z = start - division * i;

                phi = Math.Sqrt(1 - Math.Pow(z, 2));
                x = radius * Math.Cos(theta) * phi;
                y = radius * Math.Sin(theta) * phi;
                z *= radius;

                objectGame[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                // Временно! TO DO
                objectGame[i].transform.position = new Vector3(Convert.ToSingle(x) + 3.74f, Convert.ToSingle(y) + 2.39f, Convert.ToSingle(z) + 4.06f);
                objectGame[i].transform.parent = parent.transform;
                objectGame[i].transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            }

            parent.transform.rotation = Quaternion.Euler(xBias, yBias, 0);
        }

        void Start()
        {
            //Vector3 coord = GetCartesian(3, 45, 45);
            //Debug.Log("x: " + coord.x + " y: " + coord.y + " z: " + coord.z);

            start = (1 - 1.0 / quantity);
            end = (1.0 / quantity - 1);

            Create();
        }

        GameObject parent;

        [Range(-0.999f, 0.999f)]
        public double start;
        [Range(0.999f, -0.999f)]
        public double end;

        [Range(0f, 360f)]
        public float xBias = 0;
        [Range(0f, 360f)]
        public float yBias = 0;

        public double radius = 3;
        public int quantity = 1000;

        public bool rebuild = false;

        void Update()
        {
            if (rebuild)
            {
                Destroy(parent);
                Create();
                rebuild = false;
            }
        }

    }
}
