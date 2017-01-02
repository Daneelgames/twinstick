using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	void FixedUpdate ()
    {
        if(GameManager.instance.playerController != null && !GameManager.instance.cutScene)
        {
            float posX = Mathf.Lerp(transform.position.x, GameManager.instance.playerController.cameraFocus.transform.position.x, 0.1f);
            float posY = Mathf.Lerp(transform.position.y, GameManager.instance.playerController.cameraFocus.transform.position.y + 3, 0.1f);

            if (posX > GameManager.instance._sm.cameraMaxX)
                posX = GameManager.instance._sm.cameraMaxX;
            if (posX < GameManager.instance._sm.cameraMinX)
                posX = GameManager.instance._sm.cameraMinX;

            transform.position = new Vector3(posX, posY, transform.position.z);
            transform.LookAt(new Vector3(posX, posY - 2f, GameManager.instance.playerController.cameraFocus.transform.position.z));
        }
    }
}