using System;
using Controller;
using Reference;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    // ESC 暂停面板
    public class ESCPanel:MonoBehaviour
    {
        [SerializeField,Header("ESC面板")] public Button[] ScreenButtons;
        [SerializeField,Header("ESC面板")] public Button[] MouseButtons;
        [SerializeField,Header("鼠标")] public Mouse Mouse;

        private void OnEnable()
        {
            Time.timeScale = 0;
        }
        
        private void OnDisable()
        {
            Time.timeScale = 1;
        }

        // 重新开始
        public void Restart()
        {
            BattleController.Clear();
            GameController.IsGameStart = false;
            GlobalReference.UI.GameEnd.gameObject.SetActive(false);
            GlobalReference.UI.BattleUI.LearnSkill.gameObject.SetActive(false);
            GlobalReference.UI.BattleUI.gameObject.SetActive(false);
            GlobalReference.UI.LevelTaskSelector.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }

        // 回到主界面
        public void BackToMain()
        {
            BattleController.Clear();
            GameController.IsGameStart = false;
            GlobalReference.UI.GameEnd.gameObject.SetActive(false);
            GlobalReference.UI.BattleUI.LearnSkill.gameObject.SetActive(false);
            GlobalReference.UI.BattleUI.gameObject.SetActive(false);
            gameObject.SetActive(false);
            GlobalReference.UI.MainScreen.gameObject.SetActive(true);
        }

        // 改变分辨率
        public void ChangeScreen(int ScreenType)
        {
            switch (ScreenType)
            {
                case 0:
                    Screen.SetResolution(1366,768,false);
                    break;
                case 1:
                    Screen.SetResolution(1440,900,false);
                    break;
                case 2:
                    Screen.fullScreen = true;
                    Screen.SetResolution(Screen.currentResolution.width,Screen.currentResolution.height,true);

                    break;
            }
            GameController.ScreenType = ScreenType;

            for (var i = 0; i < ScreenButtons.Length; i++)
            {
                Button button = ScreenButtons[i];
                if (i == ScreenType)
                {
                    button.interactable = false;
                    button.GetComponentInChildren<Text>().color = Color.black;
                }
                else
                {
                    button.interactable = true;
                    button.GetComponentInChildren<Text>().color = Color.white;
                }
            }
        }

        // 改变鼠标模式(解决部分人鼠标显示大小异常)
        public void ChangeMouse(int change)
        {
            switch (change)
            {
                case 0:
                    Mouse.gameObject.SetActive(false);

                    break;
                case 1:
                    Mouse.gameObject.SetActive(true);
                    break;
            }
            for (var i = 0; i < MouseButtons.Length; i++)
            {
                Button button = MouseButtons[i];
                if (i == change)
                {
                    button.interactable = false;
                    button.GetComponentInChildren<Text>().color = Color.black;
                }
                else
                {
                    button.interactable = true;
                    button.GetComponentInChildren<Text>().color = Color.white;
                }
            }
        }
    }
    
}