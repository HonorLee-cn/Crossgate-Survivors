using System.Collections.Generic;
using CGTool;
using Controller;
using DG.Tweening;
using GameData;
using Prefeb;
using Reference;
using UnityEngine;

namespace Resources.BattleSkills.s17
{
    public class Skill:SkillSetting.SkillBase,SkillSetting.ISkill
    {
        public override void Init(PlayerUnit playerUnit, SkillSetting.SkillInfo skillInfo)
        {
            InitBase(playerUnit, skillInfo);
        }
        
        public override void CastSkill()
        {

        }
        
        public override void OnCDComplate()
        {
            
        }

        public override void LevelUp()
        {
            Level++;
            
            int percent = (int) SkillInfo.Params["Percent"] + Level * (int) SkillInfo.Params["PercentRatePerLevel"];
            int diff = percent - (int) SkillInfo.Params["Percent"];
            Params["Percent"] = percent;
            PlayerUnit.Attr.DodgeCD_Addon_Rate += diff;
            if (Level % 5 == 0)
            {
                PlayerUnit.Attr.MaxDodge += 1;
                PlayerUnit.Attr.Dodge += 1;
            }

        }
        public override Dictionary<string, object> GetLevelParams(int level)
        {
            Dictionary<string,object> param = new Dictionary<string, object>(SkillInfo.Params);
            param["Percent"] = (int) SkillInfo.Params["Percent"] + level * (int) SkillInfo.Params["PercentRatePerLevel"];
            return param;
        }
    }    
}

