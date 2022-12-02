using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walking : FSMNode
{

    public VillagerFSM villager;
    public FSMNode[] connectedNodes;
    private int walkHash = Animator.StringToHash("Walk");
    GameObject target = null;
    GameObject[] trees;
    public override void Do()
    {
        StartCoroutine(DoWalking());
    }

    public override void OnEnter()
    {
        villager.currentNode = this;
        target = ChooseTree();

        if (villager.fruit == 2 || target == null) // Go to village if inventory is full or if no other trees with fruit exist
        {
            villager.target = GameObject.FindGameObjectWithTag("village");
            target = villager.target;
            transform.LookAt(villager.target.transform.position);
            villager._anim.SetTrigger(walkHash);
        } else //Go to the next tree
        {
            villager.target = target;
            transform.LookAt(villager.target.transform.position);
            villager._anim.SetTrigger(walkHash);
         }      
       
    }

    public override void OnExit()
    {
    }


    IEnumerator DoWalking()
    {
        if (target.tag.Equals("fruitTree")) { //If the villager is walking towards a tree
            if ((transform.position - target.transform.position).magnitude <= villager.pickingDistance && !villager.atTree) //If at the tree
            {
                villager.atTree = true;
                connectedNodes[0].OnEnter(); //Transition to PickingFruit
                OnExit();
            }
            else
            {
                transform.position += transform.forward * villager.speed * villager.scheduler.timeSinceLastUpdate;
                transform.position =
                    new Vector3(
                        transform.position.x,
                        Terrain.activeTerrain.SampleHeight(transform.position),
                        transform.position.z);
                yield return null;
            }
        } else if(target.tag.Equals("village")) //If the villager is walking towards the vilalge
        {
                if ((transform.position - target.transform.position).magnitude <= villager.pickingDistance + 10 && !villager.atVillage) //If at the village
                {
                    villager.atVillage = true;
                    connectedNodes[1].OnEnter(); //Transition to DroppingFruit
                    OnExit();
                }
                else
                {
                    transform.position += transform.forward * villager.speed * villager.scheduler.timeSinceLastUpdate;
                    transform.position =
                        new Vector3(
                            transform.position.x,
                            Terrain.activeTerrain.SampleHeight(transform.position),
                            transform.position.z);
                    yield return null;
                }
        }
        
    }

    public GameObject ChooseTree() //Return a gameobject of tag fruitTree that is the closest AND has fruit
    {
        trees = GameObject.FindGameObjectsWithTag("fruitTree");
        GameObject tempTarget = null;
        float closestTree = -1;
        for (int i = 0; i < trees.Length;i++)
        {            
            float treeDistance = Vector3.Distance(villager.transform.position, trees[i].transform.position);
            Fruit f = trees[i].GetComponent<Fruit>();
            if ((treeDistance < closestTree || closestTree == -1) && (f.fruit > 0))
            {
                tempTarget = trees[i];
                closestTree = treeDistance;
            }
        }
        return tempTarget;
    }
}
