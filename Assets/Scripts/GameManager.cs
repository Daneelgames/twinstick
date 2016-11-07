﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
    
    public static GameManager instance = null;

    public SceneDetails _sm; // scene manager
    
    public string characterSpawnerName = "StartSpawner";

    public bool cutScene = false;
    public GameObject cameraHolder;
    public Camera mainCam;
    public Animator camAnim;

    public GuiController gui;

    public GameObject playerInGame;
    public PlayerMovement playerController;

    public List<GameObject> weapons;
    public GameObject playerPrefab;

    public int playerHealth = 10;
    public CampfireController startCampfire;
    public int bullets = 50;
    public int shells = 20;
    public List<GameObject> playerWeapons;


    public List<MobSpawnerController> spawners;

    public GameObject weaponToPick = null;
    public InteractiveObject npcToInteract = null;

    public Animator dialogAnimator;
    public Text dialogText;

    public bool pointerOverMenu = false;

    public ActionFeedbackController actionFeedbackController;

    //private bool loadSpawnerFromSave = false;

    public List<Stateful> statefulObjectsOnscene = new List<Stateful>();

    void Awake()
    {
        if (instance == null)
            instance = this;

        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void InitializeScene(SceneDetails scene)
    {
        gui.InstantBlack();

        GameObject newPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        newPlayer.name = "Player";
        playerInGame = newPlayer;
        playerController = playerInGame.GetComponent<PlayerMovement>();

        startCampfire = GameObject.Find(characterSpawnerName).GetComponent<CampfireController>();

        if (playerWeapons.Count > 0)
            playerController.SetWeapon(playerWeapons.Count - 1);

        playerController.playerHealth.SetHealth(StateManager.instance.playerHealth);
        gui.SetHealth();

        List<GameObject> spwnrs = new List<GameObject>(GameObject.FindGameObjectsWithTag("MobSpawner"));
        foreach (GameObject i in spwnrs)
        {
            spawners.Add(i.GetComponent<MobSpawnerController>());
        }
        Time.timeScale = 1;

        _sm = scene;


        PlayerSetPos();

        WeaponToPick(null);
        NpcToInteract(null, "Inspect");
        
        bool noCs = true;

        if (_sm.introCutScene != null) // play scene intro cutScene
        {
            if (_sm.introCutScene.gameObject.activeSelf && _sm.introCutScene.playOnStartOfScene)
            {
                _sm.introCutScene.StartCs();
                noCs = false;
            }
        }

        if (noCs)
        {
            gui.Fade("Game");
        }

        mainCam.backgroundColor = RenderSettings.fogColor;

        //SET CAM TO PLAYER
        float posX = Mathf.Lerp(cameraHolder.transform.position.x, playerInGame.transform.position.x, 5f);
        float posY = Mathf.Lerp(cameraHolder.transform.position.y, playerInGame.transform.position.y + 3, 5f);

        if (posX > _sm.cameraMaxX)
            posX = _sm.cameraMaxX;
        if (posX < _sm.cameraMinX)
            posX = _sm.cameraMinX;

        cameraHolder.transform.position = new Vector3(posX, posY, cameraHolder.transform.position.z);
        cameraHolder.transform.LookAt(new Vector3(posX, posY - 2f, playerInGame.transform.position.z));

    }

    public void GetValuesFromSaveFile()
    {
        playerHealth = StateManager.instance.playerHealth;
        if (StateManager.instance.playerAmmo.Count > 0)
        {
            bullets = StateManager.instance.playerAmmo[0];

            if (StateManager.instance.playerAmmo.Count > 1)
                shells = StateManager.instance.playerAmmo[1];
        }

        // iterate through lists, make playerWeaponList
        foreach (string name in StateManager.instance.playerWeapons)
        {
            foreach (GameObject i in weapons)
            {
                if (i.name == name)
                    playerWeapons.Add(i);
            }
        }

        // quest items

        // get spawner on save
        if (StateManager.instance.playerSpawner != null)
        {
            characterSpawnerName = StateManager.instance.playerSpawner;
            //loadSpawnerFromSave = true;
        }

        if (StateManager.instance.sceneSaved != "")
        {
            if (StateManager.instance.sceneSaved != SceneManager.GetActiveScene().name)
            {
                characterSpawnerName = StateManager.instance.playerSpawner;
                LoadToNewScene(StateManager.instance.sceneSaved, characterSpawnerName);
            }
        }
    }

    public void AddStateful(Stateful statefulObj)
    {
        statefulObjectsOnscene.Add(statefulObj);
    }

    void PlayerSetPos()
    {
        playerInGame.SetActive(true);
        startCampfire.SpawnPlayer();
    }
    
    public void PlayerDead()
    {
        StartCoroutine("RespawnPlayer");
    }

    public void SetStartCampfire(CampfireController savePoint)
    {
        startCampfire = savePoint;
    }

    IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(1f);
        playerInGame.SetActive(true);
        startCampfire.SpawnPlayer();
        RespawnMobs();
    }

    public void RespawnMobs()
    {
        foreach (MobSpawnerController i in spawners)
        {
            i.DestroyMob();
            i.Spawn();
        }
    }

    public void WeaponToPick(GameObject weapon)
    {
        weaponToPick = weapon;
        if (weapon != null)
            actionFeedbackController.SetFeedback(true, "Inspect");
        else
            actionFeedbackController.SetFeedback(false, "");
    }


    public void NpcToInteract(InteractiveObject npc, string type)
    {
        npcToInteract = npc;
        if (npc != null)
        {
            actionFeedbackController.SetFeedback(true, type);
        }
        else
        {
            actionFeedbackController.SetFeedback(false, "");
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Submit"))
        {
            if (weaponToPick != null)
            {
                weaponToPick.GetComponent<WeaponController>().PickUp();
                WeaponToPick(null);
                actionFeedbackController.SetFeedback(false, "");
            }
            else if (npcToInteract != null)
            {
                npcToInteract.Talk();
                actionFeedbackController.SetFeedback(false, "");
            }
        }
    }

    public void AddPlayerWeapon(GameObject weapon)
    {
        foreach(GameObject i in weapons)
        {
            if (weapon.name == i.name)
            {
                playerWeapons.Add(i);
                playerController.SetWeapon(playerWeapons.Count - 1);
                break;
            }
        }
    }
    public void RemovePlayerWeapon(GameObject weapon)
    {
        playerWeapons.Remove(weapon);
    }

    public void SetAmmo (WeaponController.Type type, int amount)
    {
        switch (type)
        {
            case WeaponController.Type.Bullet:
                bullets += amount;
                break;

            case WeaponController.Type.Shell:
                shells += amount;
                break;
        }
    }

    public void PointerOverMenu(bool entered)
    {
        pointerOverMenu = entered;
    }

    public void MoveToNewScene(string sceneName, string spawnerName)
    {
        characterSpawnerName = spawnerName;
        StateManager.instance.SetSpawner(spawnerName);

        ClearStatefulObjectsList();

        SceneManager.LoadScene(sceneName);
    }

    void LoadToNewScene(string sceneName, string spawnerName)
    {
        characterSpawnerName = spawnerName;
        //StateManager.instance.SetSpawner(spawnerName);

        ClearStatefulObjectsList();

        SceneManager.LoadScene(sceneName);
    }

    void ClearStatefulObjectsList()
    {
        foreach (Stateful i in statefulObjectsOnscene)
        {
            i.DisableOnSceneChange();
        }

        statefulObjectsOnscene.Clear();
    }

    public void CutScenePlay(bool playing)
    {
        cutScene = playing;

        if (playerController != null)
            playerController.gameObject.SetActive(!playing);
    }
}