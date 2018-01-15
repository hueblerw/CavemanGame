using System;
using System.Collections.Generic;
using UnityEngine;

// Based on code by quill18 in his video series linked to here: https://www.youtube.com/watch?v=bpB4BApnKhM
public class TwoDWorldView {

    private static TwoDWorldView myInstance;

    private const int BLANK_TILE_INDEX = 5;
    private const int PIXELSPERTILE = 64;
    private const int MAP_LAYERS = 4;
    private const int MAX_RIVER_WIDTH = 40;
    private Color dryRiverColor = new Color(.839f, .573f, .039f);
    public const float TILESIZE = 60.0f;
    public const float HEIGHTSCALE = TILESIZE / (float) World.WIDTH_HEIGHT_RATIO;
    private const double MAX_SNOW_CHANGE = 20.0;
    private const double BLANK_ALPHA_VALUE = 22.0;

    private int worldX;
    private int worldZ;
    private double[,] elevationVertices;
    private Texture mapTexture;
    private Texture downstreamRiverLayer;
    private Texture upstreamRiverLayer;
    private Texture snowTextureLayer;

    private Texture2D[] mapLayersArray;
    private Color waterColor;
    private Texture2D localMapTiles;
    // Caches
    private List<Texture2D> riverTilesCache;
    private Dictionary<double, Texture2D> snowTileCache;
    private Texture2D dryRiverTiles;
    private int[,] riverWidthCache;
    private int[,] habitatIndexCache;

    // Singleton Constructors
    public static TwoDWorldView getInstance()
    {
        if (myInstance == null)
        {
            myInstance = new TwoDWorldView();
        }
        return myInstance;
    }

    private TwoDWorldView()
    {
        Debug.Log("View Initialized!");
        riverTilesCache = new List<Texture2D>();
        snowTileCache = new Dictionary<double, Texture2D>();
        snowTileCache.Add(0.00, CreateBlankWhiteSquareWithAlpha(0));
        worldX = World.getMyInstance().X;
        worldZ = World.getMyInstance().Z;
        // Construct the elevation vertices
        elevationVertices = ConvertElevationToVertices();
        riverWidthCache = new int[worldX, worldZ];
        habitatIndexCache = new int[worldX, worldZ];
    }


    public void BuildWorldMap(GameObject worldMap, Texture2D mapTiles, Texture2D riverTiles, bool tileMap)
    {
        localMapTiles = mapTiles;
        riverTilesCache.Add(riverTiles);
        waterColor = riverTiles.GetPixel(31, 31);
        dryRiverTiles = ChangeColor(riverTiles, waterColor, dryRiverColor);
        MeshFilter meshFilter = worldMap.GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = worldMap.GetComponent<MeshRenderer>();
        MeshCollider meshCollider = worldMap.GetComponent<MeshCollider>();
        // Create and attach the mesh
        Mesh mesh = BuildMesh();
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
        // Create and attach the texture
        if (tileMap)
        {
           mapTexture = BuildHabitatTexture(localMapTiles);
        }
        else
        {
            mapTexture = BuildElevationTexture();
        }
        // Build the Rivers
        InitialDrawRivers();
        // meshRenderer.sharedMaterial.mainTexture = mapTexture;
        meshRenderer = AttachTextures(meshRenderer);
    }


    // Converts the model's elevation number to a map of vertices which can be used by the view
    private double[,] ConvertElevationToVertices()
    {
        elevationVertices = new double[worldX + 1, worldZ + 1];
        for (int x = 0; x < worldX + 1; x++)
        {
            for (int z = 0; z < worldZ + 1; z++)
            {
                elevationVertices[x, z] = VertexAverage(x, z);
            }
        }
        return elevationVertices;
    }

    private double VertexAverage(int x, int z)
    {
        double sum = 0.0;
        Vector2[] coor = Support.getCoordinatesAroundVertex(x, z, worldX, worldZ);
        for (int i = 0; i < coor.Length; i++)
        {
            sum += World.getMyInstance().worldArray[(int)coor[i].x, (int)coor[i].y].elevation;
        }
        return (sum / coor.Length);
    }

