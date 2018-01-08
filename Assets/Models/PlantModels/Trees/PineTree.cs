
public class PineTree : Tree {

    private const double PINESEEDCONSTANT = 2.777777778 * 0.003856682;
    private const double PINENEEDLECONSTANT = .000650815125;

    public PineTree(double regenerationRate) : base(regenerationRate)
    {
        type = "pine";
        seedConstant = 2.777777778 * 0.003856682;
    }

    public override double getTreeFoilage(double percentage, int quality, bool swamp, Days[] days)
    {
        double foilage = 0.0;
        if (swamp)
        {
            foilage = getTreesOnTile(percentage, quality, swamp) * PINENEEDLECONSTANT * Subhabitat.SWAMPCONSTANT;
        }
        else
        {
            foilage = getTreesOnTile(percentage, quality, swamp) * PINENEEDLECONSTANT;
        }
        return foilage * PINENEEDLECONSTANT * Date.DAYS_PER_YEAR;
    }

}
