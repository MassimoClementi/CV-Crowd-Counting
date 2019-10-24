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
    public int x_max;
    public int y_max;
    public float x_spacing;
    public float y_spacing;

    //Define private script variables
    int rotation = 180;

    // Start is called before the first frame update
    void Start()
    {
        for (int x = 0; x < x_max; x++){
             for (int y = 0; y < y_max; y++){
                //Create person
                //TODO: random placement and variable rotation
                GameObject person = Instantiate(myPrefab,new Vector3((x-x_max/2)*x_spacing,0,
                                                 (y-y_max/2)*y_spacing),
                                                  Quaternion.AngleAxis(rotation,Vector3.up));

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

                //TODO: random clothes (according to the genders)
                //      i'd suggest arrays
             }
        }

    }

    // Update is called once per frame
    void Update(){
    }


}
