using System;
using System.Collections.Generic;
using CGTool;
using Controller;
using GameData;
using Reference;
using UnityEngine;

namespace Prefeb
{
    // 交互对象体
    public class InteractiveUnit:MonoBehaviour
    {
        [SerializeField,Header("物件动画")] public AnimePlayer AnimePlayer;
        [SerializeField,Header("物件Sprite")] public SpriteRenderer SpriteRenderer;
        
        private uint _serial;
        public InteractiveSetting.Type _type;
        public string Notice;

        private Dictionary<InteractiveSetting.Type, List<string>> NPCTalk =
            new Dictionary<InteractiveSetting.Type, List<string>>()
            {
                [InteractiveSetting.Type.Shop] = new List<string>()
                {
                    "好货上架~童叟无欺！",
                    "请问有什么需要吗？",
                    "我这里有很多好东西哦！",
                    "骚年~你渴望变强么?"
                },
                [InteractiveSetting.Type.Doctor] = new List<string>()
                {
                    "我是医生，你是患者么？",
                    "我有药!你有病么?",
                    "放心!咱可不是庸医!",
                    "妙手回春!你信么?",
                },
                [InteractiveSetting.Type.Teacher] = new List<string>()
                {
                    "别看我懒,我可是个老师!",
                    "骚年~你渴望变强么?",
                    "学以致用!变强才能杀怪!",
                    "你看这本武学秘籍,只要2分...啊不~只要500就卖给你了",
                },
            };
        
        public InteractiveSetting.Type Type
        {
            get => _type;
            set
            {
                _type = value;
                switch (_type)
                {
                    case InteractiveSetting.Type.Shop:
                        Notice = "[F] 欢乐购物";
                        break;
                    case InteractiveSetting.Type.Doctor:
                        Notice = "[F] 快速治疗 (500)";
                        break;
                    case InteractiveSetting.Type.Teacher:
                        Notice = "[F] 学习技能 (500)";
                        break;
                    case InteractiveSetting.Type.Treasure:
                        Notice = "[F] 开箱开箱!!";
                        break;
                }
            }
        }
        public uint AnimeSerial
        {
            get => AnimePlayer.Serial;
            set
            {
                AnimePlayer.Serial = value;
            }
        }
        
        public uint SpriteSerial
        {
            get => _serial;
            set
            {
                _serial = value;
                GraphicInfoData data = GraphicInfo.GetGraphicInfoDataBySerial(_serial);
                GraphicData graphicData = Graphic.GetGraphicData(data);
                SpriteRenderer.sprite = graphicData.Sprite;
            }
        }

        private void Start()
        {
            // 定时随机说话
            InvokeRepeating(nameof(Talk), 0, 10);
        }

        private void Talk()
        {
            string talk = NPCTalk[Type][UnityEngine.Random.Range(0, NPCTalk[Type].Count)];
            GameController.AddFloatingText(transform.position, talk, Color.white, 18);
        }

        // 玩家接近时显示交互提示
        private void OnTriggerEnter2D(Collider2D other)
        {
            if(!other.gameObject.CompareTag("PlayUnit")) return;
            GlobalReference.BattleData.InteractiveUnit = this;
            GlobalReference.UI.BattleUI.InteractiveNotice.Show(Notice);
        }
        
        // 玩家离开时隐藏交互提示
        private void OnTriggerExit2D(Collider2D other)
        {
            if(!other.gameObject.CompareTag("PlayUnit")) return;
            GlobalReference.BattleData.InteractiveUnit = null;
            GlobalReference.UI.BattleUI.InteractiveNotice.Hide();
        }
    }
}