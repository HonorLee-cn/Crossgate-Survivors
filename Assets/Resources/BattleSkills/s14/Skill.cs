using System;
using System.Collections.Generic;
using CGTool;
using Controller;
using DG.Tweening;
using GameData;
using Prefeb;
using Reference;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Resources.BattleSkills.s14
{
    public class Skill:SkillSetting.SkillBase,SkillSetting.ISkill
    {
        private UnitLightSetting.UnitLightInfo _unitLightInfo;
        private Guid _guid = Guid.NewGuid();
        public override void Init(PlayerUnit playerUnit, SkillSetting.SkillInfo skillInfo)
        {
            InitBase(playerUnit, skillInfo);
            int index = (int) Params["Light"];
            _unitLightInfo = UnitLightSetting.UnitLightInfos[index];
            playerUnit.UnitLight.Show(_unitLightInfo.Color);
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
            Params["Range"] = (float) Params["Range"] + Level * (float) Params["RangePerLevel"];
            
            float range = (float) Params["Range"];
            gameObject.transform.localScale = new Vector3(range, range, range);
        }
        public override Dictionary<string, object> GetLevelParams(int level)
        {
            Dictionary<string,object> param = new Dictionary<string, object>(SkillInfo.Params);
            return param;
        }
        
        //生命光环

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("EnemyUnit")) return;
            PlayerUnit unit = other.GetComponent<PlayerUnit>();
            unit.UnitLight.Show(_unitLightInfo.Color);
            int val = (int) unit.Attr.GetPropertyValue(_unitLightInfo.Prop);
            int increace = (int) (val * _unitLightInfo.ValueRate / 100);
            unit.UnitLight.IncreaseValue.Add(_guid,increace);
            unit.Attr.SetPropertyValue(_unitLightInfo.Prop, val+increace);
            unit.Attr.HP += increace;
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("EnemyUnit")) return;
            PlayerUnit unit = other.GetComponent<PlayerUnit>();
            unit.UnitLight.Hide();
            int increace = unit.UnitLight.IncreaseValue[_guid];
            unit.UnitLight.IncreaseValue.Remove(_guid);
            int val = (int) unit.Attr.GetPropertyValue(_unitLightInfo.Prop);
            unit.Attr.SetPropertyValue(_unitLightInfo.Prop, val-increace);
        }
    }    
}

