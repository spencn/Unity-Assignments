using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;

/// <summary>
/// This is the physics engine. It should be attached to an empty game object in
/// the scene. It is invoked via Unity's FixedUpdate function. It manages a set of
/// rigid bodies of type PPhysicsBody. These must be added to the physics engine
/// via a call to AddBody.
/// </summary>
public class PPhysicsEngine : MonoBehaviour {

	private const float groundedDistanceThreshold = 0.1f;
	public float previousIntegrateTime = 0.0f;
	public float startIntegrateTime = 0.0f;
	List<PPhysicsBody> bodies = new List<PPhysicsBody>();
	List<Contact> contacts = new List<Contact>();
	public void AddBody(PPhysicsBody newBody) {
		Debug.Assert(newBody != null);
		Debug.Assert (bodies!=null);
		bodies.Add (newBody);
	}

	/// <summary>
	/// Determines whether two axis-aligned bounded boxes (rectangles) intersect.
	/// </summary>
	/// <param name="LL1">lower-left corner of first box</param>
	/// <param name="UR1">upper-right corner of first box</param>
	/// <param name="LL2">lower-left corner of second box</param>
	/// <param name="UR2">upper-right corner of second box</param>
	bool Intersecting(Vector2 LL1, Vector2 UR1, Vector2 LL2, Vector2 UR2) {
		return !(UR1.x<LL2.x || UR2.x<LL1.x || UR1.y<LL2.y || UR2.y<LL1.y);
	}

	/// <summary>
	/// Returns true if the two bodies were not intersecting, but now are intersecting
	/// </summary>
	/// <param name="b1">first body</param>
	/// <param name="b2">second body</param>
	bool AreColliding(PPhysicsBody b1, PPhysicsBody b2) {
		return !Intersecting (b1.LLold, b1.URold, b2.LLold, b2.URold)
			&& Intersecting(b1.LL, b1.UR, b2.LL, b2.UR);
	}

	/// <summary>
	/// Returns a list of all <i>new</i> collisions. Collisions describe pairs of bodies
	/// that did not collide during the last iteration of the physics engine, but
	/// do collide now.
	/// </summary>
	/// <returns>The for collisions.</returns>
	List<Contact> CheckForCollisions() {
		List<Contact> contacts = new List<Contact>();

		// Compare each pair of bodies to see if they have collided. Be
		// careful to consider each pair only once.
		for(int i=0; i<bodies.Count-1; i++) {
			for(int j = i+1; j<bodies.Count; j++) {
				if(AreColliding(bodies[i], bodies[j])) {
					Contact c = new Contact(bodies[i], bodies[j]);
					contacts.Add(c);
				}
			}
		}
		return contacts;
	}

	void IntegrateAll(float deltaTime) {
		foreach(PPhysicsBody pb in bodies) {
			pb.Integrate(deltaTime);
		}
	}

	/// <summary>
	/// Returns true if b is in resting on (or just above) another body. Specifically,
	/// is there another component below this one within the distance
	/// 'groundedDistanceThreshold'.
	/// </summary>
	/// <param name="b">The component we are checking for grounded</param>
	public bool Grounded(PPhysicsBody b) {
		// iterate through all bodies; if b is just above any of them, return true
		foreach(PPhysicsBody b2 in bodies) {
			if(b != b2) {
				if(b.LL.x < b2.UR.x && b.UR.x > b2.LL.x
						&& Mathf.Abs(b.LL.y - b2.UR.y) <= groundedDistanceThreshold) {
					return true;
				}
			}
		} 
		return false;
	}
	
	void ResolveCollisions(List<Contact> contacts) {
		foreach(Contact c in contacts) {
			c.ResolveContact();
		}
	}
	
	void CommitAll() {
		foreach (PPhysicsBody pb in bodies) {
			pb.Commit();
		}
	}



