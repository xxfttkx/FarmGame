using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SwitchBounds : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SwitchConfinerShape();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
    }

    private void OnAfterSceneLoadedEvent()
    {
        SwitchConfinerShape();
    }

    private void SwitchConfinerShape()
    {
        var boundsConfiner = GameObject.FindGameObjectWithTag("BoundsConfiner");
        if (boundsConfiner == null) return;
        var confinerShape = boundsConfiner.GetComponent<PolygonCollider2D>();
        if (confinerShape == null) return;
        CinemachineConfiner confiner = GetComponent<CinemachineConfiner>();

        confiner.m_BoundingShape2D = confinerShape;

        //Call this if the bounding shape's points change at runtime
        confiner.InvalidatePathCache();
    }
}
