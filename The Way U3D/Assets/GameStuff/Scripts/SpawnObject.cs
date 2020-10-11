using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObject : MonoBehaviour {
    public GameObject ai;
    public float spawnTimeRate = 5;
    public bool loop = false;
    public bool spawnAfter = false;

    private bool spawned = false;
    // Use this for initialization
    void Start () {
        if (!spawnAfter)
        {
            Instantiate(ai,transform.position, transform.rotation);
            spawned = true;
            StartCoroutine(setSpawnedFalse());
        } else
        {
            StartCoroutine(SpawnAfter());
        }
    }
	
	// Update is called once per frame
	void Update () {
		if (!spawned & loop)
        {
            Instantiate(ai, transform.position, transform.rotation);
            spawned = true;
            StartCoroutine(setSpawnedFalse());
            loop = false;
        }
	}

    IEnumerator setSpawnedFalse()
    {
        yield return new WaitForSeconds(spawnTimeRate);
        spawned = false;
    }

    IEnumerator SpawnAfter()
    {
        yield return new WaitForSeconds(spawnTimeRate);
        Instantiate(ai, transform.position, transform.rotation);
        spawned = true;
        StartCoroutine(setSpawnedFalse());
    }
}
