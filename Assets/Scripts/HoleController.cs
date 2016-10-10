using UnityEngine;
using System.Collections;

public class HoleController : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.tag == "Player")
        {
            GameManager.instance.playerController.holeInteractor.ToggleOverHole(true);
        }
        else if (coll.tag == "ExpDrop" || coll.tag == "Mob" || coll.tag == "Bullet")
        {
            HoleInteractor hi = coll.GetComponent<HoleInteractor>();
            if (hi != null)
                hi.ToggleOverHole(true);
        }
        else if (coll.tag == "Weapon")
        {
            WeaponController wc = coll.GetComponent<WeaponController>();
            if (wc != null && !wc.inHands)
            {
                wc.holeInteractor.ToggleOverHole(true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.tag == "Player")
        {
            GameManager.instance.playerController.holeInteractor.ToggleOverHole(false);
        }
        else if (coll.tag == "ExpDrop" || coll.tag == "Mob" || coll.tag == "Bullet")
        {
            HoleInteractor hi = coll.GetComponent<HoleInteractor>();
            if (hi != null)
                hi.ToggleOverHole(false);
        }
        else if (coll.tag == "Weapon")
        {
            WeaponController wc = coll.GetComponent<WeaponController>();
            if (wc != null && !wc.inHands)
            {
                wc.holeInteractor.ToggleOverHole(false);
            }
        }
    }
}