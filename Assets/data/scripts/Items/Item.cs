using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Base class for all droppable items in the game (power, lives, bombs, etc.).</summary>
public abstract class Item
{
    // Variable Declaration

    /// <summary> The type of this item.</summary>
    protected ItemType itemType;

    /// <summary> This method is fired when this item touches the player.</summary>
    public abstract void OnPlayerTouch();

    /// <summary> Use this method to get the type of this item. </summary>
    public ItemType GetItemType()
    {
        return itemType;
    }

    /// <summary> Enumeration of all items in the game, use this to define/compare the itemType variable.</summary>
    public enum ItemType
    {
        smallPower, bigPower, life, bomb, point, fullPower
    }
}