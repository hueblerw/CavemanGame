using System;

public class Grass : Plant {

    // Constants
    public const double GRASSCALORIECONTENT = .067311605;
    private const double DESERTGROWTHFACTOR = .3;

    public Grass(double regenerationRate) : base(regenerationRate)
    {
    }

    // Return the grass number for today
    public double getGrass(bool desert, int quality, double percentage, double last5Rain, double temp)
    {
        // =(1.2-ABS(70-AA7)/70)*400*SUM($AF$1:$AF$3)*(($AJ$4-50)/200+1)+(1.2-ABS(70-AA7)/70)*400*(($AJ$4-50)/200+1)*SUM($AD$1:$AD$3)*0.3*(0.5+SUM(AB7:AB11)/10)
        double tempfactor = Math.Max((1.2 - Math.Abs(70.0 - temp) / 70.0), 0.0);
        double qualityfactor = ((quality - 50.0) / 200.0 + 1.0);
        // Calculate the grass
        double grass = 0.0;
        if (!desert)
        {
            grass = tempfactor * 400 * percentage * qualityfactor;
        }
        else
        {
            grass = tempfactor * 400 * percentage * qualityfactor * DESERTGROWTHFACTOR * (.5 + (last5Rain / 10.0));
        }
        
        return grass;
    }

}
