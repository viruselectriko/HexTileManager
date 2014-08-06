using UnityEngine;
using System.Collections;

public class TileMain : MonoBehaviour {

    //TileMap Texture2D
    public Texture2D SpriteSheet;
    //Size of the Pixels in The TileMap
    public Vector2 PixelSize;
    //Size of the Layer
    public Vector2 LayerSize;
    //Layer order
    public float Level = 0;
    //Show/Hide Properties 
    public bool showProperties = true;
    //No. of elements in array
    public int tilesNo;
    //Tiles array
    public Sprite[] Tiles = new Sprite[100];
    //Texture array
    //public Texture2D[] tilesTexture = new Texture2D[37];
    //GuiStyle
    public GUIStyle texButton;
    //Selection grid
    public int tileGridId;
    //position of the marker
    [HideInInspector]
    public Vector3 MarkerPosition;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    //Drawing the Grid
    void OnDrawGizmosSelected()
    {
        // store map width, height and position
        var mapWidth = this.LayerSize.x * this.PixelSize.x/100;
        var mapHeight = this.LayerSize.y * this.PixelSize.y/100;
        var position = this.transform.position;

        // draw layer border
        Gizmos.color = Color.white;
        Gizmos.DrawLine(position, position + new Vector3(mapWidth, 0, 0));
        Gizmos.DrawLine(position, position + new Vector3(0, mapHeight, 0));
        Gizmos.DrawLine(position + new Vector3(mapWidth, 0, 0), position + new Vector3(mapWidth, mapHeight, 0));
        Gizmos.DrawLine(position + new Vector3(0, mapHeight, 0), position + new Vector3(mapWidth, mapHeight, 0));

        // draw tile cells
        Gizmos.color = Color.grey;
        for (float i = 1; i < this.LayerSize.x; i++)
        {
            Gizmos.DrawLine(position + new Vector3(i * this.PixelSize.x/100, 0, 0), position + new Vector3(i * this.PixelSize.x/100, mapHeight, 0));
        }

        for (float i = 1; i < this.LayerSize.y; i++)
        {
            Gizmos.DrawLine(position + new Vector3(0, i * this.PixelSize.y/100, 0), position + new Vector3(mapWidth, i * this.PixelSize.y/100, 0));
        }
        // Draw marker position
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(this.MarkerPosition, new Vector3(this.PixelSize.x/100, this.PixelSize.y/100, 1) * 1.1f);

    }
}
