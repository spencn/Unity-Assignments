using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : FSMNode
{

    public VillagerFSM villager;
    public FSMNode[] connectedNodes;

    

    private int idleHash = Animator.StringToHash("Idle");

    public override void Do()
    {
        if (villager.fruit < 2 && FruitExists())
        {
            connectedNodes[0].OnEnter(); //Transition to walking if there are trees with fruit
        }
        OnExit();
    }

    public override void OnEnter()
    {
        villager.currentNode = this;
        villager._anim.SetTrigger(idleHash);
    }

    public override void OnExit()
    {
        
    }

    public bool FruitExists()
    {
        GameObject[] trees = GameObject.FindGameObjectsWithTag("fruitTree");
        for (int i = 0; i < trees.Length; i++)
        {
            Fruit f = trees[i].GetComponent<Fruit>();
            if (f.fruit != 0)
            {
                return true;
            }
        }
        return false;
    }
}
