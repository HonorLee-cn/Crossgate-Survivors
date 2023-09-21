using System;
using GameData;
using Prefeb;
using Reference;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    // 选择关卡框架
    public class SelectLevel:MonoBehaviour
    {
        [SerializeField,Header("关卡选择容器")] public GameObject LevelSelectorContainer;

        private void OnEnable()
        {
            //清空LevelSelectorContainer.transform子对象
            for (int i = 0; i < LevelSelectorContainer.transform.childCount; i++)
            {
                Destroy(LevelSelectorContainer.transform.GetChild(i).gameObject);
            }

            for (int i = 0; i < LevelSetting.LevelInfos.Count; i++)
            {
                LevelSetting.LevelInfo info = LevelSetting.LevelInfos[i];
                LevelSelector selector = Instantiate(GlobalReference.Prefeb.LevelSelector, LevelSelectorContainer.transform);
                selector.SetLevelInfo(info);
            }
        }
    }
}