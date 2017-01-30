using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{

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

    public List<string> weapons;
    public GameObject playerPrefab;

    public CampfireController startCampfire;
    public int bullets = 50;
    public int shells = 20;
    public string activeWeapon;

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

    public Animator healthFeedbackAnimator;
    public MainAudioController gmAu;
    public MusicPlayerController musicController;
    public CanvasContainer canvasContainer;
    bool sessionStarted = false;
    CameraZoneController activeCamera;
    void Awake()
    {
        if (instance == null)
            instance = this;

        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        CreatePlayer();
        CreateCanvas();
    }

    void CreatePlayer()
    {
        GameObject newPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        newPlayer.name = "Player";
        playerInGame = newPlayer;
        playerController = playerInGame.GetComponent<PlayerMovement>();
        playerInGame.transform.SetParent(transform);
    }

    void CreateCanvas()
    {
        GameObject cnvs = Instantiate(canvasPrefab, transform.position, Quaternion.identity) as GameObject;

        cnvs.name = "CanvasMain";
        cnvs.transform.SetParent(transform);
        cnvs.transform.position = Vector2.zero;

        canvasContainer = cnvs.GetComponent<CanvasContainer>();
        dialogAnimator = canvasContainer.dialogAnimator;
        dialogText = canvasContainer.dialogText;
        actionFeedbackController = canvasContainer.actionFeedback;
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
        _sm = scene;
        if (!sessionStarted)
        {
            characterSpawnerName = _sm.spawner;
            sessionStarted = true;
        }
        startCampfire = GameObject.Find(characterSpawnerName).GetComponent<CampfireController>();

        playerController.playerHealth.SetHealth(StateManager.instance.playerHealth);
        gui.SetHealth();

        Time.timeScale = 1;

        canvasContainer.map.LoadMap(_sm.mapName);
        if (_sm.mapName != "")
            canvasContainer.map.SetPlayerPosition(SceneManager.GetActiveScene().name);

        PlayerSetPos();

        NpcToInteract(null, "Inspect");

        //        print("events");
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

        // SEND MESSAGES ON START OF SCENE

        /*
                for (int i = StateManager.instance.messages.Count - 1; i > -1; i --)
                {
                    for (int j = statefulObjectsOnscene.Count - 1; j > -1; j--)
                    {
                        if (StateManager.instance.messages[i] == statefulObjectsOnscene[j].name)
                        {
                            MessageReciever msg = statefulObjectsOnscene[j].gameObject.GetComponent<MessageReciever>();
                            msg.GetMessage();
                            if (msg.csToStart)
                                cs = true;
                            StateManager.instance.RemoveMessage(StateManager.instance.messages[i]);
                        }
                    }
                }
        */

        List<string> messengersNames = new List<string>();
        foreach (string m in StateManager.instance.messages.ToList())
        {
            //print (m);
            foreach (Stateful s in statefulObjectsOnscene)
            {
                //print (s);
                if (m == s.name)
                {
                    MessageReciever msg = s.gameObject.GetComponent<MessageReciever>();
                    msg.GetMessage();
                    //print (msg);
                    messengersNames.Add(m);
                    if (msg.csToStart)
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
        SetActiveWeapon(StateManager.instance.activeWeapon);

        playerController.SetFlashlight(StateManager.instance.GetFlashlight());
        playerController.playerHealth.health = StateManager.instance.playerHealth;
    }

    public void SetActiveCamera(CameraZoneController zone)
    {
        if (activeCamera)
            activeCamera.active = false;

        activeCamera = zone;
        activeCamera.active = true;
    }

    public void SendMessages(bool needToFadeIn) // CALL IN MIDDLE OF SCENE
    {
        bool cs = false;
        List<string> messengersNames = new List<string>();
        foreach (string m in StateManager.instance.messages.ToList())
        {
            foreach (Stateful s in statefulObjectsOnscene)
            {
                print(s.name);
                if (m == s.name)
                {
                    s.mr.GetMessage();
                    messengersNames.Add(m);
                    if (s.mr.csToStart)
                        cs = true;
                }
            }
        }

        print("cs is " + cs + " ; need to fade is " + needToFadeIn);
        if (!cs && needToFadeIn) // IF NO CUT SCENE TO PLAY AND NEED TO FADE TO GAME 
            GameManager.instance.gui.Fade("Game");

        foreach (string n in messengersNames)
        {
            StateManager.instance.RemoveMessage(n);
        }
    }

    public void GetValuesFromSaveFile()
    {
        if (StateManager.instance.playerAmmo.Count > 0)
        {
            bullets = StateManager.instance.playerAmmo[0];

            if (StateManager.instance.playerAmmo.Count > 1)
                shells = StateManager.instance.playerAmmo[1];
        }


        // get spawner on save
        if (StateManager.instance.playerSpawner != null)
        {
            characterSpawnerName = StateManager.instance.playerSpawner;
            //loadSpawnerFromSave = true;
        }

        if (StateManager.instance.sceneSaved != "")
        {
            //if (StateManager.instance.sceneSaved != SceneManager.GetActiveScene().name)
            {
                characterSpawnerName = StateManager.instance.playerSpawner;
                LoadToNewScene(StateManager.instance.sceneSaved, characterSpawnerName);
            }
        }
    }

    void Start()
    {
        //remove trash from statefulObjectsOnscene
        for (int i = statefulObjectsOnscene.Count - 1; i > -1; i--)
        {
            if (statefulObjectsOnscene[i] == null)
                statefulObjectsOnscene.RemoveAt(i);
        }
    }

    public void AddStateful(Stateful statefulObj)
    {
        bool noDouble = true;
        foreach (Stateful st in statefulObjectsOnscene)
        {
            if (st == statefulObj)
            {
                noDouble = false;
                break;
            }
        }
        if (noDouble)
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
        gui.Fade("Black");
        yield return new WaitForSecondsRealtime(1f);
        mainCam.cullingMask = (1 << 13); // only player dead
        mainCam.cullingMask ^= (1 << 5);
        playerInGame.SetActive(false);
        gui.Fade("Game");
        mainCam.backgroundColor = Color.black;
        camAnim.SetBool("PlayerDead", true);
        yield return new WaitForSecondsRealtime(7f);
        gui.Fade("Black");
        yield return new WaitForSecondsRealtime(1f);
        mainCam.cullingMask = ~(1 << 13);
        mainCam.cullingMask &= ~(1 << 12);
        camAnim.SetBool("PlayerDead", false);
        StateManager.instance.GameLoad();
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
        if (playerController.playerHealth.health > 0 && !playerController.attacking && !playerController.aim && !playerController.reloading && !playerController.healing && !playerController.moveBack && !gui.fade)
        {
            if (Input.GetButtonDown("Submit") && !inventory.active && npcToInteract != null)
            {
                npcToInteract.Talk();
                //actionFeedbackController.SetFeedback(false, "");
            }

            if (Input.GetButtonDown("ToggleInventory"))
            {
                inventory.ToggleInventory();
            }
            if (Input.GetButtonDown("ToggleMap") && !inventory.active)
            {
                if (StateManager.instance.GetMap(_sm.mapName))
                    canvasContainer.map.ToggleMap();
            }
        }
    }

    public void SetMonstersActive(bool active)
    {
        foreach (Stateful st in statefulObjectsOnscene)
        {
            if (st.gameObject.tag == "Mob")
            {
                st.SetMeshActive(active);
            }
        }
    }

    public void SetActiveWeapon(string weaponName)
    {
        activeWeapon = weaponName;
        playerController.SetWeapon(activeWeapon);
        StateManager.instance.SetActiveWeapon(weaponName);
    }

    public void SetAmmo(WeaponController.Type type, int amount)
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
        //save stateful positions
        foreach (Stateful i in statefulObjectsOnscene)
        {
            i.SavePosition();
        }


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

        print("new scene");
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
        //        print("clearList");
        statefulObjectsOnscene.Clear();
    }

    public void CutScenePlay(bool playing)
    {
        cutScene = playing;

        if (playerController != null)
            playerController.gameObject.SetActive(!playing);

        if (!playing && Time.timeScale < 1)
        {
            Time.timeScale = 1;
        }
    }

    public Stateful GetStatefulOnScene(string n)
    {
        foreach (Stateful st in statefulObjectsOnscene)
        {
            if (st.name == n)
            {
                return st;
            }
        }

        return null;
    }
}