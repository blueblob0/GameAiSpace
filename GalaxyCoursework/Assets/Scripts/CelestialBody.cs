﻿//script made by: up651590
using UnityEngine;

using System.Collections;
public abstract class CelestialBody : MonoBehaviour {
  
    
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
    protected GameObject SpawnSatalite(float moveAmount, float minDist, float maxDist, string prefabName, Vector2 crcle)
    {
        // start by moving out a bit from the planet 
        float move = moveAmount + Random.Range(minDist, maxDist);       
        Vector3 starPos;

        starPos = crcle * move;        
        starPos.z = starPos.y + transform.position.z;
        starPos.x = starPos.x + transform.position.x;
        starPos.y = 0;
        
        GameObject a = Instantiate(Resources.Load(prefabName)) as GameObject;
        Satalite holds = a.GetComponent<Satalite>();
        holds.SetScale();
        holds.orbitingBody = gameObject;
        holds.distPlanet = move;
        a.name = moveAmount.ToString();
        a.transform.SetParent(gameObject.transform);
        a.transform.position = starPos;
        return a;
    }

       
    protected abstract void  SetScale();

    public void SetMass(int newMass)
    {
        mass = newMass;
        SetScale();
    }

    //used to reduce the alpha of the star when the player moves in
   protected IEnumerator ReduceAlpha(Renderer starRend)
    {
        while (starRend.material.color.a > 0f)
        {
            
            Color c = starRend.material.color;
            c.a -= 0.1f;
            //Debug.Log(theRend.material.color.a);
            starRend.material.color = c;
            yield return new WaitForSeconds(0.1f);
        }
    }

    //used to increase the alpha of the star when the player moves out
    protected virtual IEnumerator IncreaseAlpha(Renderer starRend)
    {
        while (starRend.material.color.a < 1f)
        {
            Color c = starRend.material.color;
            c.a += 0.1f;
            //Debug.Log(theRend.material.color.a);
            starRend.material.color = c;
            yield return new WaitForSeconds(0.1f);
        }
    }


}

//used to store the details of the satalite 
public struct SataliteDetails
{
    public Vector3 location;
    public float distFromBody;
    public bool haveLifeHold;
    public SataliteDetails(Vector3 aLocation,float hold,bool life)
    {
        location = aLocation;
        distFromBody = hold;
        haveLifeHold = life;
    }
}



