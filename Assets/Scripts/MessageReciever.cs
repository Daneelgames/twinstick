using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MessageReciever : MonoBehaviour {

    public CutSceneController csToStart;
    public List<Stateful> objectsToActivate;

    public void GetMessage()
    {
        //csToStart.Fade("Game");
        if (csToStart)
            csToStart.StartCs();
        
        if (objectsToActivate.Count > 0)
        {
            foreach (Stateful st in objectsToActivate)
            {
                st.ObjectActive(true);
                st.gameObject.SetActive(true);
            }
        }
    }
}