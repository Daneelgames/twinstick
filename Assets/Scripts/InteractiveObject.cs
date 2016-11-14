﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InteractiveObject : MonoBehaviour {

    public bool locker = false; // 0th dialog = idle; 1th dialog = opened
    public string keyName = "";
    public GameObject objToActivate;

    public bool dropItem = false;
    public string dropName = "";
    public string inventoryDescription = "";
    public Sprite inventoryImg;

    public bool savePoint = false;
    public CampfireController localSpawner;

    public bool door = false;
    public bool passage = false;
    public string scene = "";
    public string spawner = "";

    public int activeDialogIndex = 0;
    public int activePhraseIndex = 0;
    public List<Dialog> dialogues = new List<Dialog>();

    public float phraseCooldown = 0f;

    public bool inDialog = false;
    public Stateful stateful;

    public bool canInteract = false;

    [System.Serializable]
    public class Dialog
    {
        public List<string> phrases;
    }

    public void Talk()
    {
        if (!inDialog)
        {
            if (!door && !passage)
            {
                if (canInteract && dialogues.Count > activeDialogIndex && dialogues[activeDialogIndex].phrases.Count > 0)
                {
                    if (locker)
                    {
                        if (StateManager.instance.HaveItem(keyName))
                        {
                            activeDialogIndex = 1;
                        }
                        else
                            activeDialogIndex = 0;
                    }

                    inDialog = true;
                    activePhraseIndex = 0;
                    SetPhrase();
                    canInteract = false;
                    //GameManager.instance.NpcToInteract(null, "");
                }
            }
            else
            {
                StartCoroutine("ExitDoor");
            }
        }
    }

    IEnumerator ExitDoor()
    {
        Time.timeScale = 0f;
        // screen fade
        GameManager.instance.gui.Fade("Black");
        yield return new WaitForSecondsRealtime(1f);
        GameManager.instance.MoveToNewScene(scene, spawner);
    }
    
    IEnumerator Save()
    {
        Time.timeScale = 0f;
        GameManager.instance.SaveAnimatorBooleans();
        GameManager.instance.gui.Save();
        GameManager.instance.SetStartCampfire(localSpawner);
        StateManager.instance.GameSave();
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 1f;
    }

    void SetPhrase()
    {
        if (activePhraseIndex < dialogues[activeDialogIndex].phrases.Count)
        {
            phraseCooldown = 1f;
            Time.timeScale = 0;
            GameManager.instance.dialogText.text = dialogues[activeDialogIndex].phrases[activePhraseIndex];
            GameManager.instance.dialogAnimator.SetTrigger("Active");
        }
        else //end of dialog
        {
            GameManager.instance.dialogAnimator.SetTrigger("Inactive");

            if (activeDialogIndex < dialogues.Count - 1 && !locker) //loop last dialog
                activeDialogIndex += 1;

            if (stateful != null)
                stateful.InteractiveObjectSetActiveDialog(activeDialogIndex);

            inDialog = false;

            if (door)
                StartCoroutine("ExitDoor");

            if (savePoint)
                StartCoroutine("Save");

            if (!door && !savePoint)
                Time.timeScale = 1;
            

            if (dropItem)
            {
                GameManager.instance.NpcToInteract(null, "");
                StateManager.instance.AddItem(dropName);

                stateful.ObjectActive(false);

                gameObject.SetActive(false);
            }
            else if (locker && activeDialogIndex == 1) // door opened
            {
                GameManager.instance.NpcToInteract(null, "");
                if (objToActivate)
                    objToActivate.SetActive(true);

                StateManager.instance.RemoveItem(keyName);

                stateful.ObjectActive(false);

                gameObject.SetActive(false);
            }
            else
            {
                StartCoroutine("CanInteractAfterTime");
            }
        }
    }

    IEnumerator CanInteractAfterTime()
    {
        yield return new WaitForSeconds(0.5f);
        canInteract = true;
    }

    void Update()
    {
        if (inDialog)
        { 
            if (phraseCooldown > 0)
            {
                phraseCooldown -= Time.unscaledTime;
            }
            else
            {
                if(Input.GetButtonDown("Submit")) // update phrase
                {
                    activePhraseIndex += 1;
                    SetPhrase();
                }
            }
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        if (!passage)
        {
            if (coll.gameObject.tag == "Player" && GameManager.instance.playerController.playerHealth.health > 0)
            {
                if (door)
                    GameManager.instance.NpcToInteract(this, "Door");
                else if (savePoint)
                    GameManager.instance.NpcToInteract(this, "Save");
                else
                    GameManager.instance.NpcToInteract(this, "Inspect");

                canInteract = true;
            }
        }
        else if (coll.gameObject.tag == "Player")
        {
            Talk(); // move to next scene
        }
    }
    void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.tag == "Player" && GameManager.instance.playerController.playerHealth.health > 0)
        {
            canInteract = false;
            GameManager.instance.NpcToInteract(null, "");
        }
    }

    public void SetActiveDialog(int index)
    {
        activeDialogIndex = index;
    }
}