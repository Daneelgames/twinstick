using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
    
    public static GameManager instance = null;

    public GuiController gui;

    public GameObject playerInGame;
    public PlayerMovement playerController;
    public List<GameObject> playerWeapons;

    public int playerExp = 0;

    public GameObject weaponToPick = null;

    public List<GameObject> weapons;
    public GameObject playerPrefab;

    public int bullets = 50;
    public int bulletsMax = 240;

    public int shells = 20;
    public int shellsMax = 100;

    public int explosive = 15;
    public int explosiveMax = 50;

    void Awake()
    {
        if (instance == null)
            instance = this;

        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start () {
        GameObject newPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        newPlayer.name = "Player";
        playerInGame = newPlayer;
        playerController = playerInGame.GetComponent<PlayerMovement>();
        GameObject newWeapon = Instantiate (weapons[0], Vector3.zero, Quaternion.identity) as GameObject;

        playerController.SetWeapon(newWeapon, true);

        gui.SetHealth();
    }
    
    public void GetExp()
    {
        playerExp += 1;
    }

    public void WeaponToPick(GameObject weapon)
    {
        weaponToPick = weapon;
    }

    void Update()
    {
        if (Input.GetButtonDown("Submit") && weaponToPick != null)
        {
            playerController.SetWeapon(weaponToPick, true);
            WeaponToPick(null);
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
}
