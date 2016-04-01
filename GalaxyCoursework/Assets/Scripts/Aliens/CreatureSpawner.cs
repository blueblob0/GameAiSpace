using UnityEngine;
using System.Collections.Generic;

/*
 * This script is attached to planets and will spawn a 
 * a certain amount of the creatures provided
 */

public class CreatureSpawner : MonoBehaviour {

    //Set in inspector
    public GameObject[] creatureTypes;

    //Quick way of checking what creature type has been spawned
    private bool[] creatureSpawned;

    //Check if the creatures have been scaled
    private bool allScaled;


    private List<GameObject> spawnedCreatures;

    // Use this for initialization
    void Start() {
        //Init
        allScaled = false;
        spawnedCreatures = new List<GameObject>();

        //Init the bool array
        creatureSpawned = new bool[creatureTypes.Length];
        //How many total types to spawn
        int amountOfTypestoSpawn = Random.Range(1, creatureTypes.Length + 1);
        //Loop through the array and start spawning creatures
        for(int i = 0; i < amountOfTypestoSpawn; i++) {
            if(!creatureSpawned[i]) {
                //Instantiate the prefab
                GameObject creature = Instantiate(creatureTypes[i]);
                //Set the tranform
                creature.transform.SetParent(transform);
                creature.transform.localPosition = Vector3.zero;
                creature.transform.localRotation = Quaternion.identity;
                creature.transform.localScale = Vector3.one;
                creatureSpawned[i] = true;
                //Finally, add to list
                spawnedCreatures.Add(creature);
            }
        }
    }

    void Update() {
        if(!allScaled) {
            allScaled = true;
            for(int i = 0; i < spawnedCreatures.Count; i++) {
                if(spawnedCreatures[i].GetComponent<AlienCreature>().isSpawned() && !spawnedCreatures[i].GetComponent<AlienCreature>().checkScaled()) {
                    spawnedCreatures[i].transform.localScale *= 0.1f;
                    spawnedCreatures[i].GetComponent<AlienCreature>().hasBeenScaled(true);
                } else if(!spawnedCreatures[i].GetComponent<AlienCreature>().checkScaled()) {
                    allScaled = false;
                }

            }
        }
    }
}
