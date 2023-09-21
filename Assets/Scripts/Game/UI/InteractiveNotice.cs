using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    // 人物上方交互提示
    public class InteractiveNotice:MonoBehaviour
    {
        [SerializeField,Header("说明")] public Text Notice;

        public void Show(string text)
        {
            gameObject.SetActive(true);
            Notice.text = text;
        }
        
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}