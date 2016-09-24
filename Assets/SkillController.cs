using UnityEngine;
using System.Collections;

public class SkillController : MonoBehaviour {

    public enum Type {BlackMuscle, BloodLust, BoilingVeins, BoneMarrow, EagleEyes, Euphoria, ExtraFeet, GammaGuts};

    public string skillName;
    public string skillDescription;
    public Sprite skillSprite;
    public int skillCost = 0;

    public Type skill = Type.BlackMuscle;

    
}