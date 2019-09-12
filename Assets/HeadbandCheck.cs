using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadbandCheck : MonoBehaviour
{
    public bool poinerOnBackground = false;

    public void Enter ()
    {
        poinerOnBackground = true;
    }

    public void Exit()
    {
        poinerOnBackground = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !poinerOnBackground)
        {
            gameObject.SetActive(false);
        }
    }
}
