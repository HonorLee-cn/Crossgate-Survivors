/**
 * 魔力宝贝图档解析脚本 - CGTool
 * 
 * @Author  HonorLee (dev@honorlee.me)
 * @Version 1.0 (2023-04-15)
 * @License GPL-3.0
 *
 * GraphicInfo.cs 图档索引解析类
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace CGTool
{
    //GraphicInfo数据块
    public class GraphicInfoData
    {
        //版本号
        public int Version;
        //4 bytes   索引
        public uint Index;
        //4 bytes   Graphic 地址
        public uint Addr;
        //4 bytes   Graphic 数据长度
        public uint Length;
        //4 bytes   Graphic 偏移 - X
        public int OffsetX;
        //4 bytes   Graphic 偏移 - Y
        public int OffsetY;
        //4 bytes   Graphic 宽
        public uint Width;
        //4 bytes   Graphic 高
        public uint Height;
        //4 bytes   Graphic East占地
        public int East;
        //4 bytes   Graphic South 占地
        public int South;
        //bool      穿越标识
        public bool Blocked;
        //1 byte    作为地面无层级遮挡[Test]
        public bool AsGround;
        //4 bytes   未知标识
        public byte[] Unknow;
        //4 bytes   编号
        public uint Serial;
        public int[] UnpackedPaletIndex;
    }

    public class GraphicInfo:MonoBehaviour
    {
        // private static Logger _logger = new Logger("GraphicInfo", false);
        //版本索引字典    版本编号
        private static Dictionary<int, List<GraphicInfoData>> _cache = new Dictionary<int, List<GraphicInfoData>>();

        //版本-Addr映射字典   版本编号 -> Index -> GraphicInfoData
        private static Dictionary<int, Dictionary<uint, GraphicInfoData>>
            _indexDict = new Dictionary<int, Dictionary<uint, GraphicInfoData>>();
        
        //版本-Map编号映射字典  版本编号 -> MapSerial -> GraphicInfoData
        private static Dictionary<int, Dictionary<uint, GraphicInfoData>>
            _SerialDict = new Dictionary<int, Dictionary<uint, GraphicInfoData>>();

        
        private static Dictionary<int,string> _graphicInfoVersionPrefix = new Dictionary<int, string>()
        {
            //龙之沙漏 之前版本前Info数据
            {0,@"GraphicInfo_\d+"},
            //龙之沙漏 版本Info数据
            {1,@"GraphicInfoEx_\d+"}
        };
        
        private static List<string> _graphicInfoPaths = new List<string>();

        public static void Init()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(CGTool.BaseFolder);
            FileInfo[] files = directoryInfo.GetFiles();
            //查找所有GraphicInfo数据文件
            for (int i = 0; i < _graphicInfoVersionPrefix.Count; i++)
            {
                foreach (FileInfo fileInfo in files)
                {
                    if (Regex.IsMatch(fileInfo.Name, _graphicInfoVersionPrefix[i]))
                    {
                        _graphicInfoPaths.Add(fileInfo.Name);
                        List<GraphicInfoData> list = GetGraphicInfo(i);
                        Debug.Log("初始化GraphicInfo数据,版本号:" + i + ",数据量:" + list.Count + "条");
                        break;
                    }
                }
            }
        }

        //获取GraphicInfo数据,Info数据加载后会缓存
        public static List<GraphicInfoData> GetGraphicInfo(int Version)
        {
            //返回缓存数据
            if (_cache.ContainsKey(Version)) return _cache[Version];
            
            //初始化映射库
            _indexDict.Add(Version,new Dictionary<uint, GraphicInfoData>());
            _SerialDict.Add(Version,new Dictionary<uint, GraphicInfoData>());
            //加载并初始化数据
            List<GraphicInfoData> infoDatas = _loadGraphicInfo(Version);
            _cache.Add(Version, infoDatas);
            
            return infoDatas;
        }
        //通过编号获取GraphicInfo数据
        public static GraphicInfoData GetGraphicInfoDataBySerial(int Version, uint Serial)
        {
            GraphicInfoData graphicInfoData = null;
            if (_SerialDict.ContainsKey(Version))
            {
                _SerialDict[Version].TryGetValue(Serial, out graphicInfoData);
                // graphicInfoData = _mapSerialDict[Version][MapSerial];
            }

            return graphicInfoData;
        }
        //通过编号获取GraphicInfo数据
        public static GraphicInfoData GetGraphicInfoDataBySerial(uint Serial)
        {
            int Version = Serial >= 2000000 ? 1 : 0;
            GraphicInfoData graphicInfoData = null;
            if (_SerialDict.ContainsKey(Version) && _SerialDict[Version].ContainsKey(Serial))
            {
                
                graphicInfoData = _SerialDict[Version][Serial];
            }

            return graphicInfoData;
        }
        //通过索引获取GraphicInfo数据
        public static GraphicInfoData GetGraphicInfoDataByIndex(int Version, uint Index)
        {
            GraphicInfoData graphicInfoData = null;
            if (_indexDict.ContainsKey(Version) && _indexDict[Version].ContainsKey(Index))
            {
                graphicInfoData = _indexDict[Version][Index];
            }

            return graphicInfoData;
        }
        
        
        
        //初始化加载GraphicInfo
        private static List<GraphicInfoData> _loadGraphicInfo(int Version)
        {
            //查找Info文件
            string fileName = _graphicInfoPaths[Version];
            FileInfo file = new FileInfo(CGTool.BaseFolder + "/" + fileName);
            if (!file.Exists) return null;

            //创建流读取器
            FileStream fileStream = file.OpenRead();
            BinaryReader fileReader = new BinaryReader(fileStream); 
            
            //解析Info数据表
            List<GraphicInfoData> infoDatas = new List<GraphicInfoData>();
            long DataLength = fileStream.Length/40;
            for (int i = 0; i < DataLength; i++)
            {
                GraphicInfoData graphicInfoData = new GraphicInfoData();
                graphicInfoData.Version = Version;
                graphicInfoData.Index = BitConverter.ToUInt32(fileReader.ReadBytes(4),0);
                graphicInfoData.Addr = BitConverter.ToUInt32(fileReader.ReadBytes(4),0);
                graphicInfoData.Length = BitConverter.ToUInt32(fileReader.ReadBytes(4),0);
                graphicInfoData.OffsetX = BitConverter.ToInt32(fileReader.ReadBytes(4),0);
                graphicInfoData.OffsetY = BitConverter.ToInt32(fileReader.ReadBytes(4),0);
                graphicInfoData.Width = BitConverter.ToUInt32(fileReader.ReadBytes(4),0);
                graphicInfoData.Height = BitConverter.ToUInt32(fileReader.ReadBytes(4),0);
                graphicInfoData.East = fileReader.ReadByte();
                graphicInfoData.South = fileReader.ReadByte();
                graphicInfoData.Blocked =  fileReader.ReadByte() == 0;
                graphicInfoData.AsGround = fileReader.ReadByte() == 1;
                graphicInfoData.Unknow = fileReader.ReadBytes(4);
                graphicInfoData.Serial = BitConverter.ToUInt32(fileReader.ReadBytes(4),0);

                //建立映射表
                if(!_indexDict[Version].ContainsKey(graphicInfoData.Index)) _indexDict[Version].Add(graphicInfoData.Index, graphicInfoData);
                if(graphicInfoData.Serial > 0 && !_SerialDict[Version].ContainsKey(graphicInfoData.Serial)) _SerialDict[Version].Add(graphicInfoData.Serial, graphicInfoData);
                
                infoDatas.Add(graphicInfoData);

                // _logger.Write("Index: " + graphicInfoData.Index + " Addr: " + graphicInfoData.Addr + 
                //               " Width: " + graphicInfoData.Width + 
                //               " Height: " + graphicInfoData.Height +
                //               " OffsetX: " + graphicInfoData.OffsetX +
                //               " OffsetY: " + graphicInfoData.OffsetY +
                //               " East: " + graphicInfoData.East +
                //               " South: " + graphicInfoData.South +
                //               " Blocked: " + graphicInfoData.Blocked +
                //               " Unknow: " + BitConverter.ToString(graphicInfoData.Unknow).Replace("-", ",") +
                //               " MapSerial: " + graphicInfoData.MapSerial);
            }
            // CGTool.Logger.Write("加载GraphicInfo - 版本: " + Version + " 文件: " + fileName + " 贴图总量: "+ infoDatas.Count);
            return infoDatas;
        }
    }
}