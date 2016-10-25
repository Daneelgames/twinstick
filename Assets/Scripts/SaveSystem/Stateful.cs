using UnityEngine;
using System.Collections;

public class Stateful : MonoBehaviour {

    public InteractiveObject interactive;

    void Start()
    {
        if (StateManager.instance.GetActive(gameObject.name) == false) //is active?
        {
            gameObject.SetActive(false);
        }
        if (interactive != null)
        {
            interactive.SetActiveDialog(0); // set index
        }
    }

    public void ObjectInactive()
    {
        StateManager.instance.SetObjectInactive(gameObject.name);
    }

    public void InteractiveObjectSetActiveDialog(int index)
    {

    }
}