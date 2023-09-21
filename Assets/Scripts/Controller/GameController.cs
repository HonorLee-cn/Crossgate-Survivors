using System;
using Base;
using CGTool;
using DG.Tweening;
using Game.UI;
using GameData;
using Prefeb;
using Reference;
using UnityEngine;
using Util;

namespace Controller
{
    // 游戏主控
    public class GameController
    {
        // 分辨率类型
        public static int ScreenType = 0;
        // 背景音ID
        public static int BGMId = 0;
        // 游戏是否开始
        public static bool IsGameStart = false;
        
        // 当前人物数据
        private static PlayerSetting.PlayerInfo _PlayerInfo;
        // 当前关卡数据
        private static LevelSetting.LevelInfo _LevelInfo;
        // 游戏开启倒计时
        private static int _countDown = 3;
         
        // 选定角色职业
        public static PlayerSetting.PlayerInfo PlayerInfo
        {
            get => _PlayerInfo;
            set
            {
                _PlayerInfo = value;
                CheckStartAble();
            }
        }
        
        // 选定关卡
        public static LevelSetting.LevelInfo LevelInfo
        {
            get => _LevelInfo;
            set
            {
                _LevelInfo = value;
                CheckStartAble();
            }
        }
        
        // 检测是否可以开始游戏
        public static void CheckStartAble()
        {
            if (PlayerInfo == null || LevelInfo == null)
            {
                GlobalReference.UI.StartButton.interactable = false;
            }
            else
            {
                GlobalReference.UI.StartButton.interactable = true;
            }
        }
        
        // 开始游戏
        public static void StartGame()
        {
            // 重设倒计时
            _countDown = 3;
            
            // 生成并初始化本次游戏数据
            GlobalReference.BattleData = new BattleData()
            {
                LevelInfo = _LevelInfo,
                PlayerInfo = _PlayerInfo
            };
            
            // 生成地图
            MapController.InitMapData();
            
            // 游戏数据初始化设定
            GlobalReference.UI.LevelTaskSelector.gameObject.SetActive(false);
            GlobalReference.BattleData.RedBottleCount = 3;
            GlobalReference.BattleData.BlueBottleCount = 3;
            GlobalReference.BattleData.Money = 0;
            GlobalReference.BattleData.TotalMoney = 0;
            GlobalReference.BattleData.TotalKillCount = 0;
            GlobalReference.BattleData.LastEnemyCount = 0;
            GlobalReference.UI.BattleUI.CurrentLevelName.text = _LevelInfo.Name;
            GlobalReference.UI.BattleUI.CurrentLevelDifficulty.text = "Lv." + _LevelInfo.Difficulty;

            // 生成角色
            SpawnUser();
            
            // 开启UI
            GlobalReference.UI.BattleUI.gameObject.SetActive(true);
            
            // 开始游戏倒计时
            StartCountDown();

        }

