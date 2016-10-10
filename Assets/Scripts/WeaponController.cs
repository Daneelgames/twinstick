using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponController : MonoBehaviour {

    public enum Type {Bullet, Shell, Explosive};

    public bool automatic = false;

    public float reloadTime = 0.25f;

    public GameObject shotHolder;

    public List<GameObject> bullets;

    public float curReload = 0f;

    bool canPick = false;
    public bool inHands = false;

    public Type weaponAmmoType = Type.Bullet;

    public HoleInteractor holeInteractor; 


    public void SwitchInhands(bool hands)
    {
        canPick = false;

        if (!hands)
        {
            transform.SetParent(null);
            transform.localEulerAngles = new Vector3(0, 0, Random.Range(0, 360));
        }
        inHands = hands;

    }

    void OnTriggerStay2D(Collider2D coll)
    {
        if (coll.tag == "Player" && !canPick && !inHands)
        {
            GameManager.instance.WeaponToPick(gameObject);
            canPick = true;
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.tag == "Player")
        {
            GameManager.instance.WeaponToPick(null);
            canPick = false;
        }
    }

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