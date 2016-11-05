using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	void FixedUpdate ()
    {
        if(GameManager.instance.playerInGame != null && !GameManager.instance.cutScene)
        {
            float posX = Mathf.Lerp(transform.position.x, GameManager.instance.playerInGame.transform.position.x, 0.125f);
            float posY = Mathf.Lerp(transform.position.y, GameManager.instance.playerInGame.transform.position.y + 3, 0.125f);

            if (posX > GameManager.instance._sm.cameraMaxX)
                posX = GameManager.instance._sm.cameraMaxX;
            if (posX < GameManager.instance._sm.cameraMinX)
                posX = GameManager.instance._sm.cameraMinX;

            transform.position = new Vector3(posX, posY, transform.position.z);
            transform.LookAt(new Vector3(posX, posY - 2f, GameManager.instance.playerInGame.transform.position.z));
        }
    }
}