using System;

public class Tree : Plant {

    protected const double FORESTLEAVESCONSTANT = 0.0019283411;  // Food units per tree per day from forest leaves at full bloom
    protected const double SWAMPLEAFAGE = 1.25;

    protected string type;
    protected double seedConstant;

    public Tree(double regenerationRate) : base(regenerationRate)
    {
    }

    public string getType()
    {
        return type;
    }

    public int getTreesOnTile(double percentage, int quality, bool swamp)
    {
        double trees = percentage * (quality * 30.0 + 500.0);
        if (swamp)
        {
            return (int) Math.Round(trees * Subhabitat.SWAMPCONSTANT, 0);
        }
        else
        {
            return (int) Math.Round(trees, 0);
        }
    }

    // subtract usage from all trees numbers.
    public double getSeeds(double percentage, int quality, int usage, bool swamp)
    {
        return (getTreesOnTile(percentage, quality, swamp) - usage) * seedConstant;
    }

    public virtual double getTreeFoilage(int day, double percentage, int quality, int todayTemp, int usage, bool swamp)
    {
        throw new InvalidCastException();
    }

}
