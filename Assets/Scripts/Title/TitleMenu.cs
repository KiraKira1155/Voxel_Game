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
        if (TitleManager.I.CheckForButton("TitleButton", worldSelectButton))
            TitleManager.I.SetMenu(TitleManager.Menu.worldSelect);

        if (TitleManager.I.CheckForButton("TitleButton", optionButton))
            TitleManager.I.SetMenu(TitleManager.Menu.option);

        if (TitleManager.I.CheckForButton("TitleButton", endButton))
            TitleManager.I.SetMenu(TitleManager.Menu.end);
    }
}
