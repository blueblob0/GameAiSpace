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

    //How much to scale the creatures by
    public int scaleValue = 100;

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
        creature.transform.localScale /= scaleValue;                                        //Scale the creature down
        creature.transform.localPosition = Vector3.up * creature.transform.localScale.y * 3;    //Move the creature up
    }

    void Update() {
        
    }
}
