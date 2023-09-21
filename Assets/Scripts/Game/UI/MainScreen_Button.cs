using Reference;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
    // 主界面按钮
    public class MainScreen_Button:MonoBehaviour,IPointerEnterHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            GlobalReference.UI.MainScreen.Select(transform.GetSiblingIndex());

            foreach (MainScreen_Button mainScreenButton in transform.parent.GetComponentsInChildren<MainScreen_Button>())
            {
                mainScreenButton.GetComponentInChildren<Text>().color = Color.white;    
            }
            GetComponentInChildren<Text>().color = Color.yellow;
        }
        
    }
}