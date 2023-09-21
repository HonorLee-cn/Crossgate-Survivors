/**
 * 魔力宝贝图档解析脚本 - CGTool
 * 
 * @Author  HonorLee (dev@honorlee.me)
 * @Version 1.0 (2023-04-15)
 * @License GPL-3.0
 *
 * Graphic.cs 图档解析类
 */

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace CGTool
{
    public class GraphicData
    {
        //版本号
        public int Version;
        //索引
        public uint Index;
        //地图编号
        public uint MapSerial;
        //图档宽度
        public uint Width;
        //图档高度
        public uint Height;
        //图档偏移X
        public int OffsetX;
        //图档偏移Y
        public int OffsetY;
        //Palet调色板Index
        public int PaletIndex;
        //图档Sprite
        public Sprite Sprite;
        //图档主色调,用于小地图绘制
        public Color32 PrimaryColor;
    }
    public class Graphic
    {
        //缓存Addr  Version -> Addr -> PaletIndex -> GraphicData
        private static Dictionary<int, Dictionary<uint, Dictionary<int, GraphicData>>> _cache =
            new Dictionary<int, Dictionary<uint, Dictionary<int, GraphicData>>>();
        //
        // //缓存Index映射 Version -> Index -> PaletIndex -> GraphicData
        // private static Dictionary<int, Dictionary<uint, Dictionary<int, GraphicData>>> _indexCache =
        //     new Dictionary<int, Dictionary<uint, Dictionary<int, GraphicData>>>();
        //
        // //缓存MapSerial映射 Version -> MapSerial -> PaletIndex -> GraphicData
        // private static Dictionary<int, Dictionary<uint, Dictionary<int, GraphicData>>> _serialCache =
        //     new Dictionary<int, Dictionary<uint, Dictionary<int, GraphicData>>>();
        
        private static Dictionary<int,string> _graphicVersionPrefix = new Dictionary<int, string>()
        {
            //龙之沙漏 之前版本前图档数据
            {0,@"Graphic_\d+"},
            //龙之沙漏 版本图档数据
            {1,@"GraphicEx_\d+"}
        };
        
        private static List<string> _graphicPaths = new List<string>();
        
        //地图地面Texture合批
        //地图图档PaletIndex -> MapS 索引
        // private static Dictionary<int,int> _mapPaletMap = new Dictionary<int, int>();
        //地图图档Serial -> Palet ->MapIndex 索引
        private static Dictionary<int,Dictionary<int,int>> _mapSerialMap = new Dictionary<int,Dictionary<int,int>>();
        //地图图档MapIndex -> Palet -> Texture 索引
        private static Dictionary<int,Dictionary<int,Texture2D>> _mapTextureMap = new Dictionary<int,Dictionary<int,Texture2D>>();
        //地图图档MapIndex -> MapIndexList 索引库存,索引库容量为 2048x2048尺寸Texture可存储64x48地面图档的数量,即 2048/48(42) * 2048/64(32) ~= 1344
        //解压地面图档时动态分配索引库存 MapIndex -> Count
        private static List<int> _mapIndexLib = new List<int>();
        //Graphic字节读取器缓存
        private static BinaryReader[] _fileReaderCache = new BinaryReader[_graphicVersionPrefix.Count];

        // 初始化
        public static void Init()
        {
            //查找目录文件
            DirectoryInfo directoryInfo = new DirectoryInfo(CGTool.BaseFolder);
            FileInfo[] files = directoryInfo.GetFiles();
            for (int i = 0; i < _graphicVersionPrefix.Count; i++)
            {
                foreach (FileInfo file in files)
                {
                    if (Regex.IsMatch(file.Name, _graphicVersionPrefix[i]))
                    {
                        _graphicPaths.Add(file.Name);
                        BinaryReader fileReader;
                        string fileName = _graphicPaths[i];
                        FileInfo fileInfo = new FileInfo(CGTool.BaseFolder + "/" + fileName);
                        if (!fileInfo.Exists) return;
                        //创建流读取器
                        FileStream fileStream = fileInfo.OpenRead();
                        fileReader = new BinaryReader(fileStream);
                        _fileReaderCache[i] = fileReader;
                        break;
                    }    
                }
                
            }
        }

        //根据地址获取GraphicData
        public static GraphicData GetGraphicData(GraphicInfoData graphicInfoData,int PaletIndex=0,bool asMapGround=false)
        {
            GraphicData graphicData = null;

            //缓存数据
            if (_cache.ContainsKey(graphicInfoData.Version))
            {
                if (_cache[graphicInfoData.Version].ContainsKey(graphicInfoData.Addr))
                {
                    if (_cache[graphicInfoData.Version][graphicInfoData.Addr].ContainsKey(PaletIndex))
                    {
                        graphicData = _cache[graphicInfoData.Version][graphicInfoData.Addr][PaletIndex];
                    }
                }
            }
            //无缓存则加载数据
            if (graphicData == null) graphicData = _loadGraphicData(graphicInfoData, PaletIndex, asMapGround);
            
            return graphicData;
        }

        //初始化加载GraphicData
        private static GraphicData _loadGraphicData(GraphicInfoData graphicInfoData, int PaletIndex = 0,
            bool asMapGround = false)
        {
            GraphicData graphicData = new GraphicData();

            //获取图像数据
            List<Color32> pixels = UnpackGraphic(graphicInfoData, PaletIndex);
            graphicData.PrimaryColor = pixels.Last();
            pixels.RemoveAt(pixels.Count - 1);

            //直接通过Texture2D做偏移,并转为Sprite的偏移量
            Vector2 offset = new Vector2(0f, 1f);
            offset.x += -(graphicInfoData.OffsetX * 1f) / graphicInfoData.Width;
            offset.y -= (-graphicInfoData.OffsetY * 1f) / graphicInfoData.Height;
            
            //创建Texture2D对象
            Texture2D texture2D;
            Sprite sprite;

            texture2D = new Texture2D((int) graphicInfoData.Width, (int) graphicInfoData.Height,
                TextureFormat.RGBA4444, false, false);
            
            texture2D.filterMode = FilterMode.Point;
            texture2D.SetPixels32(pixels.ToArray());
            // texture2D.LoadRawTextureData(rawTextureData);
            texture2D.Apply();
            
            sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), offset, 1,1,SpriteMeshType.FullRect);


            //写入数据
            graphicData.Version = graphicInfoData.Version;
            graphicData.Index = graphicInfoData.Index;
            graphicData.MapSerial = graphicInfoData.Serial;
            graphicData.Width = graphicInfoData.Width;
            graphicData.Height = graphicInfoData.Height;
            graphicData.OffsetX = graphicInfoData.OffsetX;
            graphicData.OffsetY = graphicInfoData.OffsetY;
            graphicData.PaletIndex = PaletIndex;
            graphicData.Sprite = sprite;

            //缓存
            if (!_cache.ContainsKey(graphicInfoData.Version))
                _cache.Add(graphicInfoData.Version, new Dictionary<uint, Dictionary<int, GraphicData>>());
            if(!_cache[graphicInfoData.Version].ContainsKey(graphicInfoData.Addr)) _cache[graphicInfoData.Version].Add(graphicInfoData.Addr,new Dictionary<int, GraphicData>());
            if (!_cache[graphicInfoData.Version][graphicInfoData.Addr].ContainsKey(PaletIndex))
                _cache[graphicInfoData.Version][graphicInfoData.Addr].Add(PaletIndex, graphicData);
            
            return graphicData;
        }
    
        //地图sprite缓存  <地图索引,<调色板索引,Sprite>>
        // private static Dictionary<int,Dictionary<int,Dictionary<int,GraphicData>>> _mapSpriteMap = new Dictionary<int, Dictionary<int, Dictionary<int, GraphicData>>>();

        //预备地图缓存
        public static Dictionary<int, GraphicData> PrepareMapGroundTexture(int MapID, int PaletIndex,
            List<GraphicInfoData> groundInfos)
        {
            //如果已经缓存过,则直接返回
            // if(_mapSpriteMap.ContainsKey(MapID) && _mapSpriteMap[MapID].ContainsKey(PaletIndex)) return _mapSpriteMap[MapID][PaletIndex];
            //如果没有缓存过,则创建缓存
            // if(!_mapSpriteMap.ContainsKey(MapID)) _mapSpriteMap.Add(MapID,new Dictionary<int, Dictionary<int, GraphicData>>());
            Dictionary<int, GraphicData> graphicDataDic = new Dictionary<int, GraphicData>();
            // _mapSpriteMap[MapID].Add(PaletIndex, graphicDataDic);
            
            List<GraphicData> graphicDatas = new List<GraphicData>();
            Texture2D texture2D = null;
            
            for (var i = 0; i < groundInfos.Count; i++)
            {
                //每1344个图像合并一次,即不超过2048*2048尺寸
                if (i % 1344 == 0)
                {
                    //合并
                    if (i != 0) Combine(texture2D, graphicDatas);
                    
                    //清空
                    graphicDatas.Clear();
                    int height = 2048;

                    if (i + 1344 > groundInfos.Count-1)
                    {
                        height = Mathf.CeilToInt((groundInfos.Count - i) / 32f) * 48;
                    }
                    texture2D = new Texture2D(2048, height, TextureFormat.RGBA4444, false, true);
                    texture2D.filterMode = FilterMode.Point;
                    //默认填充全透明
                    Color32[] colors = new Color32[2048 * height];
                    for (int j = 0; j < colors.Length; j++)
                    {
                        colors[j] = new Color32(0,0,0,0);
                    }
                    texture2D.SetPixels32(colors);
                }
                
                GraphicInfoData graphicInfoData = groundInfos[i];
                GraphicData graphicData = new GraphicData();
                
                //获取图像数据
                List<Color32> pixels = UnpackGraphic(graphicInfoData, PaletIndex);
                graphicData.PrimaryColor = pixels.Last();
                pixels.RemoveAt(pixels.Count - 1);
                
                int x = i % 32 * 64;
                int y = i / 32 * 48;

                texture2D.SetPixels32(x, y, (int) graphicInfoData.Width, (int) graphicInfoData.Height,
                    pixels.ToArray());

                //写入数据
                graphicData.Version = graphicInfoData.Version;
                graphicData.Index = graphicInfoData.Index;
                graphicData.MapSerial = graphicInfoData.Serial;
                graphicData.Width = graphicInfoData.Width;
                graphicData.Height = graphicInfoData.Height;
                graphicData.OffsetX = graphicInfoData.OffsetX;
                graphicData.OffsetY = graphicInfoData.OffsetY;
                graphicData.PaletIndex = PaletIndex;
                
                
                graphicDatas.Add(graphicData);
            }
            
            //最后一次合并
            if (graphicDatas.Count > 0) Combine(texture2D, graphicDatas);

            void Combine(Texture2D texture2D,List<GraphicData> graphicDatas)
            {
                texture2D.Apply();
                for (var i = 0; i < graphicDatas.Count; i++)
                {
                    GraphicData graphicDataPiece = graphicDatas[i];
                    //直接通过Texture2D做偏移,并转为Sprite的偏移量
                    Vector2 offset = new Vector2(0f, 1f);
                    offset.x += -(graphicDataPiece.OffsetX * 1f) / graphicDataPiece.Width;
                    offset.y -= (-graphicDataPiece.OffsetY * 1f) / graphicDataPiece.Height;
                        
                    int X = i % 32 * 64;
                    int Y = i / 32 * 48;
                        
                    Sprite sprite = Sprite.Create(texture2D, new Rect(X, Y, (int)graphicDataPiece.Width, (int)graphicDataPiece.Height),offset, 1, 1, SpriteMeshType.FullRect);
                    graphicDataPiece.Sprite = sprite;

                    graphicDataDic.Add((int) graphicDataPiece.MapSerial, graphicDataPiece);
                }
            }

            return graphicDataDic;
        }
        private static List<Color32> UnpackGraphic(GraphicInfoData graphicInfoData,int PaletIndex){
            List<Color32> pixels = new List<Color32>();
            //获取调色板
            List<Color32> palet = Palet.GetPalet(PaletIndex);

            //调整流指针
            BinaryReader fileReader = _fileReaderCache[graphicInfoData.Version];
            fileReader.BaseStream.Position = graphicInfoData.Addr;

            //读入目标字节集
            byte[] Content = fileReader.ReadBytes((int) graphicInfoData.Length);

            //读取缓存字节集
            BinaryReader contentReader = new BinaryReader(new MemoryStream(Content));

            //16字节头信息
            byte[] HEAD = contentReader.ReadBytes(2);
            int Version = contentReader.ReadByte();
            int Unknow = contentReader.ReadByte();
            uint Width = contentReader.ReadUInt32();
            uint Height = contentReader.ReadUInt32();
            uint Length = contentReader.ReadUInt32();

            
            //数据长度
            uint contentLen = Length - 16;
            int pixelLen = (int) (graphicInfoData.Width * graphicInfoData.Height);

            int[] paletIndex;
            if (graphicInfoData.UnpackedPaletIndex == null)
            {
                //解压数据
                byte[] contentBytes = contentReader.ReadBytes((int) contentLen);
                NativeArray<byte> bytes = new NativeArray<byte>((int) contentBytes.Length, Allocator.TempJob);
                
                bytes.CopyFrom(contentBytes);

                // Debug.Log(contentBytes.Length + "   " + bytes.Length);
                NativeArray<int> colorIndexs =
                    new NativeArray<int>(pixelLen, Allocator.TempJob);


                DecompressJob decompressJob = new DecompressJob()
                {
                    bytes = bytes,
                    compressd = Version != 0,
                    colorIndexs = colorIndexs
                };
                // decompressJob.Execute();
                decompressJob.Schedule().Complete();
                bytes.Dispose();
                paletIndex = colorIndexs.ToArray();
                graphicInfoData.UnpackedPaletIndex = paletIndex;
                colorIndexs.Dispose();
            }
            else
            {
                paletIndex = graphicInfoData.UnpackedPaletIndex;
            }

            //释放连接
            contentReader.Dispose();
            contentReader.Close();
            
            //主色调色值
            int r = 0;
            int g = 0;
            int b = 0;
            foreach (int index in paletIndex)
            {
                // Debug.Log(index);
                Color32 color32 = palet[index];
                pixels.Add(color32);
                r += color32.r;
                g += color32.g;
                b += color32.b;
            }
            //主色调计算及提亮
            r = r / pixels.Count * 3;
            g = g / pixels.Count * 3;
            b = b / pixels.Count * 3;
            if (r > 255) r = 255;
            if (g > 255) g = 255;
            if (b > 255) b = 255;

            int len = (int) (graphicInfoData.Width * graphicInfoData.Height);

            if (pixels.Count != len)
            {
                if (pixels.Count > len)
                {
                    pixels = pixels.GetRange(0, len);
                }
                else
                {
                    Color32[] temc = new Color32[len - pixels.Count];
                    ArrayList.Repeat(Color.clear, len - pixels.Count).CopyTo(temc);
                    pixels.AddRange(temc);
                }
            }

            
            //主色调加入最后
            pixels.Add(new Color32((byte) r, (byte) g, (byte) b, 255));

                
            return pixels;
        }
    }
    
    
    //解压缩交给IJob处理
    [BurstCompile]
    public struct DecompressJob : IJob
    {
        [ReadOnly]
        public NativeArray<byte> bytes;
        public bool compressd;
        public NativeArray<int> colorIndexs;
    
        private int _maxIndex;
        private int _index;
        private int _colorIndex;
    
        private int NextByte()
        {
            _index++;
            if (_index > _maxIndex) return -1;
            return bytes[_index];
        }
        private void AddColorIndex(int index)
        {
            colorIndexs[_colorIndex] = index;
            _colorIndex++;
        }
        [BurstCompile]
        public void Execute()
        {
            _maxIndex = bytes.Length - 1;
            _index = -1;
            _colorIndex = 0;
            
            if (!compressd)
            {
                while (_index<=_maxIndex)
                {
                    int index = NextByte();
                    if(index==-1) break;
                    AddColorIndex(index);
                }
            }
            else
                //压缩型数据解压
            {
                // int count = 0;
                while (_index<=_maxIndex)
                {
                    // count++;
                    int head = NextByte();
                    if(head==-1) break;

                    int repeat = 0;
                    if (head < 0x10)
                    {
                        repeat = head;
                        for (var i = 0; i < repeat; i++)
                        {
                            AddColorIndex(NextByte());
                        }
    
                    }
                    else if (head < 0x20)
                    {
                        repeat = head % 0x10 * 0x100 + NextByte();
                        for (var i = 0; i < repeat; i++)
                        {
                            AddColorIndex(NextByte());
                        }
    
                    }
                    else if (head < 0x80)
                    {
                        repeat = head % 0x20 * 0x10000 + NextByte() * 0x100 + NextByte();
                        for (var i = 0; i < repeat; i++)
                        {
                            AddColorIndex(NextByte());
                        }
    
                    }
                    else if (head < 0x90)
                    {
                        repeat = head % 0x80;
                        int index = NextByte();
                        for (var i = 0; i < repeat; i++)
                        {
                            AddColorIndex(index);
                        }
    
                    }
                    else if (head < 0xa0)
                    {
                        int index = NextByte();
                        repeat = head % 0x90 * 0x100 + NextByte();
                        for (var i = 0; i < repeat; i++)
                        {
                            AddColorIndex(index);
                        }
    
                    }
                    else if (head < 0xc0)
                    {
                        int index = NextByte();
                        repeat = head % 0xa0 * 0x10000 + NextByte() * 0x100 + NextByte();
                        for (var i = 0; i < repeat; i++)
                        {
                            AddColorIndex(index);
                        }
    
                    }
                    else if (head < 0xd0)
                    {
                        repeat = head % 0xc0;
                        for (var i = 0; i < repeat; i++)
                        {
                            AddColorIndex(256);
                        }
    
                    }
                    else if (head < 0xe0)
                    {
                        repeat = head % 0xd0 * 0x100 + NextByte();
                        for (var i = 0; i < repeat; i++)
                        {
                            AddColorIndex(256);
                        }
    
                    }
                    else
                    {
                        repeat = head % 0xe0 * 0x10000 + NextByte() * 0x100 + NextByte();
                        for (var i = 0; i < repeat; i++)
                        {
                            AddColorIndex(256);
                        }
                    }
                }
            }
        }
    }
}
