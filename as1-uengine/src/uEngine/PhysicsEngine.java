package uEngine;

import java.util.*;
import java.util.concurrent.CopyOnWriteArraySet;

public class PhysicsEngine {
	
	Set<CollisionPair> _currentCollisions = new CopyOnWriteArraySet<CollisionPair>();
	
	/**
	 * returns true if the given point is within the bounds of the given game object.
	 * @param p
	 * @param g
	 * @return
	 */
	private boolean within(Vector2 p, GameObject g) {
		return p.x >= g.transform.position.x
				&& p.x < g.transform.position.x + g.transform.size.x
				&& p.y >= g.transform.position.y
				&& p.y < g.transform.position.y + g.transform.size.y;
	}
	
	private boolean colliding(GameObject g1, GameObject g2) {
		Vector2 ll = g1.transform.position;
		Vector2 lr = new Vector2(g1.transform.position.x + g1.transform.size.x-1,
									g1.transform.position.y);
		Vector2 ul = new Vector2(g1.transform.position.x,
									g1.transform.position.y+g1.transform.size.y-1);
		Vector2 ur = new Vector2(g1.transform.position.x + g1.transform.size.x-1,
									g1.transform.position.y+g1.transform.size.y-1);
		
		return within(ll,g2) || within(lr,g2) || within(ul,g2) || within(ur,g2);
	}
	
	private void checkAndHandleCollisionEntry(List<GameObject> gameObjects) {
		// Compare each pair of game objects to see if they intersect. If they do,
		// call the onCollisionEnter method on both game objects. Note we call
		// onCollisionEnter only the first time the game objects intersect, and
		// will not call it again unless they have ceased to intersect before the
		// next intersection.
		
		// Our algorithm is very inefficient, doing a pairwise comparison over all
		// game objects. We trigger the collision on the first of the game objects.
		// We compare all pairs twice, so we trigger the collision on both objects.
		for(GameObject g1 : gameObjects) {
			for(GameObject g2 : gameObjects) {
				CollisionPair cp = new CollisionPair(g1,g2);
				if(g1!=g2 && colliding(g1,g2) && !_currentCollisions.contains(cp)) {
					g1.onCollisionEnter(g2);
					_currentCollisions.add(cp);
				}
			}
		}
	}
	
	private void checkAndHandleCollisionExit() {
		// Check each current pair of collisions, and if they are no longer colliding,
		// trigger their collision exit event and remove the collision from the
		// current list.
		List<CollisionPair> noLongerColliding = new ArrayList<CollisionPair>();
		for(CollisionPair cp : _currentCollisions) {
			if(!colliding(cp.g1, cp.g2)) {
				noLongerColliding.add(cp);
				cp.g1.onCollisionExit(cp.g2);
			}
		}
		for(CollisionPair cp : noLongerColliding) {
			_currentCollisions.remove(cp);
		}
	}
	
	public void checkCollisions(List<GameObject> gameObjects) {
		checkAndHandleCollisionEntry(gameObjects);
		checkAndHandleCollisionExit();
	}
	
	class CollisionPair {
		public GameObject g1;
		public GameObject g2;
		
		public CollisionPair(GameObject g1, GameObject g2) {
			this.g1 = g1;
			this.g2 = g2;
		}
		
		@Override
		public boolean equals(Object other) {
		    //System.out.println("checking equals");

			if (other == null) return false;
		    if (!(other instanceof CollisionPair))return false;
		    CollisionPair cp = (CollisionPair)other;
			return g1 == cp.g1 && g2 == cp.g2;
		}
	}
}
