using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IfHaveItemOnSceneStart : MonoBehaviour
{
    public string item = "";

    public Animator anim;
    public List<string> animSetBoolTrue = new List<string>();
    public List<string> animSetBoolFalse = new List<string>();

    public List<GameObject> goActive = new List<GameObject>();
    public List<GameObject> goInactive = new List<GameObject>();

    void Start()
    {
        if (item != "")
        {
            foreach (string i in StateManager.instance.questItems)
            {
                if (item == i)
                {
                    SetAnimatorBools();
                    SetObjectsActive();
                    break;
                }
            }
        }
    }

    void SetAnimatorBools()
    {
        if (anim)
        {
            foreach (string i in animSetBoolFalse)
            {
                anim.SetBool(i, false);
            }

            foreach (string i in animSetBoolTrue)
            {
                anim.SetBool(i, true);
            }
        }
    }

    void SetObjectsActive()
    {
        foreach(GameObject i in goActive)
        {
            i.SetActive(true);
        }

        foreach (GameObject i in goInactive)
        {
            i.SetActive(false);
        }
    }
}