using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
    
    public static GameManager instance = null;

    public SceneDetails _sm;
    
    public string characterSpawnerName = "StartSpawner";

    public Camera mainCam;

    public Animator camAnim;

    public GuiController gui;

    public GameObject playerInGame;
    public PlayerMovement playerController;
    public List<GameObject> playerWeapons;

    public List<GameObject> weapons;
    public GameObject playerPrefab;

    public int bullets = 50;
    public int bulletsMax = 240;

    public int shells = 20;
    public int shellsMax = 100;

    public int explosive = 15;
    public int explosiveMax = 50;

    public CampfireController startCampfire;

    public List<MobSpawnerController> spawners;

    public GameObject weaponToPick = null;
    public InteractiveObject npcToInteract = null;

    public Animator dialogAnimator;
    public Text dialogText;

    public bool pointerOverMenu = false;

    public ActionFeedbackController actionFeedbackController;


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
        GameObject newPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        newPlayer.name = "Player";
        playerInGame = newPlayer;
        playerController = playerInGame.GetComponent<PlayerMovement>();
        
        startCampfire = GameObject.Find(characterSpawnerName).GetComponent<CampfireController>();
        startCampfire.SpawnPlayer();
        
        if (playerWeapons.Count > 0)
            playerController.SetWeapon(playerWeapons.Count - 1);

        gui.SetHealth();

        List<GameObject> spwnrs = new List<GameObject>(GameObject.FindGameObjectsWithTag("MobSpawner"));
        foreach (GameObject i in spwnrs)
        {
            spawners.Add(i.GetComponent<MobSpawnerController>());
        }
        Time.timeScale = 1;
        gui.Fade("ToGame");

        _sm = scene;

        PlayerSetPos();

        WeaponToPick(null);
        NpcToInteract(null, "Inspect");
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
            actionFeedbackController.SetFeedback(false, "Inspect");
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
            actionFeedbackController.SetFeedback(false, type);
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
            }
            else if (npcToInteract != null)
            {
                npcToInteract.Talk();
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
                if (bullets > bulletsMax)
                    bullets = bulletsMax;
                break;

            case WeaponController.Type.Shell:
                shells += amount;
                if (shells > shellsMax)
                    shells = shellsMax;
                break;

            case WeaponController.Type.Explosive:
                explosive += amount;
                if (explosive > explosiveMax)
                    explosive = explosiveMax;
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
        print(characterSpawnerName);
        SceneManager.LoadScene(sceneName);
    }
}