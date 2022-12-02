package uEngine;

public class Transform extends Component {
	public Vector2 position;
	public Vector2 size;
	public float depth;
	
	public Transform() {
		position = new Vector2(0,0);
		size = new Vector2(20,20);
		depth = 0;
	}
	
	public String toString() {
		return position + "@" + depth;
	}
}
