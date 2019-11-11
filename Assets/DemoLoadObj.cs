using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoLoadObj : MonoBehaviour
{
    public void LoadModel(string path, string nameModel)
    {
        Mesh meshHolder = FastObjImporter.Instance.ImportFile(path + "/" + nameModel);
        meshHolder.name = nameModel;
        GetComponent<MeshFilter>().mesh = meshHolder;

        Destroy(GetComponent<SphereCollider>());
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        meshCollider.enabled = true;
        meshCollider.sharedMesh = meshHolder;
    }
}
