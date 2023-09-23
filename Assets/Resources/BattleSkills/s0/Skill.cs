using System.Collections.Generic;
using Controller;
using DG.Tweening;
using GameData;
using Prefeb;
using UnityEngine;

namespace Resources.BattleSkills.s0
{
    public class Skill:SkillSetting.SkillBase,SkillSetting.ISkill
    {
        private static string PrefebPath = "BattleSkills/s0/Skill";
        private GameObject _prefeb;
        private int _castCount;
        private float _scale = 0.5f;
        private float _rangeFix = 25f;
        public override void Init(PlayerUnit playerUnit, SkillSetting.SkillInfo skillInfo)
        {
            InitBase(playerUnit, skillInfo);
            _prefeb = UnityEngine.Resources.Load<GameObject>(PrefebPath);
        }
        
        public override void CastSkill()
        {
            float rate = 0.3f;
            _castCount = (int)Params["Count"];
            InvokeRepeating(nameof(Attack), 0, rate);
        }

        public void Attack()
        {
            if (_castCount == 0)
            {
                CancelInvoke(nameof(Attack));
                return;
            }
            bool flip = _castCount % 2 == 0;
            _castCount--;
            Vector3 from = PlayerUnit.transform.position + new Vector3(0, 20f, 0);
            Vector3 targetPoint = PlayerUnit.TargetPosition;
            float Range = (float)Params["Range"];
            float angleZF = Range / 2;
            float angleZT = -Range / 2;
            
            GameObject chop = GameObject.Instantiate(_prefeb, transform);
            Vector3 rot = new Vector3(0, 0, angleZF);
            float y = flip ? 180 : 0;
            rot.y = y;
            chop.transform.localScale = new Vector3(_scale, _scale, _scale);
            chop.transform.localRotation = Quaternion.Euler(rot.normalized);
            
            
            // 计算角度,以to在from正上方为0度,计算偏移角度
            Vector3 directionVector = targetPoint - from;
            float angle = Mathf.Atan2(directionVector.y, directionVector.x) * Mathf.Rad2Deg;
            angle = angle - 90;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            SkillState skillState = chop.AddComponent<SkillState>();
            skillState.Damage = PlayerUnit.Attr.ATK * (int) Params["Percent"] / 100;
            skillState.IsFromEnemy = PlayerUnit.IsEnemy;
            skillState.HitBackDistance = 10;
            skillState.TargetAudio = 133;
            
            chop.transform.DOLocalRotate( new Vector3(0, y, angleZT), 0.1f).SetEase(Ease.Linear).onComplete = () =>
            {
                Destroy(chop);
                transform.rotation = Quaternion.Euler(0, 0, 0);
            };
        }

        public override void OnCDComplate()
        {
            // Debug.Log("射击技能CD完成");
        }

        

        public override void LevelUp()
        {
            Level++;
            Params["Percent"] = (int) SkillInfo.Params["Percent"] + (int) SkillInfo.Params["Percent"] * Level *
                (int) SkillInfo.Params["PercentRatePerLevel"] / 100;
            Params["Distance"] = (int)SkillInfo.Params["Distance"] + (int)SkillInfo.Params["DistancePerLevel"] * Level;
            Params["Range"] = (float)SkillInfo.Params["Range"] + (float)SkillInfo.Params["RangePerLevel"] * Level;
            Params["Count"] = (int) SkillInfo.Params["Count"] + Level / 3 * (int)SkillInfo.Params["CountPer3Level"];
            _scale += 0.05f;
        }
        
        public override Dictionary<string, object> GetLevelParams(int level)
        {
            Dictionary<string,object> param = new Dictionary<string, object>(SkillInfo.Params);
            param["Percent"] = (int) SkillInfo.Params["Percent"] + (int) SkillInfo.Params["Percent"] * level *
                (int) SkillInfo.Params["PercentRatePerLevel"] / 100;
            param["Distance"] = (int)SkillInfo.Params["Distance"] + (int)SkillInfo.Params["DistancePerLevel"] * level;
            param["Range"] = (float)SkillInfo.Params["Range"] + (float)SkillInfo.Params["RangePerLevel"] * level;
            param["Count"] = (int) SkillInfo.Params["Count"] + level / 3 * (int)SkillInfo.Params["CountPer3Level"];
            return param;
        }
    }    
}

