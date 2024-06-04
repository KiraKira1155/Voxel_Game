using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TitleManager : Singleton<TitleManager>
{
    [SerializeField] private Image titleBackGround;
    [SerializeField] private byte r, g, b;
    private PointerEventData pointerEventData;
    [SerializeField] private GraphicRaycaster raycaster = null;
    [SerializeField] private EventSystem eventSystem = null;

    [SerializeField] [ReadOnly] private Menu menu;

    [SerializeField] private GameObject titleCamera;

    [SerializeField] private GameObject titleMenuObj;
    [SerializeField] private GameObject worldSelectMenuObj;
    [SerializeField] private GameObject optionMenuObj;
    [SerializeField] private GameObject endMenuObj;

    [SerializeField] private TitleMenu titleMenu = new TitleMenu();
    [SerializeField] private WorldSelectMenu worldSelectMenu = new WorldSelectMenu();
    [SerializeField] private OptionMenu optionMenu = new OptionMenu();
    [SerializeField] private EndMenu endMenu = new EndMenu();

    [SerializeField] private TextMeshProUGUI loadTime;
    float loadTimeCnt;

    public enum Menu
    {
        none,
        title,
        worldSelect,
        option,
        end
    }

    public void SetMenu(Menu menu)
    {
        this.menu = menu;
    }

    private void Awake()
    {
        pointerEventData = new PointerEventData(eventSystem);

        Init();
    }

    public void DoStart()
    {
        menu = Menu.title;
        SelectMenu();
        StartCoroutine(ChengeColor());
    }

    public void DoUpdate()
    {
        if (KeyConfig.GetKeyDown(KeyConfig.KeyName.LeftClick))
            ClickEvent();
    }

    public void CheckInputSeed()
    {
        worldSelectMenu.OrganizeInputSeed();
    }

    private void ClickEvent()
    {
        switch (menu)
        {
            case Menu.title:
                titleMenu.ClickButton();
                break;

            case Menu.worldSelect:
                worldSelectMenu.ClickButton();
                break;

            case Menu.option:
                optionMenu.ClickButton();
                break;

            case Menu.end:
                endMenu.ClickButton();
                break;
        }
        SelectMenu();
    }

    private void SelectMenu()
    {
        ActiveFalseMenu();
        switch (menu)
        {
            case Menu.title:
                titleMenuObj.SetActive(true);
                break;

            case Menu.worldSelect:
                worldSelectMenuObj.SetActive(true);
                break;

            case Menu.option:
                optionMenuObj.SetActive(true);
                break;

            case Menu.end:
                endMenuObj.SetActive(true);
                break;
        }
    }

    private void ActiveFalseMenu()
    {
        titleMenuObj.SetActive(false);
        worldSelectMenuObj.SetActive(false);
        optionMenuObj.SetActive(false);
        endMenuObj.SetActive(false);
    }

    public void StartGame()
    {
        MyMonoBehaviour.I.SetGameScene(MyMonoBehaviour.GameScene.MainGame);
        loadTime.text = "0.0s";
        StartCoroutine(SetActiveFalse());
    }

    public void LoadCnt()
    {
        loadTimeCnt += Time.deltaTime;
        loadTime.text = loadTimeCnt.ToString() + "s";
    }

    IEnumerator SetActiveFalse()
    {
        yield return new WaitUntil(() => MyMonoBehaviour.I.successStart);
        titleCamera.SetActive(false);

        yield break;
    }

    IEnumerator ChengeColor()
    {
        r = 0;
        g = 0;
        b = 0;
        titleBackGround.color = new Color32(r, g, b, 255);
        var wait = new WaitForSeconds(0.016f);
        while (!MyMonoBehaviour.I.successStart)
        {
            if (r < 255 && b == 0)
                r++;
            else if (g < 255 && r == 255)
                g++;
            else if (b < 255 && g == 255)
                b++;
            else if (r > 0)
                r--;
            else if (g > 0)
                g--;
            else if (b > 0)
                b--;

            titleBackGround.color = new Color32(r, g, b, 255);

            yield return wait;
        }

        yield break;
    }

    public bool CheckForButton(string tagName, GameObject obj)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        pointerEventData.position = Input.mousePosition;

        raycaster.Raycast(pointerEventData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.tag == tagName && result.gameObject == obj)
                return true;
        }

        return false;
    }
}
