using UnityEngine;
using System.Collections;

/// <summary>
/// Implements the avatar for a simple physics platformer. The avatar can
/// run to the right (right-arrow key) and jump (space bar.)
/// </summary>
public class AvatarScript : MonoBehaviour {
	Animator _anim;
	PPhysicsBody _phys;
	
	// Triggers used to change state
	// int idleHash = Animator.StringToHash("Idle");
	int jumpHash = Animator.StringToHash("Jump");
	int landHash = Animator.StringToHash("Land");
	int slamHash = Animator.StringToHash("Slam");
	int soarHash = Animator.StringToHash("Soar");
	int isRunningHash = Animator.StringToHash("isRunning");

	// Names of animation states
	int idleStateHash = Animator.StringToHash("Idle");
	int runningStateHash = Animator.StringToHash("Running");
	int hardLandingStateHash = Animator.StringToHash("HardLanding");
	int jumpingStateHash = Animator.StringToHash("Jumping");
	int soaringStateHash = Animator.StringToHash("Soaring");

	bool _wasGrounded = false;
	bool _jumping = false;

	bool canDoubleJump = true;

	public float cooldown = 5.0f;
	public float coolDownStart = Time.time;
	private bool specialOnCD = false;

	public void Start ()
	{
		_anim = gameObject.GetComponent<Animator>();
		_phys = gameObject.GetComponent<PPhysicsBody>();
	}

	public void Update ()
	{
		if(Time.time - cooldown >= coolDownStart) //Check if its been 5seconds since last use of special (dash)
		{
			specialOnCD = false; //Take special off cooldown
		}
		if(Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit();
		}
			
		// see if we've landed
		AnimatorStateInfo animState = _anim.GetCurrentAnimatorStateInfo(0);

		// If we land on a platform, transition from soaring animation into running or idle
		if(animState.shortNameHash == soaringStateHash) {
			if(_phys.Grounded && !_wasGrounded) {
				Debug.Log("Triggering Land");
				_anim.SetTrigger(landHash);
				_jumping = false;
				canDoubleJump = true;
			}
		}

		// If we run off the end of a platform, start the soaring animation
		if(!_phys.Grounded && _wasGrounded && _anim.GetBool(isRunningHash) && !_jumping) {
			Debug.LogFormat("Running not grounded y={0}", _phys.position.y);
			_anim.SetTrigger(soarHash);
		}

		if(Input.GetKeyDown("space")) {
			if (_phys.Grounded) {
				_phys.AddForce(new Vector2(0f, 250f));
				_anim.SetTrigger(jumpHash);
				_jumping = true;
			} else if (canDoubleJump == true && !_phys.Grounded) //if you havent double jumped, and are currently in the air, you can perform a double jump
			{
				_phys.AddForce(new Vector2(0f, 150f));
			}
		} else if (Input.GetKeyDown ("right")) {
			if(_phys.Grounded) {
				_phys.AddForce(new Vector2(10f,0f));
				_anim.SetBool(isRunningHash, true);
			}
		} else if (Input.GetKey("right")) {
			if(_phys.Grounded) {
				_phys.AddForce(new Vector2(10f,0f));
			}
		} else if (Input.GetKeyUp("right")) {
			_phys.Stop();
			_anim.SetBool(isRunningHash, false);
		} else if (Input.GetKeyDown("x"))
		{
			if(specialOnCD == false)
			{
				_phys.AddForce(new Vector2(70.0f, 0f));
				coolDownStart = Time.time;
				specialOnCD = true;
			}
		}

		_wasGrounded = _phys.Grounded;
	}
}