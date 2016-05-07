//script made by: up651590
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;


public class CreateGalaxy : MonoBehaviour
{
    public Text starListText;
    
    public const int starMuti = 100;
    public const int planetMuti = 50;
    //public int numStars = 100;
    private List<HoldStar> holdStars = new List<HoldStar>();
    private List<Star> realStars = new List<Star>();
    public const string starPrefabName = "StarPrefab";
    public const string binaryPrefabName = "BinaryStarPrefab";
    public const string ternayPrefabName = "TernaryStarPrefab";
    public const string blackHolePrefabName = "BlackHole";
    // private int spaceBetween = StarMuti*10;
    /// <summary>
    /// the center of the Galaxy might be usefull if you have mutiple galaxys 
    /// </summary>
    private Vector3 centerpos = Vector3.zero;
    //bool testing = false;
    //bool backhole = true;
    //bool backhole = false;
    BlackHole black;
    const int startBHoleMass = 500;
    private bool first = true;

    //counting varables to display star count on screen
    private int blackHoleCount =0;
    int[] starCount = new int[4] { 0, 0, 0, 0 };
    //The way geenration will work
    //create stars
    //create balckhole
    //move the stars towards blackhole 
    //stop all stars
    //work out type of star from size
    //then work out planets around star
    //planets that can have life marked 

    //extra
    //make stars just struts holding the location and mass and move based on that  

    // Use this for initialization
    void Start()
    {
        Random.seed = 5;
        Application.runInBackground = true;
        GenerateGalaxy();
        ShowStarList();
    }

    void GenerateGalaxy()
    {
        int theID = 0;
        int yPos = 0;
        int baseStarMass = 10;
        int radiusMax = 50;
        float radiusIncrease = 4 * (starMuti);
        int angelMax = 360;
        int spirals = 2;
        int sprialWidth = 50;
        float[] spiralAngel = new float[spirals];
        float angelhold = angelMax / spirals;
        for (int i = 0; i < spiralAngel.Length; i++)
        {
            spiralAngel[i] = i * angelhold;
        }

        //start by creating stars

        //This is how many "rows" of planets are in the spiral, (each row are a circle 
        for (float row = 1; row < radiusMax; row++)
        {
            // the current angle around the row 
            for (float angle = 0; angle < angelMax; angle += 10)
            {
                //then run for each arm of the spiral
                for (int i = 0; i < spiralAngel.Length; i++)
                {
                    // if the current angle is within the spiral then create some stars
                    if ((angle >= spiralAngel[i] && angle < spiralAngel[i] + sprialWidth || angle + 360 - sprialWidth <= spiralAngel[i] && angle + 360 > spiralAngel[i]))
                    {
                        float holdx = centerpos.x + (radiusIncrease * row) * Mathf.Cos(angle * Mathf.Deg2Rad);
                        float holdz = centerpos.z + (radiusIncrease * row) * Mathf.Sin(angle * Mathf.Deg2Rad);

                        float sample = Random.Range(0.5f, 0.9f); // The size of the star to generate                        
                        
                        HoldStar newStar = new HoldStar(Mathf.CeilToInt(sample * baseStarMass), theID, new Vector3(holdx * 10, yPos, holdz * 10));
                                       
                        theID++;
                        holdStars.Add(newStar);
                     
                        i = spiralAngel.Length;
                    }
                }
            }
            for (int i = 0; i < spiralAngel.Length; i++)
            {
                spiralAngel[i] += radiusIncrease / (starMuti); // This moves the angle that the star can spawn at along after each rotation
            }

        }
        //backhole = false;
        //once all the stars are made create a blackhole at the center 
        //GameObject ablack = Instantiate(Resources.Load(blackHolePrefabName)) as GameObject;
        //ablack.transform.SetParent(transform);
        //black = ablack.GetComponent<BlackHole>();
        //black.SetMass(startBHoleMass);

        black = MakeBlackHole(transform.position, startBHoleMass);
        black.SetCount(true);
        //then move the stars towards the blackhole
        MoveTowardsBlackHole(black, false);

        //Check to see if there are any other blackholes 
        HandelBackHoles();

        CatogriseStars();
       // Debug.Log(black.count);
    }

