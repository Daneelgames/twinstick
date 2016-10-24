using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
    
    public static GameManager instance = null;

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
    public CampfireController lastCampfire;

    public List<MobSpawnerController> spawners;

    public GameObject weaponToPick = null;
    public CampfireController campfireToInteract = null;
    public InteractiveObject npcToInteract = null;

    public Animator dialogAnimator;
    public Text dialogText;

    public bool pointerOverMenu = false;


    void Awake()
    {
        if (instance == null)
            instance = this;

        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        
        GameObject newPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        newPlayer.name = "Player";
        playerInGame = newPlayer;
        playerController = playerInGame.GetComponent<PlayerMovement>();

        startCampfire.SpawnPlayer();
        lastCampfire = startCampfire;

        GameObject newWeapon = Instantiate(weapons[0], Vector3.zero, Quaternion.identity) as GameObject;

        playerController.SetWeapon(newWeapon, true);

        gui.SetHealth();

        List<GameObject> spwnrs = new List<GameObject>(GameObject.FindGameObjectsWithTag("Spawner"));
        foreach (GameObject i in spwnrs)
        {
            spawners.Add(i.GetComponent<MobSpawnerController>());
        }
        
    }

    void Start()
    {
        PlayerDead();
    }
    
    public void PlayerDead()
    {
        StartCoroutine("RespawnPlayer");
    }


    IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(1f);
        playerInGame.SetActive(true);
        lastCampfire.SpawnPlayer();
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
    }

    public void CampfireToInteract(CampfireController campfire)
    {
        campfireToInteract = campfire;
    }

    public void NpcToInteract(InteractiveObject npc)
    {
        npcToInteract = npc;
    }

    void Update()
    {
        if (Input.GetButtonDown("Submit"))
        {
            if (weaponToPick != null)
            {
                playerController.SetWeapon(weaponToPick, true);
                WeaponToPick(null);
            }
            else if (campfireToInteract != null)
            {
                lastCampfire = campfireToInteract;
                playerController.playerHealth.Heal(playerController.playerHealth.maxHealth);
                RespawnMobs();
            }
            else if (npcToInteract != null)
            {
                npcToInteract.Talk();
            }
        }

        if (Input.GetButtonDown("ChangeWeapon") && playerWeapons.Count > 1)
        {
            if (playerController.weaponController.gameObject == playerWeapons[0])
            {
                print("change weapon to 1");
                playerController.SetWeapon(playerWeapons[1], false);
            }
            else
            {
                print("change weapon to 0");
                playerController.SetWeapon(playerWeapons[0], false);
            }

            gui.SetWeapon();
        }
    }

    public void AddPlayerWeapon(GameObject weapon)
    {
        playerWeapons.Add(weapon);
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
}