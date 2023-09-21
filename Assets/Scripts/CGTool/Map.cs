/**
 * 魔力宝贝图档解析脚本 - CGTool
 * 
 * @Author  HonorLee (dev@honorlee.me)
 * @Version 1.0 (2023-04-15)
 * @License GPL-3.0
 *
 * Map.cs 服务端地图解析类
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CGTool
{
    //地图文件信息
    public class MapFileInfo
    {
        public uint Serial;
        public string Name;
        public string FileName;
    }
    //地图块数据
    public class MapBlockData
    {
        public GraphicInfoData GraphicInfo;
        // public uint GraphicIndex;
        public uint MapSerial;
        public int FixPlayerZ;
        public int ObjectZIndex = 0;
    }
    //地图信息
    public class MapInfo
    {
        //地图编号
        public uint Serial;
        //地图宽度
        public int Width;
        //地图高度
        public int Height;
        // 地图名称
        public string Name;
        // 调色板号 - 默认 -1 表示自动
        public int Palet = -1;
        //未知数据
        public byte[] Unknow;
        //地面数据
        public List<MapBlockData> GroundDatas = new List<MapBlockData>();
        //地表数据
        public List<MapBlockData> ObjectDatas = new List<MapBlockData>();
        public bool[] BlockedIndexs;
        public float[] FixPlayerZs;
        //地图坐标二维数组,用以记录可行走区域并作为自动寻路的数据参考
        public bool[,] MapNodes;
    }
    
    
    public class Map
    {
        //Z轴修正值
        public static readonly int FixZIndex = 24;
        
        //缓存数据
        private static Dictionary<uint, MapInfo> _cache = new Dictionary<uint, MapInfo>();
        private static Dictionary<uint, MapFileInfo> _mapIndexFiles = new Dictionary<uint, MapFileInfo>(); 

        //初始化地图文件列表
        public static void Init()
        {
            DirectoryInfo mapDirectory = new DirectoryInfo(CGTool.MapFolder);
            FileInfo[] mapFiles = mapDirectory.GetFiles();
            string match = @"^(\d+)_?(.+)?$";
            foreach (var fileInfo in mapFiles)
            {
                string filename = fileInfo.Name;
                Match matchRet = Regex.Match(filename, match);
                if(!matchRet.Success) continue;
                
                MapFileInfo _file = new MapFileInfo();
                _file.Serial = uint.Parse(matchRet.Groups[1].Value);
                if(matchRet.Groups.Count > 1) _file.Name = matchRet.Groups[1].Value;
                _file.FileName = filename;
                _mapIndexFiles.Add(_file.Serial, _file);
            }
        }

        //获取全部地图列表
        public static List<MapFileInfo> GetMapList()
        {
            List<MapFileInfo> _list = new List<MapFileInfo>();
            foreach (var mapIndexFile in _mapIndexFiles)
            {
                _list.Add(mapIndexFile.Value);
            }

            return _list;
        }
        //获取地图数据
        public static MapInfo GetMap(uint serial)
        {
            
            //返回缓存数据
            if (_cache.ContainsKey(serial)) return _cache[serial];
            //加载数据
            MapInfo mapInfo = _loadMap(serial);
            return mapInfo;
        }

        //加载地图数据
        private static MapInfo _loadMap(uint serial)
        {
            // CGTool.Logger.Write("开始加载时间:" + DateTime.Now);
            if (!_mapIndexFiles.ContainsKey(serial)) return null;
            
            // print("找到地图文件: " + mapFileInfo.Name);
            FileStream mapFileStream = new FileStream(CGTool.MapFolder + "/" + _mapIndexFiles[serial].FileName, FileMode.Open);
            BinaryReader mapFileReader = new BinaryReader(mapFileStream);
            
            MapInfo mapInfo = new MapInfo();
            mapInfo.Serial = serial;

            //地图文件头
            byte[] mapHeader = mapFileReader.ReadBytes( 8);
            //地图名称
            byte[] mapNameBytes = mapFileReader.ReadBytes(32);
            string[] mapHead = System.Text.Encoding.GetEncoding("GBK").GetString(mapNameBytes).Split('|');
            mapInfo.Name = mapHead[0];
            
            // 调色板
            if (mapHead.Length>1){
                if(mapHead[1] != null || mapHead[1] != "") mapInfo.Palet = int.Parse(mapHead[1]);
            }


            //读取地图宽度
            byte[] bytes = mapFileReader.ReadBytes(2);
            Array.Reverse(bytes);
            mapInfo.Width = BitConverter.ToUInt16(bytes,0);
            //读取地图高度
            bytes = mapFileReader.ReadBytes(2);
            Array.Reverse(bytes);
            mapInfo.Height = BitConverter.ToUInt16(bytes,0);

            byte[] mapBytes = mapFileReader.ReadBytes((int) (mapInfo.Width * mapInfo.Height * 2));
            byte[] mapCoverBytes = mapFileReader.ReadBytes((int) (mapInfo.Width * mapInfo.Height * 2));

            mapFileReader.Dispose();
            mapFileReader.Close();
            mapFileStream.Close();

            // print(JsonUtility.ToJson(mapInfo));
            
            BinaryReader mapReader = new BinaryReader(new MemoryStream(mapBytes));
            BinaryReader mapCoverReader = new BinaryReader(new MemoryStream(mapCoverBytes));
            // BinaryReader mapInfoReader = new BinaryReader(new MemoryStream(mapInfoBytes));

            List<MapBlockData> tempGroundTiles = new List<MapBlockData>();
            List<MapBlockData> tempObjectTiles = new List<MapBlockData>();
            
            // CGTool.Logger.Write("开始解析时间:" + DateTime.Now);
            //原始数据为反转数据,即坐标起点为 1,1 排序方向为 y : 1=>0 x: 1=>0
            int len = mapInfo.Width * mapInfo.Height;
            for (int i = 0; i < len; i++)
            {
                //地面数据
                MapBlockData mapTile = null;
                bytes = mapReader.ReadBytes(2);
                Array.Reverse(bytes);
                uint mapGraphicSerial = BitConverter.ToUInt16(bytes,0);
                int Version = 0;
                if (mapGraphicSerial > 20000)
                {
                    mapGraphicSerial += 200000;
                    Version = 1;
                }
                GraphicInfoData graphicInfoData = GraphicInfo.GetGraphicInfoDataBySerial(Version, mapGraphicSerial);
                if (graphicInfoData != null)
                {
                    mapTile = new MapBlockData();
                    mapTile.GraphicInfo = graphicInfoData;
                    mapTile.MapSerial = mapGraphicSerial;
                }
                tempGroundTiles.Add(mapTile);
                
                MapBlockData mapCoverTile = null;
                bytes = mapCoverReader.ReadBytes(2);
                Array.Reverse(bytes);
                uint mapCoverGraphicSerial = BitConverter.ToUInt16(bytes,0);
                Version = 0;
                if (mapCoverGraphicSerial > 30000 || mapCoverGraphicSerial==25290)
                {
                    mapCoverGraphicSerial += 200000;
                    Version = 1;
                }
                graphicInfoData = GraphicInfo.GetGraphicInfoDataBySerial(Version, mapCoverGraphicSerial);
                if (graphicInfoData != null)
                {
                    mapCoverTile = new MapBlockData();
                    mapCoverTile.GraphicInfo = graphicInfoData;
                    mapCoverTile.MapSerial = mapCoverGraphicSerial;
                }
                tempObjectTiles.Add(mapCoverTile);
            }

            MapBlockData[] GroundTiles = new MapBlockData[len];
            MapBlockData[] ObjectTiles = new MapBlockData[len];
            bool[] blockedIndexs = new bool[len];
            float[] fixPlayerZs = new float[len];
            bool[,] nodes = new bool[mapInfo.Width, mapInfo.Height];

            // CGTool.Logger.Write("开始排序时间:" + DateTime.Now);
            //重新排序
            for (int y = 0; y < mapInfo.Height; y++)
            {
                for (int x = 0; x < mapInfo.Width; x++)
                {
                    // int index = i * (int) mapInfo.Width + ((int) mapInfo.Width - j - 1);
                    int _tmpindex = x + (mapInfo.Height - y - 1) * mapInfo.Width;
                    int index = x + y * mapInfo.Width;
                    
                    MapBlockData mapTile = tempGroundTiles[_tmpindex];
                    MapBlockData ObjectTile = tempObjectTiles[_tmpindex];

                    GroundTiles[index] = mapTile;
                    ObjectTiles[index] = ObjectTile;

                    if (mapTile==null || mapTile.GraphicInfo.Blocked) blockedIndexs[index] = true;
                    if (ObjectTile!=null && ObjectTile.GraphicInfo !=null && ObjectTile.GraphicInfo.Blocked) blockedIndexs[index] = true;
                    
                    nodes[x, y] = !blockedIndexs[index];
                    
                    //角色默认层级
                    // int objectTileZIndex = index * FixZIndex;
                    fixPlayerZs[index] = 1;
                }
            }

            //整理Object Z轴层级遮挡及角色遮挡问题
            for (int y = 0; y < mapInfo.Height; y++)
            {
                for (int x = 0; x < mapInfo.Width; x++)
                {
                    int index = x + y * mapInfo.Width;
                    int objectTileZIndex = index * FixZIndex;

                    MapBlockData ObjectTile = ObjectTiles[index];
                    if(ObjectTile==null || ObjectTile.GraphicInfo==null) continue;
                    
                    //Object默认层级
                    ObjectTile.ObjectZIndex = objectTileZIndex;

                    //角色Z轴补正
                    //在自定义排序轴(1,1,-1)情况下,角色Z轴在物件y-1位置,到x+East位置,补正为48*x
                    //在物件South+1位置,到x+East位置,补正为-48*x
                    if (!ObjectTile.GraphicInfo.AsGround)
                    {
                        for(int i = x;i<(x+ObjectTile.GraphicInfo.East-1);i++)
                        {
                            int fix = 1;
                            int oy = y - 1;
                            int _index = (int) (oy * mapInfo.Width + i);
                            if (fixPlayerZs[_index] == 1) fixPlayerZs[_index] = fix * (i - x + 1) * 240f + 0.1f;

                            // fix = -1;
                            // oy = y + ObjectTile.GraphicInfo.South;
                            // _index = (int) (oy * mapInfo.Width + i);
                            // if (fixPlayerZs[_index] == 0) fixPlayerZs[_index] = fix * (i - x + 1) * 100;
                        }
                        for(int i=y+1;i<(y+ObjectTile.GraphicInfo.South);i++)
                        {
                            int fix = 1;
                            int ox = x - 1;
                            int _index = (int) (i * mapInfo.Width + ox);
                            if (fixPlayerZs[_index] == 1) fixPlayerZs[_index] = fix * (i - y - 1) * 240f + 0.1f;
                        }
                    }
                    else
                    {
                        // ObjectTile.ObjectZIndex = 0;
                    }


                    //如果物件占地范围大于1x1,则需要处理遮挡
                    if (ObjectTile.GraphicInfo.East > 1 || ObjectTile.GraphicInfo.South > 1)
                    {
                        //取物件占地中间点位置
                        // objectTileZIndex = (x + ObjectTile.GraphicInfo.East / 2 + (y + ObjectTile.GraphicInfo.South / 2) * mapInfo.Width) * FixZIndex;
                        // ObjectTile.ObjectZIndex = objectTileZIndex;
                        //取物件左上角位置Z轴复写默认Z轴
                        // ObjectTile.ObjectZIndex = (x + (y + ObjectTile.GraphicInfo.South) * mapInfo.Width) * FixZIndex;
                        
                        

                        for (int i = x; i < (x + ObjectTile.GraphicInfo.East); i++)
                        {
                            for (int j = y; j < (y+ ObjectTile.GraphicInfo.South); j++)
                            {
                                if(i>=mapInfo.Width || j>=mapInfo.Height) continue;
                                int _index = (int) (j * mapInfo.Width + i);
                                blockedIndexs[_index] = true;
                                nodes[i, j] = false;
                            }
                        }
                    }
                }
            }

            mapInfo.GroundDatas = GroundTiles.ToList();
            mapInfo.ObjectDatas = ObjectTiles.ToList();
            mapInfo.BlockedIndexs = blockedIndexs;
            mapInfo.MapNodes = nodes;
            mapInfo.FixPlayerZs = fixPlayerZs;
            _cache[serial] = mapInfo;
            // CGTool.Logger.Write("地图解析完成时间:" + DateTime.Now);
            return mapInfo;
        }
    }
}
