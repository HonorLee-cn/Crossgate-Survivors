using System;
using CGTool;
using Controller;
using DG.Tweening;
using Reference;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    // 主界面
    public class MainScreen:MonoBehaviour
    {
        [SerializeField,Header("Logo")] public RectTransform Logo;
        [SerializeField,Header("Man")] public RectTransform Man;
        [SerializeField,Header("Selector")] public RectTransform Selector;
        [SerializeField,Header("按钮组")] public CanvasGroup Buttons;
        [SerializeField,Header("UpdateNote")] public GameObject UpdateLog;
        [SerializeField,Header("更新提示")] public Text UpdateText;
        [SerializeField,Header("当前版本")] public Text CurrentVersion;
        
        [SerializeField,Header("背景人物")] public Sprite[] Sprites;

        private void OnEnable()
        {
            GetComponent<CanvasGroup>().alpha = 1;
            GameController.ChangeBGM(220);

            // 进入动画
            Buttons.alpha = 0;
            Man.GetComponent<Image>().sprite = Sprites[UnityEngine.Random.Range(0, Sprites.Length)];
            Man.anchoredPosition = new Vector2(150, 0);
            Man.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            Logo.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            Logo.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            
            Man.DOAnchorPos(new Vector2(0, 0), 0.5f);
            Man.GetComponent<Image>().DOFade(1, 0.5f).onComplete = () =>
            {
                Logo.GetComponent<Image>().DOFade(1, 0.3f);
                Logo.DOScale(Vector3.one, 0.3f).onComplete = () =>
                {
                    Buttons.DOFade(1, 0.5f);
                };
            };
        }

        // 开始游戏
        public void StartGame()
        {
            GlobalReference.UI.LevelTaskSelector.gameObject.SetActive(true);
            GetComponent<CanvasGroup>().DOFade(0,0.3f).onComplete = () =>
            {
                gameObject.SetActive(false);
            };
        }

        // 更新日志
        public void UpdateNote()
        {
            UpdateLog.SetActive(true);
        }

        // 关闭更新日志
        public void CloseNote()
        {
            UpdateLog.SetActive(false);
        }

        // 选择箭头动画
        public void Select(int i)
        {
            int y = -(60 * i + 25);
            Selector.GetComponent<RectTransform>().DOAnchorPos(new Vector3(-40, y, 0), 0.2f);
        }
    }
}