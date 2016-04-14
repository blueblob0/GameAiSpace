using UnityEngine;
using System.Collections.Generic;

public class CreateGalaxy : MonoBehaviour
{
    public const int starMuti = 50;
    public int numStars = 100;
    private List<GameObject> stars = new List<GameObject>();
    public string starPrefabName = "StarPrefab";
    // private int spaceBetween = StarMuti*10;
    /// <summary>
    /// the center of the Galaxy might be usefull if you have mutiple galaxys 
    /// </summary>
    private Vector3 centerpos = Vector3.zero;
    bool testing = false;
    bool backhole = true;
    //bool backhole = false;
    BlackHole black;

    //The way geenration will work
    //create suns
    //create baclhole
    //move the suns towards blackhole 
    //stop all suns
    //work out type of sun from size
    //then work out planets around sun
    //planets that can have life marked 

       

    // Use this for initialization
    void Start()
    {        
        Application.runInBackground = true;
        GenerateGalaxy();


    }

    void GenerateGalaxy()
    {
        Vector3 starPos;
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

        //start by creating suns

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

                        float sample = Random.Range(0.4f, 1.2f); // The size of the star to generate 
                        GameObject Stara = null;
                        if (sample > 0)
                        {
                            starPos = new Vector3(holdx * 10, yPos, holdz * 10);
                            Stara = Instantiate(Resources.Load(starPrefabName)) as GameObject;
                            Stara.transform.SetParent(transform);

                            stars.Add(Stara);
                            Stara.transform.position = starPos;

                            Stara.GetComponent<Star>().SetMass(Mathf.CeilToInt(sample * baseStarMass));
                            Stara.GetComponent<Star>().angle = angle;
                            Stara.GetComponent<Star>().spiralAngel = spiralAngel[i];
                        }
                        i = spiralAngel.Length;
                    }
                }
            }
            for (int i = 0; i < spiralAngel.Length; i++)
            {
                spiralAngel[i] += radiusIncrease / (starMuti); // This moves the angle that the sun can spawn at along after each rotation

            }

        }
        //backhole = false;
        //once all the stars are made create a blackhole at the center 
        if (backhole)
        {
            GameObject ablack = Instantiate(Resources.Load("BlackHole")) as GameObject;
            ablack.transform.SetParent(transform);
            black = ablack.GetComponent<BlackHole>();
        }

        //then move the suns towards the blackhole
       // black.MoveTowardsBlackHole();



    }




    GameObject CheckStarAlreadyThere(int xcord, int zcord, float spaceBetween)
    {

        foreach (GameObject s in stars)
        {
            // if theese are true than the 
            // if(s.transform.position.x -spaceBetween >xcord || s.transform.position.x + spaceBetween < xcord){}

            // if this is true then the planet is within the x of this
            if (s.transform.position.x - spaceBetween < xcord && s.transform.position.x + spaceBetween > xcord)
            {
                if (s.transform.position.z - spaceBetween < zcord && s.transform.position.z + spaceBetween > zcord)
                {
                    // Debug.Log(s.transform.position.x + " " + xcord + " " + s.transform.position.z + " " + zcord);
                    // if this is ever reached then there is an overlap
                    return s;
                }
            }

        }


        return null;

    }

    GameObject CheckStarAlreadyThere(float spaceBetween, Transform theStar)
    {

        foreach (GameObject s in stars)
        {
            if (s != theStar.gameObject)
            {
                // Debug.Log("1");
                // if theese are true than the 
                // if(s.transform.position.x -spaceBetween >xcord || s.transform.position.x + spaceBetween < xcord){}

                // if this is true then the planet is within the x of this
                if (s.transform.position.x - spaceBetween < theStar.position.x && s.transform.position.x + spaceBetween > theStar.position.x)
                {
                    if (s.transform.position.z - spaceBetween < theStar.position.z && s.transform.position.z + spaceBetween > theStar.position.z)
                    {
                        //Debug.Log(s.transform.position.x + " " + theStar.position.x + " " + s.transform.position.z + " " + theStar.position.z);
                        // if this is ever reached then there is an overlap
                        return s;
                    }
                }
                Debug.LogError(s.transform.position.x + " " + theStar.position.x + " " + s.transform.position.z + " " + theStar.position.z + " " + spaceBetween);

            }

        }


        return null;

    }



    void CheckStarNear(int xcord, int zcord)
    {
        int spaceNear = 100;
        foreach (GameObject s in stars)
        {
            // if this is true then the planet is within the x of this
            if (s.transform.position.x - spaceNear < xcord && s.transform.position.x + spaceNear > xcord)
            {
                if (s.transform.position.z - spaceNear < zcord && s.transform.position.z + spaceNear > zcord)
                {
                    s.name = "1";
                    //Debug.Log(s.transform.position.x + " " + xcord + " " + s.transform.position.z + " " + zcord);
                }
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        float checkDist = -1;
        //if (Input.GetKey("h") && testing)
        if (testing)
        {
            bool checkmove = false;

            //List<GameObject> star2 = new List<GameObject>(stars);
            //List<GameObject> star2 = stars;
            //star2 = stars;
            foreach (GameObject s in stars)
            {
                Star sStar = s.GetComponent<Star>();
                Collider[] hold = Physics.OverlapSphere(s.transform.position, 100 + sStar.mass / 1);

                Vector3 holdPos = s.transform.position;
                //Debug.Log("1");
                // star2.Remove(s);
                foreach (Collider sa in hold)
                {
                    //Debug.Log("2");
                    Star saStar = sa.GetComponent<Star>();
                    if (saStar)
                    {


                        // float force = 10000 * saStar.Mass / Vector3.Distance(sa.transform.position, s.transform.position * sStar.Mass);
                        //s.transform.position = Vector3.MoveTowards(s.transform.position, sa.transform.position, force * Time.deltaTime);
                        // Debug.Log(force);    
                        //Debug.Log(s.transform.position);
                        //Debug.Log(s.transform.position);
                        //Debug.Log(sa.transform.position);
                        float force = 1000 * sStar.Mass;

                        Vector3 pos = sa.transform.position;
                        pos *= saStar.Mass;
                        force /= Vector3.Distance(s.transform.position, pos);
                        sa.transform.position = Vector3.MoveTowards(sa.transform.position, s.transform.position, force * Time.deltaTime);
                        // Debug.Log(force);    

                    }

                }

                if (Vector3.Distance(holdPos, s.transform.position) > 0.0001f)
                {
                    checkmove = true;

                }
                else
                {
                    if (Vector3.Distance(holdPos, s.transform.position) > checkDist)
                    {

                        checkDist = Vector3.Distance(holdPos, s.transform.position);

                    }


                }

            }

            if (!checkmove)
            {
                checkmove = true;
                //Debug.Log("No more move " + checkDist);
            }


            if (!checkmove)
            {
                testing = false;
            }


            if (black)
            {

                foreach (GameObject s in stars)
                {

                    float force = 1000 * black.mass / Vector3.Distance(black.transform.position, s.transform.position * s.GetComponent<Star>().mass);
                    s.transform.position = Vector3.MoveTowards(s.transform.position, black.transform.position, force * Time.deltaTime);
                    // Debug.Log(force); 
                }

            }

        }

        if (Input.GetKey("h") && !testing)
        {
            Debug.Log("No more move");
        }

    }

    /// <summary>
    /// Destory a star and handles clean up removing from the star list 
    /// </summary>
    /// <param name="toRemove">Star to be removed</param>
    public void DestroyStar(GameObject toRemove)
    {
        stars.Remove(toRemove);
        Destroy(toRemove);
    }

    /// <summary>
    /// for moving all stars towards a point
    /// </summary>
    /// <param name="moveTo"></param>
    /// <param name="massAtPoint"></param>
    public void moveStars(Vector3 moveTo,float massAtPoint)
    {
        foreach (GameObject s in stars)
        {
            float force = 100 * massAtPoint / s.GetComponent<Star>().mass; // / Vector3.Distance(transform.position, s.transform.position * s.GetComponent<Star>().mass);
            s.transform.position = Vector3.MoveTowards(s.transform.position, moveTo, force * Time.deltaTime);
        }



    }



}



