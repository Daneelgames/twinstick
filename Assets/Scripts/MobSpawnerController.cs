using UnityEngine;
using System.Collections;

public class MobSpawnerController : MonoBehaviour {

    public GameObject mobToSpawn;
    public GameObject spawnedMob;
    public bool spawned = false;

    public void Spawn()
    {
        if (!spawned)
        {
            spawnedMob = Instantiate(mobToSpawn, transform.position, transform.rotation) as GameObject;
            spawned = true;
        }
    }

    public void DestroyMob()
    {
        if (spawnedMob != null)
        {
            Destroy(spawnedMob);
        }
        spawnedMob = null;
        spawned = false;
    }
}