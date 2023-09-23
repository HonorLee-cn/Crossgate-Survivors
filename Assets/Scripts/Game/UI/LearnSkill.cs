using System;
using System.Collections.Generic;
using GameData;
using Prefeb;
using Reference;
using Resources.BattleSkills.s0;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    // 技能学习面板
    public class LearnSkill:MonoBehaviour
    {
        [SerializeField,Header("技能预制")] public LearnSkillPanel SkillPanel_Prefab;
        [SerializeField,Header("技能选择框")] public GameObject SkillSelector;
        [SerializeField,Header("确认按钮")] public Button ConfirmButton;
        [SerializeField,Header("剩余学习点数")] public Text RemainPoint;
        private int _currentSelectSkillID = -1;
        public int CurrentSelectSkillID
        {
            get => _currentSelectSkillID;
            set
            {
                _currentSelectSkillID = value;
                CheckAvailable();
            }
        }

        private void Start()
        {
            
        }

        private void OnEnable()
        {
            Time.timeScale = 0;
            LearnInit();
        }

        public void Learn()
        {
            if (CurrentSelectSkillID == -1) return;
            GlobalReference.BattleData.PlayerUnit.LearnSkill(CurrentSelectSkillID);
            GlobalReference.BattleData.LearnPoint--;
            CurrentSelectSkillID = -1;
            if (GlobalReference.BattleData.LearnPoint == 0)
            {
                gameObject.SetActive(false);
            }
            else
            {
                LearnInit();
            }
                
        }

        private void LearnInit()
        {
            RemainPoint.text = "剩余学习点数  " + GlobalReference.BattleData.LearnPoint;
            int count = SkillSelector.transform.childCount;
            for (var i = 0; i < count; i++)
            {
                Destroy(SkillSelector.transform.GetChild(i).gameObject);
            }
            MakeAvailableSkill(GlobalReference.BattleData.PlayerInfo.SpecialSkill);
            CheckAvailable();
        }

        private void OnDisable()
        {
            Time.timeScale = 1;
            //清除
            int count = SkillSelector.transform.childCount;
            for (var i = 0; i < count; i++)
            {
                Destroy(SkillSelector.transform.GetChild(i).gameObject);
            }
        }

        private void CheckAvailable()
        {
            if (CurrentSelectSkillID == -1)
            {
                ConfirmButton.interactable = false;
            }
            else
            {
                ConfirmButton.interactable = true;
            }
        }
        
        // 生成可学习技能 3 个
        private void MakeAvailableSkill(int availableSpecialSkillID = -1)
        {
            List<int> skillIDs = new List<int>();
            List<SkillSetting.SkillBase> skills = new List<SkillSetting.SkillBase>();
            PlayerUnit playerUnit = GlobalReference.BattleData.PlayerUnit;
            
            for (var i = 0; i < SkillSetting.SkillInfos.Count; i++)
            {
                SkillSetting.SkillInfo skillInfo = SkillSetting.SkillInfos[i];
                // 不可学技能
                if(!skillInfo.IsLearnable) continue;
                // 得意技但非当前角色得意技则禁止学习
                if (skillInfo.IsSpecial && availableSpecialSkillID != skillInfo.ID) continue;
                // 已学习技能但未达到最大等级
                if (playerUnit.GetLearnedSkill(skillInfo.ID) == null)
                {
                    skillIDs.Add(skillInfo.ID);
                }else if (playerUnit.GetLearnedSkill(skillInfo.ID).Level < skillInfo.MaxLevel)
                {
                    skillIDs.Add(skillInfo.ID);
                }
            }
            
            // 生成随机三个技能ID
            List<int> randomSkillIDs = new List<int>();
            for (var i = 0; i < 3; i++)
            {
                if(skillIDs.Count==0) break;
                
                int randomIndex = UnityEngine.Random.Range(0, skillIDs.Count);
                int skillId = skillIDs[randomIndex];
                randomSkillIDs.Add(skillId);
                skillIDs.RemoveAt(randomIndex);
            }
            
            // 生成技能数据
            for (var i = 0; i < randomSkillIDs.Count; i++)
            {
                int skillID = randomSkillIDs[i];
                // 通过加载反射技能脚本来生成技能实体
                LearnSkillPanel learnSkillPanel = Instantiate(SkillPanel_Prefab, SkillSelector.transform);
                SkillSetting.SkillBase skillBase = playerUnit.GetLearnedSkill(skillID);
                if (skillBase == null)
                {
                    GameObject skillObject =
                        UnityEngine.Resources.Load<GameObject>("BattleSkills/s" + skillID + "/Script");
                    if (skillObject == null)
                    {
                        Debug.LogError("SkillScript " + skillID + " Not Found");
                        continue;
                    }

                    SkillSetting.SkillBase skill = Instantiate(skillObject, learnSkillPanel.transform)
                        .GetComponent<SkillSetting.SkillBase>();
                    if (skill == null)
                    {
                        Debug.LogError("SkillBase " + skillID + " Not Found");
                        continue;
                    }
                    // 加载成功并初始化
                    skill.Init(null, SkillSetting.GetSkillInfo(skillID));
                    skill.ResetSkill();
                    skillBase = skill;
                }
                else
                {
                    // 如已学习则显示已学习字样
                    learnSkillPanel.LearnedFlag.SetActive(true);
                }
                
                learnSkillPanel.SetSkillInfo(skillBase);
                skills.Add(skillBase);
            }
        }
    }
}