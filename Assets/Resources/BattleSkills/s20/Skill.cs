using System.Collections.Generic;
using CGTool;
using Controller;
using DG.Tweening;
using GameData;
using Prefeb;
using Reference;
using UnityEngine;

namespace Resources.BattleSkills.s20
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
            int range = (int) SkillInfo.Params["Range"] + Level * (int) SkillInfo.Params["RangePerLevel"];
            Params["Range"] = range;
            float scale = 1f + range / 100f;
            GlobalReference.BattleData.PlayerUnit.LootRange.transform.localScale = new Vector3(scale, scale, scale); 

        }
        public override Dictionary<string, object> GetLevelParams(int level)
        {
            Dictionary<string,object> param = new Dictionary<string, object>(SkillInfo.Params);
            param["Range"] =  (int) SkillInfo.Params["Range"] + level * (int) SkillInfo.Params["RangePerLevel"];
            return param;
        }
    }    
}