    /// <summary>
    /// used to handel any blackholes created after the first move and move the other stars towards them
    /// </summary>
    private void HandelBackHoles()
    {
        bool keepCheck = true;

        //loop until all blackholes are found
        while (keepCheck)
        {
            BlackHole checkBlack = CheckForBlackHoles();
            if (checkBlack == null)
            {
                keepCheck = false;
            }
            else
            {

                checkBlack.SetCount(false); // setting the number of stars it can pull in 
                MoveTowardsBlackHole(checkBlack,true);
                blackHoleCount++;
            }
        }
    }

    /// <summary>
    /// used to check for any blackholes created, and move them
    /// </summary>
    BlackHole CheckForBlackHoles()
    {
        HoldStar[] stars = holdStars.ToArray();

        for (int i = 0;i< stars.Length; i++)
        {
            if (stars[i].StarMass > 50)
            {
                HoldStar s = stars[i];
                holdStars.Remove(stars[i]);
               
                return MakeBlackHole(s.starPos,s.StarMass*5);
                
            }
        }

        return null;
    }


    private void CatogriseStars()
    {
        //types of star: tbd            (array pos)        
        //Neutron Over 40 Mass           (0)
        //otherwise:
        //Singe Star 70%                (1)
        //Twin Star 20%                 (2)
        //Ternary star10%               (3)    
        //
        foreach (HoldStar s in holdStars)
        {
           if(s.StarMass > 40)
            {
                MakeStar(s, starType.Neutron);
                starCount[(int)starType.Neutron]++;
            }
            else
            {
                int holdRand = Random.Range(0,100);
                if (holdRand < 70)
                {
                    MakeStar(s, starType.SingeStar);
                    starCount[(int)starType.SingeStar]++;
                }
                else if(holdRand < 90)
                {
                    MakeStar(s, starType.BinaryStar);
                    starCount[(int)starType.BinaryStar]++;
                }
                else 
                {
                    //Debug.Log("starType.Ternarystar");
                    MakeStar(s, starType.Ternarystar);
                    starCount[(int)starType.Ternarystar]++;
                }
            }            
        }        
    }

    /// <summary>
    /// Function to spawn a given type of star
    /// </summary>
    /// <param name="starInfo"> a Class holding the stars position and mass</param>
    /// <param name="sType"> The Type of star to spawn</param>
    private void MakeStar(HoldStar starInfo, starType sType)
    {
        GameObject stara;
        if (sType == starType.BinaryStar)
        {
             stara = (GameObject)Instantiate(Resources.Load(binaryPrefabName));
        }
        else if (sType == starType.Ternarystar)
        {
            stara = (GameObject)Instantiate(Resources.Load(ternayPrefabName));
        }
        else
        {

            stara = (GameObject)Instantiate(Resources.Load(starPrefabName));
        }
        
       
        stara.transform.SetParent(transform);
        stara.transform.position = starInfo.starPos;
        Star theStar = stara.GetComponent<Star>();
        
        theStar.typeOfStar = sType;
        theStar.SetMass(starInfo.StarMass);
        
        theStar.AssignVarables();
        realStars.Add(theStar);
        if (sType == starType.Neutron)
        {
            
            stara.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
            stara.name = "Neutron";

        }
        else if (sType == starType.SingeStar)
        {
            stara.GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
            stara.name = "SingleStar";
        }
        else if (sType == starType.BinaryStar)
        {
          for(int i =0;i< theStar.theRend.Length; i++)
            {
                theStar.theRend[i].material.SetColor("_Color", Color.red);               
                
            }
            
            stara.name = "BinaryStar";
        }
        else if (sType == starType.Ternarystar)
        {
            for (int i = 0; i < theStar.theRend.Length; i++)
            {
                theStar.theRend[i].material.SetColor("_Color", Color.magenta);
                
            }
            stara.name = "Ternarystar";
        }
    }    
    
    BlackHole MakeBlackHole(Vector3 starPos, int mass)
    {
        GameObject ablack = Instantiate(Resources.Load(blackHolePrefabName)) as GameObject;
        ablack.transform.SetParent(transform);
        ablack.transform.position = starPos;

        BlackHole tempBlack = ablack.GetComponent<BlackHole>();
        tempBlack.SetMass(mass);

        return tempBlack;
    }

    //move every star towards the backhole
    public void MoveTowardsBlackHole(BlackHole bHole,bool limit)
    {
        int i = 0;

        int maxloops = 1000; //yo stop a infinate loiop
        if (limit)
        {
            maxloops = 100;
        }

        while (bHole.count > 0 && i < maxloops) //
        {
            moveStars(bHole.transform.position, bHole.mass, limit);
            RemoveStarsInsideBH(bHole); //check for stars inside and destroy
            i++;
        }

    }

