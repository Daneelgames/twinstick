using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SceneDetails : MonoBehaviour
{

    public string spawner;
    public AudioMixerSnapshot activeSnapshot;
    public CutSceneController introCutScene;
    public string mapName;
    public bool markRoomOnMap;

    void Awake()
    {
        GameManager.instance._sm = this;
    }
    void Start()
    {
        GameManager.instance.InitializeScene(this);
        GameManager.instance.mainCam.backgroundColor = RenderSettings.fogColor;
        activeSnapshot.TransitionTo(0.01f);

        if (markRoomOnMap)
        {
            GameManager.instance.canvasContainer.map.SetMarkerActive(SceneManager.GetActiveScene().name + "Room");
        }
    }
}