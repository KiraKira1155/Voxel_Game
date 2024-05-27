using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class SeedInput
{
    [SerializeField] private TMP_InputField _InputField;
    [SerializeField] private int previousInputSeed;

    public int GetInputSeed()
    {
        if(_InputField.text.Length != 0)
            return int.Parse(_InputField.text);

        return 0;
    }

    public long GetInputSeedLong()
    {
        if (_InputField.text.Length != 0)
            return long.Parse(_InputField.text);

        return 0;
    }

    public void OrganizeInputSeed()
    {
        if (_InputField.text == "00")
            _InputField.text = "0";

        if (GetInputSeedLong() > 2147483647)
            _InputField.text = previousInputSeed.ToString();
        else
            previousInputSeed = GetInputSeed();
    }
}
