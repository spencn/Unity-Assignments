package uEngine;

import java.awt.Color;

public class GameObject {
	public String id = super.toString();
	public String name;
	
	public Transform transform;
	public Material material;
	public AudioEngine audio;
	
	public Game game;
	
	public void start() {}
	public void update(float elapsedTime) {}
	public void onCollisionEnter(GameObject col) {}
	public void onCollisionExit(GameObject col) {}
	
	public String toString() {
		return name + ": " + transform;
	}
	
	public GameObject() {		
		// Create default components
		this.material = new Material();
		this.transform = new Transform();
		// Name normally provided by programmer
		name = "";
	}
}
