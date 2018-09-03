using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nm
{
    public class SceneCleaning : MonoBehaviour
    {
        public static SceneCleaning Instance;

        private void Awake()
        {
            Instance = this;
        }

        public void Clean()
        {
            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
    }
}
