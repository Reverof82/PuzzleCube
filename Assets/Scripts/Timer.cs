using UnityEngine;
using System;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public enum Mode { stopwatch, countdown };
    Mode clockMode;
    TimeSpan time;
    TimeSpan warningTime = TimeSpan.FromSeconds(30);
    TimeSpan endingTime = TimeSpan.FromSeconds(5);
    double startTime = 0;
    int numDays = 1;
    int numHours = 0;
    int numMinutes = 1;
    int numSeconds = 30;
    public Color normalColor;
    public Color warningColor;
    public Color endingColor;
    public bool changeMode = false;

    public bool start = false;

    // Use this for initialization
    void Start()
    {
        ClockMode = Mode.stopwatch;
        SetTime();
    }

    // Update is called once per frame
    void Update()
    {
        if (changeMode)
        {
            changeMode = false;
            ChangeMode();
        }
        if (StartTimer)
        {
            if (ClockMode == Mode.stopwatch)
            {
                time += TimeSpan.FromSeconds(Time.deltaTime);
            }
            else
            {
                startTime -= Time.deltaTime;
                time = TimeSpan.FromSeconds(startTime);
            }
            GetComponent<Text>().text = FormatTime();
        }
        if (ClockMode == Mode.countdown && startTime <= 0)
        {
            StartTimer = false;
            GetComponent<Text>().text = "<b>OUTATIME</b>";
        }
    }

    string FormatTime()
    {
        SetTimerColor();
        string timeText;
        
        if (NumDays >= 1)
        {
            timeText = string.Format("{0:00}:{1:00}:{2:00}:{3:00}:{4:000}", time.Days, time.Hours, time.Minutes, time.Seconds, time.Milliseconds);
        }
        else if (NumHours >= 1)
        {
            timeText = string.Format("{0:00}:{1:00}:{2:00}:{3:000}", time.Hours, time.Minutes, time.Seconds, time.Milliseconds);
        }
        else if (NumMinutes >= 1)
        {
            timeText = string.Format("{0:00}:{1:00}:{2:000}", time.Minutes, time.Seconds, time.Milliseconds);
        }
        else
        {
            timeText = string.Format("{0:00}:{1:000}", time.Seconds, time.Milliseconds);
        }
        return "<b>" + timeText + "</b>";
    }

    void SetTimerColor()
    {
        Color color = normalColor;
        if (ClockMode == Mode.countdown)
        {
            if (time <= warningTime)
            {
                color = warningColor;
            }
            if (time <= endingTime)
            {
                color = endingColor;
            }
        }
        GetComponent<Text>().color = color;
    }

    void ChangeMode()
    {
        if (ClockMode == Mode.countdown)
        {
            ClockMode = Mode.stopwatch;
            SetTime();
        }
        else if (ClockMode == Mode.stopwatch)
        {
            ClockMode = Mode.countdown;
            SetTime(NumDays, NumHours, NumMinutes, NumSeconds);
        }
    }

    void SetTime()
    {
        StartTimer = false;
        if (ClockMode == Mode.stopwatch)
        {
            startTime = 0;
            NumDays = 0;
            NumHours = 0;
            NumMinutes = 0;
            NumSeconds = 0;
            time = TimeSpan.FromSeconds(startTime);
            GetComponent<Text>().text = FormatTime();
        }
    }
    
    void SetTime(int numDays, int numHours, int numMinutes, int numSeconds)
    {
        StartTimer = false;
        if (ClockMode == Mode.countdown)
        {
            if(numDays >= 1)
            {
                NumDays = numDays * 86400;
            }
            if(numHours >= 1)
            {
                NumHours = numHours * 3600;
            }
            if (numMinutes >= 1)
            {
                NumMinutes = numMinutes * 60;
            }
            startTime = NumDays + NumHours + NumMinutes + NumSeconds;
            time = TimeSpan.FromSeconds(startTime);
            GetComponent<Text>().text = FormatTime();
        }
    }

    public bool StartTimer
    {
        get { return start; }
        set { start = value; }
    }

    public Mode ClockMode
    {
        get { return clockMode; }
        set { clockMode = value; }
    }

    public int NumDays
    {
        get { return numDays; }
        set { numDays = value; }
    }

    public int NumHours
    {
        get { return numHours; }
        set { numHours = value; }
    }

    public int NumMinutes
    {
        get { return numMinutes; }
        set { numMinutes = value; }
    }

    public int NumSeconds
    {
        get { return numSeconds; }
        set { numSeconds = value; }
    }
}
