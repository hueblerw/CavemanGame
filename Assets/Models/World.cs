using System;
using System.Collections.Generic;
using UnityEngine;

public class World
{
    private static World myInstance;

    // Constants
    public const int NUM_OF_HUMIDITY_FILES = 6;
    public const double WIDTH_HEIGHT_RATIO = 5.0f;

    // Variables
    public int X;
    public int Z;
    public Tile[,] worldArray;
    public Date date;
    // public Herds[] herds;
    // public Tribes[] tribes;
    // public Buildings[] buildings;

    // Other Variables
    public float seaLevel;
    public float maxElevationDifference;
    private float XMultiplier;
    private float ZMultiplier;
    private double[][,] humidityArray;
    private double[,] lastDayOfYearSnowCover;
    private double[,] lastDayOfYearSurfaceWater;

    // Singleton Getters
    public static World getMyInstance()
    {
        if (myInstance == null)
        {
            throw new Exception("NO WORLD EXISTS EXCEPTION!");
        }
        return myInstance;
    }

    public static World createMyInstance(int maxX, int maxZ)
    {
        if (myInstance == null)
        {
            myInstance = new World(maxX, maxZ);
        }
        return myInstance;
    }

    // Constructor
    private World(int ix, int iz)
    {
        X = ix;
        Z = iz;
        worldArray = new Tile[X, Z];
        date = new Date();
        for (int x = 0; x < X; x++)
        {
            for (int z = 0; z < Z; z++)
            {
                worldArray[x, z] = new Tile();
            }
        }
    }

    // Methods
    public string PrintTileInfo(int day, int x, int z)
    {
        string elevationInfo = "Elevation: " + worldArray[x, z].elevation;
        string weatherInfo = worldArray[x, z].dayArray[day - 1].printWeatherInfo();
        string riverDirectionInfo = worldArray[x, z].printRiverDirections();
        string habitatInfo = worldArray[x, z].habitat.ToString();
        string lifeInfo = worldArray[x, z].habitat.printLifeInfo(worldArray[x, z].dayArray);
        return elevationInfo + "\n" + weatherInfo + "\t" + riverDirectionInfo +  "\n" + habitatInfo + "\n" + lifeInfo;
    }


    public void LoadTileArrays(Array[] csvArrays)
    {
        // 0 - highTemp;
        // 1 - lowTemp;
        // 2 - variance;
        // 3 - springMidpt;
        // 4 - elevation;
        // 5 to 10 - humidity - 1 to 6;
        for (int x = 0; x < X; x++)
        {
            for (int z = 0; z < Z; z++)
            {
                int[,] temp = (int[,])csvArrays[0];
                worldArray[x, z].highTemp = temp[x, z];
                temp = (int[,])csvArrays[1];
                worldArray[x, z].lowTemp = temp[x, z];
                double[,] tempdoub = (double[,])csvArrays[2];
                worldArray[x, z].variance = tempdoub[x, z];
                tempdoub = (double[,])csvArrays[3];
                worldArray[x, z].springMidpt = tempdoub[x, z];
                tempdoub = (double[,])csvArrays[4];
                worldArray[x, z].elevation = tempdoub[x, z];
                // worldArray[x, z].humidity = csvArrays[5];
            }
        }
        // Debug.Log(csvArrays[5].GetType());
        humidityArray = (double[][,]) csvArrays[5];
    }


    public void FillOutTiles(/*Terrain theMap*/)
    {
        XMultiplier = 1f / X;
        ZMultiplier = 1f / Z;
        maxElevationDifference = MaxNetDiff();
        System.Random randy = new System.Random();
        // Debug.Log("Multipliers: " + XMultiplier + ", " + ZMultiplier);
        for (int x = 0; x < X; x++)
        {
            for (int z = 0; z < Z; z++)
            {
                // Calculate Ocean %
                CalculateOceanPercents(x, z);
                // Calculate Hill %
                CalculateHillPercentage(x, z);
                // Generate Minerals
                    // Not Implemented Yet
                // Calculate Flow Rate and Direction
                CalculateFlow(x, z, randy);
                // Generate Habitat Percents
                // Create Temp Equations
                worldArray[x, z].CreateTempEQ();
                // Create Rain Generator and Fetch Humidity Arrays
                RainGenerator.getInstance(X, Z, humidityArray);
                // Random Tree Distributor
                worldArray[x, z].randomTreeDistributor = TreeDistribution();
            }
        }

        // Generate 20 years and use it to populate the habitat percentages.
        // Temporary Tester:    *****
        lastDayOfYearSnowCover = new double[X, Z];
        lastDayOfYearSurfaceWater = new double[X, Z];
        FetchUpStreamDirections();
        // Initialize the Habitat Model
        CreateHabitats();
        // Create the first Year of the Game
        GenerateANewYear();
        // MainController.theWorld = this;
    }


