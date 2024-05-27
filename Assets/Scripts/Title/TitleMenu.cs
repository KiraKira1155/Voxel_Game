using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TitleMenu
{
    [SerializeField] private GameObject worldSelectButton;
    [SerializeField] private GameObject optionButton;
    [SerializeField] private GameObject endButton;


    public void ClickButton()
    {
        if (TitleManager.I.CheckForButton("TitleButton") && TitleManager.I.CheckForObject(worldSelectButton))
            TitleManager.I.SetMenu(TitleManager.Menu.worldSelect);

        if (TitleManager.I.CheckForButton("TitleButton") && TitleManager.I.CheckForObject(optionButton))
            TitleManager.I.SetMenu(TitleManager.Menu.option);

        if (TitleManager.I.CheckForButton("TitleButton") && TitleManager.I.CheckForObject(endButton))
            TitleManager.I.SetMenu(TitleManager.Menu.end);
    }
}
