using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IItemData
{
    string name();
    Texture texture();
    int stackMaxSize();
    EnumGameData.ItemKinds kinds();
}
