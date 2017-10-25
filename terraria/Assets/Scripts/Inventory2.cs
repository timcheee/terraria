using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Inventory2 : MonoBehaviour
{

    private RectTransform inventoryRect;

    private float inventoryWidth, inventoryHeight;

    public int slots;
    public int rows;

    public float slotPaddingLeft, slotPaddingTop;

    public float slotSize;

    public GameObject iconPrefab;
    private static GameObject hoverObject;

    public GameObject slotPrefab;
    private List<GameObject> allSlots;
    private static int emptySlots;
    public Slot selectedSlot;
    private int selectedIndex = 0;

    private static Slot from, to;

    public Canvas canvas;
    private float hoverYOffset;

    public EventSystem eventSystem;


    public static int EmptySlots
    {
        get
        {
            return emptySlots;
        }

        set
        {
            emptySlots = value;
        }
    }
    //private bool moveItemCalled = false;
    void Start()
    {
        CreateLayout();
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (!eventSystem.IsPointerOverGameObject(-1) && from != null)
            {
                from.GetComponent<Image>().color = Color.white;
                from.ClearSlot();
                Destroy(GameObject.Find("Hover"));
                to = null;
                from = null;
                hoverObject = null;
            }
        }

        if (hoverObject != null)
        {
            Vector2 position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out position);
            position.Set(position.x, position.y - hoverYOffset);
            hoverObject.transform.position = canvas.transform.TransformPoint(position);
        }

        //Za selectat item z scrollerju
        if (gameObject.transform.name == "QuickBar")
        {
            if (Input.GetAxis("Mouse ScrollWheel") < 0f) //naprej
            {
                selectedIndex++;
                if (selectedIndex < 0)
                    selectedIndex = 7;
                if (selectedIndex > 7)
                    selectedIndex = 0;

                if (selectedSlot != null)
                {
                    selectedSlot.GetComponent<Image>().color = Color.white;
                }


                selectedSlot = allSlots[selectedIndex].GetComponent<Slot>();
                selectedSlot.GetComponent<Image>().color = Color.yellow;

            }

            else if (Input.GetAxis("Mouse ScrollWheel") > 0f) // nazaj
            {
                selectedIndex--;
                if (selectedIndex < 0)
                    selectedIndex = 7;
                if (selectedIndex > 7)
                    selectedIndex = 0;

                if (selectedSlot != null)
                {
                    selectedSlot.GetComponent<Image>().color = Color.white;
                }


                selectedSlot = allSlots[selectedIndex].GetComponent<Slot>();
                selectedSlot.GetComponent<Image>().color = Color.yellow;

            }

        }
    }

    private void CreateLayout()
    {
        allSlots = new List<GameObject>();

        hoverYOffset = slotSize * 0.01f;

        EmptySlots = slots;

        inventoryWidth = (slots / rows) * (slotSize + slotPaddingLeft) + slotPaddingLeft;
        inventoryHeight = rows * (slotSize + slotPaddingTop) + slotPaddingTop;

        inventoryRect = GetComponent<RectTransform>();

        inventoryRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, inventoryWidth);
        inventoryRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, inventoryHeight);

        if (inventoryRect.name != "Quickbar")
            inventoryRect.localScale = new Vector2(0f, 0f);

        int columns = slots / rows;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                GameObject newSlot = (GameObject)Instantiate(slotPrefab);
                //newSlot.transform.localScale = new Vector2(1, 1);               
                RectTransform slotRect = newSlot.GetComponent<RectTransform>();

                newSlot.name = "Slot";
                newSlot.transform.SetParent(this.transform.parent, false);

                slotRect.localPosition = inventoryRect.localPosition + new Vector3(slotPaddingLeft * (j + 1) + (slotSize * j), -slotPaddingTop * (i + 1) - (slotSize * i));

                slotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, slotSize);
                slotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, slotSize);

                if (slotRect.tag == "Slot" || slotRect.tag == "ChestSlot") // če se gre za slot v invju, ne pa v quickbaru
                    slotRect.localScale = new Vector2(0f, 0f);

                allSlots.Add(newSlot);

            }
        }
    }

    public bool AddItem(Item item)
    {
        if (item.stackSize == 1)
        {
            PlaceEmpty(item);
            return true;
        }
        else
        {
            foreach (GameObject slot in allSlots)
            {
                Slot tmp = slot.GetComponent<Slot>();
                if (!tmp.IsEmpty)
                {
                    if (tmp.CurrentItem.type == item.type && tmp.IsAvailable)
                    {
                        tmp.AddItem(item);
                        return true;
                    }
                }
            }
            if (EmptySlots > 0)
            {
                PlaceEmpty(item);
            }
        }
        return false;
    }

    private bool PlaceEmpty(Item item)
    {
        if (EmptySlots > 0)
        {
            foreach (GameObject slot in allSlots)
            {
                Slot tmp = slot.GetComponent<Slot>();

                if (tmp.IsEmpty)
                {
                    tmp.AddItem(item);
                    EmptySlots--;
                    return true;
                }
            }
        }
        return false;
    }

    public void MoveItem(GameObject clicked)
    {
        if (from == null) //če je že klikjen (če še ni, se shrani v tmp spremenljivko from)
        {
            if (!clicked.GetComponent<Slot>().IsEmpty)
            {
                from = clicked.GetComponent<Slot>();
                from.GetComponent<Image>().color = Color.gray;

                hoverObject = (GameObject)Instantiate(iconPrefab);
                hoverObject.GetComponent<Image>().sprite = clicked.GetComponent<Image>().sprite;
                hoverObject.name = "Hover";

                RectTransform hoverTransform = hoverObject.GetComponent<RectTransform>();
                RectTransform clickedTransform = clicked.GetComponent<RectTransform>();

                hoverTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, clickedTransform.sizeDelta.x);
                hoverTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, clickedTransform.sizeDelta.y);

                hoverObject.transform.SetParent(GameObject.Find("Canvas").transform, true);
                hoverObject.transform.localScale = from.gameObject.transform.localScale;

            }
        }
        else if (to == null)
        {
            to = clicked.GetComponent<Slot>();
            Destroy(GameObject.Find("Hover"));
        }
        if (to != null && from != null)
        {
            Stack<Item> tmpTo = new Stack<Item>(to.Items);
            to.AddItems(from.Items);

            if (tmpTo.Count == 0)
            {
                from.ClearSlot();
            }
            else
            {
                from.AddItems(tmpTo);
            }

            from.GetComponent<Image>().color = Color.white;
            to = null;
            from = null;
            hoverObject = null;
        }
    }
}
