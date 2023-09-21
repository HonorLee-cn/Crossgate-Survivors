using CGTool;
using UnityEngine;

namespace Prefeb
{
    // 地图单位(不可穿越)
    public class MapUnit:MonoBehaviour
    {
        [SerializeField,Header("物件动画")] public AnimePlayer AnimePlayer;
        [SerializeField,Header("物件Sprite")] public SpriteRenderer SpriteRenderer;
        
        private uint _serial;
        public uint AnimeSerial
        {
            get => AnimePlayer.Serial;
            set
            {
                AnimePlayer.Serial = value;
            }
        }
        
        public uint SpriteSerial
        {
            get => _serial;
            set
            {
                _serial = value;
                GraphicInfoData data = GraphicInfo.GetGraphicInfoDataBySerial(_serial);
                GraphicData graphicData = Graphic.GetGraphicData(data);
                SpriteRenderer.sprite = graphicData.Sprite;
            }
        }
    }
}