using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class BiomeColliderScript : MonoBehaviour {

    //Set on spawn
    [HideInInspector] public Transform parenToSet;
    [HideInInspector] public biomes biomeType;

    /// <summary>
    /// Helper function for setting up this collider
    /// </summary>
    public void setUp() {
        StartCoroutine(setParent());
    }

    /// <summary>
    /// Sets the parent after 1 frame has passed
    /// </summary>
    /// <returns></returns>
    private IEnumerator setParent() {
        //Wait one frame because of how unity handles instantiating
        yield return null;

        //now set the parent
        transform.SetParent(parenToSet);
    }

    public void OnTriggerEnter(Collider other) {
        //Let the creature know it is in this biome
    }
}
