package uEngine;

import java.awt.*;

public class Material extends Component {
	public Color color;
	public boolean isVisible;
	
	public Material(Color color) {
		this.color = color;
		isVisible = true;
	}
	
	public Material() {
		this(Color.RED);
	}
}
