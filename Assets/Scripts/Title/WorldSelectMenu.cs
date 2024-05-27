using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldSelectMenu
{
    [SerializeField] private GameObject generateWorldButton;
    [SerializeField] private GameObject returnButton;
    [SerializeField] private GameObject seedInputObj;
    [SerializeField] private SeedInput seedInput = new SeedInput();

    public void ClickButton()
    {
        if (TitleManager.I.CheckForButton("TitleButton") && TitleManager.I.CheckForObject(generateWorldButton))
        {
            MyMonoBehaviour.I.configManager.SetSeed(seedInput.GetInputSeed());
            TitleManager.I.StartGame();
        }

        if (TitleManager.I.CheckForButton("TitleButton") && TitleManager.I.CheckForObject(returnButton))
            TitleManager.I.SetMenu(TitleManager.Menu.title);
    }

    public void OrganizeInputSeed()
    {
        seedInput.OrganizeInputSeed();
    }
}
