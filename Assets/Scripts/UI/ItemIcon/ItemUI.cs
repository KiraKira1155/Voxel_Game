using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUI : MonoBehaviour
{
    private CalculationDrawingItem itemUI = new CalculationDrawingItem();
    [SerializeField] private Renderer _renderer;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private readonly int mainTex = Shader.PropertyToID("_MainTex");

    public void DoAwake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = _renderer.material;
    }

    public void UpdateUI(EnumGameData.ItemType itemType, EnumGameData.ItemID itemID)
    {
        meshRenderer.material.SetTexture(mainTex, ItemManager.I.GetTexture(itemType, itemID));
        itemUI.ItemUI(meshFilter);
    }
}
