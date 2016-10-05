using UnityEngine;
using System.Collections;

public class SkillController : MonoBehaviour {

    public enum Type {HellRoller, FatSkin, ExtraFeet, Psycho, Baseball, Regen};

    public string skillName;
    public string skillDescription;
    public Sprite skillSprite;
    public int skillCost = 0;

    public Type skill = Type.HellRoller;

    public void StartEffect()
    {
        switch (skill)
        {
            case Type.HellRoller: // invisible roll + cooldown
                GameManager.instance.playerController.playerRoll.SetRollInvisible(true);
                break;

            case Type.FatSkin: // +4 max health
                GameManager.instance.playerController.playerHealth.AddToMaxHealth(4);
                break;

            case Type.ExtraFeet: // +25% speed
                float playerSpeed = GameManager.instance.playerController.speed;
                GameManager.instance.playerController.SetSpeed(playerSpeed + playerSpeed * 0.25f);
                break;

            case Type.Psycho: // -10% weapon cooldown
                GameManager.instance.playerController.SetWeaponCooldownPercentageBonus(25);
                break;

            case Type.Baseball: // melee attack makes bullets bounce
                GameManager.instance.playerController.SetMeleeBounce(true);
                break;

            case Type.Regen: // Slowly regenerate health. 1/30sec
                GameManager.instance.playerController.SetRegen(true);
                break;
        }
    }   

    public void EndEffect()
    {
        switch (skill)
        {
            case Type.HellRoller: // invisible roll + cooldown
                GameManager.instance.playerController.playerRoll.SetRollInvisible(false);
                break;

            case Type.FatSkin: // +4 max health
                GameManager.instance.playerController.playerHealth.AddToMaxHealth(-4);
                break;

            case Type.ExtraFeet: // +25% speed
                float playerSpeed = GameManager.instance.playerController.speed;
                GameManager.instance.playerController.SetSpeed(playerSpeed - playerSpeed * 0.25f);
                break;

            case Type.Psycho: // -10% weapon cooldown
                GameManager.instance.playerController.SetWeaponCooldownPercentageBonus(0);
                break;

            case Type.Baseball: // melee attack makes bullets bounce
                GameManager.instance.playerController.SetMeleeBounce(false);
                break;

            case Type.Regen: // regenerate health 1/30sec
                GameManager.instance.playerController.SetRegen(false);
                break;
        }
    }
}