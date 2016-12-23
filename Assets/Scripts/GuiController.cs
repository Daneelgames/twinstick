using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GuiController : MonoBehaviour
{

    public Image healthbar;
    float fill;

    public Animator saveAnimator;

    public ReloadGui reloadController;

    public Animator fadeAnimator;
    public RawImage fadeImg;

    public bool fade = false;

    public void SetHealth()
    {
        fill = GameManager.instance.playerController.playerHealth.health * 1.0f / GameManager.instance.playerController.playerHealth.maxHealth * 1.0f;
        healthbar.fillAmount = fill;
    }

    public void SetFade(bool f)
    {
        fade = f;
    }

    public void Fade(string fade)
    {
        fadeAnimator.SetTrigger(fade);
    }

    public void Save()
    {
        StartCoroutine("AnimateSave");
    }

    IEnumerator AnimateSave()
    {
        saveAnimator.SetBool("Active", true);
        yield return new WaitForSeconds(1f);
        saveAnimator.SetBool("Active", false);
    }

    public void InstantBlack()
    {
        fadeAnimator.SetTrigger("InstantBlack");
    }
}
