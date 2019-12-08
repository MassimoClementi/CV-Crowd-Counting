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
    const int amount = 600; //number of people in the crowd
    const int presence = 80; //randomicity (out of 100) whether a person is created or not
    const int nCameras = 8;
    Camera[] cameras = new Camera[nCameras];
    int[] nCrowdPrev = new int[nCameras]; //number of people in previous frame for each camera

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

        //Count number of people in each camera FOV
        for(int c=0; c<cameras.Length; c++){

            int nCrowd = 0;
            for (int i=0; i<entitiesInstantiated.Count; i++){

                //Get head position vector from HeadPositionComponent for each entity
                HeadPositionComponent headPositionComponent = entityManager.GetComponentData<HeadPositionComponent>(entitiesInstantiated[i]);
                float3 headPositionECS = headPositionComponent.Value;
                Vector3 headPosition = new Vector3(headPositionECS.x, headPositionECS.y, headPositionECS.z);

                //Conversion from world coordinates to camera coordinates
                Vector3 pos = cameras[c].WorldToViewportPoint(headPosition);

                //Chech position
                if( (pos.x >= 0 && pos.x <= 1) && (pos.y >= 0 && pos.y <= 1) && (pos.z > 0) ){
                    nCrowd++;
                }
            }

            //Print on log if number of people has changed
            if (nCrowd != nCrowdPrev[c]) Debug.Log(cameras[c].name+": "+nCrowd+" people are visible"); 
            nCrowdPrev[c] = nCrowd;
        }

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
                Value = quaternion.Euler(0,0,0)
            });
            float xPosition = 0f;
            float zPosition = 0f;

            float headx = 0f;
            float heady = 0f;
            float headz = 0f;
            
            headx = xPosition;
            headz = zPosition;
            heady = refY[id] + refY[id]/2;

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
            xIstances = 10;
            zIstances = (int)(amount/10);
        }

        //Position and rotation variables inizialitazion
        float xPosition = 0f;
        float zPosition = 0f;
        float xSpacing = 0.75f;
        float zSpacing = 0.75f;
        float yRotation = 0f;

        int posRand = 35;           //contribute of the random position (out of 100)
        int rowRandCumul = 35;      //contribute of the random cumulative whole rows displacement (out of 100)
        int columnRand = 90;        //contribute of the random column displacement (out of 100)
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
                yRotation = UnityEngine.Random.Range(0f,180f);
                
                //Pick a random person from the converted entity prefabs and instantiate an entity
                index = UnityEngine.Random.Range(0,entities.Count);
                entity = entityManager.Instantiate(entities[index]);

                //Set component data for the instantiated entity
                entityManager.SetComponentData(entity, new Translation{
                    Value = new float3(xPosition, refY[index], zPosition)
                });
                entityManager.SetComponentData(entity, new Rotation{
                    Value = quaternion.Euler(0,yRotation,0)
                });

                headx = xPosition;
                headz = zPosition;
                heady = refY[index] + refY[index]/2;
                
                entityManager.SetComponentData(entity, new HeadPositionComponent{
                    Value = new float3 (headx, heady, headz)
                });

                //Add instantiated entity to list
                entitiesInstantiated.Add(entity);
                }
            }
        }

        //Add head position to specific list
        ManageHeadPositions(entitiesInstantiated);
    }

    void InitializeCameraList(){
        cameras[0] = GameObject.Find("Camera1").GetComponent<Camera>();
        cameras[1] = GameObject.Find("Camera2").GetComponent<Camera>();
        cameras[2] = GameObject.Find("Camera3").GetComponent<Camera>();
        cameras[3] = GameObject.Find("Camera4").GetComponent<Camera>();
        cameras[4] = GameObject.Find("Camera1hd").GetComponent<Camera>();
        cameras[5] = GameObject.Find("Camera2hd").GetComponent<Camera>();
        cameras[6] = GameObject.Find("Camera3hd").GetComponent<Camera>();
        cameras[7] = GameObject.Find("Camera4hd").GetComponent<Camera>();
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
        
        //Just to test where heads are positioned
        int counter = 0;
        foreach (Vector3 hp in headPositions){
            GameObject head = new GameObject("Head "+counter++);
            head.GetComponent<UnityEngine.Transform>().position = hp;
        }
        
    }
}