using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents.Sensors;

public enum MotionFlags
{
    none = -1,
    Gravity2D,
    ControlledJump2D
}

public struct MotionContext
{
    public Vector2 position;
    public Vector2 velocity;
    public Vector2 acceleration;
    public VectorSensor observations;

    public MotionContext(Vector2 pos, Vector2 vel, Vector2 acc, VectorSensor obs)
    {
        position = pos;
        velocity = vel;
        acceleration = acc;
        observations = obs;
    }
}

[RequireComponent(typeof(VectorSensorComponent))]
public class MotionKernel : MonoBehaviour
{
    [Header("Motion Scripts")]
    public List<NonControlled> nonControlleds;          // e.g. gravity
    public List<Controlled> controlleds;                // e.g. walking, jumping

    [Space(10)]
    [Header("Controller Scripts")]
    public List<Controller> controllers;                // rulebased , ml-agents, player input
                                                        
    [Space(25)]                                         
    [Header("Observation Setting")]                     
                                                        
    [Space(7.5f)]                                       
    // observation collect assentation table            
    [Tooltip("my MotionConetext")]                      
    public bool selfContext = true;                     //
                                                        
    [Space(5f)]                                         
    [Tooltip("others MotionContext and Range, target's unity tags")]
    public bool othersContext = false;                  //
    [Range(0,10f)]                                      
    public float othersRangeForTransforms = 5f;         //
    public List<string> othersTagsForTransforms;        //
    [Space(10f)]                                        
                                                        
    [Tooltip("special states and Range, target's unity tags ,  e.g. hp, specific items, etc.")]
    public List<float> specialObservations;             // this extra spaces can use with additional Component.
                                                        // e.g. Agent Component, RuleBsed's Event function, etc.
    [Range(0, 10f)]                                     
    public float rangeForSpecial = 5f;                  //
    public List<string> tagsForSpecial;                 //

    /********************************* Map Information **********************************/
    // ISensor 기반으로 tilemap 정보 반영할 수 있도록 클래스 정의 필요
    // 이후 해당 클래스를 통해 map 정보 observation에 추가
    [Space(15)]                                         
    [Tooltip("map tiles position information")]         
    public bool showMapInfo = false;                    //
    [Tooltip("whither add map tiles types information")]
    public bool showMapDetail = false;                  //
                                                        
    VectorSensorComponent observationComponent;         // 
    MotionContext motionContext;                        //

    GameObject[] targetInSceneWithMotionKernel;         //

    [SerializeField]
    public bool initialGameObject = false;

    // update observations : motionContext.observations

    private void UpdateObservations()
    {
        // active 여부도 봐야함
        if (selfContext)
        {
            motionContext.observations.AddObservation(motionContext.position);
            motionContext.observations.AddObservation(motionContext.velocity);
            motionContext.observations.AddObservation(motionContext.acceleration);
        }

        if (othersContext)
        {
            foreach (string tag in othersTagsForTransforms)
            {
                GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
                foreach (GameObject target in targets)
                {
                    Vector2 dir = target.transform.position - transform.position;
                    float dist = dir.magnitude;
                    if (dist <= othersRangeForTransforms)
                    {
                        motionContext.observations.AddObservation(dir.normalized);
                        motionContext.observations.AddObservation(dist);
                        Rigidbody2D targetRb = target.GetComponent<Rigidbody2D>();
                        if (targetRb != null)
                        {
                            motionContext.observations.AddObservation(targetRb.velocity);
                        }
                        else
                        {
                            motionContext.observations.AddObservation(Vector2.zero);
                        }
                    }
                }
            }
        }
        if (specialObservations.Count > 0)
        {
            foreach (string tag in tagsForSpecial)
            {
                GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
                foreach (GameObject target in targets)
                {
                    Vector2 dir = target.transform.position - transform.position;
                    float dist = dir.magnitude;
                    if (dist <= rangeForSpecial)
                    {
                        // Example: If the target has a Health component, add its health value
                        //Health healthComponent = target.GetComponent<Health>();
                        //if (healthComponent != null)
                        //{
                        //    motionContext.observations.AddObservation(healthComponent.currentHealth);
                        //}
                        //else
                        //{
                        //    motionContext.observations.AddObservation(0f); // Default value if no Health component
                        //}
                    }
                }
            }
        }
        // Map information observation can be added here based on the game's map structure
    }

    private void Awake()
    {


        observationComponent = GetComponent<VectorSensorComponent>();

        observationComponent.CreateSensors();
        //observationComponent.GetSensor().AddObservation(0f);

        motionContext = new MotionContext(Vector2.zero, Vector2.zero, Vector2.zero, null);


    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
}
