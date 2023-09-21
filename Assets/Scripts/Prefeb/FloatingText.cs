using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Prefeb
{
    // 浮动 文字
    public class FloatingText:MonoBehaviour
    {
        [SerializeField,Header("文字框")] public Text Text;

        public void Show(string text,Color color = default,int size = 14)
        {
            
            Text.text = text;
            Text.color = color;
            Text.fontSize = size;
            gameObject.SetActive(true);

            Text.DOFade(0, 2f);
            
            Vector3 pos = transform.position;
            pos += new Vector3(0, 30, 0);
            Text.transform.DOMove(pos, 2f).onComplete = () =>
            {
                Destroy(gameObject);
            };

        }
    }
}