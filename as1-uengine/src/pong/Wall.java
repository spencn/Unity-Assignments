package pong;

import java.awt.Color;

import uEngine.*;

public class Wall extends GameObject {

	private Vector2 _size = new Vector2(10,10);
	private Vector2 _pos = new Vector2(0,0);
	private String _name = name;
	
	public void start() {
		name = _name;
		
		transform.position = _pos;
		transform.size = _size;
		material.color = Color.orange;
	}
	
	public Wall(String name, Vector2 pos, Vector2 size) {
		_name = name;
		_pos = pos;
		_size = size;
	}
}