    /// <summary>
    /// make sure we remove planets inside the balckhole 
    /// </summary>
    public void RemoveStarsInsideBH(BlackHole bHole)
    {
       // SphereCollider blackColl = black.GetComponent<SphereCollider>();

        HoldStar[] tempArray = holdStars.ToArray();

        //Collider[] hitColliders = Physics.OverlapSphere(transform.position, transform.lossyScale.x);

        for (int i = 0; i < tempArray.Length; i++)
        {
            float dis = Vector3.Distance(bHole.transform.position, tempArray[i].starPos); // Get Distance Between two stars 

            if (dis < ((bHole.transform.localScale.x / 2) + tempArray[i].radius()))
            {
                //remove from the list so we dont try and acces a empty gameobjecct 
                //the count is the number of stars a blackhole can absorbe before it stops moving them
                //count--;
                bHole.reduceCount(Mathf.CeilToInt(Mathf.Log10(tempArray[i].StarMass)));
                holdStars.Remove(tempArray[i]);
            }           
            
        }
    }

    /// <summary>
    /// USed for when stars collide during the start time as oncollision does not work here 
    /// </summary>
    public void RemoveStarsInStar(HoldStar theStar)
    {
        HoldStar[] tempArray = holdStars.ToArray();

        //Collider[] hitColliders = Physics.OverlapSphere(transform.position, transform.lossyScale.x);
        bool restartCheck = false;
        for (int i = 0; i < tempArray.Length; i++)
        {
            float dis = Vector3.Distance(theStar.starPos, tempArray[i].starPos); // Get Distance Between two stars 
            
            if (tempArray[i].id != theStar.id && dis < (theStar.radius()+ tempArray[i].radius()) && tempArray[i].StarMass <= theStar.StarMass)
            {
                theStar.IncreaseMass(tempArray[i].StarMass);    
                            
                holdStars.Remove(tempArray[i]);
                restartCheck = true;
                break;
            }
        }

        //if the star has got bigger need to check all stars around it again
        if (restartCheck)
        {
            RemoveStarsInStar(theStar);
        }
       
    }


    /// <summary>
    /// for moving all stars towards a point
    /// </summary>
    /// <param name="moveTo"></param>
    /// <param name="massAtPoint"></param>
    public void moveStars(Vector3 moveTo,float massAtPoint,bool limit)
    {
        for (int i = 0; i < holdStars.Count; i++)
        {
            float force = 10 * massAtPoint / holdStars[i].StarMass; // / Vector3.Distance(transform.position, s.transform.position * s.GetComponent<Star>().mass);
            if (limit && Vector3.Distance(holdStars[i].starPos, moveTo)>4000)
            {
               

            }
            else
            {
                if (limit )
                {
                    force /= 2;

                }

                holdStars[i].starPos = Vector3.MoveTowards(holdStars[i].starPos, moveTo, force);// * Time.deltaTime);
            }

        }
        
        //We hold the list in an array temporerally so we can remove stars without error

        //as you cant remove items from a list as you cycle through it 
        HoldStar[] tempArray = holdStars.ToArray(); 
        foreach (HoldStar s in tempArray)
        {
            RemoveStarsInStar(s);
        }
    }



    void ShowStarList()
    {
        string hold = "StarList" + "\n";
        hold += "Black Hole Count: " +  blackHoleCount + "\n";

        hold += "Neutron Count: " + starCount[(int)starType.Neutron] + "\n";
        hold += "Singe Star Count: " + starCount[(int)starType.SingeStar] + "\n";
        hold += "Binary StarCount: " + starCount[(int)starType.BinaryStar] + "\n";
        hold += "Ternary Star Count: " + starCount[(int)starType.Ternarystar] + "\n";


        starListText.text = hold;

    }

   

}


//Class to hold the star and move it before we spawn it
public class HoldStar
{
    public Vector3 starPos;
    private int starMass;
    public int StarMass
    {
        get { return starMass; }
    }

    public readonly int id;

    public float radius()
    {
        return (starMass * CreateGalaxy.starMuti) / 2;
    }

    public HoldStar(int StarMass, int Id, Vector3 StarPos)
    {
        id = Id;
        starPos = StarPos;
        starMass = StarMass;
    }

    public void IncreaseMass(int addMass)
    {
        starMass += addMass;
    }
}

public enum starType
{    
    Neutron = 0,     
    SingeStar = 1,
    BinaryStar = 2,
    Ternarystar = 3,
}
