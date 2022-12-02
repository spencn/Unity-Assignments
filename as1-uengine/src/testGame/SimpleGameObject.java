package testGame;

import uEngine.*;

public class SimpleGameObject extends GameObject {

	private float _velocity = 100f;
	
	public void start() {
		name = "Bill";
		
		Transform t = new Transform();
		t.position = new Vector2(400,300);
		transform = t;
	}
}
