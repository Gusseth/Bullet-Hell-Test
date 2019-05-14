using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugShotSpawner : MonoBehaviour {

    Shot shot;
    public GameObject shotFire;
    public GameObject source;

	// Use this for initialization
	void Start () {
        shot = new Shot();
        source = gameObject;

    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        GameObject shotSpawned = Instantiate(shotFire);
        shotSpawned.GetComponent<ShotHandler>().source = gameObject;
	}
}
