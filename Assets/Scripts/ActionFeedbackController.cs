using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ActionFeedbackController : MonoBehaviour {

    public Sprite doorSprite;
    public Sprite inspectSprite;

    public Image spr;
    public Animator anim;

    public void SetFeedback(bool active, string type)
    {
        switch (type)
        {
            case "Inspect":
                spr.sprite = inspectSprite;
                break;
            case "Door":
                spr.sprite = doorSprite;
                break;
        }

        anim.SetBool("Active", active);
    }
}
