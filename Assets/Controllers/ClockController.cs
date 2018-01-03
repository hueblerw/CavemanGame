using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;


// MUST BE ATTACHED TO THE CLOCK TO FUNCTION CORRECTLY
public class ClockController : MonoBehaviour {

    private bool clockRunning;
    private float clockSpeed;

    private WorldController worldController;

	// Use this for initialization
	void Start () {
        UnityEngine.Debug.Log("Clock Initialized!");
        worldController = FindObjectOfType<WorldController>();
        clockRunning = false;
        clockSpeed = 1.0f;
	}
	
	public void ToggleTheClock()
    {
        if (clockRunning)
        {
            clockRunning = false;
            StopCoroutine(RunTheClock());
        }
        else
        {
            clockRunning = true;
            StartCoroutine(RunTheClock());
        }
    }

    public void advance1Days()
    {
        advanceWorldXDays(1);
        displayDate();
    }

    public void advance10Days()
    {
        advanceWorldXDays(10);
        displayDate();
    }

    public void advance30Days()
    {
        advanceWorldXDays(30);
        displayDate();
    }

    public void advanceYear()
    {
        advanceWorldXDays(Date.DAYS_PER_YEAR - World.getMyInstance().date.day + 1);
        displayDate();
    }

    // Advances a day everytime speed seconds if the clock is running
    IEnumerator RunTheClock()
    {
        while (clockRunning)
        {
            advance1Days();
            yield return new WaitForSeconds(clockSpeed);
        }
    }

    private void displayDate()
    {
        gameObject.GetComponent<Text>().text = World.getMyInstance().date.ToString();
    }

    private void advanceWorldXDays(int advancedDays)
    {
        Stopwatch dayTime = Stopwatch.StartNew();
        int newDayNum = World.getMyInstance().date.day + advancedDays;
        if (newDayNum > Date.DAYS_PER_YEAR)
        {
            // Create a new Year
            Stopwatch yearTime = Stopwatch.StartNew();
            newDayNum = newDayNum - Date.DAYS_PER_YEAR;
            World.getMyInstance().GenerateANewYear();
            TwoDWorldView.getInstance().UpdateHabitatTexture();
            yearTime.Stop();
            UnityEngine.Debug.Log("New Year Generation Time: " + yearTime.ElapsedMilliseconds);
        }
        // Load New Day
        World.getMyInstance().GenerateANewDay();
        World.getMyInstance().date.advanceXDays(advancedDays);
        TwoDWorldView.getInstance().UpdateRivers(newDayNum);
        dayTime.Stop();
        // UnityEngine.Debug.Log("New Day Generation Time: " + dayTime.ElapsedMilliseconds);
    }

}
