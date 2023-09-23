using System.Collections.Generic;
using CGTool;
using Controller;
using DG.Tweening;
using GameData;
using Prefeb;
using Reference;
using UnityEngine;

namespace Resources.BattleSkills.s21
{
    public class Skill:SkillSetting.SkillBase,SkillSetting.ISkill
    {
        private static string PrefebPath = "BattleSkills/s21/Skill";
        private GameObject _windPrefeb;
        private float _distance;
        public override void Init(PlayerUnit playerUnit, SkillSetting.SkillInfo skillInfo)
        {
            InitBase(playerUnit, skillInfo);
            _windPrefeb = UnityEngine.Resources.Load<GameObject>(PrefebPath);
            // Debug.Log("技能初始化");
        }
        
        public override void CastSkill()
        {
            Vector3 from = transform.position + new Vector3(0, 0f, 0);
            Vector3 targetPoint = PlayerUnit.TargetPosition;
            _distance = (int) Params["Range"] * 10f;
            // 计算角度,以to在from正上方为0度,计算偏移角度
            Vector3 directionVector = targetPoint - from;

            Vector3 to = from + directionVector.normalized * _distance;
            GameObject wind = Instantiate(_windPrefeb, transform);
            
            wind.transform.position = from;
            // wind.transform.localRotation = Quaternion.Euler(0, 0, angle);
            SkillState skillState = wind.AddComponent<SkillState>();
            skillState.Damage = PlayerUnit.Attr.ATK * (int) Params["Percent"] / 100;
            skillState.IsFromEnemy = PlayerUnit.IsEnemy;
            skillState.IgnoreDefense = true;
            skillState.HitBackDistance = 50 + Level * 2;
            skillState.TargetAudio = 268;
            
            
            Wind windScript = wind.GetComponent<Wind>();
            int serial = Level > 8 ? 110044 : Level > 5 ? 110045 : Level > 3 ? 110046 : 110047;
            windScript.WindAnime.play((uint) serial, Anime.PlayType.Loop);
            float Radius = (int) Params["Radius"];
            wind.GetComponent<CircleCollider2D>().radius = Radius;
            
            float speed = (int) Params["Speed"] * 10f;

            Vector3 half4point = (to - from) / 4;
            
            Vector3[] path = new[]
            {
                from,
                from + half4point + new Vector3(half4point.x * Random.Range(-1.5f,1.5f), half4point.y * Random.Range(-1.5f,1.5f), 0),
                from + half4point * 2 + new Vector3(half4point.x * Random.Range(-1.5f,1.5f), half4point.y * Random.Range(-1.5f,1.5f), 0),
                to
            };
            wind.transform.DOPath(path, _distance / speed * 2, PathType.CatmullRom).SetEase(Ease.Linear).OnComplete(() =>
            {
                if (wind != null) Destroy(wind);
            });
            // wind.transform.DOMove(to, _distance / speed).SetEase(Ease.Linear).OnComplete(() =>
            // {
            //     if (wind != null) Destroy(wind);
            // });


        }
        

        public override void OnCDComplate()
        {
            
        }

        public override void LevelUp()
        {
            Level++;
            CD = SkillInfo.CD - SkillInfo.CD * Level * SkillInfo.CDReduceRatePerLevel;
            Params["Percent"] = (int) SkillInfo.Params["Percent"] + Level * (int) SkillInfo.Params["PercentPerLevel"];
            Params["Range"] = (int)SkillInfo.Params["Range"] + (int)SkillInfo.Params["RangePerLevel"] * Level;
            Params["Speed"] = (int)SkillInfo.Params["Speed"] + (int)SkillInfo.Params["SpeedPerLevel"] * Level;
            Params["Radius"] = (int)SkillInfo.Params["Radius"] + (int)SkillInfo.Params["RadiusPerLevel"] * Level;
        }

        public override Dictionary<string, object> GetLevelParams(int level)
        {
            Dictionary<string,object> param = new Dictionary<string, object>(SkillInfo.Params);
            param["Percent"] = (int) SkillInfo.Params["Percent"] + level * (int) SkillInfo.Params["PercentPerLevel"];
            param["Range"] = (int)SkillInfo.Params["Range"] + (int)SkillInfo.Params["RangePerLevel"] * level;
            param["Speed"] = (int)SkillInfo.Params["Speed"] + (int)SkillInfo.Params["SpeedPerLevel"] * level;
            param["Radius"] = (int)SkillInfo.Params["Radius"] + (int)SkillInfo.Params["RadiusPerLevel"] * level;
            return param;
        }
        
    }    
}