        // 生成角色
        public static void SpawnUser()
        {
            //生成角色单位
            PlayerUnit playerUnit = MonoBehaviour.Instantiate(GlobalReference.Prefeb.PlayerUnit,
                GlobalReference.UI.UnitContainer.transform);
            CameraFollow cameraFollow = GlobalReference.UI.PlaygroundCamera.GetComponent<CameraFollow>();
            cameraFollow.target = playerUnit.transform;
            cameraFollow.roamMode = false;
            
            Vector3 pos = Toolkit.TileLocationToPosition(new Vector2Int((int)LevelInfo.MapWidth / 2, (int)LevelInfo.MapHeight / 2));
            playerUnit.gameObject.GetComponent<RectTransform>().localPosition = pos;
            playerUnit.PlayerInfo = PlayerInfo;
            playerUnit.Serial = PlayerInfo.Serial;
            playerUnit.AnimePlayer.PaletIndex = _LevelInfo.PaletIndex;

            // 添加角色属性变化监听 以更新UI
            playerUnit.Attr.OnAttrChange += (prop,val,attr) =>
            {
                switch (prop)
                {
                    case "HP":
                        GlobalReference.UI.BattleUI.PlayerHP.Value = val;
                        break;
                    case "MaxHP":
                        GlobalReference.UI.BattleUI.PlayerHP.MaxValue = val;
                        break;
                    case "MP":
                        GlobalReference.UI.BattleUI.PlayerMP.Value = val;
                        break;
                    case "MaxMP":
                        GlobalReference.UI.BattleUI.PlayerMP.MaxValue = val;
                        break;
                    case "EXP":
                        GlobalReference.UI.BattleUI.PlayerEXP.PlayerLevel.text = "Lv." + playerUnit.Attr.Level + "   " +
                            playerUnit.Attr.EXP + "/" + playerUnit.Attr.NextLevelEXP;
                        GlobalReference.UI.BattleUI.PlayerEXP.Value = val;
                        break;
                    case "NextLevelEXP":
                        GlobalReference.UI.BattleUI.PlayerEXP.PlayerLevel.text = "Lv." + playerUnit.Attr.Level + "   " +
                            playerUnit.Attr.EXP + "/" + playerUnit.Attr.NextLevelEXP;
                        GlobalReference.UI.BattleUI.PlayerEXP.MaxValue = val;
                        break;
                    case "Level":
                        GlobalReference.UI.BattleUI.PlayerEXP.PlayerLevel.text = "Lv." + playerUnit.Attr.Level + "   " +
                            playerUnit.Attr.EXP + "/" + playerUnit.Attr.NextLevelEXP;
                        playerUnit.EffectPlayer.play(110062, Anime.PlayType.OnceAndDestroy, 2f);
                        // 升级增加学习点数
                        if(val!=1) GlobalReference.BattleData.LearnPoint++;
                        break;
                    case "ATK":
                        GlobalReference.UI.BattleUI.PlayerDatas.ATK.text = val.ToString();
                        break;
                    case "DEF":
                        GlobalReference.UI.BattleUI.PlayerDatas.DEF.text = val.ToString();
                        break;
                    case "SPD":
                        GlobalReference.UI.BattleUI.PlayerDatas.SPD.text = val.ToString();
                        break;
                    case "Dodge":
                        GlobalReference.UI.BattleUI.DodgePoint.AvailableDodgePoint = val;
                        break;
                    case "MaxDodge":
                        GlobalReference.UI.BattleUI.DodgePoint.MaxDodge = val;
                        break;
                }
                
            };
            // 初始化角色数据
            playerUnit.InitAttr();
            // 将角色升级至 1 级
            playerUnit.LevelUpTo(1);
            // 将角色添加至游戏数据
            GlobalReference.BattleData.PlayerUnit = playerUnit;
        }

        // 开始游戏倒计时
        public static  void StartCountDown()
        {
            if (_countDown < 0)
            {
                // 标记游戏开始
                IsGameStart = true;

                // 角色学习默认攻击技能和得意技
                GlobalReference.BattleData.PlayerUnit.LearSkill(_PlayerInfo.ATKSkill);
                GlobalReference.BattleData.PlayerUnit.LearSkill(_PlayerInfo.SpecialSkill);
                // 增加学习点数,开启技能选择
                GlobalReference.BattleData.LearnPoint++;
                // 回合控制器初始化
                BattleController.InitRounds();
                // 隐藏操作帮助
                GlobalReference.UI.BattleUI.OperationInfo.GetComponent<RectTransform>().DOAnchorPos(new Vector3(0,0),0.5f).onComplete = () =>
                {
                    // 显示回合导航
                    GlobalReference.UI.BattleUI.RoundPanel.Show();
                    GlobalReference.UI.BattleUI.OperationInfo.gameObject.SetActive(false);
                };
                return;
            }
            
            // 倒计时提示
            AddFloatingText(GlobalReference.BattleData.PlayerUnit.transform.position, _countDown.ToString(), Color.white,62);
            _countDown--;
            DOTween.Sequence().SetDelay(1f).onComplete = StartCountDown;
        }

