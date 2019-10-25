/*
    Authors: Massimo Clementi, Elisa Nicolussi Paolaz
    Date: 23 october 2019
    Project: Simulation of a crowded environment using Unity C# script
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instantiating_prefabs : MonoBehaviour{
    //GameObject
    public GameObject myPrefab;

    // Define script variables accessible from Unity
    public int x_n,y_n;     //number of people in x and y directions
    public float x_spacing, y_spacing;
    public int pos_rand_100;  //contribute of the random position
    public int rot_base, rot_range;

    //Define private script variables
    float x_instantiate,y_instantiate;
    Quaternion rot_instantiate;
    System.Random rnd = new System.Random();
    float rand_temp;

    // Start is called before the first frame update
    void Start()
    {
        for (int x = 0; x < x_n; x++){
             for (int y = 0; y < y_n; y++){
                
                //TODO: introduce other variable to make displacements of
                //      whole rows(sure) and columns(maybe)
                // CODE HERE

                //TODO: need to understand how to change animation (or animation start)
                //      for each person to make them move asynchronously

                //TODO: reintroduce shaders in Unity simulations

                //Manage instantiation variables
                // Placement
                    rand_temp = (float)(rnd.Next(0,pos_rand_100)-pos_rand_100/2)/100;   //cast needed
                    x_instantiate = (x + rand_temp - x_n/2) * x_spacing;
                    rand_temp = (float)(rnd.Next(0,pos_rand_100)-pos_rand_100/2)/100;
                    y_instantiate = (y + rand_temp - y_n/2) * y_spacing;

                // Rotation
                    rand_temp = (float)(rnd.Next(0,2*rot_range)-rot_range);
                    rot_instantiate = Quaternion.AngleAxis(rot_base+rand_temp,Vector3.up);

                //INSTANTIATE the person
                    GameObject person = Instantiate(myPrefab,
                                                    new Vector3(x_instantiate,0,y_instantiate),
                                                    rot_instantiate);

                /*
                //Get all the components of the GameObject
                Component[] components = person.GetComponents(typeof(Component));
                foreach(Component component in components) {
                    Debug.Log(component.ToString());
                }
                */

                //TODO: random gender
                    //person.GetComponent<UMA.CharacterSystem.DynamicCharacterAvatar>().hide = true;
                    //person.GetComponent<UMA.CharacterSystem.DynamicCharacterAvatar>().ChangeRace("HumanFemale");
                    //UMA.CharacterSystem.DynamicCharacterAvatar dca = person.GetComponent<UMA.CharacterSystem.DynamicCharacterAvatar>();
                    //dca.ChangeRace("SkyCar");

                //TODO: random clothes (according to the genders) -> i'd suggest arrays
                // CODE HERE

                //TODO: change height and weight
                // CODE HERE

             }
        }

    }

    // Update is called once per frame
    void Update(){
    }


}
