using System.Collections.Generic;
using CGTool;
using Controller;
using DG.Tweening;
using GameData;
using Prefeb;
using Reference;
using UnityEngine;

namespace Resources.BattleSkills.s9
{
    public class Skill:SkillSetting.SkillBase,SkillSetting.ISkill
    {
        public override void Init(PlayerUnit playerUnit, SkillSetting.SkillInfo skillInfo)
        {
            InitBase(playerUnit, skillInfo);
            Dash();
        }
        
        public void Dash()
        {
            float time = Random.Range(5f, 20f);
            DOTween.Sequence().SetDelay(time).onComplete = () =>
            {
                PlayerUnit.Dash();
                DOTween.Sequence().SetDelay(Level*1).onComplete = () =>
                {
                    Dash();
                };
                    
            };
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

        }
        public override Dictionary<string, object> GetLevelParams(int level)
        {
            Dictionary<string,object> param = new Dictionary<string, object>(SkillInfo.Params);
            return param;
        }
    }    
}

