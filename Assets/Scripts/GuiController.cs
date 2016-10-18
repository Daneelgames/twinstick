using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GuiController : MonoBehaviour {

    public Image healthbar;
    public Text healthCounter;
    public Animator healthAnimator;
    float fill;

    public List<Image> weapons;
    public List<Text> weaponCounters;

    public Animator ammoAnim;

    public Image bulletsAmmo;
    public Image shellsAmmo;
    public Image explosiveAmmo;

    public Text exp;

    public ReloadGui reloadController;

    public void SetHealth()
    {
        healthCounter.text = GameManager.instance.playerController.playerHealth.health + " / " + GameManager.instance.playerController.playerHealth.maxHealth;
        healthAnimator.SetTrigger("Update");

        fill = GameManager.instance.playerController.playerHealth.health * 1.0f / GameManager.instance.playerController.playerHealth.maxHealth * 1.0f;
        healthbar.fillAmount = fill;
    }

    public void SetWeapon()
    {
        for (int i = 0; i < GameManager.instance.playerWeapons.Count; i ++)
        {
            if (GameManager.instance.playerWeapons.Count > i)
            {
                //weapons[i].sprite = GameManager.instance.playerWeapons[i].GetComponent<SpriteRenderer>().sprite;
                weapons[i].color = Color.white;
                weapons[i].SetNativeSize();

                string ammo = "0";
                WeaponController wpn = GameManager.instance.playerWeapons[i].GetComponent<WeaponController>();
                switch (wpn.weaponAmmoType)
                {
                    case WeaponController.Type.Bullet:
                        ammo = wpn.ammo +"/" + GameManager.instance.bullets;
                        break;

                    case WeaponController.Type.Shell:
                        ammo = wpn.ammo + "/" + GameManager.instance.shells;
                        break;

                    case WeaponController.Type.Explosive:
                        ammo = wpn.ammo + "/" + GameManager.instance.explosive;
                        break;
                }
                weaponCounters[i].text = ammo;
            }
        }

        switch(GameManager.instance.playerWeapons.Count) // hide inactive weapon icons
        {
            case 0:
                weapons[0].color = Color.clear;
                weaponCounters[0].text = "";
                break;
            case 1:
                weapons[1].color = Color.clear;
                weaponCounters[1].text = "";
                break;
        }

        // set weapons ammo counters
        if (GameManager.instance.playerWeapons.Count > 0)
        {
            if (GameManager.instance.playerController.weaponController.gameObject == GameManager.instance.playerWeapons[0])
            {
                weapons[0].color = Color.white;

                if (GameManager.instance.playerWeapons.Count > 1)
                    weapons[1].color = Color.black;
            }
            else
            {
                weapons[0].color = Color.black;

                if (GameManager.instance.playerWeapons.Count > 1)
                    weapons[1].color = Color.white;
            }
        }

    }

    public void SetAmmo(WeaponController.Type type)
    {
        bulletsAmmo.fillAmount = GameManager.instance.bullets * 1.0f / GameManager.instance.bulletsMax * 1.0f;
        shellsAmmo.fillAmount = GameManager.instance.shells * 1.0f / GameManager.instance.shellsMax * 1.0f;
        explosiveAmmo.fillAmount = GameManager.instance.explosive * 1.0f / GameManager.instance.explosiveMax * 1.0f;

        ammoAnim.SetTrigger("Update");

        StartCoroutine("AmmoBlink", type);
    }

    IEnumerator AmmoBlink(WeaponController.Type type)
    {
        switch (type)
        {
            case WeaponController.Type.Bullet:
                bulletsAmmo.color = Color.white;
                break;

            case WeaponController.Type.Shell:
                shellsAmmo.color = Color.white;
                break;

            case WeaponController.Type.Explosive:
                explosiveAmmo.color = Color.white;
                break;
        }

        yield return new WaitForSeconds(0.1f);
        bulletsAmmo.color = Color.red;
        shellsAmmo.color = Color.red;
        explosiveAmmo.color = Color.red;
    }
}
