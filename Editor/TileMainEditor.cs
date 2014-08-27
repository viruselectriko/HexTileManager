using UnityEngine;
using UnityEditor;
using System.Collections;


[CustomEditor(typeof(TileMain))]
public class TileMainEditor : Editor
{
    //Reference to TileMain class
    TileMain tilemain;
    //store mouse hit pos
    private Vector3 mouseHitPos;
    //store actual pixels to unity unit
    private Vector2 tilesize;
    // Tile position based on mouselocation
    private Vector3 tilepos;
    // array of textures for sprites
    Texture2D[] asset;
    //bool to check if tile selection grid is hidden or not
    public bool isTilesetDone = false;
    //Checks if tile is generated or not
    public bool isTileGenerated = false;
    // Scrolebar position
    private Vector2 scrolepos = Vector2.zero;
    // String for button
    string tilebutton = "Show Tiles";
    
    //Check variable for all bool values
    public bool checkbool;
    //Check variable for all vector2 values
    public Vector2 checkvector2;
    //starts when scene view is enabled
    public void OnEnable()
    {
        // Reference to the Tilemain Script
        tilemain = (TileMain)target;
        tilesize.x = tilemain.PixelSize.x / 100;
        tilesize.y = tilemain.PixelSize.y / 100;
    }
    //All the work happinning in Scene view
    void OnSceneGUI()
    {
        if (tilemain.isdrawmode)
        {
            //Grab the current event
            Event e = Event.current;
            if (e.isMouse)
            {
                // Set the view tool active
                Tools.current = Tool.View;
                //FPS tool is selected
                Tools.viewTool = ViewTool.FPS;
            }
            //Raycast from camera to mouse position 
            Ray r = HandleUtility.GUIPointToWorldRay(new Vector2(e.mousePosition.x, e.mousePosition.y));
            mouseHitPos = r.origin;
            
            // checks if mouse is on the gameobject layer
            if (IsMouseOnLayer())
            {
                //grab the marker position from the mouse position
                tilemain.MarkerPosition = MouseOnTile();
                // refresh the sceneview to update all the chenges 
                if (e.isMouse)
                    SceneView.RepaintAll();
                //checks which mouse button is clicked and dragged
                if (e.type == EventType.MouseDown && e.button == 0 || e.type == EventType.MouseDrag && e.button == 0)
                {
                    if (isTilesetDone)
                        Draw();
                    e.Use();

                }
                if (e.type == EventType.MouseDown && e.button == 1 || e.type == EventType.MouseDrag && e.button == 1)
                {
                    Delete();
                    e.Use();

                }
            }
            //show the gui on scene view
            Handles.BeginGUI();
            GUI.Label(new Rect(10, Screen.height - 90, 100, 100), "LMB: Draw");
            GUI.Label(new Rect(10, Screen.height - 105, 100, 100), "RMB: Erase");
            Handles.EndGUI();
        }
    }
    // overrides the default GUI for the TileMain script 
    public override void OnInspectorGUI()
    {
        //showing properties window
        tilemain.showProperties = EditorGUILayout.Foldout(tilemain.showProperties, "Properties");
        if (tilemain.showProperties)
        {
            
            checkbool = tilemain.isdrawmode;
            //Create a box layout
            GUILayout.BeginVertical("box");
            tilemain.isdrawmode = GUILayout.Toggle(tilemain.isdrawmode, " Draw Mode");
            if (checkbool != tilemain.isdrawmode)
            {
                //repaints the Sceneview to show the changes
                SceneView.RepaintAll();
                // Set the view tool active
                Tools.current = Tool.View;
                //FPS tool is selected
                Tools.viewTool = ViewTool.FPS;
            }
            GUILayout.BeginHorizontal();
            //SpriteSheet GUI
            GUILayout.Label("SpriteSheet:");
            tilemain.SpriteSheet = (Texture2D)EditorGUILayout.ObjectField(tilemain.SpriteSheet, typeof(Texture2D),false);
            GUILayout.EndHorizontal();
            //SizeGui
            checkvector2 = tilemain.PixelSize;
            tilemain.PixelSize = EditorGUILayout.Vector2Field("Pixel Size: ", tilemain.PixelSize);
            if (checkvector2 != tilemain.PixelSize)
            {
                OnEnable();
            }
            tilemain.LayerSize = EditorGUILayout.Vector2Field("Layer Size: ", tilemain.LayerSize);
            tilemain.Level = EditorGUILayout.FloatField("Level :", tilemain.Level);
            // Add Collider GUI
            tilemain.addcollider = GUILayout.Toggle(tilemain.addcollider, " Add Collider (Experimental)");
            if (tilemain.addcollider)
                tilemain.coltyp = EditorGUILayout.Popup(tilemain.coltyp, tilemain.collidertype);
            GUILayout.EndVertical();
            //Generate Tiles
            if (GUILayout.Button("Generate Tiles"))
            {
                if (tilemain.SpriteSheet)
                    GenerateTiles();
                else
                    Debug.Log("Must select a texture with sprites.");
            }

           //Tiles GUI
            EditorGUILayout.LabelField("Tiles", EditorStyles.boldLabel);

            //Show/Hide Tiles
                if (GUILayout.Button(tilebutton) && tilemain.SpriteSheet && isTileGenerated)
                {
                    
                    if (tilemain.tilesNo > 0)
                    {

                        //Generates the preview Textures from the sprites
                        asset = assetPreviewGenerator();
                        if (isTilesetDone == false)
                        {
                            isTilesetDone = true;
                            tilebutton = "Hide Tiles";
                        }
                        else
                        {
                            isTilesetDone = false;
                            tilebutton = "Show Tiles";
                        }
                        
                    }
                    else
                        Debug.Log("Must select a texture with sprites.");
                    
                }

            //Show scroll bar For next layout
            scrolepos = GUILayout.BeginScrollView(scrolepos);
            //if tile preview is generated draw a selection grid with all the tiles generated
            if (isTilesetDone)
                tilemain.tileGridId = GUILayout.SelectionGrid(tilemain.tileGridId, asset, 6,tilemain.texButton);
            
            GUILayout.EndScrollView();

            
        }
        //If the values in the editor is changed
        if (GUI.changed)
        {
            //set the current object as a dirty prefab so it wont lode the default values from the prefab
            EditorUtility.SetDirty(tilemain);
            
        }
    }
    //Generation function for asset texture from an array of sprites
    public Texture2D[] assetPreviewGenerator()
    {
        //Create a dynamic array with the number of sprites generated  
        Texture2D[] images = new Texture2D[tilemain.tilesNo];

        for (int i = 0; i < tilemain.tilesNo; i++)
        {
            //Store all the images of sprite in the dynamic array
            images[i] = AssetPreview.GetAssetPreview(tilemain.Tiles[i]);
             
        }
        return (images);
    }
    // Drawing function
    private void Draw()
    {
        //Checks if a game object has been already created on that place
        if (!tilemain.transform.Find(string.Format("Tile_{0}_{1}_{2}", tilepos.x, tilepos.y, tilepos.z)))
        {

            //lets you undo editor changes
            Undo.IncrementCurrentGroup();
            // Instantiate a gameobject with the selected sprite and selected grid location and as a children of main layer 
            GameObject tile = new GameObject("tile");
            SpriteRenderer renderer = tile.AddComponent<SpriteRenderer>();
            renderer.sprite = tilemain.Tiles[tilemain.tileGridId];
            tile.transform.position = tilemain.MarkerPosition;
            if (tilemain.addcollider)
                tile.AddComponent(tilemain.collidertype[tilemain.coltyp]);
            tile.name = string.Format("Tile_{0}_{1}_{2}", tilepos.x, tilepos.y,tilepos.z);
            tile.transform.parent = tilemain.transform;
            Undo.RegisterCreatedObjectUndo(tile, "Create Tile");
        }
        // checks if a game object is already located on that location,if true change she sprite of that gameobject
        if (tilemain.transform.Find(string.Format("Tile_{0}_{1}_{2}", tilepos.x, tilepos.y, tilepos.z)))
        {
            Undo.RecordObject(tilemain.transform.Find(string.Format("Tile_{0}_{1}_{2}", tilepos.x, tilepos.y, tilepos.z)).GetComponent<SpriteRenderer>().sprite, "Change Sprite");
            tilemain.transform.Find(string.Format("Tile_{0}_{1}_{2}", tilepos.x, tilepos.y, tilepos.z)).GetComponent<SpriteRenderer>().sprite = tilemain.Tiles[tilemain.tileGridId];
        }
    }
    // Delete Function
    private void Delete()
    {
        if (tilemain.transform.Find(string.Format("Tile_{0}_{1}_{2}", tilepos.x, tilepos.y,tilepos.z)))
        {
            Undo.IncrementCurrentGroup();
            Undo.DestroyObjectImmediate(tilemain.transform.Find(string.Format("Tile_{0}_{1}_{2}", tilepos.x, tilepos.y, tilepos.z)).gameObject);

        }
    }
    // checks if the mouse is on the layer
    private bool IsMouseOnLayer()
    {
        // return true or false depending if the mouse is positioned over the map
        if (mouseHitPos.x > tilemain.transform.position.x && mouseHitPos.x < (tilemain.transform.position.x + (tilemain.LayerSize.x * tilemain.PixelSize.x / 100)) && mouseHitPos.y > tilemain.transform.position.y && mouseHitPos.y < (tilemain.transform.position.y + (tilemain.LayerSize.y * tilemain.PixelSize.y / 100)))
        {
            
            return (true);
        }
        
        return (false);
    }
    //returns the location of the marker based on mouse on grid position
    private Vector3 MouseOnTile()
    {
        //converting the mouse hit position to local coordinates
        Vector2 localmouseHitPos = mouseHitPos - new Vector3(tilemain.transform.position.x, tilemain.transform.position.y, 0);
        // return the column and row values on which the mouse is on
        tilepos = new Vector3((int)(localmouseHitPos.x / tilesize.x), (int)(localmouseHitPos.y / tilesize.y), tilemain.Level);
        //calculate the marker position based on word coordinates
        Vector2 pos = new Vector2(tilepos.x * tilesize.x, tilepos.y * tilesize.y);
        //set the marker position value    

        Vector2 marker = new Vector2(pos.x + tilesize.x / 2, pos.y + tilesize.y / 2) + new Vector2(tilemain.transform.position.x, tilemain.transform.position.y);
       
        return(new Vector3(marker.x,marker.y,(-tilemain.Level)));
                

    }
    // genaration of the tile from tile map
    void GenerateTiles()
    {
        isTileGenerated = false;
        //hides the tiles if they are already shown
        isTilesetDone = false;
        tilebutton = "Show Tiles";
       
        //location of SpriteSheet
        string path = AssetDatabase.GetAssetPath(tilemain.SpriteSheet);
        //dyanamic array to store the sprites and fill it with sprites
        object[] objs;
        objs = AssetDatabase.LoadAllAssetsAtPath(path);
       
        tilemain.tilesNo = objs.Length - 1;
        if (tilemain.tilesNo > 0)
        {
            //Storing Tiles as Sprites
            for (int i = 1; i <= objs.Length - 1; i++)
            {
                tilemain.Tiles[i - 1] = (Sprite)objs[i];
                // tilemain.tilesTexture[i - 1] = AssetPreview.GetAssetPreview(tilemain.Tiles[i-1]);
            }
            asset = assetPreviewGenerator();
            isTileGenerated = true;
        }
        else
            Debug.Log("Must select a texture with sprites.");
    }

    
}
