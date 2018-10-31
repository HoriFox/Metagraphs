using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTransform : MonoBehaviour
{

    public Transform LabelPrefab;
    private Transform mainCamera;

    public enum CurrentAxis { NotSet = -1, X = 0, Y = 1, Z = 2 };
    public CurrentAxis currentAxis = CurrentAxis.NotSet;

    private void Start()
    {
        mainCamera = Camera.main.transform;
    }

    void OnMouseDrag()
    {
        if (currentAxis == CurrentAxis.NotSet) return;
        if (currentAxis == CurrentAxis.X)
        {
            float xx = Input.GetAxis("Mouse X") / 2;
            if (mainCamera.eulerAngles.y < 270 && mainCamera.eulerAngles.y > 90)
            {
                xx *= (-1);
            }
            LabelPrefab.transform.localPosition += Vector3.right * xx;
            return;
        }
        if (currentAxis == CurrentAxis.Y)
        {
            float yy = Input.GetAxis("Mouse Y") / 2;
            if (mainCamera.eulerAngles.x < 270 && mainCamera.eulerAngles.x > 90)
            {
                yy *= (-1);
            }
            LabelPrefab.transform.localPosition += Vector3.up * yy;
            return;
        }
        if (currentAxis == CurrentAxis.Z)
        {
            float zz = Input.GetAxis("Mouse X") / 2;
            if (mainCamera.eulerAngles.y < 180 && mainCamera.eulerAngles.y > 0)
            {
                zz *= (-1);
            }
            LabelPrefab.transform.localPosition += Vector3.forward * zz;
            return;
        }
    }
}
