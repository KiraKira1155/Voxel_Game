using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
public class EnumGameData
{
    public enum ItemKinds
    {
        Null,
        blockItem,
        item,
    }
    public enum ItemType
    {
        Null,
        meleeWeapon,
        projectileWeapon,
        rod,
        pickaxe,
        axe,
        shovel,
        hoe,
        projectile,

        ore
    }
    public enum Attribute
    {
        nihility,
        fire,
        heat,
        water,
        earth,
        wind,
        gravity,
        life,
        death
    }
    public enum BlockID
    {
        air,
        bedrock,
        stone,
        grass,
        sand,
        dirt,
        oakLog,
        oakPlanks,
        bricks,
        cobblestone,
        furnace,
        glass,
        coalOre,
        oakLeaf
    }
    public enum ItemID
    {
        None = 0,

        //meleeWeapon
        wooden_sword = 1,
        stone_sword,
        iron_sword,
        golden_sword,

        //pickaxe
        wooden_pickaxe = 1000,
        stone_pickaxe,
        iron_pickaxe,
        golden_pickaxe,

        //axe
        wooden_axe = 2000,
        stone_axe,
        iron_axe,
        golden_axe,

        //shovel
        wooden_shovel = 3000,
        stone_shovel,
        iron_shovel,
        golden_shovel,

        //hoe
        wooden_hoe = 4000,
        stone_hoe,
        iron_hoe,
        golden_hoe,

        //ore
        charcoal = 5000,
        coal,
        raw_iron,
        iron_ingot,
        raw_copper,
        copper_ingot,
        raw_gold,
        gold_ingot

    }
}
