﻿/*
    Authors: Massimo Clementi, Elisa Nicolussi Paolaz
    Date: 23 october 2019
    Project: Simulation of a crowded environment using Unity C# script
             Retrieval of ground-truth head positions for machine learning training
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public struct CameraFeatures{
    public Vector3 position;
    public Quaternion rotation;
    public CameraFeatures(float px, float py, float pz, float rx, float ry, float rz){
        position.x = px;
        position.y = py;
        position.z = pz;
        rotation = Quaternion.Euler(rx,ry,rz);
    }
}

public class SyntheticCrowdGenerator : MonoBehaviour{

    // Define public script variables (accessible from Unity)
    public GameObject[] PrefabsMale,PrefabsFemale,PrefabsGirl;
    public int femaleToMaleRatio_100;
    public int xIstances,zIstances;     //number of people in x and y directions
    public int rotBase, rotRange;


    //Define private parameters
    const float xSpacing = 0.75f, zSpacing = 0.75f;
    const int girlRatio_100 = 15;       //Ratio "girls over females"
    const int posRand_10 = 35;          //contribute of the random position
    const int rowRandCumul_100 = 35;    //contribute of the random cumulative whole rows displacement
    const int columnRand_100 = 90;     //contribute of the random column displacement


    //Define private script variables
    GameObject PrefabToUse;
    List<GameObject> crowd = new List <GameObject>();   //N.B: mandatory to initialize with 0par constructor
    public static List<Vector3> headPositions = new List <Vector3>();
    List<Color> palette_dark = new List <Color>(), palette_bright = new List <Color>(),
                palette_hair_common = new List <Color>(), palette_hair_unusual = new List <Color>(),
                palette_skin_color = new List <Color>();
    System.Random rnd = new System.Random();
    int personId = 0, nCrowd = 0, nCrowd_temp = 0;
    bool femaleBool,girlBool;
    int[] crowdDensity = new int[] {50, 81, 100, 150, 200, 250}; //, 300, 400, 500, 800, 1000};
    CameraFeatures[] cameras = new CameraFeatures[4];
    int crowdDensityPos = 0, camerasPos = 0;


    void Generate(){
        personId = 0;
        // Local initialization
        float columnRandIndex = 0, rowRandCumulIndex = 0;
        InitializeColorPalettes();

        for (int z = 0; z < zIstances; z++){

            // Random components evaluation
            columnRandIndex = (float)(rnd.Next(0,columnRand_100)-columnRand_100/2)/100;
            rowRandCumulIndex += (float)(rnd.Next(0,rowRandCumul_100))/100;

             for (int x = 0; x < xIstances; x++){

                // Manage instantiation variables
                //  Placement
                    float randTemp = (float)(rnd.Next(0,posRand_10)-posRand_10/2)/100;   //cast needed
                    float xToInstantiate = (x + columnRandIndex + randTemp - xIstances/2) * xSpacing;
                    randTemp = (float)(rnd.Next(0,posRand_10)-posRand_10/2)/100;
                    float zToInstantiate = (z + rowRandCumulIndex + randTemp - zIstances/2) * zSpacing;

                //  Rotation
                    randTemp = (float)(rnd.Next(0,2*rotRange)-rotRange);
                    Quaternion rotToInstantiate = Quaternion.AngleAxis(rotBase+randTemp,Vector3.up);

                //  Gender
                    femaleBool = rnd.Next(0,100)<femaleToMaleRatio_100;
                    if (femaleBool){
                        girlBool = rnd.Next(0,100)<girlRatio_100;
                        if(girlBool) PrefabToUse = PrefabsGirl[UnityEngine.Random.Range(0, PrefabsGirl.Length)];
                        else PrefabToUse = PrefabsFemale[UnityEngine.Random.Range(0, PrefabsFemale.Length)];
                    }
                    else PrefabToUse = PrefabsMale[UnityEngine.Random.Range(0, PrefabsMale.Length)];

                // INSTANTIATE the person
                    crowd.Insert(personId,
                                 Instantiate(PrefabToUse,
                                             new Vector3(xToInstantiate,0,zToInstantiate),
                                             rotToInstantiate) as GameObject);

                // Change model colors
                    var personColorList = crowd[personId].GetComponent<UMA.CharacterSystem.DynamicCharacterAvatar>()
                                                    .characterColors;
                    
                    SetRandColor(personColorList,"Hair",palette_hair_common,palette_hair_unusual,80);   // Hair
                    personColorList.SetColor("Skin",RandColorFromList(palette_skin_color));             // Skin color           
                    SetRandColor(personColorList,"Shirt",palette_dark,palette_bright,95);               // Shirt
                    SetRandColor(personColorList,"Shirt2",palette_dark,palette_bright,35);              // ...
                    SetRandColor(personColorList,"Pants1",palette_dark,palette_bright,95);              // Pants
                    SetRandColor(personColorList,"PantsAccent",palette_dark,palette_bright,35);         // ...


                // Change height and fitness
                    float height = (float)rnd.Next(0,7)/100+0.98f;      // y from 0.98 to 1.05
                    float fitness = (float)rnd.Next(0,3)/100+1.0f;      // z from 1 to 1.3
                    crowd[personId].GetComponent<UnityEngine.Transform>().localScale = new Vector3(1f,height,fitness);

                
                // Update head positions list
                    Vector3 headPosition_temp = crowd[personId].GetComponent<Transform>().position;

                    // Determine gender-wise fine-tuned head base position
                        float base_head_offset = 1.75f;  // male
                        if (femaleBool){
                            base_head_offset -= 0.13f;   //female
                            if (girlBool) base_head_offset -= 0.3f;    //girl
                        }
                    headPosition_temp.y += (float)(float)base_head_offset * height;
                    headPositions.Insert(personId,headPosition_temp);
                   
                personId++;
             }
        }

        // Show head positions
        RenderHeadPoints(); // to see head points go into play mode, pause it and inspect the GameObjects
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
            palette_skin_color.Insert(1,new Color(1f,0.8f,0.7f));   //... here!
            palette_skin_color.Insert(2,new Color(0.9f,0.7f,0.6f));     //REPEAT...
            palette_skin_color.Insert(3,new Color(0.9f,0.7f,0.6f));     //... here!
            palette_skin_color.Insert(4,new Color(0.7f,0.4f,0.3f));
            palette_skin_color.Insert(5,new Color(0.5f,0.4f,0.2f));
            palette_skin_color.Insert(6,new Color(0.4f,0.3f,0.1f));
    }

    void DEBUG_PrintAllComponents(GameObject obj){
        // Custom function: get all the components of the GameObject and print them on Unity Logger

        Component[] components = obj.GetComponents(typeof(Component));
        foreach(Component c in components) {
            Debug.Log("name "+c.name+" type "+c.GetType() +" basetype "+c.GetType().BaseType);
        }
    }

    void RenderHeadPoints(){
        int head_counter = 0;
        foreach (Vector3 hp in headPositions){
            GameObject head_tmp = new GameObject("Head "+head_counter++);
            head_tmp.GetComponent<UnityEngine.Transform>().position = hp;
        }
    }

    void InitializeCameraFeature(){
        cameras[0] = new CameraFeatures(0.0f,6.0f,4.0f,50.0f,180.0f,0.0f);
        cameras[1] = new CameraFeatures(-6.0f,2.0f,0.0f,0.0f,90.0f,0.0f);
        cameras[2] = new CameraFeatures(0.0f,3.0f,5.0f,20.0f,180.0f,0.0f);
        cameras[3] = new CameraFeatures(-3.0f,2.5f,5.0f,20.0f,135.0f,0.0f);
    }

    IEnumerator ManagerGenerator(){
        Camera cam = Camera.main;
        InitializeCameraFeature();

        while (crowdDensityPos < crowdDensity.Length){
            // Destroy previous crowd (if existing)
            if(crowd.Count != 0){
                for(int k=0; k<crowd.Count; k++){
                    Destroy(crowd[k]);
                }
            }
            crowd.Clear();
            headPositions.Clear();

            // Compute number of instances per axis
            if (crowdDensity[crowdDensityPos] <= 100){
                xIstances = (int)(Math.Sqrt(crowdDensity[crowdDensityPos]));
                zIstances = (int)(Math.Sqrt(crowdDensity[crowdDensityPos]));
            }
            else{
                xIstances = 10;
                zIstances = (int)(crowdDensity[crowdDensityPos]/10);
            }
            Generate();

            // Set camera
            cam.transform.position = cameras[camerasPos].position;
            cam.transform.rotation = cameras[camerasPos].rotation;

            yield return new WaitForSeconds(10.0f);
            // Take screenshot
            ScreenShot.TakeScreenshot(cam);

            if (camerasPos == cameras.Length-1) crowdDensityPos++;
            camerasPos = (camerasPos+1) % cameras.Length;
        }
    }
    
    void Update(){
        Camera cam = Camera.main;

        // Count number of people in camera FOV
        nCrowd = 0;
        for (int i=0; i<personId; i++){
            Vector3 pos = cam.WorldToViewportPoint(headPositions[i]);
            if( (pos.x >= 0 && pos.x <= 1) && (pos.y >= 0 && pos.y <= 1) && (pos.z > 0) ){
                nCrowd++;
            }
        }
        if (nCrowd != nCrowd_temp) Debug.Log(nCrowd+" people are visible"); 
        nCrowd_temp = nCrowd;

        // Destroy previous crowd (if existing) and create a new one
        if (Input.GetKeyDown("n")) {
            if(crowd.Count != 0){
                for(int i=0; i<crowd.Count; i++){
                    Destroy(crowd[i]);
                }
            }
            crowd.Clear();            
            Generate();
        }

        // Autonomously generate and save dataset 
        if (Input.GetKeyDown("g")){
            StartCoroutine(ManagerGenerator());
        }
    }


}
