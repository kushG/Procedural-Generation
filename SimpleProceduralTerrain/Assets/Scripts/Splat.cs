using UnityEngine;
using System.Collections;

public class Splat : MonoBehaviour {
	public int tileSize = 10;
	public Texture2D splatTexture;
	public Vector2 splatTileSize;

	void Awake(){
		splatTileSize = new Vector2 (tileSize, tileSize);
	}

}
