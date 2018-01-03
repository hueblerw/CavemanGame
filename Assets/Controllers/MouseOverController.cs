using System;
using UnityEngine;
using UnityEngine.UI;

public class MouseOverController : MonoBehaviour {

    private Text TileInfo;
    private Collider mapCollider;
    private Renderer mapRenderer;
    private static bool worldInitialized;

    /* THOUGHT SHOULD THIS BE CALLED IN ITS OWN THREAD? 
    WOULD THAT ALLOW IT TO RUN PARALLEL TO THE GAME CALCULATIONS?
    THUS, THE PAUSED FOR UPDATING THE WORLD WOULD ONLY BE NOTICED WHEN THE UPDATE MADE THE TICKING CLOCK RUN SLOWER
    EVEN THE TICKING CLOCK COULD BE PARTIALLY ALLIEVATED BY RECORDING THE TIME TAKEN TO CALCULATE A NEW DAY / YEAR 
    AND SUBTRACTING IT FROM THE NORMAL TICK TIME IN THE YIELD
    THE ONLY HITCHES THAT WOULD THUS BE NOTICED ARE FROM WHEN THE CALCULATION TIMES ARE LONGER THAT THE CLOCK TICK RATE
    */

    // THE OBVIOUS FLAW IN THIS IS THAT GRABBING INFO FROM THE WORLD MODEL IS ASYNCRONOUS WITH ITS UPDATES WHICH COULD CAUSE PROBLEMS

    // Use this for initialization
    void Start () {
        Debug.Log("Mouse Over Initialized!");
        // Tile Info
        TileInfo = findTextWithName("TileInfo");
        // Collider & Renderer
        mapCollider = gameObject.GetComponent<Collider>();
        mapRenderer = gameObject.GetComponent<Renderer>();
        // turn on mouse
        worldInitialized = true;
    }
	
	// Update is called once per frame
	void Update () {
        if (worldInitialized)
        {
            UpdateTileInfo();
        }
	}


    // Update is called once per frame
    // Update Tile Info on MouseOver
    public void UpdateTileInfo()
    {
        // TileInfo.text = "OMG A MOUSE!!!";
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if (mapCollider.Raycast(ray, out hitInfo, Mathf.Infinity))
        {
            // Get the Tile Coordinates
            int[] coor = ConvertToTileCoordinates(hitInfo.point);
            // Highlight
            // Update the Tile Info
            int today = World.getMyInstance().date.day;
            string info = World.getMyInstance().PrintTileInfo(today, coor[0], coor[2]);
            string coorInfo = "(" + coor[0] + ", " + coor[1] + ", " + coor[2] + "):";
            TileInfo.text = "Mouse Numbers: " + hitInfo.point.x + ", " + hitInfo.point.y + ", " + hitInfo.point.z + " - " + coorInfo;
            TileInfo.text = "Tile " + coorInfo + "\n" + info;
        }
        else
        {
            // Un-highlight
        }
    }

    public Text findTextWithName(string name)
    {
        GameObject canvas = GameObject.Find("Canvas");
        Text[] texts = canvas.GetComponentsInChildren<Text>();
        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i].name == name)
            {
                return texts[i];
            }
        }
        return null;
    }

    public static void activateMouse()
    {
        worldInitialized = true;
    }

    public static void deactivateMouse()
    {
        worldInitialized = false;
    }

    // Convert hitInfo into a tiles coordinates
    private int[] ConvertToTileCoordinates(Vector3 point)
    {
        // Tile info error is something to do with the rotation or the y's!
        // If camera is rotated 90-degrees in y the error reverse itself from being and up-down to left-right displacement error.
        int[] coor = new int[3];
        // Debug.Log(point.x + ", " + point.y + ", " + point.z);
        coor[0] = (int)Math.Truncate(point.x / TwoDWorldView.TILESIZE);
        coor[1] = (int)Math.Truncate(point.y / TwoDWorldView.HEIGHTSCALE);
        coor[2] = (int)Math.Truncate(point.z / TwoDWorldView.TILESIZE);
        return coor;
    }

}
