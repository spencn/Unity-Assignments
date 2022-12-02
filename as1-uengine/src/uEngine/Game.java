package uEngine;

import java.util.*;

public abstract class Game {
	
	public static final int WINDOW_WIDTH = 800;
	public static final int WINDOW_HEIGHT = 600;
	
	public Vector2 centre = new Vector2(0,0);
	
	// We require a minimum amount of time between frames to ensure that the
	// reported elapsed time is not too small, leading to potential underflow
	// errors in game objects
	private static final long MIN_FRAME_ELAPSED_TIME = 10; // ms
	
	private RenderEngine _renderer;
	private InputEngine _input;
	private AudioEngine _audio;
	private PhysicsEngine _physics;
	
	private List<GameObject> _gameObjects = new ArrayList<GameObject>();
	
	private long _previousFrameStartTime = System.currentTimeMillis();
	
	protected void addGameObject(GameObject g) {
		// provide this game object with a reference back to the game
		g.game = this;
		
		// provide this game object with a convenience reference to the audio engine
		g.audio = _audio;
		
		// register the game object so that events are called (start, update, ...)
		_gameObjects.add(g);
	}

	protected void mainLoop() {
		// Initialize all components by calling their Start method
		for(int i = 0; i < _gameObjects.size();i++) {
			_gameObjects.get(i).start();
		}
		
		while(true){
			// Start a new frame
			long frameStartTime = System.currentTimeMillis();
			
			// Wait until a minimum time has elapsed since the start of the last frame.
			// This avoids providing an elapsed time with a very small number to
			// the update function of game objects, possibly leading to underflow in
			// their computations.
			while(frameStartTime - _previousFrameStartTime < MIN_FRAME_ELAPSED_TIME) {
				try{
					Thread.sleep(1);
				} catch(InterruptedException e) {}
				
				frameStartTime = System.currentTimeMillis();
			}
			float elapsedTime = (frameStartTime - _previousFrameStartTime) / 1000f;
			
			_input.startFrame();
			
			// Update all components by running their Update method
			for(int i = 0; i < _gameObjects.size();i++) {
				_gameObjects.get(i).update(elapsedTime);
			}
			
			// Check for collisions
			_physics.checkCollisions(_gameObjects);
			
			// Render the scene by drawing each component's sprite
			_renderer.renderScene(_gameObjects, centre);
			
			// Update frame time
			_previousFrameStartTime = frameStartTime;
		}
	}
	
	public Game() {
		// Initialize architecture
		_renderer = new RenderEngine();
		
		_input = InputEngine.getInstance();
		_input.setInputSource(_renderer.getWindow());
		
		_audio = new AudioEngine();
		
		_physics = new PhysicsEngine();
	}
}
