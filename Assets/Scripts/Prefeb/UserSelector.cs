using System;
using CGTool;
using Controller;
using GameData;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Prefeb
{
    // 角色选择器
    public class UserSelector:MonoBehaviour,IPointerClickHandler
    {
        [SerializeField,Header("背景框架")] public Image Background;
        [SerializeField,Header("人物动画")] public AnimePlayer Player;
        [SerializeField,Header("名称")] public Text NameText;
        [SerializeField,Header("简介")] public Text DescText;
        [SerializeField,Header("得意技")] public Text SkillText;
        [SerializeField,Header("伤害")] public Slider DamageSlider;
        [SerializeField,Header("血量")] public Slider HPSlider;
        [SerializeField,Header("魔法")] public Slider MPSlider;
        [SerializeField,Header("防御")] public Slider DEFSlider;
        [SerializeField,Header("速度")] public Slider SPDSlider;

        private PlayerSetting.PlayerInfo _playerInfo;
        public void SetUserInfo(PlayerSetting.PlayerInfo info)
        {
            _playerInfo = info;
            Player.Serial = info.Serial;
            Player.DirectionType = Anime.DirectionType.SouthWest;
            NameText.text = info.Name;
            DescText.text = info.Desc;
            SkillText.text = "得意技：" + info.Skill;
            DamageSlider.value = info.ATK;
            HPSlider.value = info.HP;
            MPSlider.value = info.MP;
            DEFSlider.value = info.DEF;
            SPDSlider.value = info.SPD;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            GameController.PlayerInfo = _playerInfo;
            //同级的所有UserSelector
            UserSelector[] selectors = transform.parent.GetComponentsInChildren<UserSelector>();
            for (int i = 0; i < selectors.Length; i++)
            {
                UserSelector selector = selectors[i];
                if (selector == this)
                {
                    selector.GetComponent<Button>().interactable = false;
                    selector.Player.ActionType = Anime.ActionType.Attack;
                }
                else
                {
                    selector.GetComponent<Button>().interactable = true;
                    selector.Player.ActionType = Anime.ActionType.Stand;
                }
            }
        }
    }
}