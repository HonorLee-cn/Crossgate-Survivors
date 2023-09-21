using System.Collections.Generic;
using CGTool;
using Controller;
using DG.Tweening;
using GameData;
using Prefeb;
using Reference;
using UnityEngine;

namespace Resources.BattleSkills.s10
{
    public class Skill:SkillSetting.SkillBase,SkillSetting.ISkill
    {
        public override void Init(PlayerUnit playerUnit, SkillSetting.SkillInfo skillInfo)
        {
            InitBase(playerUnit, skillInfo);
            Defence();
        }
        
        public void Defence()
        {
            float time = Random.Range(5f, 20f);
            DOTween.Sequence().SetDelay(time).onComplete = () =>
            {
                if(PlayerUnit.IsDead) return;
                PlayerUnit.IsWalkable = false;
                PlayerUnit.AnimePlayer.changeActionType(Anime.ActionType.Defence);
                int def = PlayerUnit.Attr.DEF;
                int addDef = (int) (def / 2 * Level * 1.2);
                PlayerUnit.Attr.DEF += addDef;
                PlayerUnit.Rigidbody.velocity = Vector3.zero;
                float mass = PlayerUnit.Rigidbody.mass;
                PlayerUnit.Rigidbody.mass = 10000;
                DOTween.Sequence().SetDelay(Level*2).onComplete = () =>
                {
                    if(PlayerUnit.IsDead) return;
                    PlayerUnit.AnimePlayer.changeActionType(Anime.ActionType.Walk);
                    PlayerUnit.IsWalkable = true;
                    PlayerUnit.Attr.DEF -= addDef;
                    PlayerUnit.Rigidbody.mass = mass;
                    Defence();
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

