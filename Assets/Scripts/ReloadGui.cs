using UnityEngine;
using System.Collections;

public class ReloadGui : MonoBehaviour {

    public Animator anim;
    public Animator sliderAnim;
    public GameObject greenZone;

    public bool reload = false;
    public bool slider = false;

    public int reloadAmount = 0;

    public void StartReload(float greenPosX, int rld) // greenpos min -30; max 30
    {
        reloadAmount = rld;
        sliderAnim.transform.localPosition = new Vector2(-75f, 0);
        anim.SetBool("Active", true);
        greenZone.transform.localPosition = new Vector3(greenPosX, 0, 0);
        transform.position = Camera.main.WorldToScreenPoint(new Vector3(GameManager.instance.playerInGame.transform.position.x, GameManager.instance.playerInGame.transform.position.y + 2f, GameManager.instance.playerInGame.transform.position.z));
        StartCoroutine("Reloading");
    }

    IEnumerator Reloading()
    {
        reload = true;
        yield return new WaitForSeconds(0.2f);
        slider = true;
        yield return new WaitForSeconds(0.5f);
        StartCoroutine("ReloadSuccess", 2f); // slow reload
        slider = false;
        anim.SetBool("Active", false);
    }

    IEnumerator ReloadSuccess(float wait)
    {
        // RELOAD ANIMATION HERE
        yield return new WaitForSeconds(wait);
        reload = false;
        WeaponController weaponController = GameManager.instance.playerController.weaponController;
        weaponController.Reload(reloadAmount);
        GameManager.instance.SetAmmo(weaponController.weaponAmmoType, -reloadAmount);
        GameManager.instance.gui.SetAmmo(weaponController.weaponAmmoType);
        GameManager.instance.gui.SetWeapon();
    }

    IEnumerator ReloadFailed()
    {
        yield return new WaitForSeconds(0.75f);
        reload = false;

    }

    void Update()
    {
        if (slider)
        {
            sliderAnim.transform.localPosition = new Vector2(sliderAnim.transform.localPosition.x + 300 * Time.deltaTime, 0);

            if (Input.GetButtonDown("Reload"))
            {
                float distance = Vector2.Distance(sliderAnim.transform.localPosition, greenZone.transform.localPosition);

                print(distance);

                if (distance > 35f) // fail
                {
                    StartCoroutine("ReloadFailed");
                    StopCoroutine("Reloading");
                }
                else
                {
                    StartCoroutine("ReloadSuccess", 0.75f); // fast reload
                    StopCoroutine("Reloading");
                }
                anim.SetBool("Active", false);
                slider = false;
            }

        }
    }
}
