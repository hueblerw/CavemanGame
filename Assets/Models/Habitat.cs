﻿using System;


public class Habitat {

    // Constants
    public const double RIVERWATERINGCONSTANT = .2;
    public const double FORAGECONSTANT = .2;
    public const double SEEDCONSTANT = 1.5 * 0.002314009;
    public const double SHRUBCONSTANT = 45.0 * 0.004338767;
    public const double SCRUBCONSTANT = 55.0 * 0.006363525;
    public const double DESERTSCRUBCONSTANT = 35.0 * 0.004049516;
    public const double FORESTLEAVESCONSTANT = 200.0 * 0.019283411;
    public const double PINENEEDLECONSTANT = 150.0 * 0.008677535;
    public const double TROPICALEAFGROWTH = 1.2;
    public const double ARTICLEAFGROWTH = 0.8;
    private double EnvironmentalShiftFactor = .01; // +/- 1% a year
    private double GlacialShiftFactor = .10; // +/- 10% a year
    public const double RIVER_EFFECT_FACTOR = (RIVERWATERINGCONSTANT / 2.0);  // 10% of river volume added to the tiles rainfall
    public const int TOTAL_LAND_HABITATS = 13;
    public const int INITIAL_YEAR_RUN = 20;

    // Variables
    public string dominantType;
    public double[] typePercents;


    // Constructor
    public Habitat(double oceanPer, int[] habitatCounters)
    {
        typePercents = new double[TOTAL_LAND_HABITATS + 1];
        CreateInitialPercentage(oceanPer, habitatCounters);
        dominantType = IndexToString(getDominantIndex());
    }


    // PRIVATE METHODS
    // Initialize the Habitats based on the counter array
    private void CreateInitialPercentage(double oceanPer, int[] habitatCounters)
    {
        for (int i = 0; i < habitatCounters.Length; i++)
        {
            // Debug.Log(habitatCounters[i] + ", " + oceanPer);
            typePercents[i] = Math.Round((habitatCounters[i] / 20.0) * (1.0 - oceanPer), 2);
            // Debug.Log(i + " - " + typePercents[i]);
        }
        typePercents[13] = oceanPer;
    }


    // Convert the index to a habitat string
    private string IndexToString(int index)
    {
        string name = "";
        switch (index)
        {
            case 0:
                name = "glacier";
                break;
            case 1:
                name = "dry tundra";
                break;
            case 2:
                name = "tundra";
                break;
            case 3:
                name = "boreal";
                break;
            case 4:
                name = "artic marsh";
                break;
            case 5:
                name = "desert";
                break;
            case 6:
                name = "plains";
                break;
            case 7:
                name = "forest";
                break;
            case 8:
                name = "swamp";
                break;
            case 9:
                name = "hot desert";
                break;
            case 10:
                name = "savannah";
                break;
            case 11:
                name = "monsoon forest";
                break;
            case 12:
                name = "rainforest";
                break;
            case 13:
                name = "ocean";
                break;
        }

        return name;
    }

    // Convert String name to index number
    public static int StringToIndex(string name)
    {
        int index = -1;
        switch (name)
        {
            case "glacier":
                index = 0;
                break;
            case "dry_tundra":
                index = 1;
                break;
            case "tundra":
                index = 2;
                break;
            case "boreal":
                index = 3;
                break;
            case "artic_marsh":
                index = 4;
                break;
            case "desert":
                index = 5;
                break;
            case "plains":
                index = 6;
                break;
            case "forest":
                index = 7;
                break;
            case "swamp":
                index = 8;
                break;
            case "hot_desert":
                index = 9;
                break;
            case "savannah":
                index = 10;
                break;
            case "monsoon_forest":
                index = 11;
                break;
            case "rainforest":
                index = 12;
                break;
            case "ocean":
                index = 13;
                break;
        }

        return index;
    }


    // determine the dominant type
    private string CheckDominantType()
    {
        return IndexToString(getDominantIndex());
    }


    // get the index for the dominant habitat
    public int getDominantIndex()
    {
        if (getOceanPercents() != 1.0)
        {
            int maxIndex = 0;
            for (int i = 0; i < typePercents.Length - 1; i++)
            {
                if (typePercents[i] > typePercents[maxIndex])
                {
                    maxIndex = i;
                }
            }
            return maxIndex;
        }
        else
        {
            return 13;
        }
    }


    private double getOceanPercents()
    {
        return typePercents[13];
    }


    // Return a string with the habitat stats in it
    public override string ToString()
    {
        string data = "Habitats: ";
        for (int i = 0; i < typePercents.Length; i++)
        {
            if (typePercents[i] != 0.0)
            {
                data += "\n" + typePercents[i] * 100.0 + "% " + IndexToString(i);
            }
        }
        return data;
    }

}