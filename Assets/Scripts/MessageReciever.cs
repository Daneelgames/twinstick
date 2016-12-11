using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MessageReciever : MonoBehaviour {

    public CutSceneController csToStart;
    public List<Stateful> objectsToActivate;
    public List<Stateful> objectsToDeactivate;
    public List<MessageTransmitter> transmitters;

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

        if (objectsToDeactivate.Count > 0)
        {
            foreach (Stateful st in objectsToDeactivate)
            {
                print(st + " is inactive");
                st.ObjectActive(false);
                st.gameObject.SetActive(false);
            } 
        }

        if (transmitters.Count > 0)
        {
            foreach (MessageTransmitter mt in transmitters)
            {
                mt.SendMessage(false);
            }
        }
    }
}