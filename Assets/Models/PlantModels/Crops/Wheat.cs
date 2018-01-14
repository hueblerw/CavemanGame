
public class Wheat : Crop {

	public Wheat(double regenerationRate) : base(regenerationRate)
    {
        cropName = "wheat";
        minTemp = 75;
        maxTemp = 95;
        minWater = 4.9;
        maxWater = 7.9;
        growthPeriod = 78;
        humanFoodUnits = 2.52;
    }

}
