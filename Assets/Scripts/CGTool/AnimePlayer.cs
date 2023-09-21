/**
 * 魔力宝贝图档解析脚本 - CGTool
 * 
 * @Author  HonorLee (dev@honorlee.me)
 * @Version 1.0 (2023-04-15)
 * @License GPL-3.0
 *
 * AnimePlayer.cs 动画播放器-挂载类
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CGTool
{
    //动画周期回调
    public delegate void AnimeCallback(Anime.ActionType actionType);
    
    //动画动作帧监听
    public delegate void AnimeEffectListener(Anime.EffectType effect);
    
    //动画音频帧监听
    public delegate void AnimeAudioListener(int audioIndex);
    
    //鼠标移入事件监听
    public delegate void MouseListener(AnimePlayer animePlayer);
    
    /**
     * 动画播放器,用于播放CG动画,支持多动画队列播放
     * 脚本需绑定至挂载了SpriteRenderer、Image和RectTransform的对象上
     * ########除此之外,还需绑定BoxCollider2D(可选),用于监听鼠标的移入移出事件#####此条删除
     *
     * 当动画播放完成后会自动调用onFinishCallback回调函数
     * 另外可指定onActionListener和onAudioListener监听动画动作帧和音频帧
     * 目前已知的动作帧有:
     * 击中 伤害结算
     */
    public class AnimePlayer : MonoBehaviour
    {
        //动画帧数据
        private class AnimeFrame
        {
            public int Index;
            public GraphicInfoData GraphicInfo;
            public Sprite Sprite;
            public AnimeFrameInfo AnimeFrameInfo;
        }

        //播放配置数据
        private class AnimeOption
        {
            public uint AnimeSerial;
            public Anime.DirectionType Direction;
            public Anime.ActionType actionType;
            public Anime.PlayType playType;
            public float Speed;
            public float FrameRate;
            public AnimeDetail AnimeDetail;
            public AnimeCallback onFinishCallback;
        }
        
        
        
        //当前播放
        private uint _currentSerial;
        private AnimeOption _currentAnime;
        private AnimeFrame[] _frames;
        private int _currentFrame;
        
        //是否播放
        private bool isPlayable;
        
        //待播放队列
        private Queue<AnimeOption> _animeQueue = new Queue<AnimeOption>();
        
        //计时器
        private float _timer;
        //下一帧延迟
        private float _delay;
        
        //绑定渲染对象
        [SerializeField,Header("Image渲染")] public bool isRenderByImage = false;
        [SerializeField,Header("序列帧合批")] public bool isFrameBatch = false;
        [Header("序列帧Texture")] public Texture2D frameTexture;
        private SpriteRenderer _spriteRenderer;
        private Image _imageRenderer;
        private int _paletIndex = 0;
        public int PaletIndex
        {
            get { return _paletIndex; }
            set
            {
                _paletIndex = value;
                if (_currentAnime != null) _play(_currentAnime);
            }
        }
        
        //绑定RectTransform
        private RectTransform _rectTransform;
        //绑定BoxCollider2D(可选)
        private BoxCollider2D _boxCollider2D;
        
        //动画动作帧监听
        public AnimeEffectListener onEffectListener;
        public AnimeAudioListener onAudioListener;
        //鼠标移入事件监听
        public MouseListener onMouseEnterListener;
        //鼠标移出事件监听
        public MouseListener onMouseExitListener;

        //获取偏移量(无用)
        public Vector2 offset
        {
            get
            {
                float offsetX = -_frames[_currentFrame].AnimeFrameInfo.OffsetX;
                float offsetY = _frames[_currentFrame].AnimeFrameInfo.OffsetY;
                return new Vector2(offsetX, offsetY);
            }
        }

        //实例初始化时获取相关绑定
        private void Awake()
        {
            //调整渲染
            _imageRenderer = GetComponent<Image>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _rectTransform = GetComponent<RectTransform>();
            //碰撞盒,仅当需要添加鼠标事件时使用
            // _boxCollider2D = GetComponent<BoxCollider2D>();
            
            if(_imageRenderer == null) _imageRenderer = gameObject.AddComponent<Image>();
            if(_spriteRenderer == null) _spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            if(_rectTransform == null) _rectTransform = gameObject.AddComponent<RectTransform>();
            // if(_boxCollider2D == null) _boxCollider2D = gameObject.AddComponent<BoxCollider2D>();
            
        }

        private void Start()
        {
            _updateRenderMode();
        }

        //鼠标移入监听
        private void OnMouseEnter()
        {
            if(onMouseEnterListener!=null) onMouseEnterListener(this);
        }

        //鼠标移出监听
        private void OnMouseExit()
        {
            if(onMouseExitListener!=null) onMouseExitListener(this);
        }
        
        // 使用Image模式渲染
        public bool RenderByImage
        {
            get => isRenderByImage;
            set
            {
                isRenderByImage = value;
                _updateRenderMode();
            }
        }
        
        // 设置当前播放序列,默认方向North,动作Stand,播放类型Loop,播放速度1f
        public uint Serial
        {
            get => _currentSerial;
            set
            {
                Anime.DirectionType direction =
                    _currentAnime?.Direction ?? Anime.DirectionType.North;
                Anime.ActionType actionType = _currentAnime?.actionType ?? Anime.ActionType.Stand;
                Anime.PlayType playType = _currentAnime?.playType ?? Anime.PlayType.Loop;
                float speed = _currentAnime?.Speed ?? 1f;
                AnimeCallback onFinishCallback = _currentAnime?.onFinishCallback;
                play(value, direction, actionType, playType, speed, onFinishCallback);
            }
        }
        
        // 动态调整播放类型
        public Anime.PlayType PlayType
        {
            get => _currentAnime?.playType ?? Anime.PlayType.Loop;
            set
            {
                if (_currentAnime != null)
                {
                    _currentAnime.playType = value;
                }
            }
        }

        // 更新渲染模式
        private void _updateRenderMode()
        {
            if (isRenderByImage)
            {
                _imageRenderer.enabled = true;
                _spriteRenderer.enabled = false;
            }
            else
            {
                _imageRenderer.enabled = false;
                _spriteRenderer.enabled = true;
            }
        }

        /**
         * 播放动画,调用此方法将会清空当前播放队列,调用完成可通过链式调用nextPlay方法添加动画到播放队列
         * @param Serial 动画序列号
         * @param Direction 动画方向
         * @param ActionType 动画动作
         * @param PlayType 播放类型
         * @param Speed 播放速度,以 1s 为基准,根据动画帧率计算实际播放周期时长
         * @param onFinishCallback 动画结束回调
         * @return AnimePlayer
         */
        public AnimePlayer play(uint Serial, Anime.DirectionType Direction = Anime.DirectionType.North,
            Anime.ActionType actionType = Anime.ActionType.Stand, Anime.PlayType playType = Anime.PlayType.Once,
            float Speed = 1f, AnimeCallback onFinishCallback = null)
        {
            if (_spriteRenderer == null)
            {
                // Debug.Log("AnimePlayer:SpriteRenderer is null");
                return this;
            }
            AnimeOption animeOption = CreateAnimeOption(Serial, Direction, actionType, playType, Speed, onFinishCallback);
            if (animeOption == null)
            {
                if (onFinishCallback != null) onFinishCallback(actionType);
                // Debug.Log("AnimePlayer:AnimeOption create failed");
                return this;
            }
            //清空播放队列
            _animeQueue.Clear();
            //播放
            _currentSerial = Serial;
            _play(animeOption);
            
            //链式调用,后续可通过nextPlay方法添加动画到播放队列
            return this;
        }

        //播放动画
        public AnimePlayer play(uint Serial, Anime.PlayType playType, float speed = 1f,
            AnimeCallback onFinishCallback = null)
        {
            return play(Serial,Anime.DirectionType.North,Anime.ActionType.Stand,playType,speed,onFinishCallback);
        }

        //播放一次
        public AnimePlayer playOnce(Anime.DirectionType directionType,Anime.ActionType actionType,float Speed=1f,AnimeCallback onFinishCallback=null)
        {
            return play(_currentSerial, directionType, actionType, Anime.PlayType.Once,
                Speed, onFinishCallback);
        }
        
        //播放循环
        public AnimePlayer playLoop(Anime.DirectionType directionType,Anime.ActionType actionType,float Speed=1f,AnimeCallback onFinishCallback=null)
        {
            return play(_currentSerial, directionType, actionType, Anime.PlayType.Loop,
                Speed, onFinishCallback);
        }

        //调整动画方向
        public void changeDirection(Anime.DirectionType directionType)
        {
            if (directionType == _currentAnime.Direction || directionType == Anime.DirectionType.NULL) return;
            _currentAnime = CreateAnimeOption(_currentAnime.AnimeSerial, directionType, _currentAnime.actionType,
                _currentAnime.playType, _currentAnime.Speed, _currentAnime.onFinishCallback);
            _play(_currentAnime);
        }
        public Anime.DirectionType DirectionType
        {
            get => _currentAnime?.Direction ?? Anime.DirectionType.NULL;
            set
            {
                if (_currentAnime != null)
                {
                    changeDirection(value);
                }
            }
        }
        
        //调整动画动作类型
        public void changeActionType(Anime.ActionType actionType)
        {
            if (actionType == _currentAnime.actionType) return;
            _currentAnime = CreateAnimeOption(_currentAnime.AnimeSerial, _currentAnime.Direction,actionType,
                _currentAnime.playType, _currentAnime.Speed, _currentAnime.onFinishCallback);
            _play(_currentAnime);
        }
        public Anime.ActionType ActionType
        {
            get => _currentAnime?.actionType ?? Anime.ActionType.NULL;
            set
            {
                if (_currentAnime != null)
                {
                    changeActionType(value);
                }
            }
        }

        //播放
        private void _play(AnimeOption animeOption)
        {
            isPlayable = false;
            _currentAnime = null;
            
            AnimeFrame[] frames = new AnimeFrame[animeOption.AnimeDetail.FrameCount];

            if (isFrameBatch)
            {
                Anime.BakeAnimeFrames(animeOption.AnimeDetail, _paletIndex);
                //获取动画帧数据
                for (int i = 0; i < animeOption.AnimeDetail.AnimeFrameInfos.Length; i++)
                {
                    if(!animeOption.AnimeDetail.AnimeFrameInfos[i].AnimeSprites.ContainsKey(_paletIndex)) continue;
                    if(animeOption.AnimeDetail.AnimeFrameInfos[i].AnimeSprites[_paletIndex] == null) continue;
                    //创建帧数据
                    frames[i] = new AnimeFrame();
                    frames[i].Index = i;
                    frames[i].GraphicInfo = animeOption.AnimeDetail.AnimeFrameInfos[i].GraphicInfo;
                    frames[i].Sprite = animeOption.AnimeDetail.AnimeFrameInfos[i].AnimeSprites[_paletIndex];
                    frames[i].AnimeFrameInfo = animeOption.AnimeDetail.AnimeFrameInfos[i];
                }
            }
            else
            {
                //获取动画帧数据
                for (int i = 0; i < animeOption.AnimeDetail.AnimeFrameInfos.Length; i++)
                {
                    AnimeFrameInfo animeFrameInfo = animeOption.AnimeDetail.AnimeFrameInfos[i];
                    GraphicInfoData graphicInfoData = GraphicInfo.GetGraphicInfoDataByIndex(animeOption.AnimeDetail.Version, animeOption.AnimeDetail.AnimeFrameInfos[i].GraphicIndex);
                    if (graphicInfoData == null)
                    {
                        Debug.Log("GraphicInfo Version:" + animeOption.AnimeDetail.Version + " Index:" +
                                  animeOption.AnimeDetail.AnimeFrameInfos[i] + " is null");
                        continue;
                    }

                    GraphicData graphicData = Graphic.GetGraphicData(graphicInfoData, _paletIndex);
                    if (graphicData == null)
                    {
                        Debug.Log("GraphicData Version:" + animeOption.AnimeDetail.Version + " Index:" +
                                  animeOption.AnimeDetail.AnimeFrameInfos[i] + " is null");
                        continue;
                    }
                
                    //创建帧数据
                    frames[i] = new AnimeFrame();
                    frames[i].Index = i;
                    frames[i].GraphicInfo = graphicInfoData;
                    frames[i].Sprite = graphicData.Sprite;
                    frames[i].AnimeFrameInfo = animeFrameInfo;
                }
            }
            

            _currentAnime = animeOption;
            _frames = frames;
            _currentFrame = -1;
            isPlayable = true;
            gameObject.SetActive(true);
            UpdateFrame();
        }

        //播放延时
        public void DelayPlay(float delayTime)
        {
            _delay = delayTime*1000;
        }

        public void Stop()
        {
            isPlayable = false;
            _currentAnime = null;
            _frames = null;
            _currentFrame = -1;
            gameObject.SetActive(false);
        }

        public void Pause()
        {
            isPlayable = false;
        }

        //修改播放类型---重复方法--考虑删掉
        public void ChangePlayType(Anime.PlayType playType)
        {
            if (_currentAnime == null) return;
            _currentAnime.playType = playType;
        }

        //创建动画配置
        private AnimeOption CreateAnimeOption(uint Serial, Anime.DirectionType Direction, Anime.ActionType ActionType,
            Anime.PlayType playType=Anime.PlayType.Once, float Speed = 1f, AnimeCallback onFinishCallback = null)
        {
            AnimeDetail animeDetail = Anime.GetAnimeDetail(Serial, Direction, ActionType);
            if (animeDetail == null)
            {
                // Debug.Log("AnimePlayer:AnimeDetail is null");
                return null;
            }
            AnimeOption animeOption = new AnimeOption()
            {
                AnimeSerial = Serial,
                Direction = Direction,
                actionType = ActionType,
                playType = playType,
                Speed = Speed,
                FrameRate = animeDetail.CycleTime / Speed / animeDetail.FrameCount,
                AnimeDetail = animeDetail,
                onFinishCallback = onFinishCallback,
            };
            return animeOption;
        }

        //加入链式动画播放队列
        public AnimePlayer nextPlay(uint Serial, Anime.DirectionType Direction, Anime.ActionType ActionType,
            Anime.PlayType playType=Anime.PlayType.Once, float Speed = 1f, AnimeCallback onFinishCallback = null)
        {
            AnimeOption animeOption = CreateAnimeOption(Serial, Direction, ActionType, playType, Speed, onFinishCallback);
            if (animeOption == null)
            {
                if (onFinishCallback != null) onFinishCallback(ActionType);
                return this;
            }
            if (_animeQueue.Count == 0)
            {
                _play(animeOption);
            }
            else
            {
                _animeQueue.Enqueue(animeOption);    
            }
            
            return this;
        }
        
        //加入链式动画播放队列
        public AnimePlayer nextPlay(Anime.DirectionType Direction, Anime.ActionType ActionType,
            Anime.PlayType playType=Anime.PlayType.Once, float Speed = 1f, AnimeCallback onFinishCallback = null)
        {
            return nextPlay(_currentSerial, Direction, ActionType, playType, Speed, onFinishCallback);
        }
        
        //更新计算
        private void Update()
        {
            float now = Time.time * 1000;
            if (_currentAnime != null && (now - _timer - _delay) >= _currentAnime.FrameRate) UpdateFrame();
        }

        //更新帧
        private void UpdateFrame()
        {
            _delay = 0;
            if (!isPlayable || _frames.Length == 0) return;
            
            _currentFrame++;
            
            //动画结束
            if (_currentFrame >= _currentAnime.AnimeDetail.FrameCount)
            {
                if(_currentAnime.onFinishCallback!=null) _currentAnime.onFinishCallback(_currentAnime.actionType);
                //循环播放
                if (_currentAnime.playType == Anime.PlayType.Loop)
                {
                    _currentFrame = 0;
                }else if (_currentAnime.playType == Anime.PlayType.Once || _currentAnime.playType == Anime.PlayType.OnceAndDestroy)
                {
                    if (_currentAnime.playType == Anime.PlayType.OnceAndDestroy)
                    {
                        _spriteRenderer.sprite = null;
                        _imageRenderer.sprite = null;
                        _rectTransform.sizeDelta = Vector2.zero;
                        // gameObject.SetActive(false);
                    }
                    //播放下一个动画
                    if(_animeQueue.Count>0)
                    {
                        AnimeOption animeOption = _animeQueue.Dequeue();
                        _play(animeOption);
                        return;
                    }else
                    {
                        isPlayable = false;
                        return;
                    }
                }
            }
            
            //问题帧自动跳过
            if (_currentFrame<_frames.Length && _frames[_currentFrame] == null) return;
            //自动偏移
            // float graphicWidth = _frames[_currentFrame].Sprite.rect.width;
            // float graphicHeight = _frames[_currentFrame].Sprite.rect.height;
            // float offsetX = -_frames[_currentFrame].GraphicInfo.OffsetX;
            // float offsetY = _frames[_currentFrame].GraphicInfo.OffsetY;
            
            //根据当前帧Sprite动态调整对象大小
            float width = _frames[_currentFrame].Sprite.rect.width * 1f;
            float height = _frames[_currentFrame].Sprite.rect.height * 1f;

            if (isRenderByImage)
            {
                _imageRenderer.sprite = _frames[_currentFrame].Sprite;
                _imageRenderer.SetNativeSize();
                Vector3 pos = Vector3.zero;
                pos.x = _frames[_currentFrame].GraphicInfo.OffsetX;
                pos.y = -_frames[_currentFrame].GraphicInfo.OffsetY;
                _rectTransform.localPosition = pos;
                _rectTransform.pivot = new Vector2(0f,1f);
            }
            else
            {
                _spriteRenderer.sprite = _frames[_currentFrame].Sprite;
                _rectTransform.sizeDelta = new Vector2(width, height);
                _spriteRenderer.size = new Vector2(width, height);
                _rectTransform.pivot = new Vector2(0.5f,0f);
                _rectTransform.localPosition = Vector3.zero;
            }
            frameTexture = _frames[_currentFrame].Sprite.texture;
            
            // Vector2 offset = Vector2.zero;
            // offset.x += -(_frames[_currentFrame].GraphicInfo.OffsetX * 1f) / _frames[_currentFrame].GraphicInfo.Width;
            // offset.y -= (-_frames[_currentFrame].GraphicInfo.OffsetY * 1f) / _frames[_currentFrame].GraphicInfo.Height;
            
            // _rectTransform.pivot = offset;
            
            // pos.x = (width + _frames[_currentFrame].GraphicInfo.OffsetX)/1f;
            // pos.y = (height + _frames[_currentFrame].GraphicInfo.OffsetY)/1f;
            
            
            
            // 2D碰撞器自动调整,但是动态碰撞器反而会导致重叠大物体选中效果不稳定,效果不如固定大小碰撞器好
            // if (_boxCollider2D != null)
            // {
            //     Vector2 newSize =_boxCollider2D.size 
            //     _boxCollider2D.size = new Vector2(width, height);
            // }
            // _rectTransform.pivot = new Vector2(offsetX,offsetY);
            // _rectTransform.localPosition = new Vector3(0f,  0f);
            
            
            _timer = Time.time * 1000;
            
            //动画事件帧监听
            if(_frames[_currentFrame].AnimeFrameInfo.Effect >0 && onEffectListener!=null) onEffectListener(_frames[_currentFrame].AnimeFrameInfo.Effect);
            //音频事件帧监听
            if(_frames[_currentFrame].AnimeFrameInfo.AudioIndex >0 && onAudioListener!=null) onAudioListener(_frames[_currentFrame].AnimeFrameInfo.AudioIndex);
        }
    }
}