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


    void Start(){
        for (int y = 0; y < yIstances; y++){

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
                    if (rnd.Next(0,100)<femaleToMaleRatio_100){
                        if(rnd.Next(0,100)<girlRatio_100) PrefabToUse = PrefabsGirl[Random.Range(0, PrefabsGirl.Length)];
                        else PrefabToUse = PrefabsFemale[Random.Range(0, PrefabsFemale.Length)];
                    }
                    else PrefabToUse = PrefabsMale[Random.Range(0, PrefabsMale.Length)];

                // INSTANTIATE the person
                    crowd.Insert(personId,
                                 Instantiate(PrefabToUse,
                                             new Vector3(xToInstantiate,0,yToInstantiate),
                                             rotToInstantiate) as GameObject);

                // Quick test code: it WORKS!
                // crowd[person_id].GetComponent<UMA.CharacterSystem.DynamicCharacterAvatar>().hide = true;
                // DEBUG_PrintAllComponents(crowd[person_id]);


                // TODO: random clothes (according to the genders) -> i'd suggest arrays
                // CODE HERE


                // TODO: change height and weight
                // CODE HERE


                //---
                personId++;
             }
        }

        // Print true positions of the people in the 3D scene
        PrintReferencePositions(crowd);
        // TODO: save in .csv file
        

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
    }


}
