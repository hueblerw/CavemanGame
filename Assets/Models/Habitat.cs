using System;

public class Habitat {

    // Constants
    public const double RIVERWATERINGCONSTANT = .2;
    public const double FORAGECONSTANT = .2;
    public const double SEEDCONSTANT = 1.5 * 0.002314009;
    public const double SHRUBCONSTANT = 45.0 * 0.004338767;
    public const double SCRUBCONSTANT = 55.0 * 0.006363525;
    public const double DESERTSCRUBCONSTANT = 35.0 * 0.004049516;
    public const double FORESTLEAVESCONSTANT = 200.0 * 0.019283411;
    private double EnvironmentalShiftFactor = .01; // +/- 1% a year
    private double GlacialShiftFactor = .10; // +/- 10% a year
    public const double RIVER_EFFECT_FACTOR = (RIVERWATERINGCONSTANT / 2.0);  // 10% of river volume added to the tiles rainfall
    public const int TOTAL_LAND_HABITATS = 13;
    public const int INITIAL_YEAR_RUN = 20;

    // Variables
    public string dominantType;
    public Subhabitat[] typePercents;

    // Constructor
    public Habitat(double oceanPer, int[] habitatCounters)
    {
        typePercents = new Subhabitat[TOTAL_LAND_HABITATS + 1];
        for (int i = 0; i < TOTAL_LAND_HABITATS + 1; i++)
        {
            typePercents[i] = new Subhabitat(i);
        }
        CreateInitialPercentage(oceanPer, habitatCounters);
        dominantType = Subhabitat.IndexToString(getDominantIndex());
    }


    // PRIVATE METHODS
    // Initialize the Habitats based on the counter array
    private void CreateInitialPercentage(double oceanPer, int[] habitatCounters)
    {
        for (int i = 0; i < habitatCounters.Length; i++)
        {
            // Debug.Log(habitatCounters[i] + ", " + oceanPer);
            typePercents[i].setPercentage(Math.Round((habitatCounters[i] / 20.0) * (1.0 - oceanPer), 2));
            // Debug.Log(i + " - " + typePercents[i]);
        }
        typePercents[13].setPercentage(oceanPer);
    }


    // determine the dominant type
    private string CheckDominantType()
    {
        return Subhabitat.IndexToString(getDominantIndex());
    }


    // get the index for the dominant habitat
    public int getDominantIndex()
    {
        if (getOceanPercents() != 1.0)
        {
            int maxIndex = 0;
            for (int i = 0; i < typePercents.Length - 1; i++)
            {
                if (typePercents[i].getPercentage() > typePercents[maxIndex].getPercentage())
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
        return typePercents[13].getPercentage();
    }


    private int getTrees(string type)
    {
        int sum = 0;
        for (int i = 0; i < typePercents.Length; i++)
        {
            sum += typePercents[i].getTrees(type);
        }
        return sum;
    }


    private double getSeeds()
    {
        double sum = 0;
        for (int i = 0; i < typePercents.Length; i++)
        {
            sum += typePercents[i].getSeeds();
        }
        return sum;
    }


    private double getFoilage(Days[] days)
    {
        double sum = 0;
        for (int i = 0; i < typePercents.Length; i++)
        {
            sum += typePercents[i].getFoilage(days);
        }
        return sum;
    }


    // Return a string with the habitat stats in it
    public override string ToString()
    {
        string data = "Habitats: ";
        for (int i = 0; i < typePercents.Length; i++)
        {
            if (typePercents[i].getPercentage() != 0.0)
            {
                data += "\n" + typePercents[i].getPercentage() * 100.0 + "% " + Subhabitat.IndexToString(i);
            }
        }
        return data;
    }


    public string printLifeInfo(Days[] days)
    {
        string life = "Plants: \nPine Trees: " + getTrees("pine") + "\tOaks: " + getTrees("oaks") + "\tTropical: " + getTrees("tropical");
        life += "\nSeeds for Year: " + getSeeds() + "\tFoilage for Year: " + getFoilage(days);
        return life;
    }

}
