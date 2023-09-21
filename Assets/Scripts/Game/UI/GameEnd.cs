using System;
using System.Collections.Generic;
using Reference;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace Game.UI
{
    // 游戏结束界面
    public class GameEnd : MonoBehaviour
    {
        [SerializeField, Header("结束状态")] public Text EndText;
        [SerializeField, Header("结束评语")] public Text EndDesc;
        [SerializeField, Header("最高等级")] public Text MaxLevel;
        [SerializeField, Header("学习技能")] public Text LearnSkill;
        [SerializeField, Header("杀死怪物")] public Text KillMonster;
        [SerializeField, Header("获得金币")] public Text GainGold;

        private List<String> FaileStrings = new List<string>()
        {
            "如果能重来,想必你会选李白吧",
            "就这？少年郎，你还要继续努力啊",
            "你的战斗力还不够啊，少年郎",
            "你看看你，这样的战斗力，还想救妹妹？",
            "你说你的战斗力，能救得了妹妹吗？",
            "拯救妹妹，你还差得远呢",
        };

        private List<String> SuccessStrings = new List<string>()
        {
            "你的战斗力，已经足够了",
            "你很强，但是你的战斗力还不够",
            "真厉害，你的战斗力已经很强了",
        };

        public void Show(bool loose)
        {
            if (loose)
            {
                EndText.text = "失败";
                EndText.color = Color.red;
                EndDesc.text = FaileStrings[UnityEngine.Random.Range(0, FaileStrings.Count)];
            }
            else
            {
                EndText.text = "胜利";
                EndText.color = Color.green;
                EndDesc.text = SuccessStrings[UnityEngine.Random.Range(0, SuccessStrings.Count)];
            }

            MaxLevel.text = Toolkit.FormatNumberWithCommas(GlobalReference.BattleData.PlayerUnit.Attr.Level);
            LearnSkill.text = GlobalReference.BattleData.PlayerUnit.PlayerSkills.Count.ToString();
            KillMonster.text = Toolkit.FormatNumberWithKMB(GlobalReference.BattleData.TotalKillCount);
            GainGold.text = Toolkit.FormatNumberWithKMB(GlobalReference.BattleData.TotalMoney);
        }

        public void Restart()
        {
            gameObject.SetActive(false);
            GlobalReference.UI.LevelTaskSelector.gameObject.SetActive(true);
        }

        public void BacktoMain()
        {
            gameObject.SetActive(false);
            GlobalReference.UI.MainScreen.gameObject.SetActive(true);
        }
    }
}