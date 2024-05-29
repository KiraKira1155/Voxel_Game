using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EndMenu
{
    [SerializeField] private GameObject gameEndButton;
    [SerializeField] private GameObject returnButton;

    public void ClickButton()
    {
        if (TitleManager.I.CheckForButton("TitleButton", gameEndButton))
            EndGame();

        if (TitleManager.I.CheckForButton("TitleButton", returnButton))
            TitleManager.I.SetMenu(TitleManager.Menu.title);
    }

    private void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
    }
}
