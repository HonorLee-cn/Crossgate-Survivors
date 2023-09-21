using System.Collections.Generic;
using Game.UI;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace Reference
{
    public class UIRef:MonoBehaviour
    {
        [SerializeField,Header("战场相机")] public Camera PlaygroundCamera;
        [SerializeField,Header("战场Canvas")] public Canvas PlaygroundCanvas;
        [SerializeField,Header("战场地面Tilemap")] public Tilemap Tilemap_Ground;
        [SerializeField,Header("战场物件Tilemap")] public Tilemap Tilemap_Object;
        [SerializeField,Header("战场物件单位层")] public GameObject MapUnitContainer;
        [SerializeField,Header("战场单位层")] public GameObject UnitContainer;
        [SerializeField,Header("战场UI层")] public GameObject UICanvas;
        [SerializeField,Header("关卡任务选择界面")] public GameObject LevelTaskSelector;
        [SerializeField,Header("人物选择")] public GameObject SelectUser;
        [SerializeField,Header("关卡选择")] public GameObject SelectLevel;
        [SerializeField,Header("开始按钮")] public Button StartButton;
        [SerializeField,Header("战斗UI层")] public BattleUI BattleUI;
        [SerializeField,Header("战斗结束层")] public GameEnd GameEnd;
        [SerializeField,Header("ESC面板")] public ESCPanel ESCPanel;
        [SerializeField,Header("主面板")] public MainScreen MainScreen;

        [SerializeField,Header("关卡封面")] public List<Sprite> LevelCover;
    }
}