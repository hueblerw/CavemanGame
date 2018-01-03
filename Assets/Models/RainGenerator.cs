using System;

public class RainGenerator {

    private static RainGenerator myInstance;

    // Variables
    private bool spread;
    private int X;
    private int Z;
    private double[][,] humidityArr;
    // Constants
    private const double SPAWN_MULT = .25 * 100.0;
    private const double SPAWN_LOWER_BOUND = 5.0;
    private const double SPREAD_MULT = 1.0;
    private const float DECAY_CONST = 15.0f;
    private const int ROUNDED_TO = 2;
    private const float HUMIDITY_SPREAD_STRENGTH_MULT = 2.0f;
    private const float SPREAD_CHANCE_MULT = 8.0f;
    private const float SPREAD_CHANCE_MINIMUM_BEFORE_DECAY = 10f;
    private const int DAYS_PER_HUMIDITY_ARRAY_NUM = Date.DAYS_PER_YEAR / World.NUM_OF_HUMIDITY_FILES;

    // Singleton Setup
    public static RainGenerator getInstance()
    {
        if (myInstance == null)
        {
            throw new Exception("NO RAIN_GENERATOR EXISTS EXCEPTION!");
        }
        return myInstance;
    }

    public static RainGenerator getInstance(int X, int Z, double[][,] humidityArray)
    {
        if(myInstance == null)
        {
            myInstance = new RainGenerator(X, Z, humidityArray);
        }
        return myInstance;
    }

    // Constructor
    private RainGenerator(int X, int Z, double[][,] humidityArray)
    {
        this.X = X;
        this.Z = Z;
        humidityArr = humidityArray;
    }

    // World Rainfall Generation Methods
    public double[,] GenerateWorldsDayOfRain(int day, System.Random randy)
    {
        double[,] stormArray = new double[X, Z];
        double decay = 0.0;

        // Debug.Log(day + "-" + X + ", " + Z);
        stormArray = GenerateStormCenters(day, randy);
        // Begin to loop until a storm didn't sucessfully spread
        decay = 0.0;
        spread = true;
        while (spread)
        {
            spread = false;
            stormArray = SpreadStorms(stormArray, day, decay);
            // Debug.Log("spread: " + spread);
            decay += DECAY_CONST;
        }

        return stormArray;
    }

    // Generate Storm Centers
    private double[,] GenerateStormCenters(int day, System.Random randy)
    {
        double[,] stormOrigins = new double[X, Z];

        for (int x = 0; x < X; x++)
        {
            for (int z = 0; z < Z; z++)
            {
                if (randy.Next(0, 10000) <= (CalculateHumidityFromBase(day, x, z) * SPAWN_MULT + SPAWN_LOWER_BOUND))
                {
                    stormOrigins[x, z] = -SPAWN_MULT;
                }
            }
        }

        return stormOrigins;
    }

    // Spread the Storms from those centers
    private double[,] SpreadStorms(double[,] stormArray, int day, double decay)
    {
        System.Random randy = new System.Random();
        double[,] nextWave = new double[X, Z];
        double strength;
        for (int x = 0; x < X; x++)
        {
            for (int z = 0; z < Z; z++)
            {
                if (stormArray[x, z] < 0)
                {
                    // Generate the present strength
                    if (stormArray[x, z] == -SPAWN_MULT)
                    {
                        strength = GenerateSpawnStrength(CalculateHumidityFromBase(day, x, z), randy);
                    }
                    else
                    {
                        strength = -stormArray[x, z];
                    }
                    // Spread to neighbors
                    nextWave = SpreadToCellsAround(day, x, z, stormArray, strength, decay, randy);
                    // Record your own strength
                    nextWave[x, z] = Math.Round(strength, 2);
                }
            }
        }

        return nextWave;
    }

    // Spread to Cells Around Calculation
    private double[,] SpreadToCellsAround(int day, int x, int z, double[,] stormArray, double neighbor, double decay, System.Random randy)
    {
        double[,] nextWave = stormArray;

        // Add the four possible values if legal
        if (x != 0 && stormArray[x - 1, z] <= 0)
        {
            nextWave = SpawnCheck(day, x - 1, z, neighbor, stormArray, nextWave, decay, randy);
        }
        if (z != 0 && stormArray[x, z - 1] <= 0)
        {
            nextWave = SpawnCheck(day, x, z - 1, neighbor, stormArray, nextWave, decay, randy);
        }
        if (x != X - 1 && stormArray[x + 1, z] <= 0)
        {
            nextWave = SpawnCheck(day, x + 1, z, neighbor, stormArray, nextWave, decay, randy);
        }
        if (z != Z - 1 && stormArray[x, z + 1] <= 0)
        {
            nextWave = SpawnCheck(day, x, z + 1, neighbor, stormArray, nextWave, decay, randy);
        }

        return nextWave;
    }

    // get Spawn Strenth
    private double GenerateSpawnStrength(double humidity, System.Random randy)
    {
        double multiplier = Math.Pow(randy.NextDouble(), 2.0);
        double output = (humidity + .1f) * multiplier;
        return Math.Round(output, 1);
    }

    // get Spread Strength
    private double GenerateSpreadStrength(double neighborStrength, double humidity, System.Random randy)
    {
        double addifier = randy.Next(70, 90);
        double neighborPercentage = (HUMIDITY_SPREAD_STRENGTH_MULT * humidity) + addifier;
        double newStrength = (neighborPercentage / 100f) * neighborStrength;
        // Kill off half the .1's
        if (Math.Round(newStrength, 1) == .1 && randy.Next(0, 10) < 5)
        {
            newStrength = 0f;
        }
        return Math.Round(newStrength, ROUNDED_TO);
    }

    // Add a new spawned square
    private double[,] SpawnCheck(int day, int a, int b, double neighbor, double[,] stormArray, double[,] nextWave, double decay, System.Random randy)
    {
        double spreadChance = CalculateHumidityFromBase(day, a, b) * SPREAD_CHANCE_MULT + SPREAD_CHANCE_MINIMUM_BEFORE_DECAY - decay;
        if (randy.Next(0, 100) < spreadChance * SPREAD_MULT)
        {
            double strength = -GenerateSpreadStrength(neighbor, stormArray[a, b], randy);
            if (strength < 0)
            {
                nextWave[a, b] = strength;
                spread = true;
            }
        }

        return nextWave;
    }

    // Need a method to calculate the square's humidity number based upon the day of the year
    public double CalculateHumidityFromBase(int day, int x, int z)
    {
        int arrayNum = day / DAYS_PER_HUMIDITY_ARRAY_NUM;
        int remainder = day % DAYS_PER_HUMIDITY_ARRAY_NUM;
        int nextNum;

        // Get the index of the next array
        if (arrayNum == World.NUM_OF_HUMIDITY_FILES - 1)
        {
            nextNum = 0;
        }
        else
        {
            nextNum = arrayNum + 1;
        }

        return getHumidityFromArray(nextNum, remainder, x, z);
    }

    private double getHumidityFromArray(int arrayNum, int remainder, int x, int z)
    {
        // Use the linear equation formula to find today's humidity
        double humidity = (humidityArr[arrayNum][x, z] - humidityArr[arrayNum][x, z]) * (remainder / DAYS_PER_HUMIDITY_ARRAY_NUM) + humidityArr[arrayNum][x, z];
        // Modify it for balance purposes
        humidity = Math.Round(Math.Sqrt(10f * humidity), ROUNDED_TO);
        return humidity;
    }

}
