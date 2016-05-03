using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/*
 * This script is attached to planets and will spawn a 
 * a certain amount of the creatures provided
 */

public class CreatureSpawner : MonoBehaviour {

    //Set in inspector
    public GameObject[] creatureTypes;

    // Use this for initialization
    void Start() {
        StartCoroutine(spawnCreatures());
    }

    private IEnumerator spawnCreatures() {
        //Spawn in one creature for now
        GameObject creature = GameObject.Instantiate(creatureTypes[0]);

        //Wait one frame
        yield return null;

        //Set the parent / local variables
        creature.transform.SetParent(gameObject.transform);
        creature.transform.localPosition = Vector3.zero;
        creature.transform.localScale /= 100;

        //Temp
        creature.GetComponent<AlienAI>().enabled = false;
    }

    void Update() {
        
    }
}
