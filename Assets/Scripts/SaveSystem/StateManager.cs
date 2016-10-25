using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class StateManager : MonoBehaviour {
    
    static List<string> inactiveObjects = new List<string>();

    private string sceneSaved;
    private Vector3 playerSavedPosition;
    private int playerHealth = 100;
    private List<GameObject> questItems;
    private List<GameObject> playerWeapons;
    private List<int> playerAmmo;

    public static StateManager instance;
    
    void Awake()
    {   
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void SetObjectInactive(string name)
    {
        inactiveObjects.Add(name);
    }

    public void Save()
    {

    }

    public void Load()
    {

    }

    public bool GetActive(string name)
    {
        if (inactiveObjects.Count > 0)
        {
            foreach (string i in inactiveObjects)
            {
                if (i == name)
                    return false;
            }
        }

        return true;
    }
}