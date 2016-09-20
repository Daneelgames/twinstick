using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
    
    public GameObject playerPrefab;
    public List<GameObject> weapons;

    public static GameManager instance = null;

    public GameObject playerInGame;

    public int playerExp = 0;

    public GameObject weaponToPick = null;

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
        GameObject newWeapon = Instantiate (weapons[0], Vector3.zero, Quaternion.identity) as GameObject;

        playerInGame.GetComponent<PlayerMovement>().SetWeapon(newWeapon);
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
        if (Input.GetButtonDown("Submit"))
        {
            playerInGame.GetComponent<PlayerMovement>().SetWeapon(weaponToPick);
        }
    }
}
