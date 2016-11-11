using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class StateManager : MonoBehaviour {

    public bool loadOnStart = false;

    public List<string> statefulObjects = new List<string>();
    public List<bool> activeObjects = new List<bool>();
    public List<int> activeDialogues = new List<int>();

    public string sceneSaved;
    public string playerSpawner;
    public int playerHealth = 10;
    public List<string> questItems = new List<string>();
    public List<string> playerWeapons = new List<string>();
    public List<int> playerAmmo = new List<int>();

    public List<string> statefulObjectsAnimators = new List<string>();
    public List<string> statefulObjectsAnimatorsBooleans = new List<string>();

    public static StateManager instance;
    
    void Awake()
    {   
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
            if (loadOnStart)
                GameLoad();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void SetSpawner(string spawner)
    {
        playerSpawner = spawner;
    }

    public bool GetActive(string n)
    {
        if (statefulObjects.Count > 0)
        {
            foreach (string i in statefulObjects)
            {
                if (i == n)
                {
                    if (activeObjects[statefulObjects.IndexOf(i)] == true)
                        return true;
                    else
                        return false;
                }
            }
        }
        return true;
    }
    
    public void SetStatefulObject(string objName)
    {
        bool noDouble = true;

        if (statefulObjects.Count > 0)
        {
            foreach (string i in statefulObjects)
            {
                if (i == objName)
                {
                    noDouble = false;
                    break;
                }
            }
        }

        if (noDouble)
        {
            statefulObjects.Add(objName);
            activeObjects.Add(true);
            activeDialogues.Add(0);
        }
    }

    public int GetActiveDialog(string n)
    {
        if (statefulObjects.Count > 0)
        {
            foreach (string i in statefulObjects)
            {
                if (i == n)
                {
                    return activeDialogues[statefulObjects.IndexOf(i)];
                }
            }
        }

        return 0;
    }

    public void SetActiveDialog(string objName, int activeDialog)
    {
        if (statefulObjects.Count > 0)
        {
            foreach (string i in statefulObjects)
            {
                if (i == objName)
                {
                    activeDialogues[statefulObjects.IndexOf(i)] = activeDialog;
                    break;
                }
            }
        }
    }

    public void SetObjectInactive(string objName)
    {
        if (statefulObjects.Count > 0)
        {
            foreach (string i in statefulObjects)
            {
                if (i == objName)
                {
                    activeObjects[statefulObjects.IndexOf(i)] = false;
                    break;
                }
            }
        }
    }

    public void SetAnimatorParameters(string objName, string bools)
    {
        foreach (string j in statefulObjectsAnimators)
        {
            if (j == objName)
            {
                statefulObjectsAnimatorsBooleans.RemoveAt(statefulObjectsAnimators.IndexOf(j));
                statefulObjectsAnimators.Remove(j);
                break;
            }
        }
        statefulObjectsAnimators.Add(objName);
        statefulObjectsAnimatorsBooleans.Add(bools);
    }

    public void AddItem(string item)
    {
        bool alreadyHave = false;
        foreach (string i in questItems)
        {
            if (i == item)
            {
                alreadyHave = true;
                break;
            }
        }
        if (!alreadyHave)
            questItems.Add(item);
    }

    public bool HaveItem(string item)
    {
        foreach (string i in questItems)
        {
            if (i == item)
            {
                questItems.Remove(item);
                return true;
            }
        }
        return false;
    }

    public void RemoveItem(string item)
    {
        foreach (string i in questItems)
        {
            if (i == item)
            {
                questItems.Remove(item);
                break;
            }
        }
    }

    public void GameSave()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gameState.dat");

        GameState data = new GameState();

        data.statefulObjects = new List<string>(statefulObjects);
        data.activeObjects = new List<bool>(activeObjects);
        data.activeDialogues = new List<int>(activeDialogues);

        data.sceneSaved = SceneManager.GetActiveScene().name;
        data.playerSpawner = playerSpawner = GameManager.instance.startCampfire.name;
        data.playerHealth = playerHealth = GameManager.instance.playerController.playerHealth.health;
        data.questItems = new List<string>(questItems);

        data.statefulObjectsAnimators = new List<string>(statefulObjectsAnimators);
        data.statefulObjectsAnimatorsBooleans = new List<string>(statefulObjectsAnimatorsBooleans);

        List<string> tempWeaponList = new List<string>();
        foreach (GameObject i in GameManager.instance.playerWeapons)
        {
            tempWeaponList.Add(i.name);
        }
        data.playerWeapons = new List<string>(tempWeaponList);
        data.playerAmmo = new List<int>(playerAmmo);

        bf.Serialize(file, data);
        file.Close();
    }

    public void GameLoad()
    {
        if(File.Exists(Application.persistentDataPath + "/gameState.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gameState.dat", FileMode.Open);
            GameState data = (GameState)bf.Deserialize(file);
            file.Close();

            statefulObjects = new List<string>(data.statefulObjects);
            activeObjects = new List<bool>(data.activeObjects);
            activeDialogues = new List<int>(data.activeDialogues);
            sceneSaved = data.sceneSaved;
            playerSpawner = data.playerSpawner;
            playerHealth = data.playerHealth;
            questItems = new List<string>(data.questItems);
            playerWeapons = new List<string>(data.playerWeapons);
            playerAmmo = new List<int>(data.playerAmmo);

            statefulObjectsAnimators = new List<string>(data.statefulObjectsAnimators);
            statefulObjectsAnimatorsBooleans = new List<string>(data.statefulObjectsAnimatorsBooleans);

            GameManager.instance.GetValuesFromSaveFile(); // load values on start of session
        }
    }
}

[Serializable]
class GameState
{
    public List<string> statefulObjects = new List<string>();
    public List<bool> activeObjects = new List<bool>();
    public List<int> activeDialogues = new List<int>();

    public string sceneSaved;
    public string playerSpawner;
    public int playerHealth = 100;
    public List<string> questItems;
    public List<string> playerWeapons;
    public List<int> playerAmmo;

    public List<string> statefulObjectsAnimators = new List<string>();
    public List<string> statefulObjectsAnimatorsBooleans = new List<string>();
}