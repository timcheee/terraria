using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class Slot : MonoBehaviour, IPointerClickHandler
{

    private Stack<Item> items;
    public Text stackText;

    public Sprite slotEmpty;
    public Sprite slotHightlight;

    public bool IsEmpty
    {
        get { return Items.Count == 0; }
    }

    public bool IsAvailable //če je še plac v stacku
    {
        get { return CurrentItem.stackSize > Items.Count; }
    }

    public Item CurrentItem
    {
        get { return Items.Peek(); }
    }

    public Stack<Item> Items
    {
        get { return items; }
        set { items = value; }
    }

    void Start()
    {
        Items = new Stack<Item>();
        RectTransform slotRect = GetComponent<RectTransform>();
        RectTransform txtRect = stackText.GetComponent<RectTransform>();

        int textScaleFactor = (int)(slotRect.sizeDelta.x * 0.40);
        stackText.resizeTextMaxSize = textScaleFactor;
        stackText.resizeTextMinSize = textScaleFactor;

        txtRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, slotRect.sizeDelta.y);
        txtRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, slotRect.sizeDelta.x);
    }

    void Update()
    {

    }

    public void AddItem(Item item)
    {
        Items.Push(item);

        if (Items.Count > 1)
        {
            stackText.text = Items.Count.ToString();
        }
        ChangeSprite(item.spriteNeutral, item.spriteHighlighted);
    }
    public void AddItems(Stack<Item> items)
    {
        this.Items = new Stack<Item>(items);
        stackText.text = items.Count > 1 ? items.Count.ToString() : string.Empty;

        ChangeSprite(CurrentItem.spriteNeutral, CurrentItem.spriteHighlighted);
    }

    private void ChangeSprite(Sprite neutral, Sprite highlight)
    {
        GetComponent<Image>().sprite = neutral;

        SpriteState st = new SpriteState();
        st.highlightedSprite = highlight;
        st.pressedSprite = neutral;

        GetComponent<Button>().spriteState = st;
    }

    public void UseItem()
    {
        if (!IsEmpty)
        {
            Items.Pop().Use();

            stackText.text = Items.Count > 1 ? Items.Count.ToString() : string.Empty;

            if (IsEmpty)
            {
                ChangeSprite(slotEmpty, slotHightlight);
                Inventory2.EmptySlots++;
            }
        }
    }

    public void ClearSlot()
    {
        items.Clear();
        ChangeSprite(slotEmpty, slotHightlight);
        stackText.text = string.Empty;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //ker button je bi klikjen
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("Onpointerclick");
            UseItem();
        }
    }
    private bool selected;

    public void OnPointerUp(PointerEventData eventData)
    {
        if (selected)
            selected = false;

        if (!gameObject.GetComponent<Slot>().IsEmpty)
        {

            Debug.Log("Slot ni empty!");
            GameObject.Find("Inventory").gameObject.GetComponent<Inventory2>().MoveItem(this.gameObject);
        }
    }
}
