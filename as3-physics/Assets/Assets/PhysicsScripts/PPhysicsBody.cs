using UnityEngine;
using System.Collections;

/// <summary>
/// This component can be added to any game object to mark it as a
/// rigid body that is to be managed by the physics engine.
/// This component has methods that are intended to be called by the game, and
/// methods that are intended to be called by the game engine.
/// 
/// Methods to be used by game programmer:
/// -- AddForce - adds a force to this rigid body of the given direction and magnitude
/// -- Stop - stops the rigid body dead
/// -- Grounded - determines whether this body is on the ground
/// 
/// Methods to be used by physics engine:
/// -- Integrate - applies current forces and moves rigid body
/// -- Commit - commits last integration step by writing the position to
/// 	this game object's transform
/// -- Revert - reverts the last integration step, returning to old position/velocity
/// </summary>
public class PPhysicsBody : MonoBehaviour {

	public float mass = 1f;
	public bool rebounds = true;
	public bool obeysGravity = true;
	public float gravity = -9.8f; // N/kg
	public float restitution = 1;
	public Vector2 maxVelocity = new Vector2(10f,10f);

	public string name; // For debugging

	private Vector2 force;
	private Vector2 gravityVector;
	public Vector2 _velocity;
	private Vector2 _oldVelocity;
	public Vector2 position;
	public Vector2 oldPosition; // position in previous tick


	private PPhysicsEngine engine;

	private float xLeft, xRight, yDown, yUp;

	/// <summary>
	/// Applies the given force to this body
	/// </summary>
	/// <param name="newForce">Force to apply</param>
	public void AddForce(Vector2 newForce) {
		force += newForce;
	}

	/// <summary>
	/// Stops the avatar. All forces are cleared. AddForce should not be called
	/// before next call to Integrate.
	/// </summary>
	public void Stop() {
		Velocity = Vector2.zero;
		force = Vector2.zero;
	}

	/// <summary>
	/// Property representing the current velocity. Clamps the velocity so it does
	/// not exceed maxVelocity.
	/// </summary>
	/// <value>The current velocity</value>
	public Vector2 Velocity {
		get {
			return _velocity;
		}
		set {
			_velocity = value;
			if(Mathf.Abs(_velocity.x) > maxVelocity.x) {
				_velocity.x = Mathf.Sign (_velocity.x) * maxVelocity.x;
			}
			if(Mathf.Abs(_velocity.y) > maxVelocity.y) {
				_velocity.y = Mathf.Sign (_velocity.y) * maxVelocity.y;
			}
		}
	}

	/// <summary>
	/// The last velocity (from the previous tick of the physics engine.)
	/// </summary>
	/// <value>The last velocity.</value>
	public Vector2 OldVelocity {
		get {
			return _oldVelocity;
		}
	}

	/// <summary>
	/// Returns true if this game object is in contact with (or just above) another
	/// game object.
	/// </summary>
	/// <value><c>true</c> if grounded; otherwise, <c>false</c>.</value>
	public bool Grounded {
		get {
			return engine.Grounded(this);
		}
	}


	/// <summary>
	/// The size of this body as an axis-aligned bounded box. After this function is called, the
	/// extents of the body are (pos.x-xLeft, pos.y-yDown) -> (pos.x+xRight, pos.y+yUp)
	/// 
	/// If the component has a Renderer, sets the bounds from it. If it does not have a Renderer, reports
	/// an error and puts in place dummy bounds as a 2x2 square.
	/// 
	/// Tip: If a game object has no renderer, add a LineRenderer component and manually enclose the
	/// desired area.
	/// </summary>
	private void SetBounds() {
		Bounds b;
		float xOffset, yOffset = 0f;
		Renderer r = gameObject.GetComponent<Renderer>();
		if(r) {
			b = r.bounds;
		} else {
			Debug.LogError(string.Format("SetBounds: could not find bounds for {0}", gameObject.name));
			b = new Bounds();
			b.extents = new Vector2(1f, 1f);
			b.center = new Vector2(0f, 0f);
		}

		xOffset = b.center.x - position.x;
		xLeft = b.extents.x - xOffset;
		xRight = b.extents.x + xOffset;

		yOffset = b.center.y - position.y;
		yUp = b.extents.y + yOffset;
		yDown = b.extents.y - yOffset;
	}

