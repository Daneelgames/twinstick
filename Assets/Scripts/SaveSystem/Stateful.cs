using UnityEngine;
using System.Collections;

public class Stateful : MonoBehaviour {

    public InteractiveObject interactive;
    public Animator anim;

    void Awake()
    {
        if (StateManager.instance.GetActive(gameObject.name) == false) //is inactive?
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
    
    void OnDisable()
    {
        if (anim != null)
        {
                
        }
    }

    public void InteractiveObjectSetActiveDialog(int index)
    {

    }
}