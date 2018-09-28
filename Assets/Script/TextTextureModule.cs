//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace nm
//{
//    public class TextTextureModule : MonoBehaviour
//    {
//        public Font font;
//        //public MeshFilter meshFilterObj;

//        public GameObject v0;
//        public GameObject v1;
//        public GameObject v2;
//        public GameObject v3;

//        public Material mat; // материал, чтобы объект не был розовым
//        GameObject poly;
//        Mesh mesh;

//        private void Start()
//        {
//            CreateFont("Hello");
//        }

//        private void CreateFont(string output)
//        {
//            poly = new GameObject("poly");
//            mesh = new Mesh();

//            Vector3[] vertices = new Vector3[] {
//                    // задаём позиции вершин
//                    v0.transform.position,
//                    v1.transform.position,
//                    v2.transform.position,
//                    v3.transform.position,
//                };

//            Vector3[] normals = new Vector3[] {
//                    // все помнят, что нормаль всегда перпендикулярна плоскости из вершин?
//                    new Vector3(0, 1, 0),
//                    new Vector3(0, 1, 0),
//                    new Vector3(0, 1, 0),
//                    new Vector3(0, 1, 0),
//                };

//            int[] triangles = new int[] {
//                    0, 2, 3, // первый треугольник
//                    3, 1, 0, // второй треугольник
//                };


//            mesh.vertices = vertices;
//            mesh.normals = normals;
//            mesh.triangles = triangles;

//            poly.AddComponent<MeshRenderer>().material = mat; // чтобы отобразить наш меш; сразу вешаем материал

//            poly.GetComponent<MeshRenderer>().material.mainTexture = font.material.mainTexture;

//            for (int i = 0; i < output.Length; i++)
//            {
//                CharacterInfo character;
//                font.GetCharacterInfo(output[i], out character);

//                //Debug.Log(meshFilterObj.mesh.vertices.Length);

//                Vector2[] uvs = new Vector2[4];
//                uvs[0] = character.uvBottomLeft;
//                uvs[1] = character.uvTopRight;
//                uvs[2] = character.uvBottomRight;
//                uvs[3] = character.uvTopLeft;

//                mesh.uv = uvs;

//                Vector3 newScale = poly.transform.localScale;
//                newScale.x = character.glyphWidth * 0.02f;

//                poly.transform.localScale = newScale;
//            }

//            poly.AddComponent<MeshFilter>().sharedMesh = mesh; // чтобы спроецировать наш меш; сразу указываем меш
//        }
//    }
//}
