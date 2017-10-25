using UnityEngine;
using System.Collections;


public enum ItemType { MANA, HEALTH, DIRTBLOCK, STONEBLOCK, GRASSBLOCK, CHEST, COAL, WOOD, PICKAXE, AXE, SHOVEL };

public enum ItemGroup { PLACABLE, CONSUMABLE, TOOL, WEAPON, };

public class Item : MonoBehaviour
{

    public ItemType type;
    public ItemGroup group;

    public Sprite spriteNeutral;
    public Sprite spriteHighlighted;

    public int stackSize;

    public float blockHealth;

    public void Use()
    {
        switch (type)
        {
            case ItemType.MANA:
                Debug.Log("I used mana pot");
                break;
            case ItemType.HEALTH:
                Debug.Log("I used health pot");
                break;
        }
    }

    public void SetHealthToBlocks()
    {
        switch (type)
        {
            case ItemType.DIRTBLOCK:
                blockHealth = 10;
                break;

            case ItemType.GRASSBLOCK:
                blockHealth = 10;
                break;

            case ItemType.STONEBLOCK:
                blockHealth = 30;
                break;

            case ItemType.COAL:
                blockHealth = 40;
                break;

            case ItemType.WOOD:
                blockHealth = 20;
                break;
        }
    }
}
