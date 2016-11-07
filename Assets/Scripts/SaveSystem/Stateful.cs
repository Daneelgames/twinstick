using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Stateful : MonoBehaviour {

    public InteractiveObject interactive;
    public List<string> boolsToSave = new List<string>();
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
        if (anim)
        {
            SetAnimatorBoolsOnAwake();   
        }
    }

    void Start()
    {
        GameManager.instance.AddStateful(this);
    }

    public void ObjectInactive()
    {
        StateManager.instance.SetObjectInactive(gameObject.name);
    }
    
    void SetAnimatorBoolsOnAwake()
    {
        string boolValues = "";
        print("started to save");
        if (StateManager.instance.statefulObjectsAnimators.Count > 0)
        {
            foreach (string i in StateManager.instance.statefulObjectsAnimators)
            {
                if (i == name)
                {
                    boolValues = StateManager.instance.statefulObjectsAnimatorsBooleans[StateManager.instance.statefulObjectsAnimators.IndexOf(i)];
                    break;
                }
            }
        }
        
        if (boolValues.Length > 0)
        {
            for (int i = 0; i < boolValues.Length; i ++)
            {
                if (boolValues.Substring(i) == "0")
                    anim.SetBool(boolsToSave[i], false);
                else
                    anim.SetBool(boolsToSave[i], true);
            }
        }
    }

    public void DisableOnSceneChange()
    {
        if (anim)
        {
            string bools = "";
            foreach (string j in boolsToSave)
            {
                if (anim.GetBool(j) == true)
                {
                    bools += "1";
                }
                else
                {
                    bools += "0";
                }
                print(j + " " + anim.GetBool(j));
            }

            StateManager.instance.SetAnimatorParameters(name, bools);
        }
    }

    public void InteractiveObjectSetActiveDialog(int index)
    {

    }
}