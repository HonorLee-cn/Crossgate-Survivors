using System.Collections.Generic;
using CGTool;
using Controller;
using DG.Tweening;
using GameData;
using Prefeb;
using Reference;
using UnityEngine;

namespace Resources.BattleSkills.s2
{
    public class Skill:SkillSetting.SkillBase,SkillSetting.ISkill
    {
        private static string PrefebPath = "BattleSkills/s2/Skill";
        private GameObject _arrowPrefeb;
        private float _distance;
        private int _caseCount;
        private float _scale = 0.2f;
        public override void Init(PlayerUnit playerUnit, SkillSetting.SkillInfo skillInfo)
        {
            InitBase(playerUnit, skillInfo);
            _arrowPrefeb = UnityEngine.Resources.Load<GameObject>(PrefebPath);
        }
        
        public override void CastSkill()
        {
            PlayerUnit.EffectPlayer.play(110102, Anime.PlayType.OnceAndDestroy, 2f,(a)=>{
                
                float rate = (float) Params["Rate"];

                _caseCount = (int) Params["Count"];
            
                InvokeRepeating(nameof(ArrowRain), 0, rate);
            });
        }

        private void ArrowRain()
        {
            int count = 10;
            float delay = 0.05f;
            for (int i = 0; i < count; i++)
            {
                DOTween.Sequence().SetDelay(delay).onComplete =()=>{
                    Vector3 from = transform.position + new Vector3(0, 20f, 0);
                    Vector3 targetPoint = PlayerUnit.TargetPosition;
                    //加入随机偏移
                    float randomX = Random.Range(-100f, 100f);
                    float randomY = Random.Range(-50f, 50f);
                    targetPoint += new Vector3(randomX, randomY, 0);
                
                    _distance = (int) Params["Range"] * 10f;
                    // 计算角度,以to在from正上方为0度,计算偏移角度
                    Vector3 directionVector = targetPoint - from;
                    float angle = Mathf.Atan2(directionVector.y, directionVector.x) * Mathf.Rad2Deg;
                    angle = angle - 90;
            
                    Vector3 to = from + directionVector.normalized * _distance;

                    // Debug.Log(from + " " + to + " " + angle + " " + _distance);
                    // float angle = Vector3.Angle(Vector3.right, to - from);
                    // angle -= 90;
                    GameObject arrow = Instantiate(_arrowPrefeb, transform);
                    SkillState skillState = arrow.AddComponent<SkillState>();
                    arrow.transform.position = from;
                    arrow.transform.localRotation = Quaternion.Euler(0, 0, angle);
                    skillState.Damage = PlayerUnit.Attr.ATK * (int) Params["Percent"] / 100;
                    skillState.IsFromEnemy = PlayerUnit.IsEnemy;
                    skillState.HitBackDistance = 5;
                    skillState.TargetAudio = 142;
                    // int Count = (int) Params["Count"];
                    float speed = (int) Params["Speed"] * 10f;
            
                    arrow.transform.DOMove(to, _distance/speed).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        if(arrow != null) Destroy(arrow);
                    });
                };
                delay += 0.05f;
                
            }
            _caseCount--;
            if (_caseCount == 0)
            {
                CancelInvoke(nameof(ArrowRain));
            }
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
            Params["Count"] = (int) SkillInfo.Params["Count"] + Level / 3 * (int)SkillInfo.Params["CountPer3Level"];
        }
        public override Dictionary<string, object> GetLevelParams(int level)
        {
            Dictionary<string,object> param = new Dictionary<string, object>(SkillInfo.Params);
            param["Percent"] = (int) SkillInfo.Params["Percent"] + (int) SkillInfo.Params["Percent"] * level *
                (int) SkillInfo.Params["PercentRatePerLevel"] / 100;
            param["Range"] = (int)SkillInfo.Params["Range"] + (int)SkillInfo.Params["RangePerLevel"] * Level;
            param["Speed"] = (int)SkillInfo.Params["Speed"] + (int)SkillInfo.Params["SpeedPerLevel"] * Level;
            param["Count"] = (int) SkillInfo.Params["Count"] + Level / 3 * (int)SkillInfo.Params["CountPer3Level"];
            return param;
        }
    }    
}

