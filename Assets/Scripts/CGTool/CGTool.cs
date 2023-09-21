/**
 * 魔力宝贝图档解析脚本 - CGTool
 * 
 * @Author  HonorLee (dev@honorlee.me)
 * @Version 1.0 (2023-04-15)
 * @License GPL-3.0
 *
 * CGTool.cs 入口文件
 */

using UnityEngine;

namespace CGTool
{
    public static class CGTool
    {
        //Bin基础目录
        public static string BaseFolder = System.Environment.CurrentDirectory + "/bin";
        //Palet调色板目录
        public static string PaletFolder = BaseFolder + "/pal";
        //Map地图文件目录
        public static string MapFolder = BaseFolder + "/map";

        //初始化CGTool
        public static void Init(string binPath = null)
        {
            if (!string.IsNullOrEmpty(binPath))
            {
                BaseFolder = binPath;
                PaletFolder = BaseFolder + "/pal";
                MapFolder = BaseFolder + "/map";
            }
            //初始化加载并缓存 0-15 调色板文件
            for (int i = 0; i < 16; i++) Palet.GetPalet(i);
            
            //初始化加载并缓存GraphicInfo配置表
            GraphicInfo.Init();
            
            //初始化图档解析器
            Graphic.Init();
            
            //初始化加载动画序列信息
            Anime.Init();

            //地图索引初始化
            Map.Init();

            Debug.Log("CGTool初始化完成");
        }

    }
}
