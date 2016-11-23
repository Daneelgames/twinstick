using UnityEngine;
using System.Collections;

public class MessageReciever : MonoBehaviour {

    public CutSceneController csToStart;

    public void GetMessage()
    {
        //csToStart.Fade("Game");
        csToStart.StartCs();
    }
}