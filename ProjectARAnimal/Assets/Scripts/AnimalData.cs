using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Vuforia;
// using are a way to include namespaces that contain useful classes and functions.

public class AnimalData : MonoBehaviour
{
    [Header("Animal Assets")]
    public AudioClip soundClip;
    public VideoClip videoClip;

    private ARContentManager manager;

    // Start is called before the first frame update
    void Start()
    {
        manager = FindObjectOfType<ARContentManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TargetFound()
    {
        if (manager != null)
        {
            manager.OnTargetFound(soundClip, videoClip);
        }
    }
    public void TargetLost()
    {
        if (manager != null)
        {
            manager.OnTargetLost();
        }
    }


}
