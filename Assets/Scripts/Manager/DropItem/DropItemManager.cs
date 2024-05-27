using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItemManager : Singleton<DropItemManager>
{
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private List<DropEntityBlock> dropBlockItems = new List<DropEntityBlock>();

    private void Awake()
    {
        if (!init)
            Init();
    }

    public void DoFixedUpDate()
    {
        int length = dropBlockItems.Count;
        for (int i = length - 1; i >= 0; i--)
        {
            dropBlockItems[i].DoFixedUpDate();
            if (dropBlockItems[i].pickUp)
            {
                dropBlockItems[i].IsDestroy(dropBlockItems[i].gameObject);
                dropBlockItems.RemoveAt(i);
            }
        }

    }
    public void DropWhenDestroyBlock(EnumGameData.BlockID destroyBlockID, Vector3 destructionPos) 
    {
        Vector2 rndPos;
        System.Random rnd = new System.Random();
        rndPos.x = rnd.Next(20000, 50000);
        rndPos.y = rnd.Next(20000, 50000);
        rndPos /= 100000;

        GameObject dropItem = Instantiate(blockPrefab, transform);
        dropItem.GetComponent<DropEntityBlock>().Init(destroyBlockID);
        dropItem.GetComponent<DropEntityBlock>().blockMesh.Init(dropItem, destroyBlockID);
        dropItem.transform.position = destructionPos + new Vector3(rndPos.x, 0.35f, rndPos.y);
        dropBlockItems.Add(dropItem.GetComponent<DropEntityBlock>());
    }
}
