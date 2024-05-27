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
        if (TitleManager.I.CheckForButton("TitleButton") && TitleManager.I.CheckForObject(gameEndButton))
            EndGame();

        if (TitleManager.I.CheckForButton("TitleButton") && TitleManager.I.CheckForObject(returnButton))
            TitleManager.I.SetMenu(TitleManager.Menu.title);
    }

    private void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//�Q�[���v���C�I��
#else
    Application.Quit();//�Q�[���v���C�I��
#endif
    }
}
