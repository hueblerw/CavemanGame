
public class TropicalTree : Tree {

    private const double MONSOON_LEAFAGE = 1.5;
    private const double RAINFOREST_LEAFAGE = 2.0;

    public TropicalTree(double regenerationRate) : base(regenerationRate)
    {
        type = "tropical";
        seedConstant = Habitat.SEEDCONSTANT;
    }

    public override double getTreeFoilage(double percentage, int quality, bool rainforest, Days[] days)
    {
        double foilage = 0.0;
        if (rainforest)
        {
            foilage = getTreesOnTile(percentage, quality, rainforest) * FORESTLEAVESCONSTANT * RAINFOREST_LEAFAGE;
        }
        else
        {
            foilage = getTreesOnTile(percentage, quality, rainforest) * FORESTLEAVESCONSTANT * MONSOON_LEAFAGE;
        }
        // Temperature Effect
        double sum = 0.0;
        int todayTemp;
        for (int d = 0; d < Date.DAYS_PER_YEAR; d++)
        {
            todayTemp = days[d].temp;
            if (todayTemp > 50)
                sum += foilage;
            else
            {
                if (todayTemp > 30)
                {
                    sum += ((todayTemp - 30.0) / 20.0) * foilage;
                }
            }
        }
        return sum;
    }

}
