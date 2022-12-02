package uEngine;

//import java.awt.*;
import javax.swing.JFrame;
import javax.swing.JPanel;
import java.awt.Dimension;
import java.awt.Graphics;
import java.awt.Rectangle;
import java.util.*;

public class RenderEngine {
	JFrame _frame;
	List<GameObject> _sceneGraph = new ArrayList<GameObject>();
	
	public void renderScene(List<GameObject> gameObjects, Vector2 centre) {
		_sceneGraph = gameObjects;
		_frame.repaint();
	}
	
	public JFrame getWindow() {
		return _frame;
	}
	
	public RenderEngine() {
		_frame = new JFrame("uEngine: Game");
		_frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
		_frame.setPreferredSize(new Dimension(Game.WINDOW_WIDTH,Game.WINDOW_HEIGHT));
		_frame.setMinimumSize(new Dimension(Game.WINDOW_WIDTH,Game.WINDOW_HEIGHT));
		
		_frame.add(new GamePanel());
		
		_frame.setLocationRelativeTo(null);
		_frame.pack();
		_frame.setVisible(true);
		
		Rectangle r = _frame.getBounds();
		System.out.println("Window size (" + r.width + ", " + r.height + ")");
	}
	
	class GamePanel extends JPanel {
		private static final long serialVersionUID = 1L;

		public void paintComponent(Graphics g) {
			// Iterate over each game object and render it

			// Fill in missing for loop iterator and uncomment line
			for(int i = 0; i < _sceneGraph.size(); i++) {
				GameObject go = _sceneGraph.get(i);
				g.setColor(go.material.color);
				g.fillRect(
						(int)go.transform.position.x,
						(int)go.transform.position.y,
						(int)go.transform.size.x,
						(int)go.transform.size.y);
			}		
		}
	}
}
