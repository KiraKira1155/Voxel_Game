using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : Singleton<TitleManager>
{
    [SerializeField] private Image titleBackGround;
    [SerializeField] private byte r, g, b;

    private void Awake()
    {
        Init();
    }

    private void OnEnable()
    {
        r = 0;
        g = 0;
        b = 0;
        titleBackGround.color = new Color32(r, g, b, 255);
    }

    public void DoStart()
    {
        StartCoroutine(ChengeColor());
    }

    public void DoUpdate()
    {
        if (KeyConfig.GetKeyDown(KeyConfig.KeyName.RightClick))
        {
            MyMonoBehaviour.I.SetGameScene(MyMonoBehaviour.GameScene.MainGame);
            StartCoroutine(SetActiveFalse());
        }
    }

    IEnumerator SetActiveFalse()
    {
        yield return new WaitUntil(() => MyMonoBehaviour.I.successStart);
        gameObject.SetActive(false);

        yield break;
    }

    IEnumerator ChengeColor()
    {
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
}
