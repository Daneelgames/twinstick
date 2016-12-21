using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InteractiveObject : MonoBehaviour {

    public bool locker = false; // 0th dialog = idle; 1th dialog = opened
    public string keyName = "";
    public GameObject objToActivate;

    public DigitPuzzle dgtPuzzle;

    public bool dropItem = false;
    public List<string> dropNames = new List<string>();
    //public string inventoryDescription = "";
    //public Sprite inventoryImg;

    public bool savePoint = false;
    public CampfireController localSpawner;

    public bool door = false;
    public bool passage = false;
    public string scene = "";
    public string spawner = "";
    public GameObject hint;

    public int activeDialogIndex = 0;
    public int activePhraseIndex = 0;
    public List<Dialog> dialogues = new List<Dialog>();

    public int messageDialog = 0;
    public MessageTransmitter messageTransmitter;

    public float phraseCooldown = 0f;

    public bool inDialog = false;
    public Stateful stateful;

    public bool canInteract = false;

    public GameObject cameraAnchor;
    public GameObject light;

    public bool camFade = false;
    
    [System.Serializable]
    public class Dialog
    {
        public List<string> phrases;
    }

    public void Talk()
    {
        if (!inDialog)
        {
            if (dgtPuzzle)
            {
                inDialog = true;
                StartCoroutine("StartPuzzle");
            }
            else if (!door && !passage)
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
                    if (cameraAnchor)
                        StartCoroutine("SetCamera");
                    else
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
        else if (dgtPuzzle)
        {
            if (!dgtPuzzle.complete)
            StartCoroutine("PuzzleOver", 0f);
        }
    }

    IEnumerator StartPuzzle()
    {
        print ("start puzzle");
        camFade = true;
        Time.timeScale = 0;
        GameManager.instance.gui.Fade("Black");
        yield return new WaitForSecondsRealtime(1f);
		if (light)
		{
			light.SetActive(true);
		}
        if(hint)
            hint.SetActive(false);
        GameManager.instance.CutScenePlay(true);
        GameManager.instance.camAnim.transform.position = cameraAnchor.transform.position;
        GameManager.instance.camAnim.transform.rotation = cameraAnchor.transform.rotation;
        GameManager.instance.camAnim.transform.SetParent(cameraAnchor.transform);
        GameManager.instance.gui.Fade("Game");
        dgtPuzzle.StartPuzzle(this);
        yield return new WaitForSecondsRealtime(1f);
        camFade = false;
    }

    public void PuzzleComplete()
    {
        StartCoroutine("PuzzleOver", 1f);
    }
    IEnumerator PuzzleOver(float t)
    {
        yield return new WaitForSecondsRealtime(t);
        GameManager.instance.gui.Fade("Black");
        camFade = true;
        yield return new WaitForSecondsRealtime(1f);

        Time.timeScale = 1;
        dgtPuzzle.PuzzleOver();

		if (light)
		{
			light.SetActive(false);
		}
        if(hint)
            hint.SetActive(true);

        GameManager.instance.CutScenePlay(false);
        inDialog = false;
        GameManager.instance.camAnim.transform.position = GameManager.instance.cameraHolder.transform.position;
        GameManager.instance.camAnim.transform.SetParent(GameManager.instance.cameraHolder.transform);
        GameManager.instance.camAnim.transform.rotation = Quaternion.identity;
        GameManager.instance.gui.Fade("Game");
        camFade = false;

        if (objToActivate)
        {
            objToActivate.SetActive(true);
        }

        if (dgtPuzzle.complete)
            {
                stateful.ObjectActive(false);
                gameObject.SetActive(false);
            }
    }
    IEnumerator SetCamera()
    {
        camFade = true;
        Time.timeScale = 0;
        GameManager.instance.gui.Fade("Black");
        yield return new WaitForSecondsRealtime(1f);

		if (light)
		{
			light.SetActive(true);
		}

        GameManager.instance.CutScenePlay(true);
        
        if(hint)
            hint.SetActive(false);

        GameManager.instance.camAnim.transform.position = cameraAnchor.transform.position;
        GameManager.instance.camAnim.transform.rotation = cameraAnchor.transform.rotation;
        GameManager.instance.camAnim.transform.SetParent(cameraAnchor.transform);
        GameManager.instance.gui.Fade("Game");
        yield return new WaitForSecondsRealtime(1f);
        camFade = false;
        SetPhrase();
    }


    IEnumerator ResetCamera(bool active)
    {
        GameManager.instance.gui.Fade("Black");
        yield return new WaitForSecondsRealtime(1f);

        if(hint)
            hint.SetActive(true);

		if (light)
		{
			light.SetActive(false);
		}
        
        GameManager.instance.CutScenePlay(false);

        GameManager.instance.camAnim.transform.position = GameManager.instance.cameraHolder.transform.position;
        GameManager.instance.camAnim.transform.SetParent(GameManager.instance.cameraHolder.transform);
        GameManager.instance.camAnim.transform.rotation = Quaternion.identity;
        //GameManager.instance.gui.Fade("Game");
        SendMessage(true);
        if (!active)
        {
            stateful.ObjectActive(false);
            gameObject.SetActive(false);
        }
        //yield return new WaitForSecondsRealtime(1f);
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
            phraseCooldown = 0.5f;
            Time.timeScale = 0;
            GameManager.instance.dialogText.text = dialogues[activeDialogIndex].phrases[activePhraseIndex];
            GameManager.instance.dialogAnimator.SetTrigger("Active");
        }
        else //end of dialog
        {
            GameManager.instance.dialogAnimator.SetTrigger("Inactive");
            /*
            if (messageDialog == activeDialogIndex && messageTransmitter)
            {
                messageTransmitter.SendMessage();
            }
            */

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
            

            if (locker && activeDialogIndex == 1) // door opened
            {
                GameManager.instance.NpcToInteract(null, "");

                if (objToActivate)
                    objToActivate.SetActive(true);

                StateManager.instance.RemoveItem(keyName);
                
                 if (dropItem)
                    {
                        foreach (string i in dropNames)
                        {
                            StateManager.instance.AddItem(i);
                        }
                    }
                if (cameraAnchor)
                    StartCoroutine("ResetCamera", false);
                else
                {
                    SendMessage(false);
                    stateful.ObjectActive(false);
                    gameObject.SetActive(false);
                    if (hint)
                        hint.SetActive(true);
                    return;
                }
            }
            else if (dropItem && !locker)
            {
                GameManager.instance.NpcToInteract(null, "");
                foreach (string i in dropNames)
                {
                    StateManager.instance.AddItem(i);
                }
                
                if (objToActivate)
                    objToActivate.SetActive(true);

                if (cameraAnchor)
                    StartCoroutine("ResetCamera", false);
                else
                {
                    SendMessage(false);
                    stateful.ObjectActive(false);
                    gameObject.SetActive(false);
                    if (hint)
                        hint.SetActive(true);
                    return;
                }
            }
            else
            {
                StartCoroutine("CanInteractAfterTime");
            }
        }
    }

    void SendMessage(bool needToFadeIn)
    {
        //print ("try to send message from interactive. needToFadeIn == " + needToFadeIn + "; messageDialog == " + messageDialog + "; active dialog index == " + activeDialogIndex + "message transm is " + messageTransmitter);
        if (messageDialog == activeDialogIndex && messageTransmitter)
        {
            //print ("send message");
            messageTransmitter.SendMessage(needToFadeIn);
        }
        else if (needToFadeIn)
        {
            GameManager.instance.gui.Fade("Game");
        }
    }

    IEnumerator CanInteractAfterTime()
    {
        if (cameraAnchor)
        {
            GameManager.instance.gui.Fade("Black");
            yield return new WaitForSecondsRealtime(1f);
            if (hint)
                hint.SetActive(true);
            GameManager.instance.camAnim.transform.position = GameManager.instance.cameraHolder.transform.position;
            GameManager.instance.camAnim.transform.SetParent(GameManager.instance.cameraHolder.transform);
            GameManager.instance.camAnim.transform.rotation = Quaternion.identity;
            GameManager.instance.CutScenePlay(false);
            GameManager.instance.gui.Fade("Game");
            yield return new WaitForSecondsRealtime(1f);
            SendMessage(false);
        }
        else
        {
            SendMessage(false);
            if (hint)
                hint.SetActive(true);
            yield return new WaitForSeconds(0.5f);
        }

        if (!inDialog)
            canInteract = true;
    }

    void Update()
    {
        if (inDialog && !camFade)
        { 
            if (phraseCooldown > 0)
            {
                phraseCooldown -= Time.unscaledDeltaTime;
            }
            else if (!dgtPuzzle || dgtPuzzle.complete)
            {
                if(Input.GetButtonDown("Submit")) // update phrase
                {
                    activePhraseIndex += 1;
                    phraseCooldown = 0.5f;
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