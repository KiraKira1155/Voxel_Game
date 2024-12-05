using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBlockUI : MonoBehaviour
{
    private CalculationDrawingBlock blockUI = new CalculationDrawingBlock();
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private Material[] materials = new Material[2];

    public void DoAwake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();

        materials[0] = WorldManager.I.material;
        materials[1] = WorldManager.I.transparentMaterial;
        meshRenderer.materials = materials;

    }

    public void UpdateUI(EnumGameData.BlockID blockID)
    {
        blockUI.CubeIsometricView(meshFilter, blockID);
    }
}
