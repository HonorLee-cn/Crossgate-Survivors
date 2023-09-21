using Controller;
using GameData;
using Reference;
using UnityEngine;

namespace Game.UI
{
    // 交互处理
    public class InteractiveAction:MonoBehaviour
    {
        [SerializeField,Header("购物窗口")] public GameObject ShopPanel;
        [SerializeField,Header("道具信息")] public ShopPanel_ItemInfo ItemInfo;
        [SerializeField,Header("装备信息")] public ShopPanel_ItemInfo EquipInfo;
        public void Action()
        {
            if(GlobalReference.BattleData.InteractiveUnit==null) return;
            switch (GlobalReference.BattleData.InteractiveUnit.Type)
            {
                // 商店
                case InteractiveSetting.Type.Shop:
                    Shop();
                    break;
                // 医生
                case InteractiveSetting.Type.Doctor:
                    Heal();
                    break;
                // 技能学习
                case InteractiveSetting.Type.Teacher:
                    Teach();
                    break;
            }
        }

        // 治疗
        private void Heal()
        {
            if (GlobalReference.BattleData.Money < 500)
            {
                GameController.AddFloatingText(GlobalReference.BattleData.PlayerUnit.transform.position,"穷鬼还是自己嗑药吧",Color.yellow);
                return;
            }
            if(GlobalReference.BattleData.PlayerUnit.Attr.HP==GlobalReference.BattleData.PlayerUnit.Attr.MaxHP && GlobalReference.BattleData.PlayerUnit.Attr.MP==GlobalReference.BattleData.PlayerUnit.Attr.MaxMP)
            {
                GameController.AddFloatingText(GlobalReference.BattleData.PlayerUnit.transform.position,"血魔已满",Color.yellow);
                return;
            }
            GlobalReference.BattleData.Money -= 500;
            GlobalReference.BattleData.PlayerUnit.Attr.HP = GlobalReference.BattleData.PlayerUnit.Attr.MaxHP;
            GameController.AddFloatingText(GlobalReference.BattleData.PlayerUnit.transform.position,"-500",Color.yellow);
        }

        // 学习技能
        private void Teach()
        {
            if (GlobalReference.BattleData.Money < 500)
            {
                GameController.AddFloatingText(GlobalReference.BattleData.PlayerUnit.transform.position,"穷鬼还是努力杀怪吧",Color.yellow);
                return;
            }
            GlobalReference.BattleData.Money -= 500;
            GlobalReference.BattleData.LearnPoint++;
            GameController.AddFloatingText(GlobalReference.BattleData.PlayerUnit.transform.position,"-500",Color.yellow);
        }

        // 打开商店
        private void Shop()
        {
            ShopPanel.gameObject.SetActive(true);
        }
    }
}