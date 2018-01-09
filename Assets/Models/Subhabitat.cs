using System;
using System.Collections.Generic;

public class Subhabitat
{

    public const double SWAMPCONSTANT = .8;
    private const double TROPFOILAGECONSTANT = 1.5 * 0.002314009;
    private const double MONSOONFORESTLEAFAGE = 1.5;
    private const double RAINFORESTLEAFAGE = 2.0;
    private const double SWAMPLEAFAGE = 1.25;
    public const double TROPICALEAFGROWTH = 1.2;
    public const double ARTICLEAFGROWTH = 0.8;

    private int index;
    private double percentage;
    private double usage;
    private int quality;

    private Tree tree;
    private Grass grass;

    private static Dictionary<int, string> indexToStringMap;
    private static Dictionary<string, int> stringToIndexMap;

    public Subhabitat(int index)
    {
        this.index = index;
        quality = 50;
        initializeIndexSpecificTraits();
    }

    private void initializeIndexSpecificTraits()
    {
        switch (index)
        {
            case 1:
                // dry tundra
                grass = new Grass(0.0);
                break;
            case 2:
                // tundra
                grass = new Grass(0.0);
                break;
            case 3:
                // boreal
                tree = new PineTree(0.0);
                break;
            case 4:
                // artic marsh
                tree = new PineTree(0.0);
                break;
            case 5:
                // desert
                grass = new Grass(0.0);
                break;
            case 6:
                // plains
                grass = new Grass(0.0);
                break;
            case 7:
                // forest
                tree = new OakTree(0.0);
                break;
            case 8:
                // swamp
                tree = new OakTree(0.0);
                break;
            case 9:
                // hot desert
                grass = new Grass(0.0);
                break;
            case 10:
                // savannah
                grass = new Grass(0.0);
                break;
            case 11:
                // monsoon forest
                tree = new TropicalTree(0.0);
                break;
            case 12:
                // rainforest
                tree = new TropicalTree(0.0);
                break;
        }
    }

    // Getters and Setters

    public double getPercentage()
    {
        return percentage;
    }

    public double getUsage()
    {
        return usage;
    }

    public void setPercentage(double percentage)
    {
        this.percentage = percentage;
    }

    // Get Trees

    public int getTrees(string type)
    {
        int trees = 0;
        if (tree != null && tree.getType() == type)
        {
            trees = tree.getTreesOnTile(percentage, quality, index % 4 == 0);
        }

        return trees - (int)usage;
    }

    // Get Vegetation Daily values

    public double getGrazing(int day, Days[] days)
    {
        double grazing = 0.0;
        if (grass != null)
        {
            double last5Rain = Last5DaysOfRain(day, days);
            double grassMass = grass.getGrass(index % 4 == 1, quality, percentage, last5Rain, days[day].temp) - usage;
            grazing += grassMass * Grass.GRASSCALORIECONTENT;
        }

        return grazing;
    }

    public double getSeeds(int day, Days[] days)
    {
        double seeds = 0.0;
        if (tree != null)
        {
            seeds += tree.getSeeds(percentage, quality, (int) usage, index % 4 == 0);
        }
        if (grass != null)
        {
            double last5Rain = Last5DaysOfRain(day, days);
            seeds += grass.getSeeds(index % 4 == 1, quality, percentage, last5Rain, days[day].temp, usage);
        }
        return seeds;
    }

    public double getFoilage(int day, Days[] days)
    {
        double foilage = 0.0;
        if (tree != null)
        {
            foilage += tree.getTreeFoilage(day, percentage, quality, days[day].temp, (int) usage,index % 4 == 0);
        }
        return foilage;
    }

    // Get Vegetation Yearly values

    public double getYearOfSeeds(Days[] days)
    {
        double sum = 0.0;
        for (int d = 0; d < days.Length; d++)
        {
            sum += getSeeds(d, days);
        }
        return sum;
    }

    public double getYearOfFoilage(Days[] days)
    {
        double sum = 0.0;
        for (int d = 0; d < days.Length; d++)
        {
            sum += getFoilage(d, days);
        }
        return sum;
    }

    public double getYearOfGrazing(Days[] days)
    {
        double sum = 0.0;
        for (int d = 0; d < days.Length; d++)
        {
            sum += getGrazing(d, days);
        }
        return sum;
    }

    // METHODS

    public double Last5DaysOfRain(int day, Days[] days)
    {
        return 0.0;
    }

    // INDEXING

    public static int StringToIndex(string name)
    {
        if (stringToIndexMap == null)
        {
            initializeIndexMap();
        }

        return stringToIndexMap[name];
    }

    public static string IndexToString(int index)
    {
        if (indexToStringMap == null)
        {
            initializeIndexMap();
        }

        return indexToStringMap[index];
    }

    private static void initializeIndexMap()
    {
        indexToStringMap = new Dictionary<int, string>();
        stringToIndexMap = new Dictionary<string, int>();
        addToBothDictionaries(0, "glacier");
        addToBothDictionaries(1, "dry tundra");
        addToBothDictionaries(2, "tundra");
        addToBothDictionaries(3, "boreal");
        addToBothDictionaries(4, "artic marsh");
        addToBothDictionaries(5, "desert");
        addToBothDictionaries(6, "plains");
        addToBothDictionaries(7, "forest");
        addToBothDictionaries(8, "swamp");
        addToBothDictionaries(9, "hot desert");
        addToBothDictionaries(10, "savannah");
        addToBothDictionaries(11, "monsoon forest");
        addToBothDictionaries(12, "rainforest");
        addToBothDictionaries(13, "ocean");
    }

    private static void addToBothDictionaries(int index, string habitat)
    {
        indexToStringMap.Add(index, habitat);
        stringToIndexMap.Add(habitat, index);
    }

}