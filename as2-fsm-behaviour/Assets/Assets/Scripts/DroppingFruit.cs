using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppingFruit : FSMNode
{
    public VillagerFSM villager;
    public FSMNode[] connectedNodes;
    private int dropHash = Animator.StringToHash("Drop");
    GameObject target = null;
    GameObject village;
    Fruit f;
    bool hasStarted = false;

    public override void Do()
    {
        if (hasStarted == false)
        {
            StartCoroutine(DropFruit());
        }
            
    }

    public override void OnEnter()
    {

        villager.currentNode = this;
        villager.target = GameObject.FindGameObjectWithTag("village");
        target = villager.target;
        village = GameObject.Find(target.name);
        f = village.GetComponent<Fruit>();
        villager._anim.SetTrigger(dropHash);
    }

    public override void OnExit()
    {
        villager.atVillage = false;
    }

    IEnumerator DropFruit()
    {
        hasStarted = true;
        if(villager.fruit > 0) //Transfer fruit to village inventory
        {
            yield return new WaitForSeconds(3);
            f.fruit++;
            villager.fruit--;
        }
        else if (villager.fruit == 0 && FruitExists()) //If theres a fruit tree and the villager has inventory space, go to the next tree
        {
            connectedNodes[0].OnEnter(); 
            OnExit();
        }
        else //If there are no fruit on any trees, go to idle
        {
            connectedNodes[1].OnEnter();
            OnExit();
        }
        hasStarted = false;
    }

    public bool FruitExists() // return true if theres a tree on the map that has fruit
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
