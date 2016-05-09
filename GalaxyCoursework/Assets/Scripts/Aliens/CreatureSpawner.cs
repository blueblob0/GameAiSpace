//Script made by: 626224
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
    public Transform[] spawnPlaces;

    //Keep track of what has been spawned where
    private bool[] placeTaken;

    //How much to scale the creatures by
    public int scaleValue = 100;

    //How many creatures to spawn on the planet
    private int amount;

    // Use this for initialization
    void Start() {
        placeTaken = new bool[spawnPlaces.Length];
        for(int i = 0; i < placeTaken.Length; i++) {
            placeTaken[i] = false;
        }

        //Set the amount
        amount = Random.Range(1, 5);
        //Spawn those craetures
        StartCoroutine(spawnCreatures());
    }

    private IEnumerator spawnCreatures() {
        for(int i = 0; i < amount; i++) {
            //Make sure there is a free space left
            bool allTaken = true;
            for(int k = 0; k < placeTaken.Length; k++) {
                if(!placeTaken[k]) {
                    allTaken = false;
                    break;
                }
            }
            if(allTaken) {
                yield break;
            }

            //Spawn in the creature
            GameObject creature = GameObject.Instantiate(creatureTypes[Random.Range(0, creatureTypes.Length)]);

            //Wait one frame
            yield return null;

            //Set the parent and scale
            creature.transform.SetParent(gameObject.transform);
            creature.transform.localScale /= scaleValue;

            //Get a spawn position within the surface
            int rand;
            Vector3 spawnPosition;
            do {
                rand = Random.Range(0, spawnPlaces.Length);
                spawnPosition = spawnPlaces[rand].localPosition;
            } while(placeTaken[rand]);
            placeTaken[rand] = true;
            
            //Set the position
            creature.transform.localPosition = spawnPosition;

            //Make sure the creature wont move too fast
            creature.GetComponent<AlienAI>().setPlanetScale(scaleValue);

            //Reproduce the creature a few times
            int reproAmount = Random.Range(2, 5);
            for(int j = 0; j < reproAmount; j++) {
                creature.GetComponent<AlienAI>().reproduce();
                yield return null;
            }
        }
    }
}
