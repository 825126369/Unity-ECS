using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[ExecuteAlways]
public class FindCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<Canvas>() && GetComponent<Canvas>().worldCamera == null)
        {
            GetComponent<Canvas>().worldCamera = Camera.main;
        }

        if (GetComponent<VideoPlayer>() && GetComponent<VideoPlayer>().targetCamera == null)
        {
            GetComponent<VideoPlayer>().targetCamera = Camera.main;
        }
    }

}
