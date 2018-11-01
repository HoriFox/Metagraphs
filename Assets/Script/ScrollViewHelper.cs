using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace nm
{
    public class ScrollViewHelper : MonoBehaviour
    {
        public RectTransform contentParent;
        public RectTransform textPrefab;

        public void ShowList(string[] strList)
        {
            int k = 0;
            foreach (var part in strList)
            {
                RectTransform transform = Instantiate(textPrefab, new Vector3(80f, 140f - k * 15f, 0f), Quaternion.Euler(Vector3.zero), contentParent);
                transform.name = part;
                transform.GetComponent<Text>().text = part;
                k++;
            }
        }

        public void ResetList()
        {
            foreach (RectTransform child in contentParent)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
