using System;
using System.Collections.Generic;
using CGTool;
using Controller;
using DG.Tweening;
using GameData;
using Prefeb;
using Reference;
using UnityEngine;

namespace Resources.BattleSkills.s18
{
    public class Skill:SkillSetting.SkillBase,SkillSetting.ISkill
    {
        [SerializeField,Header("技能特效")] private Transform _skillEffect;
        [SerializeField,Header("旋转")] private  RectTransform _rotator;
        
        private int _Effect;
        public override void Init(PlayerUnit playerUnit, SkillSetting.SkillInfo skillInfo)
        {
            InitBase(playerUnit, skillInfo);
            Rotate();
        }

        private void Rotate()
        {
            _rotator.DOLocalRotate(new Vector3(0,0,360f), 5f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
            _rotator.GetComponent<SpriteRenderer>().DOFade(0.2f, 1f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
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
            int Effect = (int) SkillInfo.Params["Effect"] + Level * (int) SkillInfo.Params["EffectPerLevel"];
            _Effect = Effect;
            Params["Effect"] = Effect;
            Params["Range"] = (int) SkillInfo.Params["Range"] + Level * (int) SkillInfo.Params["RangePerLevel"];
            float scale = 0.5f + Level * 0.05f;
            transform.localScale = new Vector3(scale, scale, scale);
        }
        public override Dictionary<string, object> GetLevelParams(int level)
        {
            Dictionary<string,object> param = new Dictionary<string, object>(SkillInfo.Params);
            param["Range"] = (int) SkillInfo.Params["Range"] + level * (int) SkillInfo.Params["RangePerLevel"];
            param["Effect"] = (int) SkillInfo.Params["Effect"] + level * (int) SkillInfo.Params["EffectPerLevel"];
            return param;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerUnit playerUnit = other.GetComponent<PlayerUnit>();
            if(playerUnit==null) return;
            if(playerUnit.IsEnemy == PlayerUnit.IsEnemy) return;
            playerUnit.UnitEffects.Add("冰冻", _Effect);
            playerUnit.Attr.SPD_Addon_Rate -= _Effect;
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            PlayerUnit playerUnit = other.GetComponent<PlayerUnit>();
            if(playerUnit==null) return;
            if(playerUnit.IsEnemy == PlayerUnit.IsEnemy) return;
            if(playerUnit.UnitEffects.ContainsKey("冰冻")==false) return;
            int effect = (int) playerUnit.UnitEffects["冰冻"];
            playerUnit.UnitEffects.Remove("冰冻");
            playerUnit.Attr.SPD_Addon_Rate += effect;
        }
    }    
}

