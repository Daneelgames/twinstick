using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class StateManager : MonoBehaviour
{

    public bool loadOnStart = false;

    public List<string> statefulObjects = new List<string>();
    public List<bool> activeObjects = new List<bool>();
    public List<int> activeDialogues = new List<int>();
    public List<string> transmittersMessages = new List<string>();
    public List<string> deadMobs = new List<string>();

    public string sceneSaved;
    public string playerSpawner;
    public int playerHealth = 10;
    public string activeWeapon;
    public bool flashlight = false;
    public List<string> questItems = new List<string>();
    public List<int> playerAmmo = new List<int>();
    public int painkillers = 0;

    public List<string> statefulObjectsAnimators = new List<string>();
    public List<Vector3> statefulPositions = new List<Vector3>();
    public List<string> statefulObjectsAnimatorsBooleans = new List<string>();

    public List<string> messages = new List<string>();

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
        return false;
    }

    public string GetRecieverName(string n)
    {
        if (statefulObjects.Count > 0)
        {
            foreach (string i in statefulObjects)
            {
                if (i == n)
                {
                    return transmittersMessages[statefulObjects.IndexOf(i)];
                }
            }
        }
        return "";
    }

    public void AddMessage(string message)
    {
        bool alreadyHave = false;

        foreach (string i in messages)
        {
            if (message == i)
            {
                alreadyHave = true;
                break;
            }
        }

        if (!alreadyHave)
        {
            messages.Add(message);
        }
    }

    public void RemoveMessage(string message)
    {
        messages.Remove(message);
    }

    public void SetStatefulObject(string objName, bool active, string message, Vector3 pos)
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
            activeObjects.Add(active);
            activeDialogues.Add(0);
            statefulPositions.Add(pos);
            transmittersMessages.Add(message);
        }
    }

    public void RemoveTransmitterMessage(string msg)
    {
        int index = -1;
        foreach (string m in transmittersMessages)
        {
            if (m == msg)
            {
                index = transmittersMessages.IndexOf(m);
                break;
            }
        }

        if (index > 0)
        {
            transmittersMessages[index] = "";
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

    public void SetMobDead(string mobName)
    {
        bool dbl = false;
        foreach (string i in deadMobs)
        {
            if (mobName == i)
            {
                dbl = true;
            }
        }

        if (!dbl)
        {
            deadMobs.Add(mobName);
        }
    }

    public bool GetMobDead(string mobName)
    {
        foreach (string i in deadMobs)
        {
            if (mobName == i)
            {
                return true;
            }
        }

        return false;
    }

    public void SetObjectActive(string objName, bool active)
    {
        if (statefulObjects.Count > 0)
        {
            foreach (string i in statefulObjects)
            {
                if (i == objName)
                {
                    activeObjects[statefulObjects.IndexOf(i)] = active;
                    break;
                }
            }
        }
    }
    public void SaveStatefulPosition(string statefulName, Vector3 pos)
    {
        foreach (string i in statefulObjects)
        {
            if (statefulName == i)
            {
                statefulPositions[statefulObjects.IndexOf(i)] = pos;
                break;
            }
        }
    }

    public Vector3 GetStatefulPosition(string statefulName)
    {
        foreach (string i in statefulObjects)
        {
            if (statefulName == i)
            {
                if (statefulPositions.Count > statefulObjects.IndexOf(i))
                    return statefulPositions[statefulObjects.IndexOf(i)];
            }
        }
        return new Vector3(0, -100f, 100f);
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
        if (item != "Painkillers")
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
        else if (item == "Painkillers")
        {
            painkillers += 1;
        }
    }

    public void UsePainkillers()
    {
        painkillers -= 1;
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

    public void SetFlashlight(bool active)
    {
        flashlight = active;
    }
    public bool GetFlashlight()
    {
        return flashlight;
    }

    public void SetActiveWeapon(string weaponName)
    {
        activeWeapon = weaponName;
    }

    public void GameSave()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gameState.dat");

        GameState data = new GameState();

        data.statefulObjects = new List<string>(statefulObjects);
        data.activeObjects = new List<bool>(activeObjects);
        data.activeDialogues = new List<int>(activeDialogues);
        data.deadMobs = new List<string>(deadMobs);

        data.transmittersMessages = new List<string>(transmittersMessages);

        data.sceneSaved = SceneManager.GetActiveScene().name;
        data.playerSpawner = playerSpawner = GameManager.instance.startCampfire.name;
        data.playerHealth = playerHealth = GameManager.instance.playerController.playerHealth.health;
        data.activeWeapon = activeWeapon;
        data.questItems = new List<string>(questItems);
        data.painkillers = painkillers;
        data.flashlight = flashlight;

        data.statefulObjectsAnimators = new List<string>(statefulObjectsAnimators);
        data.statefulObjectsAnimatorsBooleans = new List<string>(statefulObjectsAnimatorsBooleans);
        //data.statefulPositions = new List<Vector3>(statefulPositions);

        data.statefulPositions.Clear();
        foreach (Vector3 v in statefulPositions)
        {
            data.statefulPositions.Add(v);
        }

        data.messages = new List<string>(messages);

        data.playerAmmo = new List<int>(playerAmmo);

        bf.Serialize(file, data);
        file.Close();
    }

    public void GameLoad()
    {
        if (File.Exists(Application.persistentDataPath + "/gameState.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gameState.dat", FileMode.Open);
            GameState data = (GameState)bf.Deserialize(file);
            file.Close();

            statefulObjects = new List<string>(data.statefulObjects);
            activeObjects = new List<bool>(data.activeObjects);
            activeDialogues = new List<int>(data.activeDialogues);
            deadMobs = new List<string>(data.deadMobs);

            transmittersMessages = new List<string>(data.transmittersMessages);

            sceneSaved = data.sceneSaved;
            playerSpawner = data.playerSpawner;
            playerHealth = data.playerHealth;
            activeWeapon = data.activeWeapon;
            questItems = new List<string>(data.questItems);
            playerAmmo = new List<int>(data.playerAmmo);
            painkillers = data.painkillers;
            flashlight = data.flashlight;

            statefulObjectsAnimators = new List<string>(data.statefulObjectsAnimators);
            statefulObjectsAnimatorsBooleans = new List<string>(data.statefulObjectsAnimatorsBooleans);
            //statefulPositions = new List<Vector3>(data.statefulPositions);

            statefulPositions.Clear();
            foreach (Vector3 v in data.statefulPositions)
            {
                statefulPositions.Add(v);
            }

            messages = new List<string>(data.messages);

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
    public List<string> transmittersMessages = new List<string>();
    public List<string> deadMobs = new List<string>();

    public string sceneSaved;
    public string playerSpawner;
    public int playerHealth = 100;
    public string activeWeapon;
    public List<string> questItems;
    public List<int> playerAmmo;
    public int painkillers = 0;
    public bool flashlight;

    public List<string> statefulObjectsAnimators = new List<string>();
    public List<SerializableVector3> statefulPositions = new List<SerializableVector3>();
    public List<string> statefulObjectsAnimatorsBooleans = new List<string>();

    public List<string> messages = new List<string>();
}