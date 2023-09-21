using Controller;
using GameData;
using Reference;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Prefeb
{
    // 关卡选择器
    public class LevelSelector:MonoBehaviour,IPointerClickHandler
    {
        [SerializeField,Header("封面图")] public Image Cover;
        [SerializeField,Header("关卡名")] public Text LevelName;
        [SerializeField,Header("关卡等级")] public Text LevelLevel;
        
        private LevelSetting.LevelInfo _levelInfo;
        public void SetLevelInfo(LevelSetting.LevelInfo levelInfo)
        {
            _levelInfo = levelInfo;
            Cover.sprite = GlobalReference.UI.LevelCover[_levelInfo.CoverIndex];
            LevelName.text = _levelInfo.Name;
            LevelLevel.text = "Lv." + _levelInfo.Difficulty;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            GameController.SelectLevel(_levelInfo);
            GameController.LevelInfo = _levelInfo;
            //同级的所有LevelSelector
            LevelSelector[] selectors = transform.parent.GetComponentsInChildren<LevelSelector>();
            for (int i = 0; i < selectors.Length; i++)
            {
                LevelSelector selector = selectors[i];
                if (selector == this)
                {
                    selector.GetComponent<Button>().interactable = false;
                    
                }
                else
                {
                    selector.GetComponent<Button>().interactable = true;
                }
            }
        }
    }
}