        // 游戏结束
        public static void EndGame(bool loose = false)
        {
            IsGameStart = false;
            BattleController.Clear();
            GlobalReference.UI.BattleUI.gameObject.SetActive(false);
            GlobalReference.UI.GameEnd.gameObject.SetActive(true);
            GlobalReference.UI.GameEnd.Show(loose);
        }

        // 帧更新
        public static void Update()
        {
            if (!IsGameStart) return;
            // 监听按键
            KeyListener();
            // 监听鼠标
            MouseListener();
        }

        // 选择关卡
        public static void SelectLevel(LevelSetting.LevelInfo levelInfo)
        {
            MapController.InitMapData(levelInfo);
            // 开启摄像机漫游模式
            CameraFollow cameraFollow = GlobalReference.UI.PlaygroundCamera.GetComponent<CameraFollow>(); 
            cameraFollow.roamOriginPosition = Toolkit.TileLocationToPosition(new Vector2Int((int)levelInfo.MapWidth / 2, (int)levelInfo.MapHeight / 2));
            cameraFollow.roamRadius = 500;
            GlobalReference.UI.PlaygroundCamera.transform.position = cameraFollow.roamOriginPosition;
            cameraFollow.roamMode = true;
            if (levelInfo.Audio > 0)
            {
                ChangeBGM(levelInfo.Audio);
            }
        }
        
        // 更改背景音乐
        public static void ChangeBGM(int bgmid)
        {
            if(bgmid==BGMId) return;
            BGMId = bgmid;
            AudioSource audioSource = GlobalReference.UI.PlaygroundCanvas.GetComponent<AudioSource>();
            audioSource.Stop();
            audioSource.clip = AudioTool.GetAudio(AudioTool.Type.BGM, BGMId);
            audioSource.Play();
        }
        
        // 按键监听
        public static void KeyListener()
        {
            // 默认角色方向
            Anime.DirectionType direction = Anime.DirectionType.NULL;
            //根据wasd判断方向,w键对应东北方向,w+a键同时按下对应北方,共8个方向,同时考虑两个相反冲突方向的情况则返回Anime.DirectionType.NULL
            if (Input.GetKey(KeyCode.W))
            {
                if (Input.GetKey(KeyCode.A))
                {
                    direction = Anime.DirectionType.North;
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    direction = Anime.DirectionType.East;
                }
                else
                {
                    direction = Anime.DirectionType.NorthEast;
                }
            }
            else if (Input.GetKey(KeyCode.S))
            {
                if (Input.GetKey(KeyCode.A))
                {
                    direction = Anime.DirectionType.West;
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    direction = Anime.DirectionType.South;
                }
                else
                {
                    direction = Anime.DirectionType.SouthWest;
                }
            }
            else if (Input.GetKey(KeyCode.A))
            {
                direction = Anime.DirectionType.NorthWest;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                direction = Anime.DirectionType.SouthEast;
            }
            else
            {
                direction = Anime.DirectionType.NULL;
            }

            //根据按键改变玩家移动方向
            GlobalReference.BattleData.PlayerUnit.MoveDirection = direction;

            // 空格 闪避
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GlobalReference.BattleData.PlayerUnit.Dash();
            }

            // Q 恢复生命
            if (Input.GetKeyDown(KeyCode.Q))
            {
                GlobalReference.UI.BattleUI.HpBottle.UseBottle();
            }
            // E 恢复魔法
            if (Input.GetKeyDown(KeyCode.E))
            {
                GlobalReference.UI.BattleUI.MpBottle.UseBottle();
            }
            // F 交互
            if (Input.GetKeyDown(KeyCode.F))
            {
                GlobalReference.UI.BattleUI.InteractiveAction.Action();
            }
            // 右键 得意技
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                GlobalReference.BattleData.PlayerUnit.SpecialSkill.Cast();
            }
            // ESC 暂停
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (GlobalReference.UI.ESCPanel.gameObject.activeSelf)
                {
                    GlobalReference.UI.ESCPanel.gameObject.SetActive(false);
                }
                else
                {
                    GlobalReference.UI.ESCPanel.gameObject.SetActive(true);    
                }
                
            }
            
            // 开发环境下
