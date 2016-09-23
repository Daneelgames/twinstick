using UnityEngine;
using System.Collections;

public class DropOnDeathController : MonoBehaviour {

    public int expAmount = 5;
    public GameObject expPrefab;
    
    public void DeathDrop(bool player)
    {
        for (int i = 0; i < expAmount; i ++)
        {
            float randomH = Random.Range(-1f, 1f);
            float randomV = Random.Range(-1f, 1f);
            GameObject newExpDrop = Instantiate(expPrefab, transform.position + new Vector3(randomH, randomV, transform.position.z), Quaternion.identity) as GameObject;
            ExpDropController expController = newExpDrop.GetComponent<ExpDropController>() as ExpDropController;
            expController.Explosion(transform);

            if (player)
                expController.SetPlayer();
        }
    }
}