    public void GenerateANewYear()
    {
        // Generate Temps
        GenerateYearOfTemps();
        // Generate Rainfall & SnowCover & Surface Water
        // For Rainfall generate the entire map for one day then add them to the day array.
        GenerateYearOfRainSnowAndSurfaceWater();
        // Store the last days values for use next year
        StoreLastDayOfYear();
    }


    public void GenerateANewDay()
    {
        // Update Wildlife

    }


    // Use the Terrain Heights to find Ocean Percents
    private void CalculateOceanPercents(/*Terrain theMap,*/ int x, int z)
    {
        // Debug.Log("Sea Level: " + seaLevel);
        double sum = 0f;
        double negSum = 0f;

        // THIS USES THE TERRAIN MAP ITSELF TO CALCULATE THE OCEAN PERCENTAGE
        /*
        int sum = 0;
        int count = 0;

        float ax = x * XMultiplier;
        float az = z * ZMultiplier;
        while (ax < (x + .9f) * XMultiplier)
        {
            az = z * ZMultiplier;
            while (az < (z + .9f) * ZMultiplier)
            {
                count++;
                if (theMap.terrainData.GetInterpolatedHeight(ax, az) < seaLevel)
                {
                    sum++;
                }
                az += (ZMultiplier / 10f);
            }
            ax += (XMultiplier / 10f);
        }
        */

        // THIS VERSION ESTIMATES BASED ON:  SUM OF ALL NEGS / ABS SUM OF ALL
        Vector2[] cellsAround = Support.GetCoordinatesAround(x, z, X, Z);
        double myElevation = worldArray[x, z].elevation;
        for(int i = 0; i < cellsAround.Length; i++)
        {
            // Average it to estimate the vertex values.
            double value = (worldArray[(int)cellsAround[i].x, (int)cellsAround[i].y].elevation + myElevation) / 2.0;
            sum += Math.Abs(value);
            if (value < 0.0)
            {
                negSum += Math.Abs(value);
            }  
        }

        // worldArray[x, z].oceanPercent = (double)sum / count;
        worldArray[x, z].oceanPercent = Math.Round(negSum / sum, 2);
    }


    // Calculate the Hill Percentages as normal
    private void CalculateHillPercentage(int x, int z)
    {
        if (worldArray[x, z].oceanPercent != 1.0)
        {
            worldArray[x, z].hillPercent = (NetDiff(x, z) / maxElevationDifference);
        }
    }


    // Calculate the Flow direction and flow rate
    private void CalculateFlow(int x, int z, System.Random randy)
    {
        if (worldArray[x, z].oceanPercent == 1.0)
        {
            worldArray[x, z].flowDirection = "none";
            worldArray[x, z].soilAbsorption = 1f;
        }
        else
        {
            List<Vector3> coor = Support.GetCardinalCoordinatesAround(x, z, X, Z);
            int len = coor.Count;
            for (int v = 0; v < coor.Count; v++)
            {
                // Debug.Log(worldArray[(int)coor[v].x, (int)coor[v].y].elevation + " >= " + worldArray[x, z].elevation);
                if (worldArray[(int)coor[v].x, (int)coor[v].y].elevation >= worldArray[x, z].elevation)
                {
                    coor.Remove(coor[v]);
                    v--;
                }
            }
            // Debug.Log(coor.Count);
            Vector3[] downhills = coor.ToArray();
            if (downhills.Length == 0)
            {
                worldArray[x, z].flowDirection = "none";
                CalculateSoilAbsorption(x, z, randy);
            }
            else if (downhills.Length == 1)
            {
                worldArray[x, z].flowDirection = SetDirection(downhills[0]);
                CalculateSoilAbsorption(x, z, randy);
            }
            else
            {
                int index = randy.Next(0, downhills.Length);
                worldArray[x, z].flowDirection = SetDirection(downhills[index]);
                CalculateSoilAbsorption(x, z, randy);
            }
        }
    }


    public float[] getMinMaxElevations()
    {
        float[] values = new float[2];
        values[0] = MaxElevation();
        values[1] = MinElevation();
        return values;
    }


