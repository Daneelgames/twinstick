using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponController : MonoBehaviour {

    public float reloadTime = 0.25f;

    public GameObject shotHolder;

    public List<GameObject> bullets;


    float curReload = 0f;

	void Update () {

        if (curReload > 0)
        {
            curReload -= Time.deltaTime;
        }
	}

    public void Shot()
    {
        if (curReload <= 0)
        {
            curReload = reloadTime;
            for (int i = 0; i < bullets.Count; i++)
            {
                GameObject newBullet = Instantiate(bullets[i], shotHolder.transform.position, Quaternion.identity) as GameObject;
                BulletController newBulletController = newBullet.GetComponent<BulletController>();
                
                newBulletController.SetDirection(transform.eulerAngles.z);
            }
        }
    }
}