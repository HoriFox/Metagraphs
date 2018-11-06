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
        private StructureModule structureM;
        private static LogicModule Instance;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            structureM = StructureModule.GetInit();

            //Distribution(); TO DO
        }

        public static LogicModule GetInit()
        {
            return Instance;
        }

        public void LogicAdd(string nameNode = null) // TO DO
        {
            Dictionary<string, Structure> childList = null;
            childList = (nameNode != null) ? structureM.GetChild(nameNode) : structureM.structure;

            foreach (var part in childList)
            {
                // Если не Edge или Metaedge.
                if (!part.Value.Static)
                {
                    part.Value.position = new Vector3[1];
                    part.Value.position[0] = new Vector3(UnityEngine.Random.Range(0f, 5f), UnityEngine.Random.Range(1f, 6f), UnityEngine.Random.Range(0f, 5f));
                }
            }
        }

        public void LogicAdd2D(string nameNode) // TO DO
        {
            Dictionary<string, Structure> childList = structureM.GetChild(nameNode);
            Vector3 oldVector = structureM.structure[nameNode].GetPosition(0);
            structureM.structure[nameNode].Radius = 4;

            foreach (var part in childList)
            {
                part.Value.position[0] = new Vector3(oldVector.x + UnityEngine.Random.Range(-1f, 1f), oldVector.y, oldVector.z + UnityEngine.Random.Range(-1f, 1f));
            }
        }

        private void CalculationSolidAngle()
        {
            start = 1 - 2.0 / 360.0 * allowableAngle;
        }

        private void Distribution()
        {
            CalculationSolidAngle();

            parent = new GameObject("Parent");

            double theta;
            double phi;
            double x, y, z;

            double division = (end - start) / quantity;

            for (int i = 0; i < quantity; i++)
            {
                theta = golden_angle * i;
                z = end - division * i;

                phi = Math.Sqrt(1 - Math.Pow(z, 2));
                x = radius * Math.Cos(theta) * phi;
                y = radius * Math.Sin(theta) * phi;
                z *= radius;

                objectGame[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                // Временно! TO DO
                objectGame[i].transform.position = new Vector3(Convert.ToSingle(x), Convert.ToSingle(y), Convert.ToSingle(z));
                objectGame[i].transform.parent = parent.transform;
                objectGame[i].transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            }
            parent.transform.position = position;
            parent.transform.eulerAngles = bias;
        }

        GameObject parent;

        public Vector3 position;

        [Range(1f, 359f)] // Срезал с двух сторон по 1 для феншуя.
        public double allowableAngle = 360;

        public Vector3 bias = new Vector3(0f, 0f, 0f);

        public double radius = 3;
        public int quantity = 1000;

        public bool rebuild = false;

        private double start = -0.999;
        private double end = 0.999;

        private double golden_angle = Math.PI * (5 - Math.Sqrt(5));

        //void Update()
        //{
        //    if (rebuild)
        //    {
        //        Destroy(parent);
        //        Distribution();
        //        rebuild = false;
        //    }
        //}

    }
}
