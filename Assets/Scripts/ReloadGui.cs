using UnityEngine;
using System.Collections;

public class ReloadGui : MonoBehaviour
{

    public Animator anim;
    public GameObject sliderGO;
    public GameObject greenZone;

    public bool reload = false;
    public bool slider = false;

    public int reloadAmount = 0;
    PlayerMovement pm;

    public void StartReload(float greenPosX, int rld) // greenpos min -30; max 30
    {
        pm = GameManager.instance.playerController;

        reloadAmount = rld;
        sliderGO.transform.localPosition = new Vector2(-75f, 0);
        anim.SetBool("Reload", true);

        if (reloadAmount > 0)
            pm.SetAnimBool("Reload", true);
        else
            pm.SetAnimBool("Heal", true);

        greenZone.transform.localPosition = new Vector3(greenPosX, 0, 0);
        //transform.localPosition = Camera.main.WorldToViewportPoint(new Vector3(GameManager.instance.playerInGame.transform.position.x, GameManager.instance.playerInGame.transform.position.y, GameManager.instance.playerInGame.transform.position.z));
        StartCoroutine("Reloading");
        print("start reload");
    }

    IEnumerator Reloading()
    {
        reload = true;
        yield return new WaitForSeconds(0.2f);
        slider = true;
        yield return new WaitForSeconds(0.5f);
        StartCoroutine("ReloadSuccess", 3f); // slow reload
        slider = false;
        anim.SetBool("Reload", false);
    }

    IEnumerator ReloadSuccess(float wait)
    {
        // RELOAD ANIMATION HERE
        yield return new WaitForSeconds(wait - 2f);
        pm.SetAnimBool("HealSucc", true);
        yield return new WaitForSeconds(2f);
        reload = false;
        if (reloadAmount > 0)
        {
            WeaponController weaponController = pm.weaponController;
            weaponController.Reload(reloadAmount);
            GameManager.instance.Reload(weaponController, reloadAmount);
        }
        else
        {
            HealthController ph = pm.playerHealth;
            ph.Heal(Mathf.RoundToInt(ph.maxHealth / 2));
        }
        Finish();
    }

    IEnumerator ReloadFailed()
    {
        yield return new WaitForSeconds(1.5f);
        pm.SetAnimBool("Heal", false);
        yield return new WaitForSeconds(0.2f);
        reload = false;
        Finish();
    }

    void Finish()
    {
        pm.SetAnimBool("Reload", false);
        pm.ReloadOver();
        pm.HealOver();
        pm.SetAnimBool("HealSucc", false);
        pm.SetAnimBool("Heal", false);
    }

    public void BreakProcess()
    {
        if (reload)
        {
            StopAllCoroutines();
            reload = false;
            Finish();
        }
    }

    void Update()
    {
        if (slider)
        {
            sliderGO.transform.localPosition = new Vector2(sliderGO.transform.localPosition.x + 300 * Time.deltaTime, 0);

            if (Input.GetButtonDown("Reload") && reloadAmount > 0)
            {
                ActiveReload();
            }
            else if (Input.GetButtonDown("Heal") && reloadAmount <= 0)
            {
                ActiveReload();
            }
        }
    }

    void ActiveReload()
    {
        float distance = Vector2.Distance(sliderGO.transform.localPosition, greenZone.transform.localPosition);

        print(distance);
        if (distance > 35f) // fail
        {
            StopCoroutine("Reloading");
            StartCoroutine("ReloadFailed");
        }
        else
        {
            StopCoroutine("Reloading");
            StartCoroutine("ReloadSuccess", 0.75f); // fast reload
        }
        anim.SetBool("Reload", false);
        slider = false;
    }
}