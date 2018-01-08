

public class OakTree : Tree {

    public OakTree(double regenerationRate) : base(regenerationRate)
    {
        type = "oaks";
        seedConstant = 2.023809524 * 0.003020609;
    }

    public override double getTreeFoilage(double percentage, int quality, bool swamp, Days[] days)
    {
        double foilage = 0.0;
        if (swamp)
        {
            foilage = getTreesOnTile(percentage, quality, swamp) * FORESTLEAVESCONSTANT * SWAMPLEAFAGE;
        }
        else
        {
            foilage = getTreesOnTile(percentage, quality, swamp) * FORESTLEAVESCONSTANT;
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
