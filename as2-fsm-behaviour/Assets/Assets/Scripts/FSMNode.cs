using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FSMNode : MonoBehaviour
{
   
    public abstract void OnEnter();
    public abstract void Do();

    public abstract void OnExit();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