    private float MaxElevation()
    {
        double max = 0;
        for (int x = 0; x < X; x++)
        {
            for (int z = 0; z < Z; z++)
            {
                if (worldArray[x, z].elevation > max)
                {
                    max = worldArray[x, z].elevation;
                }
            }
        }

        return (float)max;
    }

    private float MinElevation()
    {
        double min = 1000;
        for (int x = 0; x < X; x++)
        {
            for (int z = 0; z < Z; z++)
            {
                if (worldArray[x, z].elevation < min)
                {
                    min = worldArray[x, z].elevation;
                }
            }
        }

        return (float)min;
    }


    // Finds the max elevation difference between cells on the map.
    private float MaxNetDiff()
    {
        float maxDiff = 0f;
        float diff;
        for (int x = 0; x < X; x++)
        {
            for (int z = 0; z < Z; z++)
            {
                diff = NetDiff(x, z);
                if (diff > maxDiff)
                {
                    maxDiff = diff;
                }
            }
        }
        return maxDiff;
    }


    // Finds the max elevation difference between cells on the map.
    private float NetDiff(int x, int z)
    {
        float diff = 0f;
        Vector2[] coor = Support.GetCoordinatesAround(x, z, X, Z);
        for (int i = 0; i < coor.Length; i++)
        {
            diff += Mathf.Abs((float)(worldArray[x, z].elevation - worldArray[(int)coor[i].x, (int)coor[i].y].elevation));
        }

        return diff / coor.Length;
    }


    private double[] TreeDistribution()
    {
        double[] treeDist = new double[100];
        System.Random randy = new System.Random();
        for (int i = 0; i < treeDist.Length; i++)
        {
            treeDist[i] = randy.NextDouble() * 100.0;
        }

        return treeDist;
    }


    private string SetDirection(Vector3 direction)
    {
        string output = "none";
        switch ((int)direction.z)
        {
            case 0:
                output = "left";
                break;
            case 1:
                output = "right";
                break;
            case 2:
                output = "down";
                break;
            case 3:
                output = "up";
                break;
        }

        return output;
    }


    private void CalculateSoilAbsorption(int x, int z, System.Random randy)
    {
        worldArray[x, z].soilAbsorption = ((randy.NextDouble() + .2) * (1.0 - worldArray[x, z].oceanPercent) * (1.0 - worldArray[x, z].hillPercent));
    }


    private void GenerateYearOfTemps()
    {
        // For temps generate an entire years of days for each tile at once
        System.Random randy = new System.Random();
        for (int x = 0; x < X; x++)
        {
            for (int z = 0; z < Z; z++)
            {
                for (int day = 0; day < Date.DAYS_PER_YEAR; day++)
                {
                    // Debug.Log(worldArray[x, z].dayArray[day].temp);
                    worldArray[x, z].dayArray[day].temp = worldArray[x, z].tempEQ.generateTodaysTemp(day, randy);
                }
            }
        }
    }