	/// <summary>
	/// The main method of the physics engine - integrates, checks for collisions,
	/// resolves all collisisons.
	/// </summary>
	void UpdatePhysics() {
		startIntegrateTime = Time.time - previousIntegrateTime;
		//Call integrate all
		IntegrateAll(startIntegrateTime);
		//Detect and figure out all collisions happening at this time
		contacts = CheckForCollisions();
		if(contacts.Count > 0)
		{
			ResolveCollisions(contacts);
		}

		//Commit integration
		CommitAll();
		previousIntegrateTime = startIntegrateTime; //Reset the time since last integration, to the end of this one


	}


	/// <summary>
	/// On each update tick, run the physic engine - integrate, detect collisions,
	/// respond to collisions
	/// </summary>
	void FixedUpdate () {
		UpdatePhysics();
	}
}

/// <summary>
/// Represents a collision between two rigid bodies. A collision requires that the
/// bodies were not overlapping in the last physics engine frame, but are overlapping now.
/// This class additionally resolves contacts, applying necessary impulses to the two bodies
/// to enact the results of the collision.
/// </summary>
class Contact {
	public PPhysicsBody _b1;
	public PPhysicsBody _b2;
	public Vector2 contactN = Vector2.zero;
	float restitution = 0.5f;


	public void ResolveContact() {
		// collision response
		// calculate contact normal
		
		if (_b1.UR.x == _b1.URold.x && _b1.LL == _b2.LLold) //If object 1 is the stationary one, swap as we are using a moving object for calculations
		{
			PPhysicsBody temp = _b1;
			_b2 = _b1;
			_b1 = temp;
		}

		if (_b1.rebounds) // If the object can rebound, set its restition to a higher value.
		{
			restitution = 0.8f;
		}
		Vector2 speed = _b1.URold - _b1.UR; //This is technically distance, but we are working in 1 frame, so this is distance/1 so its the speed at which object 1 is approaching object 2
		Vector2 objectDistance;
		float collisionTime = 0.2f;
		//Debug.Log(speed);
		//I think this part is correct, im taking how quick it takes an axis to approach the axis of the other object. the smaller the value means thats the side that object will strike.
		if (speed.x >= 0) // travelling right
		{
			if (speed.y < 0) // travelling down-right
			{
				objectDistance = _b1.LL - _b2.UR;	
				if(objectDistance.x/speed.x < objectDistance.y / speed.y)
				{
					contactN.x = -1;
					collisionTime = objectDistance.x / speed.x;
				} else
				{
					contactN.y = 1;
					collisionTime = objectDistance.y / speed.y;
				}
			}
			else if (speed.y > 0) // travelling up-right
			{
				objectDistance = _b1.UR - _b2.LL;
				if (objectDistance.x / speed.x < objectDistance.y / speed.y)
				{
					contactN.x = -1;
					collisionTime = objectDistance.x / speed.x;
				}
				else
				{
					contactN.y = -1;
					collisionTime = objectDistance.y / speed.y;
				}
			}
		} else if (speed.x < 0) // travelling left
		{
			if (speed.y < 0) // travelling down-left
			{
				objectDistance = _b1.LL - _b2.UR;
				if (objectDistance.x / speed.x < objectDistance.y / speed.y)
				{
					contactN.x = 1;
					collisionTime = objectDistance.x / speed.x;
				}
				else
				{
					contactN.y = 1;
					collisionTime = objectDistance.y / speed.y;
				}
			}
			else if (speed.y > 0) // travelling up-left
			{
				objectDistance = _b1.UR - _b2.LL;
				if (objectDistance.x / speed.x < objectDistance.y / speed.y)
				{
					contactN.x = 1;
					collisionTime = objectDistance.x / speed.x;
				}
				else
				{
					contactN.y = -1;
					collisionTime = objectDistance.y / speed.y;
				}
			}			
		}

		//Once contact normal is calculated. Revert back to the original position of the colliding objects
		_b1.Revert();
		_b2.Revert();

		//Calculate impulse (collision response)
		//Calculate seperating velocity
		//Relative velocity * contact normal
		Vector2 relativeVelocity = _b1.Velocity;
		relativeVelocity -= _b2.Velocity;
		Vector2 seperatingVelocity = relativeVelocity * contactN;

		//Check if it needs to be resolved
		
		//Seperating velocity of each object with restitution
		Vector2 newSepVelocity = -seperatingVelocity * restitution;
		Vector2 deltaVelocity = newSepVelocity - seperatingVelocity;

		float totalInverseMass = 1 / _b1.mass + 1 / _b2.mass;
		if(totalInverseMass <= 0)
		{
			return;
		}

		Vector2 impulse = deltaVelocity / totalInverseMass;

		//Debug.Log(contactN);
		//Debug.Log(impulse);
		//Debug.Log(collisionTime);
		//_b1.Stop();
		//_b2.Stop();
		//Add this new force to the object
		_b1.AddForce(impulse);
		_b2.AddForce(-impulse);
		//Re-integrate at the calculated time of exact collision
		_b1.Integrate(collisionTime);
		_b2.Integrate(collisionTime);


		//Find displacement of object 1 (displacement vector between urold and ur/doesnt matter which one) (its velocity cause time = 1 frame)
		//If the object is travelling right
		//If object is also travelling down
		//Store Distance between obj1ll and obj2ur
		//(if (distance.x/speed.x < distance.y/speed.y)
		//hit on the right direction
		//else
		//Hit on the top
		//ETC
		//If object is also travelling up
		//Store Distance between obj1ur and obj2ll
		//Divide distance with the speed the object is travelling in






		//First do the basic contact normal calculations, that is for objects that just move on one axis (vertical or horizontal)
		// One object is travelling vertically and the other is stationary, or both objects are travelling vertically.
		//if(((_b1.UR.x == _b1.URold.x && _b1.UR.y != _b1.URold.y) && ((_b2.UR.y == _b2.URold.y && _b2.UR.x == _b2.URold.x) || (_b2.UR.x == _b2.URold.x && _b2.UR.y != _b2.URold.y)))) // Object 1 is travelling towards object 2
		//{
		//	if(_b1.UR.y > _b1.URold.y) // Object 1 is moving up and colliding with the bottom of object 2
		//	{
		//		contactN.y = -1;
		//	} else // Object 1 is moving down and colliding with the top of object 2
		//	{
		//		contactN.y = 1;
		//	}
		//} 
		//else if (((_b2.UR.x == _b2.URold.x && _b2.UR.y != _b2.URold.y) && ((_b1.UR.y == _b1.URold.y && _b1.UR.x == _b1.URold.x) || (_b1.UR.x == _b1.URold.x && _b1.UR.y != _b1.URold.y)))) // Object 2 is travelling towards object 1
		//{
		//	if (_b2.UR.y > _b2.URold.y) // Object 2 is moving up and colliding with the bottom of object 1
		//	{
		//		contactN.y = -1;
		//	}
		//	else // Object 2 is moving down and colliding with the top of object 1
		//	{
		//		contactN.y = 1;
		//	}
		//} 
		//else if (((_b1.UR.x != _b1.URold.x && _b1.UR.y == _b1.URold.y) && ((_b2.UR.y == _b2.URold.y && _b2.UR.x == _b2.URold.x) || (_b2.UR.x != _b2.URold.x && _b2.UR.y == _b2.URold.y)))) // Object 1 is travelling towards object 2
		//{
		//	if (_b1.UR.x > _b1.URold.x) // Object 1 is moving right and colliding with the left side of object 2
		//	{
		//		contactN.x = -1;
		//	}
		//	else // Object 1 is moving right and colliding with the right side of object 2
		//	{
		//		contactN.x = 1;
		//	}
		//} 
		//else if (((_b2.UR.x != _b2.URold.x && _b2.UR.y == _b2.URold.y) && ((_b1.UR.y == _b1.URold.y && _b1.UR.x == _b1.URold.x) || (_b1.UR.x != _b1.URold.x && _b1.UR.y == _b1.URold.y)))) // Object 2 is travelling towards object 1
		//{
		//	if (_b2.UR.x > _b2.URold.x) // Object 2 is moving right and colliding with the left side of object 1
		//	{
		//		contactN.x = -1;
		//	}
		//	else // Object 2 is moving left and colliding with the right side of object 1
		//	{
		//		contactN.x = 1;
		//	}
		//}


		//Old Solution I restarted cause I found many flaws in it

		//Determine which box is the "colliding" box. IE the box that we are going to use to calculate contact normal
		//I realize this information may have been useful above to simplifiy the equations but I dont want to mess with the equations as of rn
		//Use the object that is moving, if both are moving just select object 1.
		//If object 1 is moving always use object 1
		//if((_b1.UR.y != _b1.URold.y || _b1.UR.x != _b1.URold.x) )
		//{
		//	//Seperate into 4 quadrants
		//	//Object 1 is top left of object 2. Use object 2's collision point as we are going to act as if object 2 is stationary
		//	if(_b1.URold.x < _b2.UR.x && _b1.URold.y > _b2.UR.y)
		//	{
		//		//Determine the 2 possible edges the contact normal could take place in on object 2
		//		//This would be bottom and the right for object 1
		//		//and would be the top and the left for object 2
		//		//Take the middle points of both objects and take the faster closing velocity of the 2 approaching edges
		//		Vector2 b1Right;
		//		b1Right.x = _b1.LLold.x;
		//		b1Right.y = ((_b1.URold.y + _b1.LLold.y) / 2);
		//		Vector2 b1Bottom;
		//		b1Bottom.y = _b1.LLold.y;
		//		b1Bottom.x = ((_b1.URold.x + _b1.LLold.x) / 2);
		//		Vector2 b2Top;
		//		b2Top.y = _b2.UR.y;
		//		b2Top.x = ((_b2.UR.x + _b2.LL.x) / 2);
		//		Vector2 b2Left;
		//		b2Left.x = _b2.LLold.x;
		//		b2Left.y = ((_b2.UR.y + _b2.LL.y) / 2);
		//		//Calculate closing velocity of b1Right to b2Left and b1Bottom to b2Top
		//		Vector2 travelVector1 = b2Left - b1Right;
		//		Vector2 travelVector2 = b2Top - b1Bottom;
		//		Vector2 closingVelo1;
		//		Vector2 closingVelo2;


		//	}
		//	//Object 1 is top right of object 2. Use object 2's collision point as we are going to act as if object 2 is stationary
		//	else if (_b1.URold.x > _b2.UR.x && _b1.URold.y > _b2.UR.y)
		//	{

		//	}
		//	//Object 1 is bottom left of object 2. Use object 2's collision point as we are going to act as if object 2 is stationary
		//	else if (_b1.URold.x < _b2.UR.x && _b1.URold.y < _b2.UR.y)
		//	{

		//	}
		//	//Object 1 is bottom right of object 2. Use object 2's collision point as we are going to act as if object 2 is stationary
		//	else if (_b1.URold.x > _b2.UR.x && _b1.URold.y < _b2.UR.y)
		//	{

		//	}
		//}
		//else //Only use object 2 if object 1 is stationary
		//{

		//}
		// calculate total impulse to apply using a change of velocity
	}

	/// <summary>
	/// Records the colliding physics bodies and computes the contact normal
	/// </summary>
	/// <param name="b1">the first body</param>
	/// <param name="b2">the second body</param>
	public Contact(PPhysicsBody b1, PPhysicsBody b2) {
		_b1 = b1;
		_b2 = b2;
	}
}
