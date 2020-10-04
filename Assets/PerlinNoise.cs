using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PerlinNoise : MonoBehaviour {

    [SerializeField]
    private static readonly int[] permutation = { 151,160,137,91,90,15,                 // Hash lookup table as defined by Ken Perlin.  This is a randomly
    131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,    // arranged array of all numbers from 0-255 inclusive.
    190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
    88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
    77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
    102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
    135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
    5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
    223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
    129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
    251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
    49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
    138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180
};

    //[SerializeField]
    //public int Testingpoint;
    //public double testingpoint2;
    //[SerializeField]
    //List<Vector3> psrVals;
    //[SerializeField]
    //List<double> psrVecReturn;
    //[SerializeField]
    //double[] test = new double[100];

    Hashtable p = new Hashtable();

    
    public GameObject quadIn;
    [SerializeField]
    List<GameObject> Childlist;

    public int loopvalx = 40;
    public int loopvaly = 40;

    public int perlinLayers = 1;

    public float countPerGridsquare = 10.0f;
    public float oceanLevelCutof = 0.5f;
    public float beachRange = 0.005f;
    public float mounatinStart = 0.54f;

    bool firstrun = true;
    public bool multiLayer = false;
    public bool oceanDepthToggle = true;
    public bool beachToggle = true;

    public string OutputText;

    public Color oceanColour = new Color(0.3f, 0.4f, 1.0f);
    public Color landColour = new Color(0.5f, 0.6f, 0.5f);
    public Color beachColour = new Color(0.76f, 0.7f, 0.5f);

    public enum LandColourScaling{BlacktoColour, midpointColour, colourtoWhite};
    public LandColourScaling lCS = LandColourScaling.BlacktoColour;
    // Use this for initialization

    void Awake()
    {
        //this is me doubling up the hash table to avoid overflow later
        p.Clear();

        for (int i = 0; i < 512; i++)
        {
            int adaptor = (permutation[i % 256]);
            p.Add(i, adaptor);
        }

        OutputText = "Testing";
    }

    void Start()
    {

        //This makes the tiles. Always square at the moment. And it sets the tiles to their perlin value.
        //PERMANENT NOTE i is y, j is x
        if(multiLayer == false)
        {
            generate1LayerGrid();
        }
        else
        {
            generateMultiLayerGrid(perlinLayers);
        }
        

        
    }
	
	// Update is called once per frame
	void Update () {
		//nothing to change so far
	}

    public void generate1LayerGrid()
    {
        for (int i = 0; i < loopvaly; i++)
        {
            for (int j = 0; j < loopvalx; j++)
            {
                GameObject cloneTile;
                //cloneTile = Instantiate(thingy, new Vector3(j, 0, i), Quaternion.Euler(90,0,0), this.transform);
                cloneTile = Instantiate(quadIn, this.transform.position + new Vector3(j, 0, i), Quaternion.Euler(90, 0, 0), this.transform);

                /*
                //this adds the values to a test array so its visible as the editor does not like hash tables.
                if( i <10 && j < 10)
                    test[(i*10) + j] = NoiseGenerator(j / 10.0f, i / 10.0f);
                */
                float tempstorage = (float)NoiseGenerator(j / countPerGridsquare, i / countPerGridsquare); /*= (float)test[(i * 10) + j];*/

                //sets the colour to its perlin value.
                cloneTile.GetComponent<Renderer>().material.color = new Color(tempstorage, tempstorage, tempstorage);
                Childlist.Add(cloneTile);

                //Destroy(cloneTile, 1.0f);
            }

        }
    }

    public void generateMultiLayerGrid(int layercount)
    {
        for (int i = 0; i < loopvaly; i++)
        {
            for (int j = 0; j < loopvalx; j++)
            {
                //tile remains the same
                GameObject cloneTile;
                cloneTile = Instantiate(quadIn, this.transform.position + new Vector3(j, 0, i), Quaternion.Euler(90, 0, 0), this.transform);

                float tempstorage = 0.0f;
                float maxval = 0.0f;

                ////this would be the first layer
                //tempstorage = (float)NoiseGenerator(j / countPerGridsquare, i / countPerGridsquare);
                ////second layer
                //tempstorage += (0.5f * (float)NoiseGenerator(j / (countPerGridsquare/2), i / (countPerGridsquare/2)));
                ////third layer
                //tempstorage += ( 0.25f * (float)NoiseGenerator(j / (countPerGridsquare / 4), i / (countPerGridsquare / 4)));

                for(int l = 0; l < layercount; l++)
                {
                    tempstorage += ( (1.0f/ Mathf.Pow(2.0f, (float)l)) * (float)NoiseGenerator(j / (countPerGridsquare / (int)Mathf.Pow(2.0f, (float)l)), i / (countPerGridsquare / (int)Mathf.Pow(2.0f, (float)l))));
                    maxval += (1.0f / Mathf.Pow(2.0f, (float)l));
                }

                

                //make the total one again so / by 1.75 in the example case
                //tempstorage /= 1.75f;
                tempstorage /= maxval;

                //this is a test to see where sea levels would be
                if(tempstorage < oceanLevelCutof)
                {
                    
                    if(oceanDepthToggle)
                    {
                        //so to get the ocean colour correct, you have to understand that ocean depth is the maximum possible value.
                        //so if you want 0.7, you have to times the max, if it were say 0.5 by 7/5
                        //so it would be oceanColour.r / oceanLevelCutof
                        
                        cloneTile.GetComponent<Renderer>().material.color = new Color(tempstorage * (oceanColour.r / oceanLevelCutof), tempstorage * (oceanColour.g / oceanLevelCutof), tempstorage * (oceanColour.b / oceanLevelCutof), oceanColour.a);
                        Childlist.Add(cloneTile);
                    }
                    else
                    {
                        cloneTile.GetComponent<Renderer>().material.color = oceanColour;
                        Childlist.Add(cloneTile);
                    }


                    
                }
                else if(tempstorage > oceanLevelCutof && tempstorage < (oceanLevelCutof + beachRange) && beachToggle)
                {
                    cloneTile.GetComponent<Renderer>().material.color = beachColour;
                    Childlist.Add(cloneTile);
                }
                else if(tempstorage  < mounatinStart)
                {
                    //so the thing about this and any other intermediate range, is that the range is tiny. honestly. so to hit the colour you need a base amount, plus the potential randomness
                    //the range as of writing this is 0.5 to 0.54. which is tiny. so you take of 0.5, times it by 2.5 and you have a new 0-1 range
                    //I use mountain start and ocean level cutoff as that is the if above and below. change as needed
                    //the abs is just in case someone thinks its funny to have mountains start below the sea.

                    float potentialRange = Mathf.Abs(mounatinStart - oceanLevelCutof);
                    float adjustedTempstorage = ((tempstorage - oceanLevelCutof) * (1.0f / potentialRange));

                    //landColour
                    switch(lCS)
                    {
                        case LandColourScaling.BlacktoColour:

                            cloneTile.GetComponent<Renderer>().material.color = new Color(adjustedTempstorage * landColour.r, adjustedTempstorage * landColour.g, adjustedTempstorage * landColour.b, landColour.a);
                            Childlist.Add(cloneTile);

                            break;

                        case LandColourScaling.midpointColour:
                            break;

                        case LandColourScaling.colourtoWhite:
                            //so btc takes the colour as the max, this aims to take it as the min. soooo. (1 - val) * adjust? 

                            cloneTile.GetComponent<Renderer>().material.color = new Color(landColour.r + ((1.0f - landColour.r) * adjustedTempstorage), landColour.g + ((1.0f - landColour.g) * adjustedTempstorage), landColour.b + ((1.0f - landColour.b) * adjustedTempstorage), landColour.a);
                            Childlist.Add(cloneTile);

                            break;



                    }

                    
                }
                else
                {
                    //sets the colour to its perlin value.
                    cloneTile.GetComponent<Renderer>().material.color = new Color(tempstorage, tempstorage, tempstorage);
                    Childlist.Add(cloneTile);
                }

                
                
            }

        }
    }


    double NoiseGenerator(double x, double y)
    {

        

        //the steps as they go.

        //firstly you %1 the inputs so they are inside a unit square. 
        //then you find the gradient vectors (the vectors that were pseudorandomed)
        //Then you find the 4 vectors from the vertexes to the point in the gridsquare, these are the distance vectors
        //then we dot product the two for a final influence value which gives a value between -1 and 1
        //as we have 4 values we need to interpolate between then (lerp) 

        /*
            // Below are 4 influence values in the arrangement:
            // [g1] | [g2]
            // -----------
            // [g3] | [g4]
            int g1, g2, g3, g4;
            int u, v;   // These coordinates are the location of the input coordinate in its unit square.  
                        // For example a value of (0.5,0.5) is in the exact center of its unit square.

            int x1 = lerp(g1,g2,u);
            int x2 = lerp(g3,h4,u);

            int average = lerp(x1,x2,v);
        
        */

        //infact insead of u and v being the exact location they are smoothed versions of the location.
        //so those values go through a fade curve (6t5-15t4+10t3)

        //to start with you need two diferent versions of the inputs, integers and floats. SO make them

        x %= 256;
        y %= 256;

        int xi, yi;
        xi = (int)x;
        yi = (int)y;

        double xf, yf;
        xf = (x - xi);
        yf = (y - yi);

        double u, v;
        u = fade(xf);
        v = fade(yf);

        //next you have to pseudo the vectors of the corners.
        //so we start by getting a hash value for each corner


        // [aa] | [ba]
        // -----------
        // [ab] | [bb]


        int aa, ab, ba, bb;

        //origionals
        //aa = (int)p[((int)p[xi]) + yi];
        //ab = (int)p[(int)p[xi] + (yi + 1)];

        //ba = (int)p[((int)p[xi + 1]) + yi];
        //bb = (int)p[(int)p[xi + 1] + (yi + 1)];

        //updated
        aa = (int)p[((int)p[xi]) + (yi + 1)];
        ab = (int)p[(int)p[xi] + yi];

        ba = (int)p[((int)p[xi + 1]) + (yi + 1)];
        bb = (int)p[(int)p[xi + 1] + yi];

        //test values for the editor
        //double testval1 = grad2d(aa, xf, yf - 1);
        //double testval2 = grad2d(ab, xf , yf);

        double x1, x2, average;

        //Explain?

        //xf is your vector into the gridsquare. it is the exact amount up and across from the bottom left. So it should be in ab
        //when you invert the vector its ba. aa is invert just y, bb invert x


        x1 = lerp(grad2d(aa, xf, yf - 1), grad2d(ba, xf - 1, yf - 1), u);
        x2 = lerp(grad2d(ab, xf , yf), grad2d(bb, xf - 1, yf ), u);


        //3d tests
        //x1 = lerp(grad3d(aa, xf, yf, 0), grad3d(ab, xf, yf - 1, 0), u);
        //x2 = lerp(grad3d(ba, xf - 1, yf, 0), grad3d(bb, xf - 1, yf - 1, 0), u);

        average = lerp(x2, x1, v);


        //return average;
        //this makes it between 0 and 1
        return ((average + 1) / 2);
    }

    public static double fade(double t)
    {
        // Fade function as defined by Ken Perlin.  This eases coordinate values
        // so that they will ease towards integral values.  This ends up smoothing
        // the final output.
        return t * t * t * (t * (t * 6 - 15) + 10);         // 6t^5 - 15t^4 + 10t^3
    }

    public Vector3 getPSRVec2d(int hash)
    {
        switch(hash & 0xF)
        {
            case 0x0: return new Vector3(1,0, 1);   
            case 0x1: return new Vector3(-1,0, 1);
            case 0x2: return new Vector3(1,0, -1);
            case 0x3: return new Vector3(-1,0, -1);
            case 0x4: return new Vector3(1, 0, 1);
            case 0x5: return new Vector3(-1, 0, 1);
            case 0x6: return new Vector3(1, 0, -1);
            case 0x7: return new Vector3(-1, 0, -1);
            case 0x8: return new Vector3(1, 0, 1);
            case 0x9: return new Vector3(-1, 0, 1);
            case 0xA: return new Vector3(1, 0, -1);
            case 0xB: return new Vector3(-1, 0, -1);
            case 0xC: return new Vector3(1, 0, 1);
            case 0xD: return new Vector3(-1, 0, 1);
            case 0xE: return new Vector3(1, 0, -1);
            case 0xF: return new Vector3(-1, 0, -1);


            default: return new Vector3(0,0,0); // never happens
        }
    }

    public static double grad2d(int hash, double x, double y)
    {
        //you need to dot product each of the values that come in. 
        
        //so for reasons I have yet to understand we are ignoring the cosine. Just roll with it
        //soo, its psrgen x * x and psrgen y * y and as they are unit vectors, this affects the outcomes. 

        //in order, the four unit vectors you are using are (1,1) (-1,1) (1,-1) (-1,-1) and you are mod sixteening it and repeating four times. 
        //they are now (1,0) (-1,0) (0,1) (0, -1) for... testing perpouses - COMMENTED OUT
        
        
        //switch (hash & 0xF)
        //{
        //    case 0x0: return x;
        //    case 0x1: return -x;
        //    case 0x2: return x ;
        //    case 0x3: return -x;
        //    case 0x4: return x ;
        //    case 0x5: return -x;
        //    case 0x6: return x ;
        //    case 0x7: return -x;
        //    case 0x8: return y ;
        //    case 0x9: return y ;
        //    case 0xA: return -y;
        //    case 0xB: return -y;
        //    case 0xC: return y ;
        //    case 0xD: return y ;
        //    case 0xE: return -y;
        //    case 0xF: return -y;
        //    default: return 0; // never happens
        //}
        double dotval;

        switch (hash & 0xF)
        {
            case 0x0: dotval = x + y;
                break;
            case 0x1: dotval = -x + y;
                break;
            case 0x2: dotval = x - y;
                break;
            case 0x3: dotval = -x - y;
                break;
            case 0x4: dotval = x + y;
                break;
            case 0x5: dotval = -x + y;
                break;
            case 0x6: dotval = x - y;
                break;
            case 0x7: dotval = -x - y;
                break;
            case 0x8: dotval = y + x;
                break;
            case 0x9: dotval = y - x;
                break;
            case 0xA: dotval = -y + x;
                break;
            case 0xB: dotval = -y - x;
                break;
            case 0xC: dotval = y + x;
                break;
            case 0xD: dotval = y - x;
                break;
            case 0xE: dotval = -y + x;
                break;
            case 0xF: dotval = -y - x;
                break;
            default: dotval = 44; // never happens
                break;
        }

        //they are devided by two in frame, mainly so that the outcome is between -1 and 1. were there three units it would have to be by three. 
        dotval /= 2.0f;
        return dotval;
    }

    public static double grad3d(int hash, double x, double y, double z)
    {
        switch (hash & 0xF)
        {
            case 0x0: return x + y;
            case 0x1: return -x + y;
            case 0x2: return x - y;
            case 0x3: return -x - y;
            case 0x4: return x + z;
            case 0x5: return -x + z;
            case 0x6: return x - z;
            case 0x7: return -x - z;
            case 0x8: return y + z;
            case 0x9: return -y + z;
            case 0xA: return y - z;
            case 0xB: return -y - z;
            case 0xC: return y + x;
            case 0xD: return -y + z;
            case 0xE: return y - x;
            case 0xF: return -y - z;
            default: return 0; // never happens
        }
    }

    public static double lerp(double a, double b, double x)
    {
        return a + (x * (b - a));
    }

    public void destroyTiles()
    {
        for (int i = 0; i < Childlist.Count; i++)
        {
            Destroy(Childlist[i], 0.0f);
        }

        Childlist.Clear();
    }

    //getters and seters. more to come

    public bool getMultipleLayers()
    {
        return multiLayer;
    }
    public int getLayerCount()
    {
        return perlinLayers;
    }

    

    //neither currently in use

    void OnDrawGizmos()
    {//future note, link this to transform so its acurate. I need it like this to start however. 

        //toolbox

        // this gets me the hash value (int)p[((int)p[x]) + y];
        //getPSRVec2d takes a has returns vector3 but its the corect ones for 2d space

        for (int i = 0; i <= (loopvaly / countPerGridsquare); i++)
        {
            for (int j = 0; j <= (loopvalx / countPerGridsquare); j++)
            {
                ////this is gridsquare points
                //Vector3 curentpoint = this.transform.position + new Vector3((j * countPerGridsquare) - 0.5f, 0, (i * countPerGridsquare) - 0.5f);
                

                //Gizmos.color = Color.blue;
                //Gizmos.DrawSphere(curentpoint, 0.2f);

                //Gizmos.color = Color.red;
                //Gizmos.DrawLine(curentpoint, (curentpoint + new Vector3((float)grad2d((int)p[((int)p[j]) + i], 2, 0),0,(float)grad2d((int)p[((int)p[j]) + i], 0, 2))));
                

                //this is the psrvectos
                //Gizmos.DrawLine(curentpoint, (curentpoint + getPSRVec2d((int)p[((int)p[j]) + i])));


                //debug code
                /*
                if (firstrun)
                {
                    psrVals.Add(getPSRVec2d((int)p[((int)p[j]) + i]));
                    psrVecReturn.Add(grad2d((int)p[((int)p[j]) + i], 1, 1));
                }

                */

            }
        }
        if (firstrun)
            firstrun = false;
    }
    
    void OnGUI()
    {
        //creates text 200 units down from the top and 40 units from the right side of the screen
        //that second 200 I don't know what it does in practice
        //0,0 on the text box is bottom left
        //GUI.Label(new Rect(Screen.width - 250, 0, 200, Screen.height - 80), "Octave 4");
        //GUI.Label(new Rect( 250, 0, 200, Screen.height - 80), "Octave 1");
        //GUI.Label(new Rect(480, 0, 200, Screen.height - 80), "Octave 2");
        //GUI.Label(new Rect(720, 0, 200, Screen.height - 80), "Octave 3");
    }

}
