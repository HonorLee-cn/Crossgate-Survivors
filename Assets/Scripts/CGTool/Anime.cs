/**
 * 魔力宝贝图档解析脚本 - CGTool
 * 
 * @Author  HonorLee (dev@honorlee.me)
 * @Version 1.0 (2023-04-15)
 * @License GPL-3.0
 *
 * Anime.cs 动画基础类
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace CGTool
{
    //动画信息
    public class AnimeInfo
    {
        public int Version;
        //4 bytes   动画索引
        public uint Index;
        //4 bytes   动画文件地址
        public uint Addr;
        //2 bytes   动作数量
        public int ActionCount;
        //2 bytes   未知字节
        public byte[] Unknow;
        //动画数据  Direction -> ActionType -> AnimeData
        public Dictionary<int, Dictionary<int, AnimeDetail>> AnimeDatas = new Dictionary<int, Dictionary<int, AnimeDetail>>();
    }

    //动画帧数据
    public class AnimeFrameInfo
    {
        //图档编号
        public uint GraphicIndex;
        //宽度
        public int Width;
        //高度
        public int Height;
        //偏移X
        public int OffsetX;
        //偏移Y
        public int OffsetY;
        //音效编号
        public int AudioIndex;
        //动效编号
        public Anime.EffectType Effect;
        //GraphicInfo;
        public GraphicInfoData GraphicInfo;
        //动画Sprite
        public Dictionary<int,Sprite> AnimeSprites = new Dictionary<int, Sprite>();
    }

    //动画数据
    public class AnimeDetail
    {
        public uint Index;
        public int Version;
        public int Direction;
        public int ActionType;
        public uint CycleTime;
        public uint FrameCount;
        public Dictionary<int,Texture2D> AnimeTextures = new Dictionary<int, Texture2D>();
        // public Texture2D AnimeTexture;
        public AnimeFrameInfo[] AnimeFrameInfos;
        // public byte[] unknown;
    }
    //动画相关Enum类型
    public class Anime : MonoBehaviour
    {
        //方向
        public enum DirectionType
        {
            NULL=-1,
            North=0,
            NorthEast=1,
            East=2,
            SouthEast=3,
            South=4,
            SouthWest=5,
            West=6,
            NorthWest=7
        }
        //方向九宫映射表
        public static DirectionType[,] DirectionTypeMap = new DirectionType[3,3]
        {
            {DirectionType.North,DirectionType.NorthEast,DirectionType.East},
            {DirectionType.NorthWest,DirectionType.NULL,DirectionType.SouthEast},
            {DirectionType.West,DirectionType.SouthWest,DirectionType.South}
        };
        //动作
        public enum ActionType
        {
            NULL=-1,
            Stand=0,
            Walk=1,
            BeforeRun=2,
            Run=3,
            AfterRun=4,
            Attack=5,
            Magic=6,
            Throw=7,
            Hurt=8,
            Defence=9,
            Dead=10,
            Sit=11,
            Hi=12,
            Happy=13,
            Angry=14,
            Sad=15,
            Shake=16,
            Rock=17,
            Scissors=18,
            Paper=19,
            Fishing=20,
            
        }
        //动效
        public enum EffectType
        {
            Hit=1,
            HitOver=2
        }

        public enum PlayType
        {
            Loop,
            Once,
            OnceAndDestroy
        }
        //动画列表缓存    Index -> AnimeInfo
        private static Dictionary<uint, AnimeInfo> _animeInfoCache = new Dictionary<uint, AnimeInfo>();

        //动画序列文件前缀    Direction -> Action -> AnimeData
        private static Dictionary<int,string> _animeInfoVersionPrefex = new Dictionary<int, string>()
        {
            //龙之沙漏 之前版本前Info数据
            {0,@"AnimeInfo_\d+"},
            //龙之沙漏 版本Info数据
            {1,@"AnimeInfoEx_\d+"}
        };
        private static List<string> _animeInfoFilePaths = new List<string>();

        //动画数据文件前缀
        private static Dictionary<int,string> _animeDataVersionPrefex = new Dictionary<int, string>()
        {
            //龙之沙漏 之前版本前Data数据
            {0,@"Anime_\d+"},
            //龙之沙漏 版本Data数据
            {1,@"AnimeEx_\d+"}
        };
        private static List<string> _animeDataFilePaths = new List<string>();

        //初始化并缓存动画信息
        public static void Init()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(CGTool.BaseFolder);
            FileInfo[] files = directoryInfo.GetFiles();
            
            for (int i = 0; i < _animeInfoVersionPrefex.Count; i++)
            {
                foreach (FileInfo fileInfo in files)
                {
                    if (Regex.IsMatch(fileInfo.Name, _animeInfoVersionPrefex[i]))
                    {
                        _animeInfoFilePaths.Add(fileInfo.Name);
                    }
                    if(Regex.IsMatch(fileInfo.Name,_animeDataVersionPrefex[i]))
                    {
                        _animeDataFilePaths.Add(fileInfo.Name);
                    }
                }
            }
            if(_animeInfoFilePaths.Count==0) Debug.LogError("未找到动画信息文件");
            if(_animeDataFilePaths.Count==0) Debug.LogError("未找到动画数据文件");
            if(_animeDataFilePaths.Count!=_animeInfoFilePaths.Count) Debug.LogError("动画信息文件与动画数据文件数量不匹配");
            
            //加载动画信息
            for (int i = 0; i < _animeInfoFilePaths.Count; i++)
            {
                
                Dictionary<uint, AnimeInfo> animeInfos = _loadAnimeInfo(i);
                Debug.Log("加载动画信息版本:[" + i + "]  动画数量:" + animeInfos.Count);
            }
        }
        //获取动画数据信息
        public static AnimeInfo GetAnimeInfo(uint Index)
        {
            //返回缓存
            if (_animeInfoCache.ContainsKey(Index)) return _animeInfoCache[Index];
            //动画编号大于105000的属于 龙之沙漏 版本
            int Version = 0;
            if (Index >= 105000) Version = 1;
            Dictionary<uint, AnimeInfo> animeInfos = _loadAnimeInfo(Version);
            if (animeInfos.ContainsKey(Index)) return animeInfos[Index];
            return null;
        }

        //获取动画数据
        public static AnimeDetail GetAnimeDetail(uint serial,DirectionType Direction,ActionType Action)
        {
            AnimeInfo animeInfo = GetAnimeInfo(serial);
            if (animeInfo == null) return null;
            if (animeInfo.AnimeDatas.ContainsKey((int)Direction))
            {
                if (animeInfo.AnimeDatas[(int) Direction].ContainsKey((int) Action))
                {
                    AnimeDetail animeDetail = animeInfo.AnimeDatas[(int) Direction][(int) Action];
                    // if(animeDetail.AnimeTexture == null) prepareAnimeFrames(animeDetail);
                    return animeInfo.AnimeDatas[(int)Direction][(int) Action];
                }
            }

            return null;
        }

        //预处理动画图形合批烘焙
        public static void BakeAnimeFrames(AnimeDetail animeDetail,int paletIndex = 0)
        {
            if(animeDetail.AnimeTextures.ContainsKey(paletIndex)) return;
            //所有帧的图形数据
            GraphicData[] graphicDatas = new GraphicData[animeDetail.FrameCount];
            
            //合并后的Texture2D尺寸
            uint textureWidth = 0;
            uint textureHeight = 0;
            
            
            for (var i = 0; i < animeDetail.FrameCount; i++)
            {
                //载入图档
                GraphicInfoData graphicInfoData = GraphicInfo.GetGraphicInfoDataByIndex(animeDetail.Version,animeDetail.AnimeFrameInfos[i].GraphicIndex);
                if (graphicInfoData == null) continue;
                GraphicData graphicData = Graphic.GetGraphicData(graphicInfoData, paletIndex);
                if(graphicData == null) continue;
                graphicDatas[i] = graphicData;
                if(graphicData.Height > textureHeight) textureHeight = graphicData.Height;
                textureWidth += graphicData.Width + 5;
                animeDetail.AnimeFrameInfos[i].Width = (int) graphicData.Width;
                animeDetail.AnimeFrameInfos[i].Height = (int) graphicData.Height;
                animeDetail.AnimeFrameInfos[i].OffsetX = (int) graphicInfoData.OffsetX;
                animeDetail.AnimeFrameInfos[i].OffsetY = (int) graphicInfoData.OffsetY;
                animeDetail.AnimeFrameInfos[i].GraphicInfo = graphicInfoData;
            }
            //合并图档
            Texture2D texture2dMix = new Texture2D((int) textureWidth, (int) textureHeight, TextureFormat.RGBA4444, false,false);
            Color32 transparentColor = new Color32(0, 0, 0, 0);
            Color32[] transparentColors = new Color32[texture2dMix.width * texture2dMix.height];
            for (var i = 0; i < transparentColors.Length; i++)
            {
                transparentColors[i] = transparentColor;
            }
            texture2dMix.SetPixels32(transparentColors,0);
            
            int offsetX = 0;
            for (var i = 0; i < animeDetail.FrameCount; i++)
            {
                GraphicData graphicData = graphicDatas[i];
                if(graphicData == null) continue;
                texture2dMix.SetPixels32((int) offsetX, 0, (int) graphicData.Width,
                    (int) graphicData.Height,
                    graphicData.Sprite.texture.GetPixels32());
                offsetX += (int) graphicData.Width + 5;
            }
            texture2dMix.Apply();
            
            animeDetail.AnimeTextures.Add(paletIndex,texture2dMix);
            
            //创建动画每帧Sprite
            offsetX = 0;
            for (var l = 0; l < animeDetail.FrameCount; l++)
            {
                if(graphicDatas[l] == null) continue;
                AnimeFrameInfo animeFrameInfo = animeDetail.AnimeFrameInfos[l];
                Vector2 pivot = new Vector2(0f, 1f);
                pivot.x += -(animeFrameInfo.OffsetX * 1f) / animeFrameInfo.Width;
                pivot.y -= (-animeFrameInfo.OffsetY * 1f) / animeFrameInfo.Height;
                Sprite sprite = Sprite.Create(texture2dMix, new Rect(offsetX, 0,
                        animeDetail.AnimeFrameInfos[l].Width, animeDetail.AnimeFrameInfos[l].Height),
                    pivot, 1, 1, SpriteMeshType.FullRect);
                offsetX += animeDetail.AnimeFrameInfos[l].Width + 5;
                animeFrameInfo.AnimeSprites.Add(paletIndex, sprite);
            }
            
        }
        
        //加载动画数据
        private static Dictionary<uint, AnimeInfo> _loadAnimeInfo(int Version)
        {
            //查找Info文件
            string infoFileName = _animeInfoFilePaths[Version];
            string dataFileName = _animeDataFilePaths[Version];
            FileInfo infoFile = new FileInfo(CGTool.BaseFolder + "/" + infoFileName);
            FileInfo dataFile = new FileInfo(CGTool.BaseFolder + "/" + dataFileName);
            if (!infoFile.Exists || !dataFile.Exists) return null;

            //创建流读取器
            FileStream infoFileStream = infoFile.OpenRead();
            FileStream dataFileStream = dataFile.OpenRead();
            BinaryReader infoFileReader = new BinaryReader(infoFileStream);
            BinaryReader dataFileReader = new BinaryReader(dataFileStream);

            // Dictionary<uint, AnimeInfo> animeInfos = new Dictionary<uint, AnimeInfo>();
            long DataLength = infoFileStream.Length / 12;
            for (int i = 0; i < DataLength; i++)
            {
                //初始化对象
                AnimeInfo animeInfo = new AnimeInfo();
                animeInfo.Version = Version;
                animeInfo.Index = BitConverter.ToUInt32(infoFileReader.ReadBytes(4),0);
                animeInfo.Addr = BitConverter.ToUInt32(infoFileReader.ReadBytes(4),0);
                animeInfo.ActionCount = infoFileReader.ReadUInt16();
                animeInfo.Unknow = infoFileReader.ReadBytes(2);
                dataFileStream.Position = animeInfo.Addr;
                for (int j = 0; j < animeInfo.ActionCount; j++)
                {
                    AnimeDetail animeData = new AnimeDetail();
                    animeData.Index = animeInfo.Index;
                    animeData.Version = Version;
                    animeData.Direction = dataFileReader.ReadUInt16();
                    animeData.ActionType = dataFileReader.ReadUInt16();
                    animeData.CycleTime = BitConverter.ToUInt32(dataFileReader.ReadBytes(4),0);
                    animeData.FrameCount = BitConverter.ToUInt32(dataFileReader.ReadBytes(4),0);
                    animeData.AnimeFrameInfos = new AnimeFrameInfo[animeData.FrameCount];
                    
                    
                    // if (animeInfo.Index == 101201) Debug.Log("----------------------------------");
                    for (int k = 0; k < animeData.FrameCount; k++)
                    {
                        animeData.AnimeFrameInfos[k] = new AnimeFrameInfo();
                        //GraphicIndex序号
                        animeData.AnimeFrameInfos[k].GraphicIndex = BitConverter.ToUInt32(dataFileReader.ReadBytes(4),0);
                        //未知字节
                        // animeData.unknown = dataFileReader.ReadBytes(6);
                        // if (animeInfo.Index == 101201)
                        // {
                        //     byte[] tt = dataFileReader.ReadBytes(6);
                        // }
                        // else
                        // {
                        //     dataFileReader.ReadBytes(6);
                        // }
                        animeData.AnimeFrameInfos[k].OffsetX = BitConverter.ToInt16(dataFileReader.ReadBytes(2),0);
                        animeData.AnimeFrameInfos[k].OffsetY = BitConverter.ToInt16(dataFileReader.ReadBytes(2),0);
                        
                        //标识位
                        int flag = BitConverter.ToInt16(dataFileReader.ReadBytes(2),0);

                        // if (animeData.Index == 110053) Debug.Log("FLAG---" + " " + k + "  " + flag);

                        if (flag>20000)
                        {
                            //击打判定
                            animeData.AnimeFrameInfos[k].Effect = EffectType.Hit;
                            animeData.AnimeFrameInfos[k].AudioIndex = flag - 20000;
                        }
                        else if(flag>10000)
                        {
                            //攻击动作结束判定
                            animeData.AnimeFrameInfos[k].Effect = EffectType.HitOver;
                            animeData.AnimeFrameInfos[k].AudioIndex = flag - 10000;
                        }
                        else
                        {
                            animeData.AnimeFrameInfos[k].AudioIndex = flag;
                        }
                    }

                    if (!animeInfo.AnimeDatas.ContainsKey(animeData.Direction))
                        animeInfo.AnimeDatas.Add(animeData.Direction, new Dictionary<int, AnimeDetail>());

                    if (animeInfo.AnimeDatas[animeData.Direction].ContainsKey(animeData.ActionType))
                    {
                        animeInfo.AnimeDatas[animeData.Direction][animeData.ActionType] = animeData;
                    }
                    else
                    {
                        animeInfo.AnimeDatas[animeData.Direction].Add(animeData.ActionType, animeData);
                    }

                    if (_animeInfoCache.ContainsKey(animeInfo.Index))
                    {
                        _animeInfoCache[animeInfo.Index] = animeInfo;
                    }
                    else
                    {
                        _animeInfoCache.Add(animeInfo.Index, animeInfo);
                    }
                }

            }

            infoFileReader.Dispose();
            infoFileReader.Close();
            dataFileReader.Dispose();
            dataFileReader.Close();
            infoFileStream.Close();
            dataFileStream.Close();

            return _animeInfoCache;
        }
    }
}