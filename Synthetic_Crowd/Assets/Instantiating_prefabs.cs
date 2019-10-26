﻿/*
    Authors: Massimo Clementi, Elisa Nicolussi Paolaz
    Date: 23 october 2019
    Project: Simulation of a crowded environment using Unity C# script
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instantiating_prefabs : MonoBehaviour{

    // Define public script variables (accessible from Unity)
    public GameObject myPrefab;
    public int x_n,y_n;     //number of people in x and y directions
    public float x_spacing, y_spacing;
    public int pos_rand_100; //contribute of the random position
    public int column_rand_100; //contribute of the random column displacement
    public int row_rand_cumul_100; //contribute of the random cumulative whole rows displacement
    public int rot_base, rot_range;


    //Define private script variables
    List<GameObject> crowd = new List <GameObject>();   //N.B: mandatory to initialize with 0par constructor
    int person_id = 0;
    float x_instantiate,y_instantiate;
    Quaternion rot_instantiate;
    System.Random rnd = new System.Random();
    float rand_temp;
    float column_rand_index = 0, row_rand_cumul_index = 0;


    void Start(){
        for (int y = 0; y < y_n; y++){

            // Random components evaluation
            column_rand_index = (float)(rnd.Next(0,column_rand_100)-column_rand_100/2)/100;
            row_rand_cumul_index += (float)(rnd.Next(0,row_rand_cumul_100))/100;

             for (int x = 0; x < x_n; x++){

                //TODO: need to understand how to change animation (or animation start)
                //      for each person to make them move asynchronously


                // Manage instantiation variables
                //  Placement
                    rand_temp = (float)(rnd.Next(0,pos_rand_100)-pos_rand_100/2)/100;   //cast needed
                    x_instantiate = (x + column_rand_index + rand_temp - x_n/2) * x_spacing;
                    rand_temp = (float)(rnd.Next(0,pos_rand_100)-pos_rand_100/2)/100;
                    y_instantiate = (y + row_rand_cumul_index + rand_temp - y_n/2) * y_spacing;

                //  Rotation
                    rand_temp = (float)(rnd.Next(0,2*rot_range)-rot_range);
                    rot_instantiate = Quaternion.AngleAxis(rot_base+rand_temp,Vector3.up);


                // INSTANTIATE the person
                    crowd.Insert(person_id,
                                 Instantiate(myPrefab,
                                             new Vector3(x_instantiate,0,y_instantiate),
                                             rot_instantiate) as GameObject);

                // Quick test code: it WORKS!
                // crowd[person_id].GetComponent<UMA.CharacterSystem.DynamicCharacterAvatar>().hide = true;
                // DEBUG_PrintAllComponents(crowd[person_id]);


                // TODO: random gender
                    //person.GetComponent<UMA.CharacterSystem.DynamicCharacterAvatar>().hide = true;
                    //person.GetComponent<UMA.CharacterSystem.DynamicCharacterAvatar>().ChangeRace("HumanFemale");
                    //UMA.CharacterSystem.DynamicCharacterAvatar dca = person.GetComponent<UMA.CharacterSystem.DynamicCharacterAvatar>();
                    //dca.ChangeRace("SkyCar");


                // TODO: random clothes (according to the genders) -> i'd suggest arrays
                // CODE HERE


                // TODO: change height and weight
                // CODE HERE


                //---
                person_id++;
             }
        }

        // Print true positions of the people in the 3D scene
        PrintReferencePositions(crowd);
        // TODO: save in .csv file
        

    }


    void DEBUG_PrintAllComponents(GameObject obj){
        // Custom function: get all the components of the GameObject and print them on Unity Logger

        Component[] components = obj.GetComponents(typeof(Component));
        foreach(Component component in components) {
            Debug.Log(component.ToString());
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
