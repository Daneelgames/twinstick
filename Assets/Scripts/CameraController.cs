using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	void Update ()
    {
        if(GameManager.instance.playerInGame != null)
        {
            float posX = Mathf.Lerp(transform.position.x, GameManager.instance.playerInGame.transform.position.x, 0.5f);
            float posY = Mathf.Lerp(transform.position.y, GameManager.instance.playerInGame.transform.position.y, 0.5f);
            transform.position = new Vector3(posX, posY, transform.position.z);
        }
    }
}
