using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InventoryControllerGUI : MonoBehaviour {

    public bool active = false;
    float coolDownMax = 0.5f;
    float coolDown = 0f;

    public Animator anim;
    bool horAxisInUse = false;
    bool verAxisInUse = false;

    public int cursorAt = 0;
    public int cursotAtGlobal = 0;
    public GameObject cursor;
    public List<Image> slotsImages = new List<Image>();
    public List<Animator> animators = new List<Animator>();
    
    public List<Animator> arrowsAnimators = new List<Animator>();

    public Text itemNameText;
    public Text itemDescriptionText;
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
                    horAxisInUse = true;
                    MoveCursor("Right");
                }
                else if (Input.GetAxisRaw("Horizontal") < 0)
                {
                    horAxisInUse = true;
                    MoveCursor("Left");
                }
            }
            else if (Input.GetAxisRaw("Horizontal") == 0)
            {
                horAxisInUse = false;
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
                anim.SetBool("Active", false);
                active = false;
                Time.timeScale = 1f;
            }
            else
            {
                if (Time.timeScale == 1)
                {
                    anim.SetBool("Active", true);
                    active = true;
                    Time.timeScale = 0f;
                    UpdateItems(0);

                    cursor.transform.position = slotsImages[0].transform.position;
                    animators[0].SetTrigger("Shake");

                    cursorAt = 0;
                    cursotAtGlobal = 0;
                }
            }
        }
    }

    void UpdateItems(int offset)
    {
        for (int i = 0; i < 9; i ++)
        {
            if (StateManager.instance.questItems.Count > i)
            {
                slotsImages[i].color = GameManager.instance._sm.charactersColor;

                for (int j = 0; j < GameManager.instance.inventoryItems.items.Count; j++)
                {
                    if (StateManager.instance.questItems[i + offset] == GameManager.instance.inventoryItems.items[j].name) // FOUND NEEDED ITEM
                    {
                        slotsImages[i].sprite = GameManager.instance.inventoryItems.items[j].itemSprite;
                        names[i] = GameManager.instance.inventoryItems.items[j].itemName;
                        descriptions[i] = GameManager.instance.inventoryItems.items[j].itemDescription;
                        break;
                    }
                }
            }
            else
            {
                slotsImages[i].color = Color.clear;
            }
        }
        SetText();

        if (offset > 0)
        {
            arrowsAnimators[0].gameObject.SetActive(true);
            arrowsAnimators[0].SetTrigger("Shake");
        }
        else
        {
            arrowsAnimators[0].gameObject.SetActive(false);
        }

        if (cursotAtGlobal < StateManager.instance.questItems.Count - 1)
        {
            arrowsAnimators[1].gameObject.SetActive(true);
            arrowsAnimators[1].SetTrigger("Shake");
        }
        else
        {
            arrowsAnimators[1].gameObject.SetActive(false);
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
        animators[0].SetTrigger("Shake");
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