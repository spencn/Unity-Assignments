using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickingFruit : FSMNode
{

    public VillagerFSM villager;
    public FSMNode[] connectedNodes;
    private int pickupHash = Animator.StringToHash("Pickup");
    GameObject target = null;
    GameObject tree;
    Fruit f;
    bool hasStarted = false;

    public override void Do()
    {
        if(hasStarted == false)
        {
            StartCoroutine(PickupFruit());
        }
    }

    public override void OnEnter()
    {
        villager.currentNode = this;
        target = villager.target;
        villager._anim.SetTrigger(pickupHash);
        tree = GameObject.Find(target.name);
        f = tree.GetComponent<Fruit>();
     }

    public override void OnExit()
    {
        villager.atTree = false;
    }


    IEnumerator PickupFruit()
    {
        hasStarted = true;
        if(villager.fruit <= 1 && f.fruit > 0) //If the billager has space, and there is fruit left on the tree
        {
            f.fruit--;
            villager.fruit++;
            yield return new WaitForSeconds(3); //Delay 3 seconds between getting fruit
        } else
        {
            OnExit();
            connectedNodes[0].OnEnter(); //transition to walking to either a tree or village (determined in Walking script
        }
        hasStarted = false;
    }
}
