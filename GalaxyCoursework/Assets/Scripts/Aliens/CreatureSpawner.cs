using UnityEngine;
using System.Collections;

/*
 * This script is attached to planets and will spawn a 
 * a certain amount of creatures provided
 */

public class CreatureSpawner : MonoBehaviour {

    //Set in inspector
    public GameObject[] creatureTypes;

    //Quick way of checking what creature type has been spawned
    private bool[] creatureSpawned;

	// Use this for initialization
	void Start () {
        //Init the bool array
        creatureSpawned = new bool[creatureTypes.Length];
        //How many total types to spawn
        int amountOfTypestoSpawn = Random.Range(1, creatureTypes.Length + 1);
        //Loop through the array and start spawning creatures
        for(int i = 0; i < amountOfTypestoSpawn; i++) {
            if(!creatureSpawned[i]) {
                //Instantiate the prefab
                GameObject creature = GameObject.Instantiate<GameObject>(creatureTypes[i]);
                //Set the tranform
                creature.transform.SetParent(transform);
                creature.transform.localPosition = new Vector3();
                creature.transform.localRotation = Quaternion.identity;
                creature.transform.localScale = new Vector3(1, 1, 1);
                creatureSpawned[i] = true;
            }
        }
    }
}
