using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Stateful : MonoBehaviour {

    public bool activeOnStart = true;

    public InteractiveObject interactive;
    public List<string> boolsToSave = new List<string>();
    public Animator anim;
    public MessageTransmitter mt;
    
    void Awake()
    {
        string mtRecieverName = "";
        if (mt)
            mtRecieverName = mt.recieverName;

        StateManager.instance.SetStatefulObject(gameObject.name, activeOnStart, mtRecieverName);

        if (StateManager.instance.GetActive(gameObject.name) == false) //is inactive?
        {
            gameObject.SetActive(false);
        }
        if (mt)
        {
            mt.SetRecieverName(StateManager.instance.GetRecieverName(gameObject.name));
        }
        if (interactive)
        {
            interactive.SetActiveDialog(StateManager.instance.GetActiveDialog(gameObject.name)); // set index
        }
        if (anim)
        {
            SetAnimatorBoolsOnAwake();   
        }

        GameManager.instance.AddStateful(this);
    }

    void Start()
    {
        ObjectActive(true);
        //GameManager.instance.AddStateful(this);
    }

    public void ObjectActive(bool active)
    {
        StateManager.instance.SetObjectActive(gameObject.name, active);
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

    public void SaveAnimatorBooleans()
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
        StateManager.instance.SetActiveDialog(name, index);
    }
}