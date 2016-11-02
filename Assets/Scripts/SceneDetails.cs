using UnityEngine;
using System.Collections;

public class SceneDetails : MonoBehaviour {

    public float cameraMinX = -5f;
    public float cameraMaxX = 5f;

    public Color charactersColor;

    public CutSceneController introCutScene;

    void Start()
    {
        GameManager.instance.InitializeScene(this);
    }
}
