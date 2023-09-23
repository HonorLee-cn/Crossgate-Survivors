using System.Collections.Generic;
using Controller;
using DG.Tweening;
using GameData;
using Prefeb;
using Reference;
using UnityEngine;

namespace Resources.BattleSkills.s15
{
    public class Skill:SkillSetting.SkillBase,SkillSetting.ISkill
    {
        private static string PrefebPath = "BattleSkills/s15/Skill";
        private GameObject _arrowPrefeb;
        private float _distance;
        public override void Init(PlayerUnit playerUnit, SkillSetting.SkillInfo skillInfo)
        {
            InitBase(playerUnit, skillInfo);
            _arrowPrefeb = UnityEngine.Resources.Load<GameObject>(PrefebPath);
            Debug.Log("技能初始化");
        }
        
        public override void CastSkill()
        {
            Vector3 from = transform.position + new Vector3(0, 20f, 0);
            Vector3 targetPoint = PlayerUnit.TargetPosition;
            _distance = 200;
            // 计算角度,以to在from正上方为0度,计算偏移角度
            Vector3 directionVector = targetPoint - from;
            float angle = Mathf.Atan2(directionVector.y, directionVector.x) * Mathf.Rad2Deg;
            angle = angle - 90;
            
            Vector3 to = from + directionVector.normalized * _distance;

            float scale = 0.5f + (int) Params["RangePerLevel"] * Level / 100f;
            // Debug.Log(from + " " + to + " " + angle + " " + _distance);
            // float angle = Vector3.Angle(Vector3.right, to - from);
            // angle -= 90;
            GameObject arrow = Instantiate(_arrowPrefeb, transform);
            arrow.transform.position = from;
            arrow.transform.localRotation = Quaternion.Euler(0, 0, angle);
            arrow.transform.localScale = new Vector3(scale, scale, scale);
            SkillState skillState = arrow.AddComponent<SkillState>();
            skillState.Damage = PlayerUnit.Attr.ATK * (int) Params["Percent"] / 100;
            skillState.IsFromEnemy = PlayerUnit.IsEnemy;
            skillState.HitBackDistance = 15;
            skillState.TargetAudio = 142;
            // int Count = (int) Params["Count"];
            float speed = (int) Params["Speed"] * 10f;
            
            arrow.transform.DOMove(to, _distance/speed).SetEase(Ease.Linear).OnComplete(() =>
            {
                if(arrow != null) Destroy(arrow);
            });
        }
        

        public override void OnCDComplate()
        {
            
        }

        public override void LevelUp()
        {
            Level++;
            Params["Percent"] = (int) SkillInfo.Params["Percent"] + (int) SkillInfo.Params["Percent"] * Level *
                (int) SkillInfo.Params["PercentRatePerLevel"] / 100;
            Params["Range"] = (int)SkillInfo.Params["Range"] + (int)SkillInfo.Params["RangePerLevel"] * Level;
            Params["Speed"] = (int)SkillInfo.Params["Speed"] + (int)SkillInfo.Params["SpeedPerLevel"] * Level;
        }

        public override Dictionary<string, object> GetLevelParams(int level)
        {
            Dictionary<string,object> param = new Dictionary<string, object>(SkillInfo.Params);
            param["Percent"] = (int) SkillInfo.Params["Percent"] + (int) SkillInfo.Params["Percent"] * level *
                (int) SkillInfo.Params["PercentRatePerLevel"] / 100;
            param["Range"] = (int)SkillInfo.Params["Range"] + (int)SkillInfo.Params["RangePerLevel"] * level;
            param["Speed"] = (int)SkillInfo.Params["Speed"] + (int)SkillInfo.Params["SpeedPerLevel"] * level;
            return param;
        }
        
    }    
}

