package pong;

import java.awt.Color;

import uEngine.*;

public class Ball extends GameObject {

	private final float ROOT_2 = (float)Math.sqrt(2);
	private float _speed = 120f;
	private Vector2 _velocity = new Vector2(-ROOT_2,-ROOT_2);
	
	@Override
	public void start() {
		name = "Ball";
		
		transform.size = new Vector2(40,40);
		
		audio.addClip("blip", "media/blip.wav");
		
		transform.position = new Vector2(Game.WINDOW_WIDTH/2,Game.WINDOW_HEIGHT/2);
		material.color = Color.RED;
	}
	
	@Override
	public void update(float elapsedTime) {
		transform.position.x += _velocity.x * _speed * elapsedTime;
		transform.position.y += _velocity.y * _speed * elapsedTime;
	}
	
	@Override
	public void onCollisionEnter(GameObject col) {
		audio.playOneShot("blip");
		
		// add code to change direction
		if(col.name.equals("top") || col.name.equals("bottom")) {
			_velocity.y *= -1;
		} else {
			assert(col.name.equals("right") || col.name.equals("left") || col.name.equals("paddle"));
			_velocity.x *= -1;
		}
	}
}
