
public class Carrot : Crop
{

    public Carrot(double regenerationRate) : base(regenerationRate)
    {
        cropName = "carrots";
        minTemp = 65;
        maxTemp = 85;
        minWater = 7.4;
        maxWater = 9.9;
        growthPeriod = 39;
        humanFoodUnits = .77;
    }

}
