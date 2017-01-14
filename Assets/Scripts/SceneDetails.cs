using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

public class SceneDetails : MonoBehaviour {

    public float cameraMinX = -5f;
    public float cameraMaxX = 5f;
    public AudioMixerSnapshot activeSnapshot;
    public CutSceneController introCutScene;
    public string mapName;

    void Awake()
    {
        GameManager.instance._sm = this;
    }
    void Start()
    {
        GameManager.instance.InitializeScene(this);
        GameManager.instance.mainCam.backgroundColor = RenderSettings.fogColor;
        activeSnapshot.TransitionTo(0.01f);
    }
}
