using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerFSM : FSM
{
    public int fruit = 0;
    public float speed = 5.0f;
    public float pickingDistance = 5.0f;
    public FSMScheduler scheduler;
    public Animator _anim;
    public int priority;

    public bool atTree = false;
    public bool atVillage = false;
    public GameObject target = null;

    // Start is called before the first frame update
    void Start()
    {
        currentNode = startNode;
        startNode.OnEnter(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
