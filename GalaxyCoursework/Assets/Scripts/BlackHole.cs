﻿//script made by: up651590
using UnityEngine;
using System.Collections;

public class BlackHole : CelestialBody
{
    public  int count{ get; private set; }
   
//bool check = false;
	// Use this for initialization
	protected override void  Start ()
    {
        base.Start();       
        

        if (!controler)
        {
            controler = FindObjectOfType<CreateGalaxy>();
        }
        int holdcount = count; //store count as it would be lowered by any stars spawning sindie the hole 
        controler.RemoveStarsInsideBH(this);
        count = holdcount;
    }


    protected override void SetScale()
    {
        transform.localScale = Vector3.one * mass *CreateGalaxy.starMuti/50; 
    }

    /// <summary>
    /// sets the number of stars the black hole can take in before it stops
    /// </summary>
    /// <param name="isBig">if the black hole is the one in th center</param>
    public void SetCount(bool isBig)
    {
        if (isBig)
        {
            count = 20;
        }
        else
        {
            count = 1;
        }
    }

    public void reduceCount(int amount)
    {
        count -= amount;

    }




    void OnTriggerEnter(Collider other)
    {
       // Debug.Log("1");
        if(other.tag != "CelestialBody")
        {
            return;
        }
    }


}
