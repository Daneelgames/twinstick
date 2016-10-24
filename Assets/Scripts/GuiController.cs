using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GuiController : MonoBehaviour {

    public Image healthbar;
    public Animator healthAnimator;
    float fill;

    public ReloadGui reloadController;

    public Animator fadeAnimator;

    public void SetHealth()
    {
        healthAnimator.SetTrigger("Update");
        fill = GameManager.instance.playerController.playerHealth.health * 1.0f / GameManager.instance.playerController.playerHealth.maxHealth * 1.0f;
        healthbar.fillAmount = fill;
    }

    public void Fade(string fade)
    {
        switch(fade)
        {
            case "ToBlack":
                fadeAnimator.SetTrigger("Black");
                break;
            case "ToGame":
                fadeAnimator.SetTrigger("Game");
                break;
        }
    }
}
