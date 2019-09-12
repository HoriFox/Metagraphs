using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nm
{
    public class ArcTest : MonoBehaviour
    {
        public Vector3 first = new Vector3(0, 0, 0);
        public Vector3 second = new Vector3(10, 0, 0);
        public Vector3 additional = new Vector3();

        [Range(-10.0f, 10.0f)]
        public float factorAdditional = 5.0f;

        [Range(0f, 1f)]
        public float phase = 0;

        [Range(3, 20)]
        public int quality = 10;

        public bool rebuild = true;

        public Transform myObject;
        public Transform myLine;
        public Transform myAdditional;
        private LineRenderer lineRenderer;

        private void Start()
        {
            lineRenderer = myLine.GetComponent<LineRenderer>();
        }

        private void Update()
        {
            additional = GetPerpendicular(second - first) + first;

            Vector3 m1 = Vector3.Lerp(first, additional, phase);
            Vector3 m2 = Vector3.Lerp(additional, second, phase);
            myObject.position = Vector3.Lerp(m1, m2, phase);
            myAdditional.position = additional;

            if (rebuild)
            {
                DrawArc();
                //rebuild = false;
            }
        }

        private Vector3 GetPerpendicular(Vector3 inputVector)
        {
            //Vector3 spreadPos = first + Quaternion.AngleAxis(rotateUp, Vector3.up) * inputVector;

            Vector3 rotate = new Vector3(-inputVector.y, inputVector.x, inputVector.z);

            //rotate = Quaternion.Euler(rotate.x * Mathf.Sin(rotateUp / 2), rotate.y * Mathf.Cos(rotateUp / 2), rotate.z * Mathf.Cos(rotateUp / 2)) * rotate;

            rotate = rotate.normalized * factorAdditional;
            Vector3 offset = rotate + inputVector / 2;
            return offset;
        }

        [Range(0, 360)]
        public float rotateUp = 0;

        //private List<GameObject> gameObject = new List<GameObject>();

        private void DrawArc()
        {
            //DeleteObject(gameObject);
            //gameObject = new List<GameObject>();

            float dividerPhase = 1.0f / quality;
            float currentPhase = 0.0f;

            additional = GetPerpendicular(second - first) + first;

            lineRenderer.positionCount = quality + 1;

            //Vector3 lastPoint = first;

            int numberPhase = 0;
            while (numberPhase <= quality)
            {
                Vector3 m1 = Vector3.Lerp(first, additional, currentPhase);
                Vector3 m2 = Vector3.Lerp(additional, second, currentPhase);

                //Vector3 nextPoint = Vector3.Lerp(m1, m2, currentPhase);
                //if (numberPhase != 0)
                //{
                //    gameObject.AddRange(InitObject.Instance.InitLine(false, lastPoint, nextPoint, Color.blue, "test", isSimple: true));
                //}

                //lastPoint = nextPoint;

                lineRenderer.SetPosition(numberPhase, Vector3.Lerp(m1, m2, currentPhase));
                currentPhase += dividerPhase;
                numberPhase++;
            }
        }

        private void DeleteObject(List<GameObject> gameObject)
        {
            int k = 0;
            foreach (var part in gameObject)
            {
                Destroy(part);
                k++;
            }
            gameObject.Clear();
        }
    }
}
