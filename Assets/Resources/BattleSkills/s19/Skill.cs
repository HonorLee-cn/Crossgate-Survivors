using System;
using System.Collections.Generic;
using CGTool;
using Controller;
using DG.Tweening;
using GameData;
using Prefeb;
using Reference;
using UnityEngine;

namespace Resources.BattleSkills.s19
{
    public class Skill:SkillSetting.SkillBase,SkillSetting.ISkill
    {
        [SerializeField,Header("旋转")] private  RectTransform _rotator;
        
        private int _Effect;
        private List<PlayerUnit> _playerUnits = new List<PlayerUnit>();
        public override void Init(PlayerUnit playerUnit, SkillSetting.SkillInfo skillInfo)
        {
            InitBase(playerUnit, skillInfo);
            Rotate();
            Burn();
        }

        private void Rotate()
        {
            _rotator.DOLocalRotate(new Vector3(0,0,360f), 5f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
            _rotator.GetComponent<SpriteRenderer>().DOFade(0.2f, 1f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        }

        private void Burn()
        {
            _playerUnits.ForEach(playerUnit =>
            {
                GameObject effect = new GameObject();
                SkillState skillState = effect.AddComponent<SkillState>();
                skillState.Damage = PlayerUnit.Attr.ATK * _Effect / 100;
                skillState.HitBackDistance = 0;
                skillState.IsFromEnemy = PlayerUnit.IsEnemy;
                skillState.CriticalAble = false;
                skillState.IgnoreDefense = true;
                playerUnit.TakeDamage(skillState, PlayerUnit.transform.position);
            });
            DOTween.Sequence().SetDelay(1f).onComplete = Burn;
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
            _playerUnits.Add(playerUnit);
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            PlayerUnit playerUnit = other.GetComponent<PlayerUnit>();
            if(playerUnit==null) return;
            if(playerUnit.IsEnemy == PlayerUnit.IsEnemy) return;
            _playerUnits.Remove(playerUnit);
        }
    }    
}

