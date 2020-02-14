/*

    Authors: Elisa Nicolussi Paolaz, Massimo Clementi
    
    Script to manage crowd generation in Unity
    Attach this script to the CrowdManager object

    1) In Unity environment drag the mesh models into the CrowdManager's Prefabs array
       In the CrowdGenerator.cs script add for each mesh model the corresponding transform reference height in refY array

    2) Choose a scene
    Scene1:	Unity environment -> select Cameras1, Background1, Ground1
            CrowdGenerator.cs -> const int amount = 1000;
                                 bool slope = false;
                                 const int nCameras = 8;
                                 check cameras in InitializeCameraList()

    Scene2: Unity environment -> select Cameras2, Background2, Ground2
            CrowdGenerator.cs -> const int amount = 500;
                                 bool slope = false;
                                 const int nCameras = 4;
                                 check cameras in InitializeCameraList()

    Scene3: Unity environment -> select Cameras3, Ground3
            CrowdGenerator.cs -> const int amount = 350;
                                 bool slope = false;
                                 const int nCameras = 4;
                                 check cameras in InitializeCameraList()
                                 front crowd: float yRotMin = 3*Mathf.PI/4;
                                              float yRotMax = 5*Mathf.PI/4;
                                 back crowd: float yRotMin = -Mathf.PI/3;
                                             float yRotMax = Mathf.PI/3;

    Slanted:Unity environment -> select Cameras (slope), Ground1
            CrowdGenerator.cs -> const int amount = 1000;
                                 bool slope = true;
                                 const int nCameras = 8;
                                 check cameras in InitializeCameraList()

    3) Select a light source between "Directional light" and "Point light"

    4) Press the play button, then press the "n" key to generate crowd and the "s" key to save images and ground-truths
*/


using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;


public class CrowdGenerator : MonoBehaviour 
{   
    public GameObject[] Prefabs;
    float[] refY = new float[] {1.04f, 1.05f, 1.04f, 1.13f, 1.12f, 1.00f, 1.00f, 1.15f, 1.10f, 1.15f, 
                                1.10f, 1.10f, 1.10f, 1.10f, 1.00f, 1.00f, 1.10f, 1.20f, 1.15f, 1.10f,
                                1.20f, 1.10f};
    EntityManager entityManager;
    List<Entity> entities = new List<Entity>();
    List<Entity> entitiesInstantiated = new List<Entity>();
    public static List<Vector3> headPositions = new List <Vector3>();
    const int amount = 1000; //number of people in the crowd
    const int presence = 100; //randomicity (out of 100) whether a person is created or not
    bool slope = false;
    const int nCameras = 8;
    Camera[] cameras = new Camera[nCameras];
    

    void Start(){
        entityManager = World.Active.EntityManager;

        //Convert prefabs into entities
        for(int i=0; i<Prefabs.Length; i++){
            entities.Add(GameObjectConversionUtility.ConvertGameObjectHierarchy(Prefabs[i], World.Active));
            entityManager.AddComponent(entities[i], typeof(HeadPositionComponent));
        }
    }

    void Update(){
        InitializeCameraList();
       
        //Generate crowd
        if(Input.GetKeyDown("n")){
            GenerateCrowd(amount);
        }

        //Take screenshot with each camera
        if(Input.GetKeyDown("s")){
            foreach(Camera cam in cameras){
                ScreenShot.TakeScreenshot(cam);
            }
        }

        //Test new model prefab
        if(Input.GetKeyDown("t")){
            int id = 0;
            entityManager = World.Active.EntityManager;
            Entity testentity = entityManager.Instantiate(entities[id]);
            entityManager.SetComponentData(testentity, new Translation{
                Value = new float3(0, refY[id], 0)
            });
            entityManager.SetComponentData(testentity, new Rotation{
                Value = quaternion.Euler(0,Mathf.PI,0)
            });
            float xPosition = 0f;
            float zPosition = 0f;

            float headx = 0f;
            float heady = 0f;
            float headz = 0f;
            
            headx = xPosition;
            headz = zPosition;
            heady = 3*refY[id]/2;

            entityManager.SetComponentData(testentity, new HeadPositionComponent{
                Value = new float3 (headx, heady, headz)
            });

            entitiesInstantiated.Add(testentity);
            ManageHeadPositions(entitiesInstantiated);
        }
    }

