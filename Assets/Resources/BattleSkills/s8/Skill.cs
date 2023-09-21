using System.Collections.Generic;
using CGTool;
using Controller;
using DG.Tweening;
using GameData;
using Prefeb;
using Reference;
using UnityEngine;

namespace Resources.BattleSkills.s8
{
    public class Skill:SkillSetting.SkillBase,SkillSetting.ISkill
    {
        public override void Init(PlayerUnit playerUnit, SkillSetting.SkillInfo skillInfo)
        {
            InitBase(playerUnit, skillInfo);
            Rush();
        }

        public void Rush()
        {
            float time = Random.Range(5f, 20f);
            DOTween.Sequence().SetDelay(time).onComplete = () =>
            {
                PlayerUnit.Speed = 300;
                DOTween.Sequence().SetDelay(Level*1).onComplete = () =>
                {
                    PlayerUnit.Speed = PlayerUnit.Attr.SPD;
                    Rush();
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

