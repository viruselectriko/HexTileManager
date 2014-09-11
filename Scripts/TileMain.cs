﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    //Tiles list
    public List<Sprite> Tiles = new List<Sprite>();
    //Checks if tile is generated or not
    public bool isTileGenerated = false;
    // array of textures for sprites
    public Texture2D[] asset;
    //GuiStyle
    public GUIStyle texButton;
    //Selection grid
    public int tileGridId;
    //Draw mode toggle
    public bool isdrawmode;
    //Bool for adding collider or not
    public bool addcollider;
     //Bool for adding RigidBody or not
    public bool addRigidbody;
    //collider type
    public string[] collidertype = new string[3]{"BoxCollider2D","CircleCollider2D","PolygonCollider2D"};
    //int for collider type
    public int coltyp;
    //Pixel to Unit
    public int pixeltounit = 100;
    //position of the marker
   // [HideInInspector]
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
        //checks if draw mode is on
        if (isdrawmode)
        {
            // store map width, height and position
            var mapWidth = this.LayerSize.x * this.PixelSize.x / pixeltounit;
            var mapHeight = this.LayerSize.y * this.PixelSize.y / pixeltounit;
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
                Gizmos.DrawLine(position + new Vector3(i * this.PixelSize.x / pixeltounit, 0, 0), position + new Vector3(i * this.PixelSize.x / pixeltounit, mapHeight, 0));
            }

            for (float i = 1; i < this.LayerSize.y; i++)
            {
                Gizmos.DrawLine(position + new Vector3(0, i * this.PixelSize.y / pixeltounit, 0), position + new Vector3(mapWidth, i * this.PixelSize.y / pixeltounit, 0));
            }
            // Draw marker position
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(this.MarkerPosition, new Vector3(this.PixelSize.x / pixeltounit, this.PixelSize.y / pixeltounit, 1) * 1.1f);
        }
    }
}
