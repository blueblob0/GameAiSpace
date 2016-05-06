//script made by: up651590
using UnityEngine;
using System.Collections.Generic;

public class CelestialBody : MonoBehaviour {

    
    protected CreateGalaxy controler;

    public int mass;
    

    // Use this for initialization
    protected virtual void Start () {
        controler = FindObjectOfType<CreateGalaxy>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    /// <summary>
    /// spawing Satalite around things 
    /// </summary>
    /// <param name="moveAmount">The distance you have to move to start spawning from</param>
    /// <param name="minDist"> The min dsitance from previous one</param>
    /// <param name="maxDist"> the max distance</param>
    /// <param name="prefabName"> name of prefab to spawn</param>
    /// <returns></returns>
    protected GameObject SpawnSatalite(float moveAmount, float minDist,float maxDist,string prefabName)
    {
        
        // star by moving out a bit from the planet 
        float move = moveAmount + Random.Range(minDist, maxDist);
       // GameObject a = Instantiate(Resources.Load(prefabName)) as GameObject;
       // a.transform.SetParent(transform);
        Vector3 starPos;

        starPos = Random.insideUnitCircle.normalized * move;
        //Vector3 starPos = Vector3.one * move;
        starPos.z = starPos.y + transform.position.z;
        starPos.x = starPos.x + transform.position.x;
        starPos.y = 0;

        //Debug.Log(MoveAmount + " " + move + " " + Vector3.Distance(Vector3.zero, starPos));
        GameObject a = Instantiate(Resources.Load(prefabName)) as GameObject;
        Satalite holds = a.GetComponent<Satalite>();
        holds.orbitingBody = gameObject;
        holds.distPlanet = move;
        a.name = moveAmount.ToString();
        a.transform.SetParent(gameObject.transform);                
        a.transform.position = starPos;
        return a;
    }
   
    /// <summary>
    /// spawing Satalite around things 
    /// </summary>
    /// <param name="moveAmount">The distance you have to move to start spawning from</param>
    /// <param name="minDist"> The min dsitance from previous one</param>
    /// <param name="maxDist"> the max distance</param>
    /// <param name="prefabName"> name of prefab to spawn</param>
    /// <returns></returns>
    protected GameObject SpawnSatalite(float moveAmount, float minDist, float maxDist, string prefabName, Vector2 crcle)
    {

        // star by moving out a bit from the planet 
        float move = moveAmount + Random.Range(minDist, maxDist);
        // GameObject a = Instantiate(Resources.Load(prefabName)) as GameObject;
        // a.transform.SetParent(transform);
        Vector3 starPos;

        starPos = crcle * move;
        //Vector3 starPos = Vector3.one * move;
        starPos.z = starPos.y + transform.position.z;
        starPos.x = starPos.x + transform.position.x;
        starPos.y = 0;

        //Debug.Log(MoveAmount + " " + move + " " + Vector3.Distance(Vector3.zero, starPos));
        GameObject a = Instantiate(Resources.Load(prefabName)) as GameObject;
        Satalite holds = a.GetComponent<Satalite>();
        holds.orbitingBody = gameObject;
        holds.distPlanet = move;
        a.name = moveAmount.ToString();
        a.transform.SetParent(gameObject.transform);
        a.transform.position = starPos;
        return a;
    }


    /// <summary>
    /// spawing satalites
    /// </summary>
    /// <param name="starPos">The stars position info</param>
    /// <param name="prefabName">String of th prefab to spawn</param>
    /// <returns> the satalite spawned</returns>
    protected virtual GameObject SpawnSatalite(SataliteDetails starPos, string prefabName)
    {

        // start by moving out a bit from the planet       

        //Debug.Log(MoveAmount + " " + move + " " + Vector3.Distance(Vector3.zero, starPos));        
        GameObject a = Instantiate(Resources.Load(prefabName)) as GameObject;
        Satalite holds = a.GetComponent<Satalite>();
        holds.orbitingBody = gameObject;
        holds.distPlanet = starPos.distFromBody;
        a.name = starPos.distFromBody.ToString(); 
        a.transform.SetParent(gameObject.transform);
        a.transform.localPosition = starPos.location;
        return a;
    }

    




    /// <summary>
    /// Getting the locations to sapwn satalites 
    /// </summary>
    /// <param name="moveAmount"></param>
    /// <param name="minDist"></param>
    /// <param name="maxDist"></param>
    /// <returns></returns>
    protected SataliteDetails SataliteLocation(float moveAmount, float minDist, float maxDist,Vector2 circle)
    {

        // star by moving out a bit from the planet 
        float move = moveAmount + Random.Range(minDist, maxDist);
        // GameObject a = Instantiate(Resources.Load(prefabName)) as GameObject;
        // a.transform.SetParent(transform);
        Vector3 starPos;

        starPos = circle * move;
        //Vector3 starPos = Vector3.one * move;
        starPos.z = starPos.y; //+ transform.position.z;
        //starPos.x = starPos.x + transform.position.x;
        starPos.y = 0;


        SataliteDetails a = new SataliteDetails(starPos, move);

        return a;
        //Debug.Log(MoveAmount + " " + move + " " + Vector3.Distance(Vector3.zero, starPos));

    }




}


public struct SataliteDetails
{
    public Vector3 location;
    public float distFromBody;
    public SataliteDetails(Vector3 aLocation,float hold)
    {
        location = aLocation;
        distFromBody = hold;
    }
}



