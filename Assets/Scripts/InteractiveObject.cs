using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InteractiveObject : MonoBehaviour {

    public bool door = false;
    public string scene = "";
    public string spawner = "";

    public int activeDialogIndex = 0;
    public int activePhraseIndex = 0;
    public List<Dialog> dialogues = new List<Dialog>();

    public float phraseCooldown = 0f;

    public bool inDialog = false;

    [System.Serializable]
    public class Dialog
    {
        public List<string> phrases;
    }

    public void Talk()
    {
        if (!inDialog)
        {
            if (!door)
            {
                if (dialogues.Count > activeDialogIndex && dialogues[activeDialogIndex].phrases.Count > 0)
                {
                    inDialog = true;
                    activePhraseIndex = 0;
                    SetPhrase();
                    GameManager.instance.NpcToInteract(null, "Inspect");
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
        GameManager.instance.gui.Fade("ToBlack");
        yield return new WaitForSecondsRealtime(1f);
        GameManager.instance.MoveToNewScene(scene, spawner);
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
            Time.timeScale = 1;
            GameManager.instance.dialogAnimator.SetTrigger("Inactive");

            if (activeDialogIndex < dialogues.Count - 1) //loop last dialog
                activeDialogIndex += 1;

            inDialog = false;

            if (door)
                StartCoroutine("ExitDoor");
        }
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
        if (coll.gameObject.tag == "Player" && GameManager.instance.playerController.playerHealth.health > 0)
        {
            if (!door)
                GameManager.instance.NpcToInteract(this, "Inspect");
            else
                GameManager.instance.NpcToInteract(this, "Door");
        }
    }
    void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.tag == "Player" && GameManager.instance.playerController.playerHealth.health > 0)
        {
            GameManager.instance.NpcToInteract(null, "Inspect");
        }
    }
}