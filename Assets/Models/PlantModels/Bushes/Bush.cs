
public class Bush : Plant {

    protected double shrubConstant;

	public Bush(double regenerationRate) : base(regenerationRate)
    {
    }

    public double getFoilage(double percentage, bool artic, bool tropical)
    {
        if (tropical)
        {
            return percentage * shrubConstant * Subhabitat.TROPICALEAFGROWTH;
        }
        if (artic)
        {
            return percentage * shrubConstant * Subhabitat.ARTICLEAFGROWTH;
        }

        return percentage * shrubConstant;
    }

}
