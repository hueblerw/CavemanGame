
public class TropicalTree : Tree {

    private const double MONSOON_LEAFAGE = 1.5;
    private const double RAINFOREST_LEAFAGE = 2.0;

    public TropicalTree(double regenerationRate) : base(regenerationRate)
    {
        type = "tropical";
        seedConstant = Habitat.SEEDCONSTANT;
    }

    public override double getTreeFoilage(int day, double percentage, int quality, int todayTemp, int usage, bool rainforest)
    {
        double treeBiomass = 0.0;
        if (rainforest)
        {
            treeBiomass = (getTreesOnTile(percentage, quality, rainforest) - usage) * FORESTLEAVESCONSTANT * RAINFOREST_LEAFAGE;
        }
        else
        {
            treeBiomass = (getTreesOnTile(percentage, quality, rainforest) - usage) * FORESTLEAVESCONSTANT * MONSOON_LEAFAGE;
        }
        // Temperature Effect
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
