using System;
using DG.Tweening;
using Prefeb;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    // 战斗UI
    public class BattleUI:MonoBehaviour
    {
        [Header("框架")]
        [SerializeField, Header("躲避点数")] public DodgePoints DodgePoint;
        [SerializeField, Header("BOSS血条")] public BOSSPanel BOSSPanel;
        [SerializeField, Header("BOSS血条")] public BOSSBar BOSSBar_Prefeb;
        [SerializeField, Header("总金币数")] public GameObject TotalCoin;
        [SerializeField, Header("总怪物数")] public GameObject TotalEnemy;
        [SerializeField, Header("关卡信息")] public GameObject LevelPanel;
        [SerializeField, Header("技能列表")] public GameObject SkillList;
        
        [SerializeField, Header("装备栏")] public GameObject EquipList;
        [SerializeField, Header("底部")] public GameObject Bottom;
        [SerializeField, Header("操作说明")] public GameObject OperationInfo;
        [SerializeField, Header("技能说明")] public SkillInfoPanel SkillInfoPanel;
        [SerializeField, Header("红瓶")] public HpMpBottle HpBottle;
        [SerializeField, Header("蓝瓶")] public HpMpBottle MpBottle;
        [SerializeField,Header("浮动文字层")] public GameObject FloatingTextLayer;
        [SerializeField,Header("浮动文字")] public FloatingText FloatingText;
        [SerializeField,Header("角色得意技")] public CircleCD SpecialSkill;
        
        
        [SerializeField,Header("交互提示")] public InteractiveNotice InteractiveNotice;
        [SerializeField,Header("交互处理")] public InteractiveAction InteractiveAction;


        [SerializeField, Header("学习技能")] public LearnSkill LearnSkill;
        
        [SerializeField, Header("回合导航")] public RoundPanel RoundPanel;

        [Header("数据UI")]
        [SerializeField,Header("当前金币数")] public Text CurrentCoin;
        [SerializeField,Header("当前剩余怪物数")] public Text CurrentEnemy;
        [SerializeField,Header("当前关卡名")] public Text CurrentLevelName;
        [SerializeField,Header("当前关卡难度")] public Text CurrentLevelDifficulty;
        [SerializeField,Header("当前角色血量")] public HpMpBar PlayerHP;
        [SerializeField,Header("当前角色蓝量")] public HpMpBar PlayerMP;
        [SerializeField,Header("当前角色经验条")] public EXPBar PlayerEXP;
        [SerializeField,Header("当前角色数据")] public PlayerDatas PlayerDatas;

        // 用于动画的对象
        private GameObject[] leftRightGameObjects;
        private GameObject[] topBottomGameObjects;

        private void Awake()
        {
            leftRightGameObjects = new []{TotalCoin,TotalEnemy,LevelPanel,SkillList,EquipList,OperationInfo};
            topBottomGameObjects = new []{BOSSPanel.gameObject,Bottom};
        }

        private void Start()
        {
            
            
        }

        // UI框架显示时初始化位置
        private void OnEnable()
        {
            foreach (GameObject leftRightGameObject in leftRightGameObjects)
            {
                leftRightGameObject.SetActive(true);
                Vector2 pos = leftRightGameObject.GetComponent<RectTransform>().anchoredPosition;
                pos.x = 0;
                leftRightGameObject.GetComponent<RectTransform>().anchoredPosition = pos;
            }
            foreach (GameObject topBottomGameObject in topBottomGameObjects)
            {
                topBottomGameObject.SetActive(true);
                Vector2 pos = topBottomGameObject.GetComponent<RectTransform>().anchoredPosition;
                pos.y = topBottomGameObject.GetComponent<RectTransform>().sizeDelta.y;
                if(topBottomGameObject.GetComponent<RectTransform>().anchorMin.y==0)
                    pos.y = -pos.y;
                topBottomGameObject.GetComponent<RectTransform>().anchoredPosition = pos;
            }
            DodgePoint.GetComponent<CanvasGroup>().alpha = 0;

            Show();
        }

        // UI进入动画
        public void Show()
        {
            foreach (var leftRightGameObject in leftRightGameObjects)
            {
                RectTransform rectTransform = leftRightGameObject.GetComponent<RectTransform>();
                Vector2 size = rectTransform.sizeDelta;
                Vector2 pos = rectTransform.anchoredPosition;
                int add = (rectTransform.anchorMin.x == 1 || rectTransform.anchorMax.x == 1) ? -1 : 1;
                pos.x = size.x * add + 20 * add;
                rectTransform.DOAnchorPos(pos,1f);
            }
            foreach (var topBottomGameObject in topBottomGameObjects)
            {
                RectTransform rectTransform = topBottomGameObject.GetComponent<RectTransform>();
                Vector2 size = rectTransform.sizeDelta;
                Vector2 pos = rectTransform.anchoredPosition;
                int add = rectTransform.anchorMin.y == 1 ? -1 : 1;
                pos.y = 20 * add;
                rectTransform.DOAnchorPos(pos,1f);
                
            }

            DodgePoint.GetComponent<CanvasGroup>().DOFade(1, 1f);
        }

        public void Hide()
        {
            
        }
    }
}