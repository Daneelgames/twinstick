using UnityEngine;
using System.Collections;

public class MessageTransmitter : MonoBehaviour {

    public string recieverName = "";

    public void SetRecieverName(string j)
    {
        recieverName = j;
    }

    public void SendMessage()
    {
        if (recieverName != "")
        {
            StateManager.instance.AddMessage(recieverName);
            StateManager.instance.RemoveTransmitterMessage(recieverName);
            recieverName = "";
        }
    }
}
