package testGame;

import uEngine.*;

import java.awt.Color;
import java.awt.event.KeyEvent;

public class MovableGameObject extends GameObject {
	private long _velocity = 100;
	
	private InputEngine _input = InputEngine.getInstance();
	
	public void start() {
		name = "Ben";
		transform.position = new Vector2(400,400);
		material.color = Color.BLUE;
		audio.addClip("boing", "media/boing2.wav");
	}
	
	public void update(float elapsedTime) {
		if(_input.getKey(KeyEvent.VK_RIGHT)) {
			//System.out.println("Moving object right by: " + _velocity*elapsedTime);
			transform.position.x += _velocity * elapsedTime;
		}
		if(_input.getKey(KeyEvent.VK_LEFT)) {
			//System.out.println("Moving object left by: " + _velocity*elapsedTime);
			transform.position.x -= _velocity * elapsedTime;
		}
		if(_input.getKey(KeyEvent.VK_UP)) {
			//System.out.println("Moving object up by: " + _velocity*elapsedTime);
			transform.position.y -= _velocity * elapsedTime;
		}
		if(_input.getKey(KeyEvent.VK_DOWN)) {
			//System.out.println("Moving object left by: " + _velocity*elapsedTime);
			transform.position.y += _velocity * elapsedTime;
		}
	}
	
	public void onCollisionEnter(GameObject col) {
		audio.playOneShot("boing");
	}
}