    void GenerateCrowd(int amount){
        entityManager = World.Active.EntityManager;

        //Compute number of instances per axis
        int xIstances, zIstances;
        if (amount < 100){
            xIstances = (int)(Math.Sqrt(amount));
            zIstances = xIstances;
        }
        else{
            if(slope){
                xIstances = 30;
                zIstances = (int)(amount/30);
            }
            else{
                xIstances = 20;
                zIstances = (int)(amount/20);
            }
        }

        //Position and rotation variables inizialitazion
        float yPosition = 0f;
        float yIncrease = 0f;
        float xPosition = 0f;
        float zPosition = 0f;
        float xSpacing = 0.5f;          //default 0.5f, scene3 0.75f/0.65f
        float zSpacing = 0.5f;          //default 0.5f, scene3 0.75f/0.65f
        float yRotation = 0f;
        float yRotMin = 3*Mathf.PI/4;   //3*Mathf.PI/4 (front), -Mathf.PI/3 (back)
        float yRotMax = 5*Mathf.PI/4;   //5*Mathf.PI/4 (front), Mathf.PI/3 (back)

        int posRand = 15;               //contribution of the additive component of (x,z) model position
        int rowRandCumul = 25;          //maximum range of the random row (z) cumulative displacement
        int columnRand = 40;            //maximum range of the random column (x) displacement
        float rowRandCumulIndex = 0;
        float columnRandIndex = 0;
        float randTemp = 0f;

        float headx = 0f;
        float heady = 0f;
        float headz = 0f;
        
        int index;
        Entity entity;

        for (int z=0; z<zIstances; z++){

            //Random components evaluation
            columnRandIndex = (float)(UnityEngine.Random.Range(0,columnRand)-columnRand/2)/100;
            rowRandCumulIndex += (float)(UnityEngine.Random.Range(0,rowRandCumul))/100;

            for (int x=0; x<xIstances; x++){

                if(UnityEngine.Random.Range(0,100)<=presence){
                //Compute position
                randTemp = (float)(UnityEngine.Random.Range(0,posRand)-posRand/2)/100;
                xPosition = (x + columnRandIndex + randTemp - xIstances/2) * xSpacing;
                randTemp = (float)(UnityEngine.Random.Range(0,posRand)-posRand/2)/100;
                zPosition = (z + rowRandCumulIndex + randTemp - zIstances/2) * zSpacing;

                //Compute rotation
                yRotation = UnityEngine.Random.Range(yRotMin,yRotMax);
                
                //Pick a random person from the converted entity prefabs and instantiate an entity
                index = UnityEngine.Random.Range(0,entities.Count);
                entity = entityManager.Instantiate(entities[index]);

                //Linear computation of yPosition
                yPosition = refY[index] + yIncrease;

                //Set component data for the instantiated entity
                entityManager.SetComponentData(entity, new Translation{
                    Value = new float3(xPosition, yPosition, zPosition)
                });
                entityManager.SetComponentData(entity, new Rotation{
                    Value = quaternion.Euler(0,yRotation,0)
                });

                headx = xPosition;
                headz = zPosition;
                heady = (3*refY[index]/2) + yIncrease;
                
                entityManager.SetComponentData(entity, new HeadPositionComponent{
                    Value = new float3 (headx, heady, headz)
                });

                //Add instantiated entity to list
                entitiesInstantiated.Add(entity);
                }
            }

            //Increase y positioning
            if (slope == true) yIncrease += 0.15f;
        }

        //Add head position to specific list
        ManageHeadPositions(entitiesInstantiated);
    }

    void InitializeCameraList(){
        cameras[0] = GameObject.Find("Camera1").GetComponent<Camera>();
        cameras[1] = GameObject.Find("Camera2").GetComponent<Camera>();
        cameras[2] = GameObject.Find("Camera3").GetComponent<Camera>();
        cameras[3] = GameObject.Find("Camera4").GetComponent<Camera>();
        cameras[4] = GameObject.Find("Camera5").GetComponent<Camera>();
        cameras[5] = GameObject.Find("Camera6").GetComponent<Camera>();
        cameras[6] = GameObject.Find("Camera7").GetComponent<Camera>();
        cameras[7] = GameObject.Find("Camera8").GetComponent<Camera>();
    }

    void ManageHeadPositions(List<Entity> entitiesInstantiated){
        for (int i=0; i<entitiesInstantiated.Count; i++){
            //Get head position vector from HeadPositionComponent for each entity
            HeadPositionComponent headPositionComponent = entityManager.GetComponentData<HeadPositionComponent>(entitiesInstantiated[i]);
            float3 headPositionECS = headPositionComponent.Value;
            Vector3 headPosition = new Vector3(headPositionECS.x, headPositionECS.y, headPositionECS.z);
            //Add head position vector to list
            headPositions.Add(headPosition);
        }

        //Count number of people in each camera FOV
        InitializeCameraList();
        for(int c=0; c<cameras.Length; c++){

            int nCrowd = 0;
            for (int i=0; i<entitiesInstantiated.Count; i++){
                //Conversion from world coordinates to camera coordinates
                Vector3 pos = cameras[c].WorldToViewportPoint(headPositions[i]);

                //Chech position
                if( (pos.x >= 0 && pos.x <= 1) && (pos.y >= 0 && pos.y <= 1) && (pos.z > 0) ){
                    nCrowd++;
                }
            }

            //Print on log
            Debug.Log(cameras[c].name+": "+nCrowd+" people are visible"); 
        }
    }
}