using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OptionMenu
{
    [SerializeField] private GameObject returnButton;

    public void ClickButton()
    {
        if (TitleManager.I.CheckForButton("TitleButton", returnButton))
            TitleManager.I.SetMenu(TitleManager.Menu.title);
    }
}
