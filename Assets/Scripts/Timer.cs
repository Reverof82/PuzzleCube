using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public enum Mode { stopwatch, countdown };
    Mode clockMode;
    TimeSpan time;
    double startTime = 60;
    int numMinutes = 5;

    public bool start;

    // Use this for initialization
    void Start()
    {
        StartStopTimer = false;
        ClockMode = Mode.countdown;
        ChangeMode();
    }

    // Update is called once per frame
    void Update()
    {
        if (StartStopTimer)
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
        if(ClockMode == Mode.countdown && startTime <= 0)
        {
            StartStopTimer = false;
            GetComponent<Text>().text = "<color=#ff0000><b>OUTATIME</b></color>";
        }
    }

    string FormatTime()
    {
        string timeText = string.Format("{0:00}:{1:00}:{2:00}:{3:000}", time.Hours, time.Minutes, time.Seconds, time.Milliseconds);
        string fullText = "<color=#00ff00><b>" + timeText + "</b></color>";
        return fullText;
    }

    void ChangeMode()
    {
        if (ClockMode == Mode.countdown)
        {
            if(numMinutes >= 1)
            {
                startTime = startTime * numMinutes;
            }
            time = TimeSpan.FromSeconds(startTime);
            GetComponent<Text>().text = FormatTime();
        }
    }

    public bool StartStopTimer
    {
        get { return start; }
        set { start = value; }
    }

    public Mode ClockMode
    {
        get { return clockMode; }
        set { clockMode = value; }
    }
}