    private void GenerateYearOfRainSnowAndSurfaceWater()
    {
        double[,] rainDay;
        double[,] snowCover = new double[X, Z];
        double melt;
        double downstream;
        double upstream;
        double previousDaySurfaceWater;
        double previousDaySnowCover;
        System.Random randy = new System.Random();
        for (int day = 0; day < Date.DAYS_PER_YEAR; day++)
        {
            // double[,] surfaceWater;
            rainDay = GenerateDayOfRainAndSnow(day, randy);
            for (int x = 0; x < X; x++)
            {
                for (int z = 0; z < Z; z++)
                {
                    // Save the total precipitation for that day
                    worldArray[x, z].dayArray[day].precip = rainDay[x, z];
                    // Calculate the up and downstream flow.
                    downstream = 0.0;
                    if (day != 0)
                    {
                        previousDaySurfaceWater = worldArray[x, z].dayArray[day - 1].surfaceWater;
                        previousDaySnowCover = worldArray[x, z].dayArray[day - 1].snowCover;
                    }
                    else
                    {
                        previousDaySurfaceWater = lastDayOfYearSurfaceWater[x, z];
                        previousDaySnowCover = lastDayOfYearSnowCover[x, z];
                    }

                    if (worldArray[x, z].flowDirection != "none")
                    {
                        downstream = worldArray[x, z].getFlowRate(previousDaySurfaceWater);
                    }
                    upstream = GetUpstreamFlow(x, z, day);
                    // if its raining - let it rain! / account for the snow melt!
                    if (worldArray[x, z].dayArray[day].temp > 32)
                    {
                        // Get the rainfall for the day
                        worldArray[x, z].dayArray[day].rain = rainDay[x, z];
                        // Deal with the snow melt and the surface water.
                        if (worldArray[x, z].oceanPercent != 1.0)
                        {
                            melt = SnowMelt(previousDaySnowCover, worldArray[x, z].dayArray[day].temp);
                            worldArray[x, z].dayArray[day].snowCover = previousDaySnowCover - melt;
                            worldArray[x, z].dayArray[day].surfaceWater = Math.Max(previousDaySurfaceWater - worldArray[x, z].soilAbsorption + rainDay[x, z] - downstream + upstream + melt, 0.0);
                        }
                    }
                    // if snowing - add some snow!
                    else
                    {
                        if (worldArray[x, z].oceanPercent != 1.0)
                        {
                            if (day != 0)
                            {
                                worldArray[x, z].dayArray[day].snowCover = worldArray[x, z].dayArray[day - 1].snowCover + rainDay[x, z];
                                worldArray[x, z].dayArray[day].surfaceWater = Math.Max(worldArray[x, z].dayArray[day - 1].surfaceWater - worldArray[x, z].soilAbsorption - downstream + upstream, 0.0);
                            }
                            else
                            {
                                worldArray[x, z].dayArray[day].snowCover = lastDayOfYearSnowCover[x, z] + rainDay[x, z];
                                worldArray[x, z].dayArray[day].surfaceWater = Math.Max(lastDayOfYearSurfaceWater[x, z] - worldArray[x, z].soilAbsorption - downstream + upstream, 0.0);
                            }
                        }
                    }
                }
            }
        }
    }


    private double[,] GenerateDayOfRainAndSnow(int day, System.Random randy)
    {
        double[,] rainDay = new double[X, Z];
        // Generate a map of double with rain values.
        rainDay = RainGenerator.getInstance().GenerateWorldsDayOfRain(day, randy);

        return rainDay;
    }


    private double SnowMelt(double yesterdaySnowCover, double todaysTemp)
    {
        double melt = Math.Max((todaysTemp - 32.0) * ((3.0 * 1.8) / 25.4), 0.0);
        return Math.Min(melt, yesterdaySnowCover);
    }


    private void FetchUpStreamDirections()
    {
        for (int x = 0; x < X; x++)
        {
            for (int z = 0; z < Z; z++)
            {
                // Look left
                if(x > 0)
                {
                    if (worldArray[x - 1, z].flowDirection == "right")
                    {
                        worldArray[x, z].upstreamDirections.Add("left");
                    }
                }
                // Look right
                if (x < X - 1)
                {
                    if (worldArray[x + 1, z].flowDirection == "left")
                    {
                        worldArray[x, z].upstreamDirections.Add("right");
                    }
                }
                // Look up
                if (z > 0)
                {
                    if (worldArray[x, z - 1].flowDirection == "up")
                    {
                        worldArray[x, z].upstreamDirections.Add("down");
                    }
                }
                // Look down
                if (z < Z - 1)
                {
                    if (worldArray[x, z + 1].flowDirection == "down")
                    {
                        worldArray[x, z].upstreamDirections.Add("up");
                    }
                }
            }
        }
    }


    // Change the flow rate to be dependent on river volume.
    private double GetUpstreamFlow(int x, int z, int day)
    {
        List<string> upstreams = worldArray[x, z].upstreamDirections;
        double total = 0.0;
        double prevDay = 0.0;
        for (int i = 0; i < upstreams.Count; i++)
        {
            // Fix the day if day is zero
            switch (upstreams[i])
            {
                case "left":
                    if (day != 0)
                    {
                        prevDay = worldArray[x - 1, z].dayArray[day - 1].surfaceWater;
                    }
                    else
                    {
                        prevDay = lastDayOfYearSurfaceWater[x - 1, z];
                    }
                    total += Math.Min(worldArray[x - 1, z].getFlowRate(prevDay), prevDay);
                    break;
                case "right":
                    if (day != 0)
                    {
                        prevDay = worldArray[x + 1, z].dayArray[day - 1].surfaceWater;
                    }
                    else
                    {
                        prevDay = lastDayOfYearSurfaceWater[x + 1, z];
                    }
                    total += Math.Min(worldArray[x + 1, z].getFlowRate(prevDay), prevDay);
                    break;
                case "up":
                    if (day != 0)
                    {
                        prevDay = worldArray[x, z + 1].dayArray[day - 1].surfaceWater;
                    }
                    else
                    {
                        prevDay = lastDayOfYearSurfaceWater[x, z + 1];
                    }
                    total += Math.Min(worldArray[x, z + 1].getFlowRate(prevDay), prevDay);
                    break;
                case "down":
                    if (day != 0)
                    {
                        prevDay = worldArray[x, z - 1].dayArray[day - 1].surfaceWater;
                    }
                    else
                    {
                        prevDay = lastDayOfYearSurfaceWater[x, z - 1];
                    }
                    total += Math.Min(worldArray[x, z - 1].getFlowRate(prevDay), prevDay);
                    break;
            }
        }

        return total;
    }


