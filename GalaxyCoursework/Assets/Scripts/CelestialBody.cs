using UnityEngine;
using System.Collections.Generic;

public class CelestialBody : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    /// <summary>
    /// spawing Satalite around things 
    /// </summary>
    /// <param name="MoveAmount"></param>
    /// <param name="minDist"> The min dsitance from previous one</param>
    /// <param name="MaxDist"> the max distance</param>
    /// <param name="prefabName"> name of prefab to spawn</param>
    /// <returns></returns>
    protected GameObject SpawnSatalite(float MoveAmount, float minDist,float MaxDist,string prefabName)
    {
        
        // star by moving out a bit from the planet 
        float move = MoveAmount + Random.Range(minDist, MaxDist);

        Vector3 starPos;

        starPos = Random.insideUnitCircle.normalized * move;
        //Vector3 starPos = Vector3.one * move;
        starPos.z = starPos.y;
        starPos.y = 0;

        //Debug.Log(MoveAmount + " " + move + " " + Vector3.Distance(Vector3.zero, starPos));
        GameObject a = Instantiate(Resources.Load(prefabName)) as GameObject;
        Satalite holds = a.GetComponent<Satalite>();
        holds.orbitingBody = gameObject;
        holds.distPlanet = move;
        a.name = MoveAmount.ToString();
        a.transform.SetParent(gameObject.transform);                
        a.transform.localPosition = starPos;
        return a;
    }
    
}