    private Mesh BuildMesh()
    {
        // Set some constants
        // float[,] elevations;
        int numOfTilesX = World.getMyInstance().X;
        int numOfTilesZ = World.getMyInstance().Z;
        int numOfTiles = numOfTilesX * numOfTilesZ;
        int numVertices = (numOfTilesX + 1) * (numOfTilesZ + 1);

        // Convert to mesh data
        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[2 * numOfTiles * 3];
        Vector3[] normals = new Vector3[numVertices];
        Vector2[] uv = new Vector2[numVertices];

        // Create the vertices and the normals generically
        int x, z;
        int squareIndex, triOffset;
        for (z = 0; z < numOfTilesZ + 1; z++)
        {
            for (x = 0; x < numOfTilesX + 1; x++)
            {
                vertices[z * (numOfTilesX + 1) + x] = new Vector3(x * TILESIZE, 0f, z * TILESIZE);
                // vertices[z * (numOfTilesX + 1) + x] = new Vector3(x * TILESIZE, (float) elevationVertices[x, z] * HEIGHTSCALE, z * TILESIZE);
                normals[z * (numOfTilesX + 1) + x] = Vector3.up;
                uv[z * (numOfTilesX + 1) + x] = new Vector2((float)x / (numOfTilesX + 1), (float)z / (numOfTilesZ + 1));
            }
        }
        // Create the triangle generically
        for (z = 0; z < numOfTilesZ; z++)
        {
            for (x = 0; x < numOfTilesX; x++)
            {
                squareIndex = z * numOfTilesX + x;
                triOffset = squareIndex * 6;
                // first triangle
                triangles[triOffset] = z * (numOfTilesX + 1) + x + 0;
                triangles[triOffset + 2] = z * (numOfTilesX + 1) + x + (numOfTilesX + 1) + 1;
                triangles[triOffset + 1] = z * (numOfTilesX + 1) + x + (numOfTilesX + 1) + 0; ;
                // second triangle
                triangles[triOffset + 3] = z * (numOfTilesX + 1) + x + 0;
                triangles[triOffset + 5] = z * (numOfTilesX + 1) + x + 1;
                triangles[triOffset + 4] = z * (numOfTilesX + 1) + x + (numOfTilesX + 1) + 1; ;
            }
        }

        // Create a new mesh and populate it with the data from the elevation layer
        Mesh world = new Mesh();
        world.subMeshCount = MAP_LAYERS;
        world.vertices = vertices;
        for (int i=0; i < MAP_LAYERS; i++)
        {
            world.SetTriangles(triangles, i);
        }
        world.normals = normals;
        world.uv = uv;

        // Return our mesh to the controller
        return world;
    }


    // Build the texture for the elevation map
    private Texture BuildElevationTexture()
    {
        // Initialize some variables
        int pixelsPerTile = 1;
        int adjustedX;
        int adjustedZ;
        // float[,] elevations = world.elevation.worldArray;
        float thisTileElevation;
        float greenTint;
        float redTint;
        float blueTint;
        Color color;

        // Create a texture object
        Texture2D texture = new Texture2D(worldX * pixelsPerTile, worldZ * pixelsPerTile);
        for (int x = 0; x < worldX * pixelsPerTile; x++)
        {
            for (int z = 0; z < worldZ * pixelsPerTile; z++)
            {
                adjustedX = (int)Math.Truncate((double)x / pixelsPerTile);
                adjustedZ = (int)Math.Truncate((double)z / pixelsPerTile);
                thisTileElevation = (float) World.getMyInstance().worldArray[adjustedX, adjustedZ].elevation;
                // If underwater make it a shade of blue
                if (thisTileElevation < 0.0f)
                {
                    blueTint = 1f;
                    redTint = 0f;
                    greenTint = (100f + thisTileElevation * 10f) / 253f;
                    if (greenTint < 0)
                    {
                        greenTint = 0;
                    }
                }
                // else make it a shade of green/brown
                else
                {
                    if (thisTileElevation < 20.0)
                    {
                        if (thisTileElevation < 10.0)
                        {
                            blueTint = 0f;
                            redTint = 0f;
                            greenTint = (53f + (10f - thisTileElevation) * 20f) / 253f;
                        }
                        else
                        {
                            blueTint = 0f;
                            greenTint = (103f - thisTileElevation * 5f) / 253f;
                            redTint = ((20f - thisTileElevation) * 5f) / 253f;
                        }
                    }
                    else
                    {
                        // Beyond 20 will be coded as white
                        redTint = 1f;
                        blueTint = 1f;
                        greenTint = 1f;
                    }
                }
                color = new Color(redTint, greenTint, blueTint);
                texture.SetPixel(x, z, color);
            }
        }

        // Apply the texture  and return it
        //texture.filterMode = FilterMode.Point;
        texture.Apply();
        return texture;
    }


