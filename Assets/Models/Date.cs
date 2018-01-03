

public class Date {

    // Constants
    public const int DAYS_PER_YEAR = 120;

    // The Date object
    public int year;
    public int day;

    // Constructors
    public Date()
    {
        year = 1;
        day = 1;
    }

    public Date(int day, int year)
    {
        this.day = day;
        this.year = year;
    }


    // Methods
    public void NextDay()
    {
        if (day >= DAYS_PER_YEAR)
        {
            day -= (DAYS_PER_YEAR - 1);
            year++;
        }
        else
        {
            day++;
        }
    }


    public void advanceXDays(int X)
    {
        day += (X - 1);
        NextDay();
    }


    public void advanceToNextYear()
    {
        day = 1;
        year++;
    }


    public override string ToString()
    {
        return "Year: " + year + "\n" + "Day: " + day;
    }

}
