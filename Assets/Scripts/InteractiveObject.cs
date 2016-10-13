using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InteractiveObject : MonoBehaviour {

    public int activeDialogIndex = 0;
    public List<Dialog> dialogues = new List<Dialog>();

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
            inDialog = true;

            StartCoroutine("SetPhrasesText");
            InvokeRepeating("CheckPlayerDistance", 0.5f, 0.5f);
        }
    }

    void CheckPlayerDistance()
    {
        float distance = Vector2.Distance(transform.position, GameManager.instance.playerInGame.transform.position);
        if (distance > 3)
        {
            GameManager.instance.dialogAnimator.SetTrigger("Inactive");
            inDialog = false;
            CancelInvoke("CheckPlayerDistance");
            StopCoroutine("SetPhrasesText");
        }
    }

    IEnumerator SetPhrasesText ()
    {
        for (int i = 0; i < dialogues[activeDialogIndex].phrases.Count; i++)
        {
            GameManager.instance.dialogText.text = dialogues[activeDialogIndex].phrases[i];
            GameManager.instance.dialogAnimator.SetTrigger("Active");
            yield return new WaitForSeconds(3f);
        }
        GameManager.instance.dialogAnimator.SetTrigger("Inactive");
        if (activeDialogIndex < dialogues.Count - 1) //end of dialog. loop last dialog
            activeDialogIndex += 1;

        yield return new WaitForSeconds(1f);
        CancelInvoke("CheckPlayerDistance");
        inDialog = false;
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "Player" && GameManager.instance.playerController.playerHealth.health > 0)
        {
            GameManager.instance.NpcToInteract(this);
        }
    }
    void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.tag == "Player" && GameManager.instance.playerController.playerHealth.health > 0)
        {
            GameManager.instance.NpcToInteract(null);
        }
    }
}
