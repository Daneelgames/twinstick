using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CutSceneController : MonoBehaviour {

    public bool playOnStartOfScene = false;
    public Animator csAnim;
    public List<GameObject> cameraAnchors;
    public List<Animator> actorsAnimators;

    public Stateful stateful;

    void Awake()
    {
        csAnim.SetBool("Active", false);
    }

    public void StartCs()
    {
        GameManager.instance.CutScenePlay(true);
        csAnim.SetBool("Active", true);
    }

    public void Fade(string fade) // Black, Game
    {
        GameManager.instance.gui.Fade(fade);
    }

    public void StopCs()
    {
        GameManager.instance.gui.Fade("Game");
        GameManager.instance.CutScenePlay(false);

        GameManager.instance.camAnim.transform.position = GameManager.instance.cameraHolder.transform.position;
        GameManager.instance.camAnim.transform.SetParent(GameManager.instance.cameraHolder.transform);
        GameManager.instance.camAnim.transform.rotation = Quaternion.identity;


        stateful.ObjectActive(false);
        gameObject.SetActive(false);
    }

    public void SetCameraPosition(int i)
    {
        GameManager.instance.camAnim.transform.position = cameraAnchors[i].transform.position;
        GameManager.instance.camAnim.transform.rotation = cameraAnchors[i].transform.rotation;
        GameManager.instance.camAnim.transform.SetParent(cameraAnchors[i].transform);
    }

    public void SetPhrase(string text)
    {
        GameManager.instance.dialogText.text = text;
        if (text.Length > 0)
            GameManager.instance.dialogAnimator.SetTrigger("Active");
        else
            GameManager.instance.dialogAnimator.SetTrigger("Inactive");
    }

    //public void SetAnimatorBool(int actor, string boolName, bool active)
    public void SetAnimatorBool(AnimationEvent animationEvent)
    {
        bool active = false;
        if (animationEvent.floatParameter > 0.5)
            active = true;

        actorsAnimators[animationEvent.intParameter].SetBool(animationEvent.stringParameter, active);
    }

    public void SetAnimatorTrigger(AnimationEvent animationEvent)
    {
        actorsAnimators[animationEvent.intParameter].SetTrigger(animationEvent.stringParameter);
    }
}