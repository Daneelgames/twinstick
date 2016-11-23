using UnityEngine;
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

    public InteractiveObject npcToInteract = null;

    public Animator dialogAnimator;
    public Text dialogText;

    public bool pointerOverMenu = false;

    public ActionFeedbackController actionFeedbackController;
    public InventoryControllerGUI inventory;

    //private bool loadSpawnerFromSave = false;

    public List<Stateful> statefulObjectsOnscene = new List<Stateful>();

    public GameObject canvasPrefab;

    public InventoryItemsList inventoryItems;

    void Awake()
    {
        if (instance == null)
            instance = this;

        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        CreateCanvas();
    }

    void CreateCanvas()
    {
        GameObject cnvs = Instantiate(canvasPrefab, transform.position, Quaternion.identity) as GameObject;

        cnvs.name = "CanvasMain";
        cnvs.transform.SetParent(transform);
        cnvs.transform.position = Vector2.zero;

        CanvasContainer canvasContainer = cnvs.GetComponent<CanvasContainer>();
        dialogAnimator = canvasContainer.dialogAnimator;
        dialogText = canvasContainer.dialogText;
        actionFeedbackController = canvasContainer.actionFeedback;
        gui.healthAnimator = canvasContainer.healthbarAnimator;
        gui.healthbar = canvasContainer.healthbar;
        gui.saveAnimator = canvasContainer.saveAnimator;
        gui.reloadController = canvasContainer.reloadController;
        gui.fadeAnimator = canvasContainer.fadeAnimator;
        gui.fadeImg = canvasContainer.fadeImg;
        inventory = canvasContainer.inventory;

        canvasContainer.SetRenderCamera();
    }

    public void InitializeScene(SceneDetails scene)
    {
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

        NpcToInteract(null, "Inspect");
        
        bool cs = false;

        if (_sm.introCutScene != null) // play scene intro cutScene
        {
            if (_sm.introCutScene.gameObject.activeSelf && _sm.introCutScene.playOnStartOfScene)
            {
                gui.InstantBlack();
                _sm.introCutScene.StartCs();
                cs = true;
            }
        }

        // SEND MESSAGES

        List<string> messengersNames = new List<string>();
        foreach (string m in StateManager.instance.messages)
        {
            foreach (Stateful s in statefulObjectsOnscene)
            {
                print(s.name);
                if (m == s.name)
                {
                    s.gameObject.GetComponent<MessageReciever>().GetMessage();
                    messengersNames.Add(m);
                    cs = true;
                }
            }
        }

        foreach (string n in messengersNames)
        {
            StateManager.instance.RemoveMessage(n);
        }

        /////////////

        if (!cs)
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

        //SendMessages();

    }

    /*
    void SendMessages()
    {
        List<string> messengersNames = new List<string>();
        foreach (string m in StateManager.instance.messages)
        {
            foreach (Stateful s in statefulObjectsOnscene)
            {
                print(s.name);
                if (m == s.name)
                {
                    s.gameObject.GetComponent<MessageReciever>().GetMessage();
                    messengersNames.Add(m);
                }
            }
        }

        foreach (string n in messengersNames)
        {
            StateManager.instance.RemoveMessage(n);
        }
    }
    */

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
        if (!playerController.aim)
        {
            if (Input.GetButtonDown("Submit"))
            {
                if (npcToInteract != null && !inventory.active)
                {
                    npcToInteract.Talk();
                    //actionFeedbackController.SetFeedback(false, "");
                }
            }

            if (Input.GetButtonDown("ToggleInventory"))
            {
                inventory.ToggleInventory();
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

    public void SaveAnimatorBooleans()
    {
        foreach (Stateful i in statefulObjectsOnscene)
        {
            i.SaveAnimatorBooleans();
        }
    }

    void ClearStatefulObjectsList()
    {
        SaveAnimatorBooleans();
        statefulObjectsOnscene.Clear();
    }

    public void CutScenePlay(bool playing)
    {
        cutScene = playing;

        if (playerController != null)
            playerController.gameObject.SetActive(!playing);
    }
}