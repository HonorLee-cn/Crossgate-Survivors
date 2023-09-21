using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    // 加载界面
    public class LoadingScreen:MonoBehaviour
    {
        [SerializeField,Header("Logo")] public Image Logo;
        [SerializeField,Header("Loading")] public Text Loading;
        [SerializeField,Header("Error")] public GameObject ErrorObject;
        [SerializeField,Header("ErrorText")] public Text ErrorText;
    }
}