#if UNITY_EDITOR
            // V 调试按键
            if (Input.GetKeyDown(KeyCode.V))
            {
                // GlobalReference.BattleData.PlayerUnit.LearSkill(19);
                GlobalReference.BattleData.PlayerUnit.Attr.EXP = GlobalReference.BattleData.PlayerUnit.Attr.NextLevelEXP;
                // GlobalReference.BattleData.Money += 10000;
                // GlobalReference.UI.BattleUI.InteractiveAction.ShopPanel.gameObject.SetActive(true);

                // for (int i = 0; i < ItemSetting.ItemInfos.Count; i++)
                // {
                //     ItemSetting.ItemInfo itemInfo = ItemSetting.ItemInfos[i];
                //     GraphicInfoData graphicInfoData = GraphicInfo.GetGraphicInfoDataBySerial((uint) itemInfo.Serial);
                //     if (graphicInfoData == null) Debug.Log(itemInfo.Name + " " + itemInfo.Serial);
                // }
            }
            // K 一键清怪
            if (Input.GetKeyDown(KeyCode.K))
            {
                foreach (PlayerUnit battleDataEnemyUnit in GlobalReference.BattleData.EnemyUnits)
                {
                    battleDataEnemyUnit.InstantDeath();
                }
            }
#endif
        }

        // 鼠标监听
        public static void MouseListener()
        {
            // 默认角色方向
            Anime.DirectionType direction = Anime.DirectionType.NULL;
            //根据鼠标位置判断方向
            Vector3 mousePosition = Input.mousePosition;
            Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            Vector3 directionVector = mousePosition - screenCenter;
            float angle = Mathf.Atan2(directionVector.y, directionVector.x) * Mathf.Rad2Deg;
            angle = angle - 90;
            //根据角度获取方向,鼠标在屏幕正上方时,angle角度在22.5~-22.5之间,方向对应东北方向,鼠标在屏幕正下方时,方向对应西南方向,共8个方向
            // 计算向量与正北方向的角度,考虑正反向旋转,消除误差
            if (angle <= 25 && angle > -25) direction = Anime.DirectionType.NorthEast;
            else if (angle <= -25 && angle > -75) direction = Anime.DirectionType.East;
            else if (angle <= -75 && angle > -105) direction = Anime.DirectionType.SouthEast;
            else if (angle <= -105 && angle > -155) direction = Anime.DirectionType.South;
            else if (angle <= -155 && angle > -205) direction = Anime.DirectionType.SouthWest;
            else if (angle <= -205 && angle > -255) direction = Anime.DirectionType.West;
            else if (angle <= -255 && angle > -285) direction = Anime.DirectionType.NorthWest;
            else if (angle <= -285 && angle > -335) direction = Anime.DirectionType.North;
            else if (angle <= -335 && angle > -385) direction = Anime.DirectionType.NorthEast;
            
            else if (angle > 25 && angle < 75) direction = Anime.DirectionType.North;
            else if (angle > 75 && angle < 105) direction = Anime.DirectionType.NorthWest;
            else if (angle > 105 && angle < 155) direction = Anime.DirectionType.West;
            else if (angle > 155 && angle < 205) direction = Anime.DirectionType.SouthWest;
            else if (angle > 205 && angle < 255) direction = Anime.DirectionType.South;
            else if (angle > 255 && angle < 285) direction = Anime.DirectionType.SouthEast;
            else if (angle > 285 && angle < 335) direction = Anime.DirectionType.East;
            else if (angle > 335 && angle < 385) direction = Anime.DirectionType.NorthEast;

            // 改变角色面朝方向
            GlobalReference.BattleData.PlayerUnit.FaceDirection = direction;
        }

        // 添加浮动文字
        public static void AddFloatingText(Vector3 position,string text,  Color color = default,int size = 14)
        {
            FloatingText floatingText = GameObject.Instantiate(GlobalReference.UI.BattleUI.FloatingText,
                GlobalReference.UI.BattleUI.FloatingTextLayer.transform);
            floatingText.transform.position = position;
            floatingText.Show(text, color, size);
        }
    }
}