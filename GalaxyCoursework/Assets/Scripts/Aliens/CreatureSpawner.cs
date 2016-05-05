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

    //How many creatures to spawn on the planet
    private int amount;

    // Use this for initialization
    void Start() {
        //Set the amount
        amount = Random.Range(1, 5);
        //Spawn those craetures
        StartCoroutine(spawnCreatures());
    }

    private IEnumerator spawnCreatures() {
        for(int i = 0; i < amount; i++) {
            //Spawn in one creature for now
            GameObject creature = GameObject.Instantiate(creatureTypes[Random.Range(1, creatureTypes.Length)]);

            //Wait one frame
            yield return null;

            //Set the parent and scale
            creature.transform.SetParent(gameObject.transform);
            creature.transform.localScale /= scaleValue;

            //Get a spawn position within the surface
            Vector3 spawnPosition = new Vector3((Random.value * 2) - 1, 0, (Random.value * 2) - 1);
            //The off set to put the creature on the ground
            Vector3 spawnOffset = Vector3.up * creature.transform.localScale.y * 3;
            //Combine the two values
            spawnPosition += spawnOffset;
            //Set the position
            creature.transform.localPosition = spawnPosition;

            //Make sure the creature wont move too fast
            creature.GetComponent<AlienAI>().setSpeedScale(scaleValue);
        }
    }
}
