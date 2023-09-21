using Game.UI;
using GameData;
using Reference;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Prefeb
{
    // 左下技能CD效果
    public class SkillItem:MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
    {
        [SerializeField,Header("技能等级")] public Text LevelText;
        [SerializeField,Header("倒计时")] public CircleCD CircleCD;
        
        public SkillSetting.SkillBase Skill;
        
        private int _level = 0;
        public int Level
        {
            get => _level;
            set
            {
                _level = value;
                if (_level == Skill.SkillInfo.MaxLevel)
                {
                    LevelText.text = "Max";
                }
                else
                {
                    LevelText.text = "Lv." + value;
                }

            }
        }

        // public void CastSkill()
        // {
        //     if(Skill.CD==0) return;
        //     CircleCD.StartCD(Skill.CD);
        // }

        public void OnPointerEnter(PointerEventData eventData)
        {
            GlobalReference.UI.BattleUI.SkillInfoPanel.gameObject.SetActive(true);
            GlobalReference.UI.BattleUI.SkillInfoPanel.SetSkillInfo(Skill);
            GlobalReference.UI.BattleUI.SkillInfoPanel.transform.position = transform.position + new Vector3(0, 0.2f, 0);
            
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            GlobalReference.UI.BattleUI.SkillInfoPanel.gameObject.SetActive(false);
            
        }
    }
}