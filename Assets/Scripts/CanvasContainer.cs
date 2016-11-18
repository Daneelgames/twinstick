using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CanvasContainer : MonoBehaviour {

    public Canvas cnvs;

    public Animator dialogAnimator;
    public Text dialogText;
    public ActionFeedbackController actionFeedback;

    public Image healthbar;
    public Animator healthbarAnimator;
    public Animator saveAnimator;
    public ReloadGui reloadController;
    public Animator fadeAnimator;
    public RawImage fadeImg;

    public InventoryControllerGUI inventory;

    public void SetRenderCamera()
    {
        cnvs.worldCamera = Camera.main;
        cnvs.planeDistance = 0.25f;
    }
}