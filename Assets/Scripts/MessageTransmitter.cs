using UnityEngine;
using System.Collections;

public class MessageTransmitter : MonoBehaviour {

    public string recieverName = "";
    public bool sendOnStart = false;

    public void SetRecieverName(string j)
    {
        recieverName = j;
    }

    void Start()
    {
        if (recieverName != "" && sendOnStart)
        {
            SendMessage(false);
        }
    }

    public void SendMessage(bool needToFadeIn)
    {
        if (recieverName != "")
        {
            StateManager.instance.AddMessage(recieverName);
            StateManager.instance.RemoveTransmitterMessage(recieverName);
            recieverName = "";

            GameManager.instance.SendMessages(needToFadeIn);
        }
        else
        {
            print (gameObject.name + "'s transmitter doesn't have a recieverName");
        }
    }
}