    // Build a texture for the habitat display
    private Texture BuildHabitatTexture(Texture2D mapTiles)
    {
        // Initialize some variables
        int tileIndex;

        // Create a texture object
        Texture2D texture = new Texture2D(worldX * PIXELSPERTILE, worldZ * PIXELSPERTILE);
        for (int x = 0; x < worldX; x++)
        {
            for (int z = 0; z < worldZ; z++)
            {
                tileIndex = World.getMyInstance().worldArray[x, z].habitat.getDominantIndex();
                habitatIndexCache[x, z] = tileIndex;
                texture = placeTileInTexture(x, z, tileIndex, texture, mapTiles);
            }
        }

        // Apply the texture  and return it
        // texture.filterMode = FilterMode.Point;
        texture.Apply();
        Debug.Log("Habitat Applied");
        return texture;
    }


    public Texture UpdateHabitatTexture()
    {
        int tileIndex;
        Texture2D texture = (Texture2D) mapTexture;
        for (int x = 0; x < worldX; x++)
        {
            for (int z = 0; z < worldZ; z++)
            {
                tileIndex = World.getMyInstance().worldArray[x, z].habitat.getDominantIndex();
                // Overwrite the cell on the map only if the habitat texture has changed.
                if (habitatIndexCache[x, z] != tileIndex)
                {
                    habitatIndexCache[x, z] = tileIndex;
                    texture = placeTileInTexture(x, z, tileIndex, texture, localMapTiles);
                }
            }
        }

        texture.Apply();
        return texture;
    }


    public void InitialDrawRivers()
    {
        int day = 1;
        int downRiverIndex;
        int riverWidth;
        Texture2D downTexture = new Texture2D(worldX * PIXELSPERTILE, worldZ * PIXELSPERTILE);
        Texture2D upTexture = new Texture2D(worldX * PIXELSPERTILE, worldZ * PIXELSPERTILE);
        Texture2D snowTexture = new Texture2D(worldX * PIXELSPERTILE, worldZ * PIXELSPERTILE);
        for (int x = 0; x < worldX; x++)
        {
            for (int z = 0; z < worldZ; z++)
            {
                riverWidth = CalculateWidth(x, z, day);
                snowTexture = AddSnowTileToTexture(x, z, World.getMyInstance().worldArray[x, z].dayArray[day - 1].snowCover, snowTexture);
                riverWidthCache[x, z] = riverWidth;
                downRiverIndex = getDownstreamIndex(x, z);
                if (riverWidth != 0)
                {
                    downTexture = placeTileInTexture(x, z, downRiverIndex, downTexture, riverTilesCache[riverWidth - 1]);  // 0 -> riverWidth - 1
                    upTexture = DrawUpstreamSegments(x, z, day, riverWidth - 1, upTexture);  // 0 -> riverWidth - 1
                }
                else
                {
                    if (downRiverIndex == 0)
                    {
                        downTexture = placeTileInTexture(x, z, downRiverIndex, downTexture, dryRiverTiles);
                    }
                    else
                    {
                        downTexture = placeTileInTexture(x, z, BLANK_TILE_INDEX, downTexture, riverTilesCache[0]);
                        upTexture = DrawUpstreamSegments(x, z, day, 0, upTexture);
                    }
                }
            }
        }

        downTexture.Apply();
        upTexture.Apply();
        snowTexture.Apply();
        downstreamRiverLayer = downTexture;
        upstreamRiverLayer = upTexture;
        snowTextureLayer = snowTexture;
        Debug.Log("Rivers Applied");
    }


