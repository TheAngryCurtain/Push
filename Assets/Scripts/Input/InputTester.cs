using UnityEngine;
using System.Collections;
using GameInput;
using System;

public class InputTester : MonoBehaviour, IControllable
{
    private TrickAnalyzer _analyzer;

    void Start()
    {
        InputController.Instance.SetPlayerControllable(0, this);

        _analyzer = new TrickAnalyzer();
    }

    public void HandleAxis(Axis a, float h, float v)
    {
        //Debug.LogFormat("Axis: {0}, h: {1}, v: {2}", a.ToString(), h, v);

        if (a == Axis.RStick)
        {
            _analyzer.Analyze(h, v);
        }
    }

    public void HandleButtonHeld(Button b, float duration)
    {
        //Debug.LogFormat("Holding {0} ({1})", b.ToString(), duration);
    }

    public void HandleButtonPress(Button b)
    {
        //Debug.LogFormat("Pressing {0}", b.ToString());
    }

    public void HandleButtonRelease(Button b)
    {
        //Debug.LogFormat("Releasing {0}", b.ToString());
    }
}
