using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMScheduler : MonoBehaviour
{

    public VillagerFSM[] villagers;
    public float timeOfCurrentUpdate;
    public float timeOfLastUpdate = 0.0f;
    public float timeSinceLastUpdate;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UpdateFSMPrio0());
        StartCoroutine(UpdateFSMPrio1());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator UpdateFSMPrio0()
    {
        while (true)
        {
            timeSinceLastUpdate = Time.deltaTime;
            for (int i = 0; i < villagers.Length; i++)
            {
                if (villagers[i].priority == 0)
                {
                    villagers[i].currentNode.Do();
                    yield return null; //High prio villagers will call do every frame
                }
            }
        }

    }

    IEnumerator UpdateFSMPrio1()
    {
        while(true)
        {
            //Save frame time data for use in the transform method in walking
            timeOfCurrentUpdate = Time.time;
            timeSinceLastUpdate = timeOfCurrentUpdate - timeOfLastUpdate;
            timeOfLastUpdate = timeOfCurrentUpdate;
            for(int i = 0; i  < villagers.Length; i++)
            {
                if(villagers[i].priority == 1)
                {
                    villagers[i].currentNode.Do();
                    yield return new WaitForSeconds(0.1f); //Low prio villagers will call do every 0.1s
                }
            }
        }

    }

    

}
