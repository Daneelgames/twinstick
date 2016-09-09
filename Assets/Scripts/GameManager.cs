using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public GameObject player;
    public List<GameObject> weapons;

	// Use this for initialization
	void Start () {
        GameObject newPlayer = Instantiate(player, Vector3.zero, Quaternion.identity) as GameObject;
        newPlayer.name = "Player";
        GameObject newWeapon = Instantiate (weapons[0], Vector3.zero, Quaternion.identity) as GameObject;
        newWeapon.transform.SetParent(newPlayer.transform);
        newWeapon.name = "Weapon";
        newWeapon.transform.localPosition = new Vector3(0, 0.5f, -1f);
        newPlayer.GetComponent<PlayerMovement>().SetWeapon(newWeapon);
    }
}
