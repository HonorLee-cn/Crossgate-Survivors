using System;
using System.Collections.Generic;
using CGTool;
using GameData;
using Reference;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Util
{
    
    public static class Toolkit
    {
        public static bool IsDebug = false;
        //将数字转变为K,M字符串,并用逗号分割
        public static string FormatNumberWithKMB(double number)
        {
            if (number < 1000)
            {
                return number.ToString("F0"); // 不需要分隔符
            }
            else if (number < 1000000)
            {
                return (number / 1000).ToString("F1") + "K"; // 转为千（K）并保留一位小数
            }
            else if (number < 1000000000)
            {
                return (number / 1000000).ToString("F1") + "M"; // 转为百万（M）并保留一位小数
            }
            else
            {
                return (number / 1000000000).ToString("F1") + "B"; // 转为十亿（B）并保留一位小数
            }
        }
        //将数字进行逗号分割
        public static string FormatNumberWithCommas(double number)
        {
            string formattedNumber = string.Format("{0:N0}", number); // 使用 "N0" 格式化符来添加逗号分隔符
            return formattedNumber;
        }

        //推过递归方式计算,假如hp=50,每级增长为0.2,那么1级为50+50*0.2=60.2级为(50+50*0.2)+(50+50*0.2)*0.2,以此类推
        public static int Calculate(int baseVal,float rate,int level)
        {
            if (level == 1)
            {
                return baseVal;
            }
            else
            {
                return (int) (Calculate(baseVal,rate,level-1) + Calculate(baseVal,rate,level-1) * rate);
            }
        }

        // 根据position获取当前位置Tilemap的坐标
        public static Vector3Int GetTilePosition(UnityEngine.Vector3 position)
        {
            return GlobalReference.UI.Tilemap_Ground.WorldToCell(position);
        }

        // 获取当前人物在Tilemap中所处位置的周边的随机位置
        // 获取的位置距离当前人物位置的半径x大于屏幕宽度/2，半径y大于屏幕高度/2
        // 但不超过地图Tilemap尺寸范围(2<x<levelinfo.MapWidth,2<y<levelinfo.MapHeight)
        // Tilemap为Isometric类型，宽高为64*48
        public static Vector3Int GetRandomLoc(Vector3Int loc,int MapWidth,int MapHeight)
        {
            int safeRange = 8;
            int maxRange = 20;

            List<Vector3Int> safeRangeList = new List<Vector3Int>();
            for (int x = loc.x - maxRange; x < loc.x + maxRange;x++)
            {
                for (int y = loc.y - maxRange; y < loc.y + maxRange;y++)
                {
                    if (x > 2 && x < MapWidth-2 && y > 2 && y < MapHeight-2)
                    {
                        if ((x < loc.x - safeRange && x> loc.x -maxRange) || (x > loc.x + safeRange && x < loc.x + maxRange) || (y < loc.y - safeRange && y > loc.y - maxRange) || (y > loc.y + safeRange && y < loc.y + maxRange))
                        {
                            safeRangeList.Add(new Vector3Int(x,y,0));
                        }
                    }
                }
            }
            
            Vector3Int randomLoc = safeRangeList[Random.Range(0, safeRangeList.Count)];
            return randomLoc;
        }
        
        //获取地图坐标对应的位置
        public static Vector3 TileLocationToPosition(Vector2Int location, int tileWidth = 64, int tileHeight = 48)
        {
            float halfWidth = tileWidth / 2f;
            float halfHeight = tileHeight / 2f;
            Vector3 pos = new Vector3();
            pos.x = location.y * -halfWidth + location.x * halfWidth - halfWidth;
            pos.y = location.y * halfHeight + location.x * halfHeight - halfHeight;
            return pos;
        }
        
        public static Anime.DirectionType GetTargetDirection(Vector2 fromPos,Vector2 targetPos)
        {
            Anime.DirectionType direction = Anime.DirectionType.NULL;
            Vector3 directionVector = fromPos - targetPos;
            float angle = Mathf.Atan2(directionVector.y, directionVector.x) * Mathf.Rad2Deg;
            angle = angle - 90;
            //根据角度获取方向,鼠标在屏幕正上方时,angle角度在22.5~-22.5之间,方向对应东北方向,鼠标在屏幕正下方时,方向对应西南方向,共8个方向
            // 计算向量与正北方向的角度
            if (angle <= 25 && angle > -25) direction = Anime.DirectionType.NorthEast;
            else if (angle <= -25 && angle > -75) direction = Anime.DirectionType.East;
            else if (angle <= -75 && angle > -105) direction = Anime.DirectionType.SouthEast;
            else if (angle <= -105 && angle > -155) direction = Anime.DirectionType.South;
            else if (angle <= -155 && angle > -205) direction = Anime.DirectionType.SouthWest;
            else if (angle <= -205 && angle > -255) direction = Anime.DirectionType.West;
            else if (angle <= -255 && angle > -285) direction = Anime.DirectionType.NorthWest;
            else if (angle <= -285 && angle > -335) direction = Anime.DirectionType.North;
            else if (angle <= -335 && angle > -385) direction = Anime.DirectionType.NorthEast;
            
            else if (angle > 25 && angle < 75) direction = Anime.DirectionType.North;
            else if (angle > 75 && angle < 105) direction = Anime.DirectionType.NorthWest;
            else if (angle > 105 && angle < 155) direction = Anime.DirectionType.West;
            else if (angle > 155 && angle < 205) direction = Anime.DirectionType.SouthWest;
            else if (angle > 205 && angle < 255) direction = Anime.DirectionType.South;
            else if (angle > 255 && angle < 285) direction = Anime.DirectionType.SouthEast;
            else if (angle > 285 && angle < 335) direction = Anime.DirectionType.East;
            else if (angle > 335 && angle < 385) direction = Anime.DirectionType.NorthEast;

            return direction;
        }
    }
}