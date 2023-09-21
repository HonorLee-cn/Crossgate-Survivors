using System;
using Controller;
using GameData;
using UnityEngine;

namespace Game.UI
{
    // 关卡选择器
    public class LevelTaskSelector:MonoBehaviour
    {
        private void Start()
        {
            LevelSetting.LevelInfo levelInfo = LevelSetting.LevelInfos[0];
            GameController.SelectLevel(levelInfo);
            GameController.CheckStartAble();
        }
    }
}