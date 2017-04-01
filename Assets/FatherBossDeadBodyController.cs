using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FatherBossDeadBodyController : MonoBehaviour
{
    public List<GameObject> meatMeshes;
    public int leftRow = 0;
    public int rightRow = 1;
    void Start()
    {
        // random angle
        Quaternion turnRotation = Quaternion.identity;
        float y = Random.Range(0, 360);
        turnRotation.eulerAngles = new Vector3(0, y, 0);
        transform.rotation = turnRotation;

        //random mesh
        int r = Random.Range(0, meatMeshes.Count);
        for (int i = 0; i < meatMeshes.Count; i++)
        {
            if (i == r) meatMeshes[i].SetActive(true);
            else meatMeshes[i].SetActive(false);
        }
    }

    public void DisableBody()
    {
        gameObject.SetActive(false);
    }
}