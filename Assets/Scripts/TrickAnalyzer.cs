using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum eRiderStance
{
    Regular,
    Goofy
}

public enum eStanceType
{
    Regular,
    Nollie,
    Fakie,
    Switch
}

public class TrickAnalyzer
{
    #region Sector
    private class Sector
    {
        public Vector2 Bounds;

        public Sector(int x, int y)
        {
            Bounds = new Vector2(x, y);
        }

        public bool Contains(int angle)
        {
            if (angle >= Bounds.x && angle < Bounds.y)
            {
                return true;
            }

            return false;
        }
    }
    #endregion

    #region Trick
    public class Trick
    {
        public string Name;
        public int[] RequiredInput;

        public Trick(string name, int[] inputs)
        {
            Name = name;
            RequiredInput = inputs;
        }

        public bool Validate(List<int> inputs)
        {
            //Debug.LogFormat("---- Validating {0}, required length: {1} -> input count: {2}", Name, RequiredInput.Length, inputs.Count);
            for (int i = 0; i < RequiredInput.Length; ++i)
            {
                if (inputs.Count < RequiredInput.Length)
                {
                    return false;
                }

                if (inputs[i] != RequiredInput[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
    #endregion

    private const float Threshold = 0.8f;
    private const int MaxInputs = 10;
    private const float AllowedTime = 0.5f;

    private eRiderStance _currentStance = eRiderStance.Regular;
    private eStanceType _currentStanceType = eStanceType.Regular;
    private Vector2 _startVector = new Vector2(-1, 0);
    private Vector2 _inputVector = new Vector2();
    private int _lastInput = -1;
    private int _currentInput;
    private List<int> _inputList;
    private float _lastInputTime = 0f;
    private List<Sector> _sectors = new List<Sector>()
    {
        // 10 sectors, each 36 degrees apart
        new Sector(0, 36),
        new Sector(36, 72),
        new Sector(72, 108),
        new Sector(108, 144),
        new Sector(144, 180),
        new Sector(180, 216),
        new Sector(216, 252),
        new Sector(252, 288),
        new Sector(288, 324),
        new Sector(324, 360)
    };

    #region Trick Lists
    private List<Trick> _currentTrickSet;

    private List<Trick> _regTricks;
    private List<Trick> _nollieTricks;
    #endregion

    public TrickAnalyzer()
    {
        _inputList = new List<int>(MaxInputs);

        _regTricks = new List<Trick>();
        _regTricks.Add(new Trick("Ollie", new int[] { 2, 7 }));

        if (_currentStance == eRiderStance.Regular)
        {
            _regTricks.Add(new Trick("Kickflip", new int[] { 2, 8 }));
            _regTricks.Add(new Trick("Heelflip", new int[] { 2, 6 }));
            _regTricks.Add(new Trick("BS Pop Shove", new int[] { 2, 1, 0 }));
            _regTricks.Add(new Trick("FS Pop Shove", new int[] { 2, 3, 4 }));
            _regTricks.Add(new Trick("Varial Kickflip", new int[] { 3, 2, 8 }));
            _regTricks.Add(new Trick("Varial Heelflip", new int[] { 1, 2, 6 }));
            _regTricks.Add(new Trick("Hardflip", new int[] { 1, 2, 3, 8 }));
            _regTricks.Add(new Trick("Inward Heelflip", new int[] { 3, 2, 1, 6 }));
            _regTricks.Add(new Trick("360 Flip", new int[] { 4, 3, 2, 8 }));
            // laser
            // 360 hard
            // 360 inward
        }
        else
        {
            // trick inputs are reversed for goofy
            _regTricks.Add(new Trick("Kickflip", new int[] { 2, 6 }));
            _regTricks.Add(new Trick("Heelflip", new int[] { 2, 8 }));
        }

        _nollieTricks = new List<Trick>();
        _nollieTricks.Add(new Trick("Nollie", new int[] { 7, 2 }));
    }

    public void UpdateStance(eStanceType stance)
    {
        _currentStanceType = stance;
    }

    public void Analyze(float h, float v)
    {
        if ((h > Threshold || h < -Threshold) || (v > Threshold || v < -Threshold))
        {
            _inputVector.x = h;
            _inputVector.y = v;

            // check stick angle
            int angle = (int)Vector2.Angle(_startVector, _inputVector);
            Vector3 cross = Vector3.Cross(_startVector, _inputVector);
            if (cross.z < 0)
            {
                angle = 360 - angle;
            }

            // check if the angle is in a valid input position
            for (int i = 0; i < _sectors.Count; ++i)
            {
                if (_sectors[i].Contains(angle))
                {
                    _currentInput = i;
                    if (_lastInput != _currentInput)
                    {
                        RecordInput(_currentInput);
                        _lastInputTime = Time.time;
                        _lastInput = _currentInput;
                    }
                }
            }
        }

        // if there is no valid input in the allowed time, clear the inputs
        if (_inputList.Count > 0 && Time.time - _lastInputTime > AllowedTime)
        {
            ClearInput();
        }
    }

    private void RecordInput(int input)
    {
        _inputList.Add(input);
        Debug.LogWarningFormat("=> {0}", input);

        // trick sequences should be long enough
        if (_inputList.Count > 1)
        {
            ValidateTrick();
        }
    }

    private void ClearInput()
    {
        _inputList.Clear();
        _lastInput = -1;
        Debug.Log(">> inputs cleared");
    }

    private void ValidateTrick()
    {
        _currentTrickSet = _regTricks;
        if (_currentStanceType == eStanceType.Regular)
        {
            _currentTrickSet = _regTricks;
        }
        else if (_currentStanceType == eStanceType.Nollie)
        {
            _currentTrickSet = _nollieTricks;
        }

        // run through and check tricks from the set
        for (int i = 0; i < _currentTrickSet.Count; ++i)
        {
            if (_currentTrickSet[i].Validate(_inputList))
            {
                Debug.LogErrorFormat("{0} {1}", _currentStanceType.ToString(), _currentTrickSet[i].Name);
                ClearInput();
                return;
            }
        }
    }
}
