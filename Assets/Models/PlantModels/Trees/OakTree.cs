

public class OakTree : Tree {

    public OakTree(double regenerationRate) : base(regenerationRate)
    {
        type = "oaks";
        seedConstant = 2.023809524 * 0.003020609;
    }

    public override double getTreeFoilage(int day, double percentage, int quality, int todayTemp, int usage, bool swamp)
    {
        double treeBiomass = 0.0;
        if (swamp)
        {
            treeBiomass = (getTreesOnTile(percentage, quality, swamp) - usage) * FORESTLEAVESCONSTANT * SWAMPLEAFAGE;
        }
        else
        {
            treeBiomass = (getTreesOnTile(percentage, quality, swamp) - usage) * FORESTLEAVESCONSTANT;
        }
        if (todayTemp > 50)
            return treeBiomass;
        else
        {
            if (todayTemp > 30)
            {
                return ((todayTemp - 30.0) / 20.0) * treeBiomass;
            }
        }
        return treeBiomass;
    }

}
