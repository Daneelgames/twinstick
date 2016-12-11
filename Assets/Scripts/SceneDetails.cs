using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneDetails : MonoBehaviour {

    public float cameraMinX = -5f;
    public float cameraMaxX = 5f;

    public CutSceneController introCutScene;

    void Start()
    {
        GameManager.instance.InitializeScene(this);
        GameManager.instance.mainCam.backgroundColor = RenderSettings.fogColor;
    }
}
