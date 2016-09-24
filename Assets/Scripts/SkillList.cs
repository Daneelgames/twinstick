using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillList : MonoBehaviour {

    public List<SkillController> playerSkills;

    public List<SkillController> gameSkills;

    public void AddPlayerSkill(SkillController skill)
    {
        playerSkills.Add(skill);
    }
}