using System;
using GameData;
using Prefeb;
using Reference;
using UnityEngine;

namespace Game.UI
{
    // 人物选择框架
    public class SelectUser:MonoBehaviour
    {
        [SerializeField,Header("人物选择容器")] public GameObject UserSelectorContainer;

        private void OnEnable()
        {
            //清空UserSelectorContainer.transform子对象
            for (int i = 0; i < UserSelectorContainer.transform.childCount; i++)
            {
                Destroy(UserSelectorContainer.transform.GetChild(i).gameObject);
            }

            for (int i = 0; i < PlayerSetting.PlayerInfos.Count; i++)
            {
                PlayerSetting.PlayerInfo info = PlayerSetting.PlayerInfos[i];
                UserSelector selector = Instantiate(GlobalReference.Prefeb.UserSelector, UserSelectorContainer.transform);
                selector.SetUserInfo(info);
            }
        }
    }
}