    public void UpdateRivers(int day)
    {
        int downRiverIndex;
        int riverWidth;
        Texture2D downTexture = (Texture2D) downstreamRiverLayer;
        Texture2D upTexture = (Texture2D) upstreamRiverLayer;
        Texture2D snowTexture = (Texture2D) snowTextureLayer;
        for (int x = 0; x < worldX; x++)
        {
            for (int z = 0; z < worldZ; z++)
            {
                riverWidth = CalculateWidth(x, z, day);
                snowTexture = AddSnowTileToTexture(x, z, World.getMyInstance().worldArray[x, z].dayArray[day - 1].snowCover, snowTexture);
                if (riverWidth != riverWidthCache[x, z])
                {
                    riverWidthCache[x, z] = riverWidth;
                    downRiverIndex = getDownstreamIndex(x, z);
                    if (riverWidth != 0)
                    {
                        downTexture = placeTileInTexture(x, z, downRiverIndex, downTexture, riverTilesCache[riverWidth - 1]);  // 0 -> riverWidth - 1
                        upTexture = DrawUpstreamSegments(x, z, day, riverWidth - 1, upTexture);  // 0 -> riverWidth - 1
                    }
                    else
                    {
                        if (downRiverIndex == 0)
                        {
                            downTexture = placeTileInTexture(x, z, downRiverIndex, downTexture, dryRiverTiles);
                        }
                        else
                        {
                            downTexture = placeTileInTexture(x, z, BLANK_TILE_INDEX, downTexture, riverTilesCache[0]);
                            upTexture = DrawUpstreamSegments(x, z, day, 0, upTexture);
                        }
                    }
                }
            }
        }

        downTexture.Apply();
        upTexture.Apply();
        snowTexture.Apply();
        downstreamRiverLayer = downTexture;
        upstreamRiverLayer = upTexture;
        snowTextureLayer = snowTexture;
    }


    private Texture2D placeTileInTexture(int x, int z, int tileIndex, Texture2D finishedTexture, Texture2D textureBase)
    {
        Color[] colorArray = getRiverColorArrayByIndex(tileIndex, textureBase);
        finishedTexture.SetPixels(x * PIXELSPERTILE, z * PIXELSPERTILE, PIXELSPERTILE, PIXELSPERTILE, colorArray);
        return finishedTexture;
    }


    private Texture2D getRiverSection(int tileIndex, int riverWidth)
    {
        Color[] colorArray;
        if (riverWidth > 0)
        {
            colorArray = getRiverColorArrayByIndex(tileIndex, riverTilesCache[riverWidth - 1]);
        }
        else
        {
            // colorArray = getRiverColorArrayByIndex(tileIndex, dryRiverTiles);
            colorArray = getRiverColorArrayByIndex(BLANK_TILE_INDEX, riverTilesCache[0]);
        }
        Texture2D texture = new Texture2D(PIXELSPERTILE, PIXELSPERTILE);
        texture.SetPixels(colorArray);
        return texture;
    }


    private Color[] getRiverColorArrayByIndex(int tileIndex, Texture2D textureBase)
    {
        return textureBase.GetPixels(tileIndex * PIXELSPERTILE, 0, PIXELSPERTILE, PIXELSPERTILE);
    }


    private int getDownstreamIndex(int x, int z)
    {
        Tile currentTile = World.getMyInstance().worldArray[x, z];
        if (currentTile.oceanPercent < 1.0)
        {
             return directionToRiverPicIndex(currentTile.flowDirection);
        }
        else
        {
            return BLANK_TILE_INDEX;
        }
    }