	/// <summary>
	/// Set initial values for velocity, force, etc. Grab initial position from transform.
	/// Add this body to the physics engine. Set the bounds.
	/// </summary>
	void Start () {
		//Initial values needed will be velocity, force,position, gravity vector and the engine itself
		engine = GameObject.FindGameObjectWithTag("PhysicsEngine").GetComponent<PPhysicsEngine>();
		//_velocity.x = 0;
		//_velocity.y = 0;
		//force.x = 0;
		//force.y = 0;
		Stop();
	    position.x = transform.position.x;
	    position.y = transform.position.y;
		oldPosition.x = transform.position.x;
		oldPosition.y = transform.position.y;
		gravityVector.y = mass*gravity;
		//Store initial transformation based on the transform values
		//Engine add body
		engine.AddBody(this);
		//Call set bounds
		SetBounds();
	}

	/// <summary>
	/// Apply the current forces to this body, determining its change in position,
	/// then acceleration, then new velocity. When the Commit function is called,
	/// the position in the transform attached to this game object will be updated.
	/// 
	/// Method is public, but should only be called within the physics engine.
	/// </summary>
	public void Integrate(float deltaTime) {

		//Stop();
		//This method will take the position and time to calculate position from velocity and acceleration
		//Uses implicit euler approximation
		//4 major steps in Integrate
		//First: Sum all forces, get a value f, the total force applied to this physics body
		//Method AddForce will add to the existing force vector, meaning we can skip this step as force will always be up to date
		//Need to account for gravity here
		if (!Grounded && obeysGravity)
		{
			AddForce(gravityVector);
		}
		//Second: Calculate the final acceleration at this time using this force
		Vector2 acceleration = force / mass;
		//Third: Calculate the velocity at this time
		Velocity = _oldVelocity + acceleration * deltaTime;
		//Finally with the previous calculations we can determine a new postiion of the body.
		position = oldPosition + _velocity * deltaTime + acceleration * ((deltaTime*deltaTime)/2);


	}

	/// <summary>
	/// Reverts the integration step, restoring the old position and velocity
	/// </summary>
	public void Revert() {
		position = oldPosition;
		Velocity = _oldVelocity;
	}

	/// <summary>
	/// Committing this step writes the new position to the object's transform
	/// </summary>
	public void Commit() {
		transform.position = new Vector3(position.x, position.y, 0f);
		oldPosition = position;
		_oldVelocity = Velocity;
		Stop();
	}

	/// <summary>
	/// Returns the lower-left coordinate of this body
	/// </summary>
	/// <value>The lower-left coordinate of this body</value>
	public Vector2 LL {
		get {
			return new Vector2(
				position.x - xLeft, position.y - yDown);
		}
	}

	/// <summary>
	/// Returns the upper-right coordinate of this body
	/// </summary>
	/// <value>The upper-right coordinate of this body</value>
	public Vector2 UR {
		get {
			return new Vector2(
				position.x + xRight, position.y + yUp);
		}
	}

	/// <summary>
	/// Returns the previous lower-left coordinate of this body (from the last tick
	/// of the physics engine.)
	/// </summary>
	/// <value>The previous lower-left coordinate of this body</value>
	public Vector2 LLold {
		get {
			return new Vector2(
				oldPosition.x - xLeft, oldPosition.y - yDown);
		}
	}

	/// <summary>
	/// Returns the previous upper-right coordinate of this body (from the last tick
	/// of the physics engine.)
	/// </summary>
	/// <value>The previous upper-right coordinate of this body</value>
	public Vector2 URold {
		get {
			return new Vector2(
				oldPosition.x + xRight, oldPosition.y + yUp);
		}
	}
}
