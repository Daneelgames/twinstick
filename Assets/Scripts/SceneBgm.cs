using UnityEngine;
using System.Collections;

public class SceneBgm : MonoBehaviour
{

    public AudioClip clip;
    void Start()
    {

        if (clip && GameManager.instance.musicController.activeAu.clip != clip) // send this track to main music player
        {
            GameManager.instance.musicController.SetBgm(clip);
        }
    }
}