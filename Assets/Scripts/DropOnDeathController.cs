using UnityEngine;
using System.Collections;

public class DropOnDeathController : MonoBehaviour {

    public int expAmount = 5;
    public GameObject exp1Prefab;
    public GameObject exp10Prefab;

    public void DeathDrop(bool player)
    {
        while (expAmount >= 10)
        {
            float randomH = Random.Range(-1f, 1f);
            float randomV = Random.Range(-1f, 1f);
            GameObject newExpDrop = Instantiate(exp10Prefab, transform.position + new Vector3(randomH, randomV, transform.position.z), Quaternion.identity) as GameObject;
            ExpDropController expController = newExpDrop.GetComponent<ExpDropController>() as ExpDropController;
            expController.Explosion(transform);

            expAmount -= 10;

            if (player)
                expController.SetPlayer();
        }

        while (expAmount >= 1)
        {
            float randomH = Random.Range(-1f, 1f);
            float randomV = Random.Range(-1f, 1f);
            GameObject newExpDrop = Instantiate(exp1Prefab, transform.position + new Vector3(randomH, randomV, transform.position.z), Quaternion.identity) as GameObject;
            ExpDropController expController = newExpDrop.GetComponent<ExpDropController>() as ExpDropController;
            expController.Explosion(transform);

            expAmount -= 1;

            if (player)
                expController.SetPlayer();
        }

    }
}