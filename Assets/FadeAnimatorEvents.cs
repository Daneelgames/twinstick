using UnityEngine;
using System.Collections;

public class FadeAnimatorEvents : MonoBehaviour
{

    public void Fade(int value)
    {
        bool fade = false;
        if (value == 1)
            fade = true;
        GameManager.instance.gui.SetFade(fade);
    }
}