    private Texture2D DrawUpstreamSegments(int x, int z, int day, int riverWidth, Texture2D upTexture)
    {
        Tile currentTile = World.getMyInstance().worldArray[x, z];
        Texture2D texture = new Texture2D(PIXELSPERTILE, PIXELSPERTILE);
        int upStreamDirectionsCount = currentTile.upstreamDirections.Count;
        if (currentTile.oceanPercent != 1.0 &&  upStreamDirectionsCount > 0)
        {
            int directionIndex = directionToRiverPicIndex(currentTile.upstreamDirections[0]);
            int upStreamWidth = getUpstreamWidth(x, z, day, currentTile.upstreamDirections[0]);
            texture = getRiverSection(directionIndex, upStreamWidth);
            if (upStreamDirectionsCount > 1)
            {
                for (int i = 1; i < upStreamDirectionsCount; i++)
                {
                    directionIndex = directionToRiverPicIndex(currentTile.upstreamDirections[i]);
                    upStreamWidth = getUpstreamWidth(x, z, day, currentTile.upstreamDirections[i]);
                    texture = MergeRiverImages(texture, getRiverSection(directionIndex, upStreamWidth));
                }
            }
        }
        else
        {
            texture = getRiverSection(BLANK_TILE_INDEX, 1);  // THIS IS ALWAYS WIDTH 1
        }

        return placeTileInTexture(x, z, 0, upTexture, texture);
    }


    private int directionToRiverPicIndex(string direction)
    {
        switch (direction)
        {
            case "none":
                return 0;
            case "up":
                return 1;
            case "left":
                return 2;
            case "down":
                return 3;
            case "right":
                return 4;
        }
        return BLANK_TILE_INDEX;
    }


    private int CalculateWidth(int x, int z, int day)
    {
        // Get width from river volume - but add it later
        double surfaceWater = World.getMyInstance().worldArray[x, z].dayArray[day - 1].surfaceWater;
        int width = 0;
        if (surfaceWater > 0.0)
        {
            width = (int) (surfaceWater / 20.0);
            width++;
        }
        // Check if you need to generate new river width.
        int initCacheCount = riverTilesCache.Count;
        if (width > initCacheCount)
        {
            for (int i = (initCacheCount + 1); i <= width; i++)
            {
                WidenRiver(i);
            }
        }
        return width;
    }


    private Texture2D MergeRiverImages(Texture2D ImageA, Texture2D ImageB)
    {
        Color[] arrayA = ImageA.GetPixels();
        Color[] arrayB = ImageB.GetPixels();
        for (int i = 0; i < arrayA.Length; i++)
        {
            if(arrayA[i].a == 0f)
            {
                arrayA[i] = arrayB[i];
            }
        }
        Texture2D texture = new Texture2D(PIXELSPERTILE, PIXELSPERTILE);
        texture.SetPixels(arrayA);
        return texture;
    }


    private void WidenRiver(int riverWidth)
    {
        if (riverWidth <= MAX_RIVER_WIDTH)
        {
            Texture2D texture = new Texture2D(riverTilesCache[riverWidth - 2].width, riverTilesCache[riverWidth - 2].height);
            Graphics.CopyTexture(riverTilesCache[riverWidth - 2], texture);
            texture = ExpandRiver(texture, riverWidth);
            riverTilesCache.Add(texture);
        }
        else
        {
            riverTilesCache.Add(riverTilesCache[MAX_RIVER_WIDTH - 1]);
        }
    }


    private Texture2D ExpandRiver(Texture2D riverListTexture, int riverWidth)
    {
        Texture2D texture = new Texture2D(PIXELSPERTILE, PIXELSPERTILE);
        Texture2D riverMapTexture = riverListTexture;
        for (int n = 1; n < 5; n++)
        {
            Graphics.CopyTexture(getRiverSection(n, riverWidth - 1), texture);
            if (n % 2 == 0)
            {
                texture = ExpandUpDownRiver(texture, riverWidth);
            }
            else
            {
                texture = ExpandLeftRightRiver(texture, riverWidth);
            }
            riverMapTexture = placeTileInTexture(n, 0, 0, riverMapTexture, texture);
        }

        return riverMapTexture;
    }


    private Texture2D ExpandUpDownRiver(Texture2D riverListTexture, int riverWidth)
    {
        int i = 0;
        int j = 0;
        int first = -1;
        while(i < PIXELSPERTILE)
        {
            while (j < PIXELSPERTILE && first == -1)
            {
                if (riverListTexture.GetPixel(i, j).a != 0)
                {
                    first = j;
                    if (riverWidth % 2 == 1)
                    {
                        riverListTexture.SetPixel(i, j - 1, waterColor);
                    }
                    else
                    {
                        riverListTexture.SetPixel(i, j + riverWidth - 1, waterColor);
                    }
                }
                j++;
            }
            i++;
            j = 0;
            first = -1;
        }
        return riverListTexture;
    }


