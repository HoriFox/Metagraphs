using UnityEngine;

namespace nm
{
    public class ResourceManager : MonoBehaviour
    {
        public GameObject[] prefabs;

        public static ResourceManager Instance;
        private void Awake()
        {
            Instance = this;
        }
        public static ResourceManager GetInstance()
        {
            return Instance;
        }

        public GameObject GetPrefab(string name)
        {
            return Get(name + " (UnityEngine.GameObject)", Instance.prefabs);
        }

        public static T Get<T>(string name, T[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].ToString() == name)
                {
                    return array[i];
                }
            }
            return default(T);
        }
    }
}
