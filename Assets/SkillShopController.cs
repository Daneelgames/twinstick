using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SkillShopController : MonoBehaviour {

    public Animator anim;

    public GameObject skillSelector;
    public int activeSkillNumber = 0;
    public List<SkillController> skills;
    public List<Image> skillIcons;

    public Text skillName;
    public Text skillDescription;
    public Text skillCost;

    public GameObject buyButton;

    void Start()
    {
        SetSkills();
    }

    public void SetSkills()
    {
        List<SkillController> skillsTemp = new List<SkillController>(GameManager.instance._skillList.gameSkills);

        //print(skillsTemp.Count + "after init");

        for (int i = 0; i < 4; i++)
        {
            int random = Random.Range(0, skillsTemp.Count);
            skills.Add(skillsTemp[random]);
            skillsTemp.RemoveAt(random);

            //print(skillsTemp.Count + "after delete");
            //skillsTemp.Sort();
            //print(skillsTemp.Count + "after sort");
        }
    }

    public void ShopToggle(bool _active)
    {
        anim.SetBool("Active", _active);

        if (_active)
        {
            InvokeRepeating("CheckDistance", 0.1f, 0.1f);

            if (skills.Count > 0)
            {
                SelectSkill(Random.Range(0, skills.Count));
                SetSkillsSprites();
            }
            else
            {
                NoSkills();
            }
        }
        else
            GameManager.instance.PointerOverMenu(false);
    }
    
    void NoSkills()
    {
        skillSelector.SetActive(false);
        buyButton.SetActive(false);

        skillName.text = "";
        skillDescription.text = "NO ITEMS!";

        for (int i = 0; i < 4; i++)
        {
            skillIcons[i].enabled = false;
        }
    }

    void SetSkillsSprites()
    {
        skillSelector.SetActive(true);
        buyButton.SetActive(true);
        for (int i = 0; i < 4; i ++)
        {
            if (skills.Count > i)
            {
                skillIcons[i].enabled = true;
                skillIcons[i].sprite = skills[i].skillSprite;
            }
            else
                skillIcons[i].enabled = false;
        }
    }

    void CheckDistance()
    {
        float distance = Vector2.Distance(GameManager.instance.lastCampfire.transform.position, GameManager.instance.playerInGame.transform.position);
        //print(distance);
        if (distance > 4)
        {
            ShopToggle(false);
            CancelInvoke();
        }
    }

    public void PointerEntered(bool entered)
    {
        GameManager.instance.PointerOverMenu(entered);
    }

    public void SelectSkill(int number)
    {
        activeSkillNumber = number;
        skillSelector.transform.localPosition = skillIcons[number].gameObject.transform.localPosition;

        skillName.text = skills[activeSkillNumber].skillName;
        skillDescription.text = skills[activeSkillNumber].skillDescription;
        skillCost.text = skills[activeSkillNumber].skillCost + "$";
    }

    public void Buy()
    {
        if (GameManager.instance.playerExp >= skills[activeSkillNumber].skillCost)
        {
            GameManager.instance.RemoveExp(skills[activeSkillNumber].skillCost);
            GameManager.instance._skillList.AddPlayerSkill(skills[activeSkillNumber]);
            skills.RemoveAt(activeSkillNumber);
            ShopToggle(true);
        }
    }
}