    private Texture2D ExpandLeftRightRiver(Texture2D riverListTexture, int riverWidth)
    {
        int i = 0;
        int j = 0;
        int first = -1;
        while (j < PIXELSPERTILE)
        {
            while (i < PIXELSPERTILE && first == -1)
            {
                if (riverListTexture.GetPixel(i, j).a != 0)
                {
                    first = j;
                    if (riverWidth % 2 == 1)
                    {
                        riverListTexture.SetPixel(i - 1, j, waterColor);
                    }
                    else
                    {
                        riverListTexture.SetPixel(i + riverWidth - 1, j, waterColor);
                    }
                }
                i++;
            }
            j++;
            i = 0;
            first = -1;
        }

        return riverListTexture;
    }


    private MeshRenderer AttachTextures(MeshRenderer meshRenderer)
    {
        Material[] mapTextures = new Material[MAP_LAYERS];
        for(int i = 0; i < mapTextures.Length; i++)
        {
            mapTextures[i] = new Material(meshRenderer.materials[i]);
        }
        mapTextures[0].mainTexture = mapTexture;
        mapTextures[1].mainTexture = downstreamRiverLayer;
        mapTextures[2].mainTexture = upstreamRiverLayer;
        mapTextures[3].mainTexture = snowTextureLayer;
        meshRenderer.sharedMaterials = mapTextures;
        return meshRenderer;
    }


    private Texture2D AddSnowTileToTexture(int x, int z, double snowCover, Texture2D snowTexture)
    {
        double alpha = SnowCoverToAlpha(snowCover);
        Texture2D value;
        if (!snowTileCache.TryGetValue(alpha, out value))
        {
            value = CreateBlankWhiteSquareWithAlpha(alpha);
            snowTileCache.Add(alpha, value);
        }

        return placeTileInTexture(x, z, 0, snowTexture, value);
    }


    private Texture2D CreateBlankWhiteSquareWithAlpha(double alpha)
    {
        Texture2D snow = new Texture2D(PIXELSPERTILE, PIXELSPERTILE);
        Color snowColor = Color.white;
        snowColor.a = (float) alpha;
        for (int i = 0; i < PIXELSPERTILE; i++)
        {
            for (int j = 0; j < PIXELSPERTILE; j++)
            {
                snow.SetPixel(i, j, snowColor);
            }
        }
        return snow;
    }


    private double SnowCoverToAlpha(double snowCover)
    {
        if (snowCover >= MAX_SNOW_CHANGE)
        {
            return 230;
        }
        else if(snowCover == 0.0)
        {
            return 0;
        }
        else
        {
            return Math.Round((70.0 + (snowCover / MAX_SNOW_CHANGE) * 180.0) / 255.0, 2);
        }
    }


    private Texture2D ChangeColor(Texture2D riverTiles, Color waterColor, Color dryRiverColor)
    {
        Texture2D texture = new Texture2D(riverTiles.width, riverTiles.height);
        for (int i = 0; i < riverTiles.width; i++)
        {
            for (int j = 0; j < riverTiles.height; j++)
            {
                if (riverTiles.GetPixel(i, j) == waterColor)
                {
                    texture.SetPixel(i, j, dryRiverColor);
                }
                else
                {
                    texture.SetPixel(i, j, riverTiles.GetPixel(i, j));
                }
            }
        }

        return texture;
    }


    private int getUpstreamWidth(int x, int z, int day, string direction)
    {
        switch (direction)
        {
            case "up":
                return CalculateWidth(x, z + 1, day);
            case "left":
                return CalculateWidth(x - 1, z, day);
            case "down":
                return CalculateWidth(x, z - 1, day);
            case "right":
                return CalculateWidth(x + 1, z, day);
        }
        return 0;
    }


    private string printRiverTexture(Texture2D texture)
    {
        string output = "";
        for (int i = 0; i < texture.height; i++)
        {
            for (int j = 0; j < texture.width; j++)
            {
                if (texture.GetPixel(i, j).a != 0.0)
                {
                    output += "B ";
                }
                else
                {
                    output += "| ";
                }
            }
            output += "\n";
        }
        return output;
    }

}
