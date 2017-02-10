using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InventoryControllerGUI : MonoBehaviour {

    public bool active = false;
    float coolDownMax = 0.15f;
    float coolDown = 0f;

    public Animator anim;
    bool horAxisInUse = false;
    bool verAxisInUse = false;

    public int cursorAt = 0;
    public int cursotAtGlobal = 0;
    int oldOffset = 0;
    public GameObject cursor;
    public List<Image> slotsImages = new List<Image>();
    public List<Image> equipFeedback = new List<Image>();

    public Text itemNameText;
    public Text itemDescriptionText;
    public Text painkillersAmountText;
    public Text revolverBulletsAmountText;
    public List<GameObject> arrows;
    public List<string> names = new List<string>();
    public List<string> descriptions = new List<string>();

    void Update()
    {
        if (coolDown > 0)
        {
            coolDown -= Time.unscaledDeltaTime;
        }
        if (active && coolDown <= 0)
        {
            if (!horAxisInUse)
            {
                if (Input.GetAxisRaw("Horizontal") > 0)
                {
                    print ("timescale is " + Time.timeScale + "; hor > 0");
                    horAxisInUse = true;
                        coolDown = coolDownMax;
                    MoveCursor("Right");
                }
                else if (Input.GetAxisRaw("Horizontal") < 0)
                {
                    print ("timescale is " + Time.timeScale + "; hor < 0");
                    horAxisInUse = true;
                        coolDown = coolDownMax;
                    MoveCursor("Left");
                }
            }
            else if (Input.GetAxisRaw("Horizontal") == 0)
            {
                horAxisInUse = false;
            }

            if (Input.GetButtonDown("Submit"))
            {
                foreach (string j in GameManager.instance.weapons)
                {
                    if (StateManager.instance.questItems.Count > 0 && j == StateManager.instance.questItems[cursotAtGlobal])
                    {
                        if (GameManager.instance.activeWeapon == j)
                        {
                            GameManager.instance.SetActiveWeapon("");
                        }
                        else
                        {
                            GameManager.instance.SetActiveWeapon(j);
                        }
                        UpdateItems(oldOffset);
                        coolDown = coolDownMax;
                        break;
                    }
                }
            }

            /*
            if (Vector2.Distance(cursor.transform.position, slotsImages[cursorAt].transform.position) > 120)
            {
                float posX = Mathf.Lerp(cursor.transform.position.x, slotsImages[cursorAt].transform.position.x, 0.5f);
                cursor.transform.position = new Vector2(posX, cursor.transform.position.y);
            }
            */
        }
    }

    public void ToggleInventory()
    {
        if (coolDown <= 0)
        {
            coolDown = coolDownMax;

            if (active)
            {
                anim.SetBool("InventoryActive", false);
                active = false;
                Time.timeScale = 1f;
            }
            else
            {
                if (Time.timeScale == 1)
                {
                    anim.SetBool("InventoryActive", true);
                    active = true;
                    Time.timeScale = 0f;
                    UpdateItems(0);

                    GameManager.instance.gui.SetHealth();

                    cursor.transform.position = slotsImages[0].transform.position;

                    cursorAt = 0;
                    cursotAtGlobal = 0;
                    painkillersAmountText.text = StateManager.instance.painkillers + "";
                    revolverBulletsAmountText.text = StateManager.instance.revolverBullets + "";
                }
            }
        }
    }

    void UpdateItems(int offset)
    {
        oldOffset = offset;
        bool haveItems = false; 
        for (int i = 0; i < 9; i ++)
        {
            if (StateManager.instance.questItems.Count > i)
            {
                slotsImages[i].color = Color.white;

                for (int j = 0; j < GameManager.instance.inventoryItems.items.Count; j++)
                {
                    if (StateManager.instance.questItems[i + offset] == GameManager.instance.inventoryItems.items[j].name) // FOUND NEEDED ITEM
                    {
                        if (StateManager.instance.questItems[i + offset] == GameManager.instance.activeWeapon)
                        {
                            equipFeedback[i].color = Color.white;
                        }
                        else
                            equipFeedback[i].color = Color.clear;

                        slotsImages[i].sprite = GameManager.instance.inventoryItems.items[j].itemSprite;
                        names[i] = GameManager.instance.inventoryItems.items[j].itemName;
                        descriptions[i] = GameManager.instance.inventoryItems.items[j].itemDescription;
                        haveItems = true;
                        break;
                    }
                }
            }
            else
            {
                slotsImages[i].color = Color.clear;
                equipFeedback[i].color = Color.clear;
            }
        }
        if (!haveItems)
        {
            for(int i = 0; i < names.Count; i ++)
            {
                names[i] = "";
            }
            
            for(int i = 0; i < descriptions.Count; i ++)
            {
                descriptions[i] = "";
            }
        }

        SetText();

        if (offset > 0)
        {
            arrows[0].gameObject.SetActive(true);
        }
        else
        {
            arrows[0].gameObject.SetActive(false);
        }

        if (cursotAtGlobal < StateManager.instance.questItems.Count - 1)
        {
            arrows[1].gameObject.SetActive(true);
        }
        else
        {
            arrows[1].gameObject.SetActive(false);
        }

    }

    void MoveCursor(string direction)
    {
        switch (direction)
        {
            case "Right":
                if (cursorAt < 8 && cursotAtGlobal < StateManager.instance.questItems.Count - 1)
                {
                    cursorAt += 1;
                    cursotAtGlobal += 1;
                }
                else if (cursotAtGlobal < StateManager.instance.questItems.Count - 1)
                {
                    cursotAtGlobal += 1;
                    // update
                    UpdateItems(Mathf.Abs(cursotAtGlobal - cursorAt));
                }
                else
                {
                    cursorAt = 0;
                    cursotAtGlobal = 0;
                    // update
                    UpdateItems(Mathf.Abs(cursotAtGlobal - cursorAt));
                }
                break;

            case "Left":
                if (cursorAt > 0)
                {
                    cursorAt -= 1;
                    cursotAtGlobal -= 1;
                }
                else if (cursotAtGlobal > 0)
                {
                    cursotAtGlobal -= 1;
                    // update
                    UpdateItems(Mathf.Abs(cursotAtGlobal - cursorAt));
                }
                else if (StateManager.instance.questItems.Count > 0)
                {
                    if (StateManager.instance.questItems.Count < 10)
                    {
                        cursorAt = StateManager.instance.questItems.Count - 1;
                        cursotAtGlobal = StateManager.instance.questItems.Count - 1;
                    }
                    else
                    {
                        cursorAt = 8;
                        cursotAtGlobal = StateManager.instance.questItems.Count - 1;
                        // update
                        UpdateItems(Mathf.Abs(cursotAtGlobal - cursorAt));
                    }
                }
                break;
        }


        cursor.transform.position = slotsImages[cursorAt].transform.position;
        SetText();
    }

    void SetText()
    {
        if (names.Count > 0)
        {
            itemNameText.text = names[cursorAt];
        }
        else
        {
            itemNameText.text = "";
        }

        if (descriptions.Count > 0)
        {
            itemDescriptionText.text = descriptions[cursorAt];
        }
        else
        {
            itemDescriptionText.text = "";
        }
    }
}