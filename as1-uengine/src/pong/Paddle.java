package pong;

import java.awt.Color;
import java.awt.event.KeyEvent;

import uEngine.*;

public class Paddle extends GameObject {

	private final float _speed = 170f;
	
	private boolean _atTop = false;
	private boolean _atBottom = false;
	
	@Override
	public void start() {
		name = "paddle";
		
		transform.position = new Vector2(700,300);
		transform.size = new Vector2(20,130);
		material.color = Color.BLUE;
	}
	
	@Override
	public void update(float elapsedTime) {
		if(InputEngine.getInstance().getKey(KeyEvent.VK_DOWN) && !_atBottom) {
			// move paddle down
			transform.position.y += elapsedTime * _speed;
		} else if (InputEngine.getInstance().getKey(KeyEvent.VK_UP) && !_atTop) {
			// move paddle up
			transform.position.y -= elapsedTime * _speed;
		}
	}
	
	@Override
	public void onCollisionEnter(GameObject col) {
		if(col.name.equals("bottom")) {
			_atBottom = true;
		}
		if(col.name.equals("top")) {
			_atTop = true;
		}
	}
	
	@Override
	public void onCollisionExit(GameObject col) {
		if(col.name.equals("bottom")) {
			_atBottom = false;
		}
		if(col.name.equals("top")) {
			_atTop = false;
		}
	}
}