using GameData;
using Reference;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
    
    // 学习技能信息面板
    public class LearnSkillPanel:MonoBehaviour,IPointerClickHandler
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
        [SerializeField,Header("技能下一级说明")] public Text SkillAblityNextLevel;
        [SerializeField,Header("已学习标识")] public GameObject LearnedFlag;

        private SkillSetting.SkillBase _skillBase;
        
        public void SetSkillInfo(SkillSetting.SkillBase skillBase)
        {
            _skillBase = skillBase;
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
            SkillAblityNextLevel.text = skillBase.GetLevelDesc(skillBase.Level + 1);
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            LearnSkillPanel[] panels = transform.parent.GetComponentsInChildren<LearnSkillPanel>();
            foreach (var panel in panels)
            {
                panel.GetComponent<Button>().interactable = true;
            }
            GetComponent<Button>().interactable = false;
            GlobalReference.UI.BattleUI.LearnSkill.CurrentSelectSkillID = _skillBase.SkillInfo.ID;
        }
    }
}