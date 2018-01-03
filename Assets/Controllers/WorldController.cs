using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshRenderer))]
public class WorldController : MonoBehaviour {

    // Constants
    private const int NUM_OF_LOADIN_FILES = 6;
    public const int NUM_OF_HUMIDITY_FILES = 6;
    public const float TIME_INTERVAL = 1.0f;
    private const string FILE_PATH_PREFIX = @"CSV\";

    // Variables
    public Texture2D mapTiles;
    public Texture2D riverTiles;

    public List<Texture2D> tempDisplay = new List<Texture2D>();

    // Use this for initialization
    void Start () {
        UnityEngine.Debug.Log("World Initialized!");
        BuildWorld("NiceMapA");
        BuildView();
        gameObject.AddComponent<MouseOverController>();
    }

    public void BuildWorld(string fileName)
    {
        // Load in the world from file.
        Stopwatch sw = Stopwatch.StartNew();
        string sampleFilePath = FILE_PATH_PREFIX + "HighTemp" + fileName;
        int worldX;
        int worldZ = getWorldDimensionsFromCSV(sampleFilePath, out worldX);
        World theWorld = World.createMyInstance(worldX, worldZ);
        Array[] csvArrays = LoadWorldFiles(fileName);
        // Load the file values into the model
        theWorld.LoadTileArrays(csvArrays);
        theWorld.FillOutTiles();
        UnityEngine.Debug.Log("Build World Time:" + sw.ElapsedMilliseconds);
        sw.Stop();
    }

    public void BuildView()
    {
        Stopwatch sw = Stopwatch.StartNew();
        TwoDWorldView.getInstance().BuildWorldMap(gameObject, mapTiles, riverTiles, true);
        UnityEngine.Debug.Log("Build View Time:" + sw.ElapsedMilliseconds);
        sw.Stop();
    }

    // METHODS FOR LOADING FILES

    private Array[] LoadWorldFiles(string fileName)
    {
        Array[] csvArrays = new Array[NUM_OF_LOADIN_FILES];
        // 0 - highTemp;
        // 1 - lowTemp;
        // 2 - variance;
        // 3 - springMidpt;
        // 4 - elevation;
        // 5 to 10 - humidity - 1 to 6;
        double[][,] humidity = new double[NUM_OF_HUMIDITY_FILES][,];
        csvArrays[0] = readIntCSVFile(FILE_PATH_PREFIX + "HighTemp" + fileName);
        csvArrays[1] = readIntCSVFile(FILE_PATH_PREFIX + "LowTemp" + fileName);
        csvArrays[2] = readCSVFile(FILE_PATH_PREFIX + "Variance" + fileName);
        csvArrays[3] = readCSVFile(FILE_PATH_PREFIX + "Midpt" + fileName);
        csvArrays[4] = readCSVFile(FILE_PATH_PREFIX + "Elevation" + fileName);
        for (int i = 0; i < NUM_OF_HUMIDITY_FILES; i++)
        {
            humidity[i] = readCSVFile(FILE_PATH_PREFIX + "Humidity" + fileName + "-" + (i + 1));
        }
        csvArrays[5] = humidity;
        // Debug.Log(humidity.GetType());
        return csvArrays;
    }

    // CSV File Readers
    private int getWorldDimensionsFromCSV(string filePath, out int worldX)
    {
        TextAsset CSVFile = Resources.Load(filePath) as TextAsset;
        string[] rows = CSVFile.text.Split('\n');
        string[] line = rows[0].Split(',');
        worldX = rows.Length;
        int worldZ = line.Length;
        UnityEngine.Debug.Log("Dimensions: X - " + worldX + ", Z - " + worldZ);
        return worldZ;
    }


    // Generates a double array
    private double[,] readCSVFile(string filePath)
    {
        // Debug.Log(filePath);
        TextAsset CSVFile = Resources.Load(filePath) as TextAsset;
        string[] rows = CSVFile.text.Split('\n');
        // Debug.Log("*****************************************");
        double[,] data = new double[World.getMyInstance().X, World.getMyInstance().Z];
        for (int Row = 0; Row < rows.Length; Row++)
        {
            string[] Line = rows[Row].Split(',');
            for (int i = 0; i < Line.Length; i++)
            {
                data[i, Row] = Convert.ToDouble(Line[i]);
            }
        }
        return data;
    }

    // Generates a integer array
    private int[,] readIntCSVFile(string filePath)
    {
        // Debug.Log(filePath);
        TextAsset CSVFile = Resources.Load(filePath) as TextAsset;
        string[] rows = CSVFile.text.Split('\n');
        // Debug.Log("*****************************************");
        int[,] data = new int[World.getMyInstance().X, World.getMyInstance().Z];
        for (int Row = 0; Row < rows.Length; Row++)
        {
            string[] Line = rows[Row].Split(',');
            for (int i = 0; i < Line.Length; i++)
            {
                data[i, Row] = Convert.ToInt32(Line[i]);
            }
        }
        return data;
    }

    // METHODS FOR SAVING GAME TO FILE

    // Not implemented yet!

}
