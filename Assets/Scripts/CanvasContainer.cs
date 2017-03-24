using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CanvasContainer : MonoBehaviour
{

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
    public MapController map;
    public InventoryControllerGUI inventory;
    public void SetRenderCamera()
    {
        cnvs.renderMode = RenderMode.ScreenSpaceCamera;
        cnvs.worldCamera = Camera.main.transform.Find("CanvasCamera").GetComponent<Camera>();
        cnvs.planeDistance = 0.25f;
    }
    public void SetCanvasCam (Camera cam)
    {
        cnvs.worldCamera = cam;
    }
}