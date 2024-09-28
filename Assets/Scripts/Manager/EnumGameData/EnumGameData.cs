using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
public class EnumGameData
{
    public enum ItemKinds
    {
        Hand,
        blockItem,
        item,
    }
    public enum ItemType
    {
        Hand,
        meleeWeapon,
        projectileWeapon,
        rod,
        pickaxe,
        axe,
        shovel,
        hoe,
        projectile,

        material,
        ore,

        block
    }
    public enum WeaponAttribute
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

    public enum BlockType
    {
        cube
    }

    public enum BlockID
    {
        air,
        bedrock,
        dirt,
        grass,
        coarseDirt,
        snow,
        snowDirt,
        sand,
        redSand,
        gravel,
        stone,
        cobble,
        deepslate,
        cobbleDeepslate,
        andesite,
        granite,
        diorite,
        coalOre,
        ironOre,
        goldOre,
        diamondOre,
        redstoneOre,
        copperOre,
        emeraldOre,
        ancientDebris,
        coalBlock,
        ironBlock,
        goldBlock,
        diamondBlock,
        redstoneBlock,
        copperBlock,
        emeraldBlock,
        netheriteBlock,
        oakLog,
        oakLeaf,
        glass
    }
    public enum ItemID
    {
        None = 0,

        //meleeWeapon
        wood_sword = 1,
        stone_sword,
        iron_sword,
        gold_sword,
        diamond_sword,

        //pickaxe
        wood_pickaxe = 25,
        stone_pickaxe,
        iron_pickaxe,
        gold_pickaxe,
        diamond_pickaxe,

        //axe
        wood_axe = 50,
        stone_axe,
        iron_axe,
        gold_axe,
        diamond_axe,

        //shovel
        wood_shovel = 75,
        stone_shovel,
        iron_shovel,
        gold_shovel,
        diamond_shovel,

        //hoe
        wood_hoe = 100,
        stone_hoe,
        iron_hoe,
        gold_hoe,
        diamond_hoe,

        //projectile
        snowball = 150,

        //material
        flint = 200,

        //ore
        charcoal = 500,
        coal,
        raw_iron,
        iron_ingot,
        raw_copper,
        copper_ingot,
        raw_gold,
        gold_ingot,
        diamond,

        //block
        bedrock = 600,
        dirt,
        grass,
        coarseDirt,
        snow,
        snowDirt,
        sand,
        redSand,
        gravel,
        stone,
        cobble,
        deepslate,
        cobbleDeepslate,
        andesite,
        granite,
        diorite,
        coalOre,
        ironOre,
        goldOre,
        diamondOre,
        redstoneOre,
        copperOre,
        emeraldOre,
        ancientDebris,
        coalBlock,
        ironBlock,
        goldBlock,
        diamondBlock,
        redstoneBlock,
        copperBlock,
        emeraldBlock,
        netheriteBlock,
        log,
        leaf,
        glass
    }
}
