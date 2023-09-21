using GameData;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    // 左下技能信息提示面板
    public class SkillInfoPanel:MonoBehaviour
    {
        [SerializeField,Header("技能名称")] public Text SkillName;
        [SerializeField,Header("技能类型")] public Text SkillType;
        [SerializeField,Header("技能描述")] public Text SkillDesc;
        [SerializeField,Header("技能图标")] public Image SkillIcon;
        [SerializeField,Header("技能等级")] public Text SkillLevel;
        [SerializeField,Header("技能冷却")] public Text SkillCD;
        [SerializeField,Header("技能消耗")] public Text SkillCost;
        [SerializeField,Header("技能详细说明")] public Text SkillAblity;
        [SerializeField,Header("技能升级说明")] public Text SkillAblityLevelUp;
        
        public void SetSkillInfo(SkillSetting.SkillBase skillBase)
        {
            SkillName.text = skillBase.SkillInfo.Name;
            string type = "被动技能";
            if (skillBase.IsAtkSkill)
            {
                type = "普通攻击";
            }
            else if(skillBase.IsSpecialSkill){
                type = "得意技";    
            }
            else if(skillBase.CD>0){
                type = "主动技能";
            }
            SkillType.text = type;
            SkillDesc.text = skillBase.SkillInfo.Desc;
            SkillIcon.sprite =  UnityEngine.Resources.Load<Sprite>("SkillIcons/" + skillBase.SkillInfo.Icon);
            SkillLevel.text = "Lv." + skillBase.Level;
            if (skillBase.CD > 0)
            {
                SkillCD.text = "冷却时间 " + skillBase.CD + "s";
            }
            else
            {
                SkillCD.text = "冷却时间 无";
            }
            if (skillBase.MPCost > 0)
            {
                SkillCost.text = "魔法消耗 " + skillBase.MPCost + "MP";
            }else{
                SkillCost.text = "";
            }
            SkillAblity.text = skillBase.AbilityDesc;
            SkillAblityLevelUp.text = skillBase.AbilityLevelUpDesc;


        }
    }
}