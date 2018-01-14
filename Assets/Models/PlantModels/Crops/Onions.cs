
public class Onions : Crop
{

    public Onions(double regenerationRate) : base(regenerationRate)
    {
        cropName = "onions";
        minTemp = 70;
        maxTemp = 90;
        minWater = 7.4;
        maxWater = 9.9;
        growthPeriod = 51;
        humanFoodUnits = .84;
    }

}
