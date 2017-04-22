using UnityEngine;
using System.Collections;

public class CameraZoneController : MonoBehaviour
{
    public GameObject camAnchor;
    public bool follow = false;
    public bool active = false;

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player" && !active)
        {
            GameManager.instance.SetActiveCamera(this);
        }
    }

    void FixedUpdate()
    {
        if (active && follow)
        {
            if (GameManager.instance.playerController && !GameManager.instance.playerController.grabTransform && !GameManager.instance.cutScene)
            {
                float posX = Mathf.Lerp(camAnchor.transform.position.x, GameManager.instance.playerController.cameraFocus.transform.position.x, 0.66f);
                float posY = Mathf.Lerp(camAnchor.transform.position.y, GameManager.instance.playerController.cameraFocus.transform.position.y + 1, 0.66f);
                float posZ = Mathf.Lerp(camAnchor.transform.position.z, GameManager.instance.playerController.cameraFocus.transform.position.z, 0.66f);
                camAnchor.transform.LookAt(new Vector3(posX, posY, posZ));
            }
        }
    }
}