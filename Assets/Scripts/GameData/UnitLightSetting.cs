using System.Collections.Generic;
using UnityEngine;

namespace GameData
{
    //光环设置
    public class UnitLightSetting
    {
        public class UnitLightInfo
        {
            //光环名称
            public string Name;
            //光环对应加成属性
            public string Prop;
            //光环加成数值
            public int Value;
            //光环加成数值比例
            public int ValueRate;
            //光环颜色
            public Color Color;
        }

        public static List<UnitLightInfo> UnitLightInfos = new List<UnitLightInfo>()
        {
            new UnitLightInfo()
            {
                Name  = "力量光环",
                Prop = "ATK",
                ValueRate = 20,
                Color = new Color(0f,1,1f,0.4f)
            },
            new UnitLightInfo()
            {
                Name  = "防御光环",
                Prop = "DEF",
                ValueRate = 20,
                Color = new Color(1f,1,0f,0.4f)
            },
            new UnitLightInfo()
            {
                Name  = "速度光环",
                Prop = "SPD",
                ValueRate = 20,
                Color = new Color(0f,1f,0,0.4f)
            },
            new UnitLightInfo()
            {
                Name  = "生命光环",
                Prop = "MaxHP",
                ValueRate = 20,
                Color = new Color(1f,0,0,0.4f)
            },
        };
    }
}