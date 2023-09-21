using System;
using System.Collections.Generic;
using CGTool;
using GameData;
using Prefeb;
using Reference;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using Util;
using Random = UnityEngine.Random;

namespace Controller
{
    // 地图控制器
    public class MapController
    {
        private static LevelSetting.LevelInfo _levelInfo;
        private static List<Sprite> mapSprites = new List<Sprite>();
        private static List<Sprite> decorationSprites = new List<Sprite>();
        private static List<Sprite> unitSprites = new List<Sprite>();
        private static Sprite BorderSprite;

        // 初始化地图数据
        public static void InitMapData(LevelSetting.LevelInfo levelInfo = null)
        {
            ClearTiles();
            mapSprites = new List<Sprite>();
            decorationSprites = new List<Sprite>();
            unitSprites = new List<Sprite>();
            if (levelInfo != null)
            {
                _levelInfo = levelInfo;
            }
            else
            {
                _levelInfo = GlobalReference.BattleData.LevelInfo;
            }

            // 生成地面
            for (var i = 0; i < _levelInfo.GroundSerials.Count; i++)
            {
                uint serial = _levelInfo.GroundSerials[i];
                GraphicInfoData graphicInfo = GraphicInfo.GetGraphicInfoDataBySerial(serial);
                GraphicData graphicData = Graphic.GetGraphicData(graphicInfo,_levelInfo.PaletIndex);
                mapSprites.Add(graphicData.Sprite);
            }
            // 生成地面装饰
            for (var i = 0; i < _levelInfo.DecorationsSerials.Count; i++)
            {
                uint serial = _levelInfo.DecorationsSerials[i];
                GraphicInfoData graphicInfo = GraphicInfo.GetGraphicInfoDataBySerial(serial);
                GraphicData graphicData = Graphic.GetGraphicData(graphicInfo,_levelInfo.PaletIndex);
                decorationSprites.Add(graphicData.Sprite);
            }
            // 生成地图单位
            for (var i = 0; i < _levelInfo.UnitSerials.Count; i++)
            {
                uint serial = _levelInfo.UnitSerials[i];
                GraphicInfoData graphicInfo = GraphicInfo.GetGraphicInfoDataBySerial(serial);
                GraphicData graphicData = Graphic.GetGraphicData(graphicInfo,_levelInfo.PaletIndex);
                unitSprites.Add(graphicData.Sprite);
            }
            // 生成地图边界
            BorderSprite = Graphic.GetGraphicData(GraphicInfo.GetGraphicInfoDataBySerial(_levelInfo.BorderSerial),_levelInfo.PaletIndex).Sprite;
            RenderMap();
            CreateObjects();
        }
        
        // 渲染地图
        public static void RenderMap()
        {
            List<Vector3Int> groundPositions = new List<Vector3Int>();
            List<Vector3Int> decorationPositions = new List<Vector3Int>();
            List<Tile> groundTiles = new List<Tile>();
            List<Tile> decorationTiles = new List<Tile>();
            
            for (int x = 0; x < _levelInfo.MapWidth; x++)
            {
                for (int y = 0; y < _levelInfo.MapHeight; y++)
                {
                    Tile tile = Tile.CreateInstance(typeof(Tile)) as Tile;
                    tile.sprite = mapSprites[Random.Range(0, mapSprites.Count-1)];
                    groundPositions.Add(new Vector3Int(x, y, 0));
                    groundTiles.Add(tile);
                    
                    //  地图装饰生成概率
                    bool isDecoration = Random.Range(0, 100) < 2;
                    if (!isDecoration) continue;
                    tile = Tile.CreateInstance(typeof(Tile)) as Tile;
                    tile.sprite = decorationSprites[Random.Range(0, decorationSprites.Count-1)];
                    int zindex = (int)(x + y * _levelInfo.MapWidth);
                    decorationPositions.Add(new Vector3Int(x, y, zindex));
                    decorationTiles.Add(tile);
                }
            }
            GlobalReference.UI.Tilemap_Ground.SetTiles(groundPositions.ToArray(), groundTiles.ToArray());
            GlobalReference.UI.Tilemap_Object.SetTiles(decorationPositions.ToArray(), decorationTiles.ToArray());
        }

        // 生成地图单位
        public static void CreateObjects()
        {
            for (int x = 0; x < _levelInfo.MapWidth; x++)
            {
                for (int y = 0; y < _levelInfo.MapHeight; y++)
                {
                    if(x==0 || x==_levelInfo.MapWidth-1 || y==0 || y==_levelInfo.MapHeight-1)
                    {
                        MapUnit borderUnit = MonoBehaviour.Instantiate(GlobalReference.Prefeb.MapUnit,GlobalReference.UI.MapUnitContainer.transform);
                        Vector3 borderPos = Toolkit.TileLocationToPosition(new Vector2Int(x, y));
                        borderPos += new Vector3(32, 24);
                        borderUnit.GetComponent<RectTransform>().localPosition = borderPos;
                        borderUnit.SpriteRenderer.sprite = BorderSprite;
                        continue;
                    }
                    //  地图单位生成概率
                    bool isUnit = Random.Range(0, 1000) < 5;
                    if (!isUnit) continue;
                    MapUnit mapUnit = MonoBehaviour.Instantiate(GlobalReference.Prefeb.MapUnit,GlobalReference.UI.MapUnitContainer.transform);
                    Vector3 pos = Toolkit.TileLocationToPosition(new Vector2Int(x, y));
                    pos += new Vector3(64, 48);
                    mapUnit.transform.position = pos;
                    mapUnit.SpriteRenderer.sprite = unitSprites[Random.Range(0, unitSprites.Count-1)];
                }
            }
        }

        // 清除地图
        private static void ClearTiles()
        {
            GlobalReference.UI.Tilemap_Ground.ClearAllTiles();
            GlobalReference.UI.Tilemap_Object.ClearAllTiles();
            foreach (Transform child in GlobalReference.UI.UnitContainer.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            foreach (Transform child in GlobalReference.UI.MapUnitContainer.transform)
            {
                GameObject.Destroy(child.gameObject);
            }


        }
        
        
    }
}