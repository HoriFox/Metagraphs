using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nm
{
    public class AxlesInteraction : MonoBehaviour
    {
        public Transform labelPrefab;
        public GameObject view;
        Renderer viewRenderer;
        private Transform mainCamera;
        private Color32 saveColor;
        private Color32 selectColor = new Color32(255, 183, 0, 255);
        private bool isMouseOver = false;
        private bool isDrag = false;

        public enum CurrentAxis { X = 0, Y = 1, Z = 2 };
        public CurrentAxis currentAxis;
        Engine engine;

        private void Start()
        {
            engine = Engine.GetInit();
            mainCamera = Camera.main.transform;
            viewRenderer = view.GetComponent<Renderer>();
            saveColor = viewRenderer.material.color;
        }

        private void Update()
        {
            if (isMouseOver && engine.selectedAxis == -1)
            {
                viewRenderer.material.color = selectColor;
                engine.selectedAxis = (int)currentAxis;
            }
            if (!isMouseOver && !isDrag && engine.selectedAxis == (int)currentAxis)
            {
                viewRenderer.material.color = saveColor;
                engine.selectedAxis = -1;
            }
        }

        private void OnMouseEnter()
        {
            isMouseOver = true;
        }

        private void OnMouseExit()
        {
            isMouseOver = false;
        }

        void OnMouseUp()
        {
            isDrag = false;
        }

        private void OnMouseDrag()
        {
            isDrag = true;
            if (currentAxis == CurrentAxis.X)
            {
                float xx = Input.GetAxis("Mouse X") / 2;
                if (mainCamera.eulerAngles.y < 270 && mainCamera.eulerAngles.y > 90)
                {
                    xx *= (-1);
                }
                labelPrefab.transform.localPosition += Vector3.right * xx;
                return;
            }
            if (currentAxis == CurrentAxis.Y)
            {
                float yy = Input.GetAxis("Mouse Y") / 2;
                if (mainCamera.eulerAngles.x < 270 && mainCamera.eulerAngles.x > 90)
                {
                    yy *= (-1);
                }
                labelPrefab.transform.localPosition += Vector3.up * yy;
                return;
            }
            if (currentAxis == CurrentAxis.Z)
            {
                float zz = Input.GetAxis("Mouse X") / 2;
                if (mainCamera.eulerAngles.y < 180 && mainCamera.eulerAngles.y > 0)
                {
                    zz *= (-1);
                }
                labelPrefab.transform.localPosition += Vector3.forward * zz;
                return;
            }
        }

        //public float doubleTapDelay = 0.4f;
        //private float tapCount = 0;
        //private void DoubleTap(string methodLuck)
        //{
        //    tapCount++;
        //    if (tapCount == 2)
        //    {
        //        CancelInvoke("FailDoubleTap");
        //        tapCount = 0;
        //        Invoke(methodLuck, 0f);
        //        return;
        //    }

        //    Invoke("FailDoubleTap", doubleTapDelay);
        //}
        //private void FailDoubleTap()
        //{
        //    tapCount = 0;
        //}
    }
}
