/*
    Authors: Massimo Clementi, Elisa Nicolussi Paolaz
    Date: 23 october 2019
    Project: Simulation of a crowded environment using Unity C# script
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyntheticCrowdGenerator : MonoBehaviour{

    // Define public script variables (accessible from Unity)
    public GameObject[] PrefabsMale,PrefabsFemale,PrefabsGirl;  //specific male and female prefabs
    public int femaleToMaleRatio_100;
    public int girlRatio_100;
    public int xIstances,yIstances;     //number of people in x and y directions
    public float xSpacing, ySpacing;
    public int posRand_10; //contribute of the random position
    public int columnRand_100; //contribute of the random column displacement
    public int rowRandCumul_100; //contribute of the random cumulative whole rows displacement
    public int rotBase, rotRange;


    //Define private script variables
    GameObject PrefabToUse;
    List<GameObject> crowd = new List <GameObject>();   //N.B: mandatory to initialize with 0par constructor
    int personId = 0;
    float xToInstantiate,yToInstantiate;
    Quaternion rotToInstantiate;
    System.Random rnd = new System.Random();
    float randTemp;
    float columnRandIndex = 0, rowRandCumulIndex = 0;
    bool maleBool;
    List<Color> palette_dark = new List <Color>(), palette_bright = new List <Color>(),
                palette_hair_common = new List <Color>(), palette_hair_unusual = new List <Color>(),
                palette_skin_color = new List <Color>();
    int nCrowd = 0;
    int nCrowd_temp = 0;
    Camera cam;
    Vector3 headPosition_temp;
    float offset = (float)1; //TODO: set properly depending on Prefab and height
    List<Vector3> headPositions = new List <Vector3>(); //TODO: save to .txt or .csv

    void Start(){

        for (int y = 0; y < yIstances; y++){

            InitializeColorPalettes();

            // Random components evaluation
            columnRandIndex = (float)(rnd.Next(0,columnRand_100)-columnRand_100/2)/100;
            rowRandCumulIndex += (float)(rnd.Next(0,rowRandCumul_100))/100;

             for (int x = 0; x < xIstances; x++){

                //TODO: need to understand how to change animation (or animation start)
                //      for each person to make them move asynchronously


                // Manage instantiation variables
                //  Placement
                    randTemp = (float)(rnd.Next(0,posRand_10)-posRand_10/2)/100;   //cast needed
                    xToInstantiate = (x + columnRandIndex + randTemp - xIstances/2) * xSpacing;
                    randTemp = (float)(rnd.Next(0,posRand_10)-posRand_10/2)/100;
                    yToInstantiate = (y + rowRandCumulIndex + randTemp - yIstances/2) * ySpacing;

                //  Rotation
                    randTemp = (float)(rnd.Next(0,2*rotRange)-rotRange);
                    rotToInstantiate = Quaternion.AngleAxis(rotBase+randTemp,Vector3.up);

                //  Gender
                    maleBool = rnd.Next(0,100)<femaleToMaleRatio_100;
                    if (maleBool){
                        if(rnd.Next(0,100)<girlRatio_100) PrefabToUse = PrefabsGirl[Random.Range(0, PrefabsGirl.Length)];
                        else PrefabToUse = PrefabsFemale[Random.Range(0, PrefabsFemale.Length)];
                    }
                    else PrefabToUse = PrefabsMale[Random.Range(0, PrefabsMale.Length)];

                // INSTANTIATE the person
                    crowd.Insert(personId,
                                 Instantiate(PrefabToUse,
                                             new Vector3(xToInstantiate,0,yToInstantiate),
                                             rotToInstantiate) as GameObject);

                // Change model colors
                    var personColorList = crowd[personId].GetComponent<UMA.CharacterSystem.DynamicCharacterAvatar>()
                                                    .characterColors;
                    
                    SetRandColor(personColorList,"Hair",palette_hair_common,palette_hair_unusual,75);   // Hair
                    personColorList.SetColor("Skin",RandColorFromList(palette_skin_color));             // Skin color           
                    SetRandColor(personColorList,"Shirt",palette_dark,palette_bright,90);               // Shirt
                    SetRandColor(personColorList,"Shirt2",palette_dark,palette_bright,20);               // ...
                    SetRandColor(personColorList,"Pants1",palette_dark,palette_bright,90);              // Pants
                    SetRandColor(personColorList,"PantsAccent",palette_dark,palette_bright,30);         // ...


                // TODO: change height and weight
                // CODE HERE

                
                // Update head positions list
                    headPosition_temp = crowd[personId].GetComponent<Transform>().position;
                    headPosition_temp.y += offset;
                    Debug.Log(headPosition_temp);
                    headPositions.Insert(personId,headPosition_temp);

                personId++;
             }
        }

        // Print true positions of the people in the 3D scene
        //PrintReferencePositions(crowd);
        // TODO: save in .csv file
        

    }


    bool ProbabEvaluation(int trsh){
        // Compute the probab boolean value
        return rnd.Next(0,100)<trsh;
    }

    void SetRandColor(UMA.CharacterSystem.DynamicCharacterAvatar.ColorValueList personColorList,
                        string name,List<Color> palette_1,List<Color> palette_2,int trsh){
        /*  Evaluate the given probability distribution and set the desired field with a random
            color from the winning palette
        */ 

        if(ProbabEvaluation(trsh)){
            personColorList.SetColor(name,RandColorFromList(palette_1));
        } else {
            personColorList.SetColor(name,RandColorFromList(palette_2));
        }
    }

    Color RandColorFromList(List<Color> palette){
        return palette[rnd.Next(0,palette.Count)];
    }

    void InitializeColorPalettes(){

            palette_dark.Insert(0,Color.black);
            palette_dark.Insert(1,Color.grey);
            palette_dark.Insert(2,new Color(0.06f,0.15f,0.28f));  // dark blue
            palette_dark.Insert(3,new Color(0.38f,0f,0.05f));     // dark red
            palette_dark.Insert(4,new Color(0.02f,0.2f,0.1f));    // dark blue


            //TODO: change these colors? Too bright?
            palette_bright.Insert(0,Color.white);
            palette_bright.Insert(1,Color.green);
            palette_bright.Insert(2,Color.yellow);
            palette_bright.Insert(3,Color.red);
            palette_bright.Insert(4,Color.magenta);
            palette_bright.Insert(5,Color.cyan);
            palette_bright.Insert(6,Color.blue);

            // Hair palettes
            palette_hair_common.Insert(0,new Color(0.3f,0.16f,0.05f));  // brown
            palette_hair_common.Insert(1,Color.black);
            palette_hair_unusual.Insert(0,new Color(0.8f,0.5f,0.25f));   // blonde
            palette_hair_unusual.Insert(1,new Color(0.5f,0.20f,0.15f));  // red
            palette_hair_unusual.Insert(2,new Color(0.5f,0.5f,0.5f));    // white

            //Skin palette (from white to dark)
            palette_skin_color.Insert(0,new Color(1f,0.8f,0.7f));   //repeat white
            palette_skin_color.Insert(0,new Color(1f,0.8f,0.7f));   //... here!
            palette_skin_color.Insert(1,new Color(0.9f,0.7f,0.6f));     //REPEAT...
            palette_skin_color.Insert(1,new Color(0.9f,0.7f,0.6f));     //... here!
            palette_skin_color.Insert(2,new Color(0.7f,0.4f,0.3f));
            palette_skin_color.Insert(3,new Color(0.5f,0.4f,0.2f));
            palette_skin_color.Insert(4,new Color(0.4f,0.3f,0.1f));
    }

    void DEBUG_PrintAllComponents(GameObject obj){
        // Custom function: get all the components of the GameObject and print them on Unity Logger

        Component[] components = obj.GetComponents(typeof(Component));
        foreach(Component c in components) {
            Debug.Log("name "+c.name+" type "+c.GetType() +" basetype "+c.GetType().BaseType);
        }
    }

    void PrintReferencePositions(List<GameObject> crowd){
        foreach (var ps in crowd) {
            print(ps.GetComponent<UnityEngine.Transform>().position);
        }
    }


    void Update(){
        cam = Camera.main;
        
        for (int i=0; i<personId; i++){
            Vector3 pos = cam.WorldToViewportPoint(headPositions[i]);
            if( (pos.x >= 0 && pos.x <= 1) && (pos.y >= 0 && pos.y <= 1) && (pos.z > 0) ){
                nCrowd++;
            }
        }
        if (nCrowd != nCrowd_temp) Debug.Log(nCrowd+" people are visible"); 
        nCrowd_temp = nCrowd;
        nCrowd = 0;
    }


}
