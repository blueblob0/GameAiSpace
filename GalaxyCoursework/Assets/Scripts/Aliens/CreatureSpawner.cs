using UnityEngine;
using System.Collections;
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

    //Keep track of the creatures spawned
    private List<GameObject> spawnedCreatureTypes;

	// Use this for initialization
	void Start () {
        //Init the list
        spawnedCreatureTypes = new List<GameObject>();

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
                spawnedCreatureTypes.Add(creature);
                //Then wait before evyerthing is scaled doen
                StartCoroutine(scaleCreatures(spawnedCreatureTypes));
            }
        }
    }

    //Called at the end to scale all the creatures down
    IEnumerator scaleCreatures(List<GameObject> creatures) {
        //Wait
        yield return null;

        //Scale of creature types
        for(int i = 0; i < creatures.Count; i++) {
            //Keep the mscaled with the ratio
            creatures[i].transform.localScale /= CreateGalaxy.starMuti;
        }
    }
}
