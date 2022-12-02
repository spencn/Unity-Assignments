package pong;

import uEngine.*;

public class PongGame extends Game {

	private static final int WALL_WIDTH = 20;
	
	private static PongGame _game;
	
	public static void main(String args[]) {
		_game = new PongGame();
		
		_game.addGameObject(new Wall("left", new Vector2(0,0), new Vector2(WALL_WIDTH,Game.WINDOW_HEIGHT)));
		_game.addGameObject(new Wall("right", new Vector2(Game.WINDOW_WIDTH-WALL_WIDTH,0), new Vector2(20,Game.WINDOW_HEIGHT)));
		_game.addGameObject(new Wall("top", new Vector2(0,0), new Vector2(Game.WINDOW_WIDTH,WALL_WIDTH)));
		_game.addGameObject(new Wall("bottom", new Vector2(0,Game.WINDOW_HEIGHT-WALL_WIDTH-WALL_WIDTH), new Vector2(Game.WINDOW_WIDTH,WALL_WIDTH)));
		
		_game.addGameObject(new Ball());
		_game.addGameObject(new Paddle());
		
		_game.mainLoop();
	}
}