using System;
using Controller;
using GameData;
using Reference;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    // 统一红蓝血瓶
    public class HpMpBottle:MonoBehaviour
    {
        [SerializeField,Header("道具图片")] public Image ItemImage;
        [SerializeField,Header("道具数量")] public Text ItemCount;
        [SerializeField, Header("冷却")] private CircleCD _circleCd;
        [SerializeField, Header("是否为血瓶")] private bool _isHp;

        private int _count;
        public int Count
        {
            get => _count;
            set
            {
                _count = value;
                ItemCount.text = _count.ToString();
                checkAvailable();
            }
        }
        
        private float _cdTime = 5f;
        private float _fillAmount = 0.2f;

        private void Awake()
        {
            _circleCd.OnCDComplete += () =>
            {
                checkAvailable();
            };
        }

        private void Start()
        {
            checkAvailable();
        }

        public void UseBottle()
        {
            if (Count == 0)
            {
                GameController.AddFloatingText(GlobalReference.BattleData.PlayerUnit.transform.position, "药水用完啦", Color.yellow);
                return;
            }

            if (!_circleCd.Available)
            {
                GameController.AddFloatingText(GlobalReference.BattleData.PlayerUnit.transform.position, "药水还在冷却中", Color.yellow);
                return;
            }
            Attr attr = GlobalReference.BattleData.PlayerUnit.Attr;
            int fill = 0;
            if (_isHp)
            {
                if (attr.HP == attr.MaxHP)
                {
                    GameController.AddFloatingText(GlobalReference.BattleData.PlayerUnit.transform.position, "血量已补满", Color.white);
                    return;
                }
                fill = (int) (_fillAmount * attr.MaxHP);
                GlobalReference.BattleData.RedBottleCount--;
                GameController.AddFloatingText(
                    GlobalReference.BattleData.PlayerUnit.transform.position + new Vector3(0, 50, 0), "+" + fill,
                    Color.green, 18);
                attr.HP += fill;
            }
            else
            {
                if (attr.MP == attr.MaxMP)
                {
                    GameController.AddFloatingText(GlobalReference.BattleData.PlayerUnit.transform.position, "魔法值已满", Color.white);
                    return;
                }
                fill = (int) (_fillAmount * attr.MaxMP);
                GlobalReference.BattleData.BlueBottleCount--;
                GameController.AddFloatingText(
                    GlobalReference.BattleData.PlayerUnit.transform.position + new Vector3(0, 50, 0), "+" + fill,
                    Color.blue, 18);
                attr.MP += fill;
            }
            _circleCd.StartCD(_cdTime);
        }
        
        private void checkAvailable()
        {
            bool available = false;
            if (_count > 0)
            {
                if(_circleCd.Available) available = true;
            }
            if (available)
            {
                ItemImage.color = new Color(255, 255, 255, 255);
            }
            else
            {
                ItemImage.color = new Color(255, 255, 255, 0.5f);
            }
        }
    }
}