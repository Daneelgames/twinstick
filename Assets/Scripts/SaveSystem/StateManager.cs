using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class StateManager : MonoBehaviour {

    public bool loadOnStart = false;

    static List<string> inactiveObjects = new List<string>();

    public string sceneSaved;
    public string playerSpawner;
    public int playerHealth = 10;
    public List<string> questItems = new List<string>();
    public List<string> playerWeapons = new List<string>();
    public List<int> playerAmmo = new List<int>();

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

    public void SetObjectInactive(string name)
    {
        inactiveObjects.Add(name);
    }

    public void GameSave()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gameState.dat");

        GameState data = new GameState();

        data.inactiveObjects = new List<string>(inactiveObjects);
        data.sceneSaved = SceneManager.GetActiveScene().name;
        data.playerSpawner = playerSpawner = GameManager.instance.startCampfire.name;
        data.playerHealth = playerHealth = GameManager.instance.playerController.playerHealth.health;
        data.questItems = new List<string>(questItems);

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

            inactiveObjects = new List<string>(data.inactiveObjects);
            sceneSaved = data.sceneSaved;
            playerSpawner = data.playerSpawner;
            playerHealth = data.playerHealth;
            questItems = new List<string>(data.questItems);
            playerWeapons = new List<string>(data.playerWeapons);
            playerAmmo = new List<int>(data.playerAmmo);
        }

        GameManager.instance.GetValuesFromSaveFile(); // load values on start of session
    }
}

[Serializable]
class GameState
{
    public List<string> inactiveObjects = new List<string>();
    public string sceneSaved;
    public string playerSpawner;
    public int playerHealth = 100;
    public List<string> questItems;
    public List<string> playerWeapons;
    public List<int> playerAmmo;
}