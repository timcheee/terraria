using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour
{

    public float maxSpeed = 10f;
    bool facingRight = true;

    Animator anim;

    bool grounded = false;
    public Transform groundCheck;
    float groundRadius = 0.2f;
    public LayerMask whatIsGround;
    public float jumpForce = 700f;

    public Inventory2 inventory;
    public Inventory2 quickbar;
    public Inventory2 chest;
    public GameObject invRef;
    public GameObject slotRef;
    GameObject[] slots;

    public GameObject[] blocks = new GameObject[10];

    Item kramp;
    void Start()
    {
        anim = GetComponent<Animator>();
        
    }

    void FixedUpdate()
    {

        grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
        anim.SetBool("Ground", grounded);

        anim.SetFloat("vSpeed", GetComponent<Rigidbody2D>().velocity.y);


        float move = Input.GetAxis("Horizontal");

        anim.SetFloat("Speed", Mathf.Abs(move));
        GetComponent<Rigidbody2D>().velocity = new Vector2(move * maxSpeed, GetComponent<Rigidbody2D>().velocity.y);

        if (move > 0 && !facingRight)
            Flip();
        else if (move < 0 && facingRight)
            Flip();
    }

    float mineTime = 0.4f;
    bool startedMining = false;
    bool toggleInventory = false;

    float cooldown = 0f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!toggleInventory)
                ShowInventory();
            else
                HideInventory();
            toggleInventory = !toggleInventory;
        }

        if (grounded && Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetBool("Ground", false);
            GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpForce));
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            kramp = new Item();
            kramp.type = ItemType.PICKAXE;
            kramp.stackSize = 1;
            kramp.group = ItemGroup.TOOL;
            kramp.spriteNeutral = Resources.Load<Sprite>("pickAxeBlue");
            kramp.spriteHighlighted = Resources.Load<Sprite>("pickAxeBlue");
            quickbar.AddItem(kramp);
            
        }

        if (Input.GetMouseButton(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (quickbar.selectedSlot != null && !quickbar.selectedSlot.IsEmpty)
            {
                if (quickbar.selectedSlot.CurrentItem.group == ItemGroup.PLACABLE)
                {
                    Vector3 placePos = new Vector3(Mathf.Round(mousePos.x), Mathf.Round(mousePos.y), 0f);
                    float distance = Vector2.Distance(placePos, transform.position);

                    if (distance < 4)
                    {
                        if (Physics2D.OverlapCircleAll(placePos, 0.25f).Length == 0)
                        {
                            GameObject currentblock = new GameObject();
                            foreach (GameObject block in blocks)
                            {
                                if (block != null)
                                {
                                    if (block.GetComponent<Item>().type == quickbar.selectedSlot.CurrentItem.type)
                                        currentblock = block;                                    
                                }
                            }

                            GameObject newBlock = Instantiate(currentblock, placePos, Quaternion.identity) as GameObject;
                            newBlock.GetComponent<Item>().SetHealthToBlocks();
                            if (!quickbar.selectedSlot.IsEmpty)
                                quickbar.selectedSlot.UseItem();
                        }
                    }

                }
                else if (quickbar.selectedSlot.CurrentItem.group == ItemGroup.TOOL)
                {
                    RaycastHit2D hit2d = Physics2D.Raycast(mousePos, Vector2.zero);
                    

                    if (hit2d.collider != null && hit2d.collider.gameObject.tag == "Blocks")
                    {
                        
                        if (cooldown <= 0)
                        {
                            float distance = Vector2.Distance(hit2d.collider.gameObject.transform.position, transform.position);

                            GameObject Target = hit2d.collider.gameObject;
                            if (Target.tag != "Player" && distance <= 3)
                            {
                                anim.SetBool("mouseDown", true);
                                hit2d.collider.gameObject.GetComponent<Item>().blockHealth -= 3;
                                    Debug.Log(hit2d.collider.gameObject.GetComponent<Item>().blockHealth);
                                    if (hit2d.collider.gameObject.GetComponent<Item>().blockHealth <= 0)
                                    {
                                        inventory.AddItem(Target.GetComponent<Item>());
                                        Destroy(hit2d.collider.gameObject);
                                    }
                                cooldown = 0.7f;
                                
                            }
                        }
                        else
                            cooldown -= Time.deltaTime;       
                    }               
                }

            }

        }
        else if(Input.GetMouseButtonDown(1))
        {
            //Odpiranje chesta
            Debug.Log("Desn klik");
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit2d = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit2d.collider != null)
            {
                Debug.Log("v collider != null");
                if (hit2d.transform.gameObject.name == "Chest")
                {
                    slots = GameObject.FindGameObjectsWithTag("ChestSlot");
                    chest.GetComponent<RectTransform>().localScale = new Vector2(1f, 1f);
                    foreach (GameObject slot in slots)
                    {
                        slot.GetComponent<RectTransform>().localScale = new Vector2(1f, 1f);
                    }
                }
            }

             
        }
        else
        {
            anim.SetBool("mouseDown", false);
        }

        if (cooldown > 0)
            cooldown -= Time.deltaTime;
    }

    public void ShowInventory()
    {
        if (gameObject.tag != "Chest")
        {
            slots = GameObject.FindGameObjectsWithTag("Slot");
            invRef.GetComponent<RectTransform>().localScale = new Vector2(1f, 1f);
            foreach (GameObject slot in slots)
            {
                slot.GetComponent<RectTransform>().localScale = new Vector2(1f, 1f);
            }
        }
        else
        {

        }
    }

    public void HideInventory()
    {
        if (gameObject.tag != "Chest")
        {
            slots = GameObject.FindGameObjectsWithTag("Slot");
            invRef.GetComponent<RectTransform>().localScale = new Vector2(0f, 0f);
            foreach (GameObject slot in slots)
            {
                slot.GetComponent<RectTransform>().localScale = new Vector2(0f, 0f);
            }
        }
    }


    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Item")
        {
            inventory.AddItem(other.GetComponent<Item>());
        }
    }

}
