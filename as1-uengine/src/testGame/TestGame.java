package testGame;

import testGame.MovableGameObject;
import testGame.SimpleGameObject;
import uEngine.*;

public class TestGame extends Game {

	private static TestGame _game;
	
	public static void main(String args[]) {
		_game = new TestGame();
		
		_game.addGameObject(new SimpleGameObject());
		_game.addGameObject(new MovableGameObject());
		
		_game.mainLoop();
	}
}