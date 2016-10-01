using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
    
    public static GameManager instance = null;

    public Animator camAnim;

    public GuiController gui;

    public GameObject playerInGame;
    public PlayerMovement playerController;
    public List<GameObject> playerWeapons;

    public int playerExp = 0;

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

    public bool pointerOverMenu = false;

    public SkillList _skillList;

    public List<SkillShopController> shops;

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
    
    public void AddShopToList(SkillShopController shop)
    {
        shops.Add(shop);
    }

    public void PlayerDead()
    {
        StartCoroutine("RespawnPlayer");
    }

    void ResetShops()
    {
        foreach (SkillShopController shop in shops)
        {
            shop.SetSkills();
        }
    }

    IEnumerator RespawnPlayer()
    {
        int skillsExp = 0;

        foreach (SkillController skill in _skillList.playerSkills)
        {
            skillsExp += skill.skillCost;
        }
        playerController.playerHealth.dropController.expAmount = playerExp + skillsExp;
        playerController.playerHealth.dropController.DeathDrop(true);
        playerExp = 0;
        _skillList.LoseAllSkills();
        gui.SetExp();

        yield return new WaitForSeconds(1f);
        playerInGame.SetActive(true);
        lastCampfire.SpawnPlayer();
        RespawnMobs();
        ResetShops();
    }

    public void RespawnMobs()
    {
        //destroy exp drop
        List<GameObject> expDrop = new List<GameObject>(GameObject.FindGameObjectsWithTag("ExpDrop"));
        foreach(GameObject i in expDrop)
        {
            i.GetComponent<ExpDropController>().DestroyOnRespawn();
        }


        foreach (MobSpawnerController i in spawners)
        {
            i.DestroyMob();
            i.Spawn();
        }
    }

    public void GetExp(int amount)
    {
        playerExp += amount;
        gui.SetExp();
    }

    public void RemoveExp(int amount)
    {
        playerExp -= amount;
        gui.SetExp();
    }

    public void WeaponToPick(GameObject weapon)
    {
        weaponToPick = weapon;
    }

    public void CampfireToInteract(CampfireController campfire)
    {
        campfireToInteract = campfire;
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
                lastCampfire.skillShop.ShopToggle(true);
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