    private void StoreLastDayOfYear()
    {
        for (int x = 0; x < X; x++)
        {
            for (int z = 0; z < Z; z++)
            {
                lastDayOfYearSnowCover[x, z] = worldArray[x, z].dayArray[Date.DAYS_PER_YEAR - 1].snowCover;
                lastDayOfYearSurfaceWater[x, z] = worldArray[x, z].dayArray[Date.DAYS_PER_YEAR - 1].surfaceWater;
            }
        }
    }


    private void CreateHabitats()
    {
        int[,][] worldCounters = CalculateInitialHabitatCounters();
        for (int x = 0; x < X; x++)
        {
            for (int z = 0; z < Z; z++)
            {
                worldArray[x, z].habitat = new Habitat(worldArray[x, z].oceanPercent, worldCounters[x, z]);
            }
        }
    }


    private int[,][] CalculateInitialHabitatCounters()
    {
        int[,][] worldCounters = InitializeHabitatCounters();
        int hotDays;
        int coldDays;
        double rainSum;
        double riverLevel;
        for (int year = 0; year < Habitat.INITIAL_YEAR_RUN; year++)
        {
            GenerateANewYear();
            for (int x = 0; x < X; x++)
            {
                for (int z = 0; z < Z; z++)
                {
                    rainSum = worldArray[x, z].HabitatYearInfo(out riverLevel, out hotDays, out coldDays);
                    worldCounters[x, z] = SetYearsHabitatCounters(worldCounters[x, z], rainSum, hotDays, coldDays, riverLevel);
                }
            }
        }

        return worldCounters;
    }


    private int[] SetYearsHabitatCounters(int[] counterArray, double rainSum, int hotDays, int coldDays, double riverLevel)
    {
        // Get the favored habitat for this year
        string wetness = DetermineWetness(rainSum + riverLevel * Habitat.RIVER_EFFECT_FACTOR);
        string temp = DetermineTemp(hotDays, coldDays);
        int index = DetermineHabitatFavored(wetness, temp);
        counterArray[index]++;
        return counterArray;
    }


    // Wetness determination based on the year's rainfall
    public static string DetermineWetness(double water)
    {
        string wetness;
        if (water < 20.0)
        {
            wetness = "dry";
        }
        else
        {
            if (water < 40.0)
            {
                wetness = "moderate";
            }
            else
            {
                if (water < 60.0)
                {
                    wetness = "wet";
                }
                else
                {
                    wetness = "very wet";
                }
            }
        }

        return wetness;
    }

    // Tempearteness determination based on the year's hot and coldays
    public static string DetermineTemp(int hotDays, int coldDays)
    {
        // Temperature ifs
        string temperature = "temperate";
        if (hotDays > 40 && coldDays < 10)
        {
            temperature = "tropical";
        }
        else
        {
            if (hotDays < 10 && coldDays > 40)
            {
                temperature = "artic";
            }
        }

        return temperature;
    }

    // Determine the Index of the habitat the weather favored this year
    public static int DetermineHabitatFavored(string wetness, string temp)
    {
        int index = 0;
        // account for temp
        switch (temp)
        {
            case "artic":
                index = 0;
                break;
            case "temperate":
                index = 4;
                break;
            case "tropical":
                index = 8;
                break;
        }
        // account for wetness
        switch (wetness)
        {
            case "dry":
                index += 1;
                break;
            case "moderate":
                index += 2;
                break;
            case "wet":
                index += 3;
                break;
            case "very wet":
                index += 4;
                break;
        }

        return index;
    }


    private int[,][] InitializeHabitatCounters()
    {
        int[,][] worldCounters = new int[X, Z][];
        for (int x = 0; x < X; x++)
        {
            for (int z = 0; z < Z; z++)
            {
                worldCounters[x, z] = new int[Habitat.TOTAL_LAND_HABITATS];
            }
        }
        return worldCounters;
    }

}