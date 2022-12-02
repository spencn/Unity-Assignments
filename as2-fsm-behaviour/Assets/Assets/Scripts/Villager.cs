using UnityEngine;
using System.Collections;

/***
 * This example code is provided to show how to interact with the animation controller
 * and how to move the avatar. This code should be replaced with your own FSM controller
 * code.
 * (Code written by Prof)
 */

public class Villager : MonoBehaviour {

	public float speed = 5.0f;
	public float pickingDistance = 5.0f;

	Animator _anim;
	
	// Triggers used to change state
	private int walkHash = Animator.StringToHash ("Walk");
	private int idleHash = Animator.StringToHash ("Idle");
	private int dropHash = Animator.StringToHash ("Drop");
	private int pickHash = Animator.StringToHash ("Pickup");

	// Names of animation states
	private int idleStateHash = Animator.StringToHash ("idle");
	private int walkingStateHash = Animator.StringToHash ("walking");
	private int dropStateHash = Animator.StringToHash ("dropping");
	private int pickupStateHash = Animator.StringToHash ("pickingFruit");


	bool atTree = false;
	public GameObject target = null;

	// Use this for initialization
	void Start () {
		_anim = GetComponent<Animator>();
		transform.LookAt(target.transform.position);
		_anim.SetTrigger(walkHash);
	}



	// Update is called once per frame
	void Update () {
		if((transform.position-target.transform.position).magnitude <= pickingDistance && !atTree) {
			atTree = true;
			_anim.SetTrigger(pickHash);
		} else if(atTree) {
			// do nothing
		} else {
			transform.position += transform.forward * speed * Time.deltaTime;
			transform.position =
				new Vector3(
					transform.position.x,
					Terrain.activeTerrain.SampleHeight(transform.position),
			    	transform.position.z);
		}
	}
}
