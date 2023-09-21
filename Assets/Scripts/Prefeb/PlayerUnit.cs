using System;
using System.Collections.Generic;
using Base;
using CGTool;
using Controller;
using DG.Tweening;
using Game.UI;
using GameData;
using Reference;
using UnityEngine;
using UnityEngine.UI;
using Util;
using Random = UnityEngine.Random;

namespace Prefeb
{
    // 角色单位
    public class PlayerUnit:MonoBehaviour
    {
        [SerializeField,Header("人物动画")] public AnimePlayer AnimePlayer;
        [SerializeField,Header("效果动画")] public AnimePlayer EffectPlayer;
        [SerializeField,Header("技能层")] public GameObject SkillContainer;
        [SerializeField,Header("拾取范围")] public GameObject LootRange;
        [SerializeField,Header("怪物攻击范围")] public GameObject AttackRange;
        [SerializeField,Header("怪物光环")] public UnitLight UnitLight;
        [SerializeField,Header("角色属性")] public Text AttrText;
        
        // 单位已经学会的技能
        [NonSerialized] public List<SkillSetting.SkillBase> PlayerSkills = new List<SkillSetting.SkillBase>();
        // 单位已经学会的技能对象
        [NonSerialized] public Dictionary<int,SkillSetting.SkillBase> SkillLevel = new Dictionary<int, SkillSetting.SkillBase>();
        // 单位被动加成效果
        [NonSerialized] public Dictionary<string,object> UnitEffects = new Dictionary<string, object>();

        // 移动方向
        private Anime.DirectionType _moveDirection = Anime.DirectionType.NULL;
        // 面向方向
        private Anime.DirectionType _faceDirection = Anime.DirectionType.SouthWest;
        
        // 单位属性
        private Attr _attr;
        
        // 移动速度
        private float _speed = 150.0f;
        
        // 单位刚体
        private Rigidbody2D _rigidbody;
        
        // 单位音频
        private AudioSource _audioSource;
        
        // 是否闪避中
        private bool _isDashing = false;
        // 是否在闪避CD
        private bool _IsDodgeCD = false;
        
        // 是否为怪物
        private bool _IsEnemy = false;
        // 是否为Boss
        private bool _IsBoss = false;
        
        
        // 是否可移动(主控)
        private bool _walkable = false;
        // 由技能决定
        [NonSerialized] public bool IsWalkable = true;
        
        // (怪物)是否接近玩家
        [NonSerialized] public bool IsNearPlayer;
        // (怪物)玩家是否在攻击范围内
        [NonSerialized] public bool IsPlayerInAttackRange;
        
        // 是否死亡
        [NonSerialized] public bool IsDead = false;
        
        // 角色技能
        [NonSerialized] public SkillSetting.SkillBase ATKSkill;     //默认攻击技能
        [NonSerialized] public SkillSetting.SkillBase SpecialSkill; //得意技

        // 绑定BOSS信息
        public LevelSetting.BossInfo BossInfo;
        
        // 单位当前目标 玩家为鼠标位置,怪物为玩家位置
        [NonSerialized] public Vector3 TargetPosition;

        
        // 单位唯一ID
        public Guid ID;
        
        // 单位死亡事件监听
        public delegate void OnDeadHandler(PlayerUnit playerUnit);
        public event OnDeadHandler OnDead;

        private void OnEnable()
        {
            // 初始化属性
            _attr = new Attr(this);
        }

        private void Start()
        {
            // 初始化
            _rigidbody = GetComponent<Rigidbody2D>();
            _audioSource = GetComponent<AudioSource>();
            AnimePlayer.gameObject.SetActive(false);
            AttrText.gameObject.SetActive(Toolkit.IsDebug);
            EffectPlayer.onAudioListener = (audio) =>
            {
                _audioSource.clip = AudioTool.GetAudio(AudioTool.Type.EFFECT, audio);
                _audioSource.Play();
            };
            EffectPlayer.play(110083, Anime.PlayType.OnceAndDestroy, 1f, (a) =>
            {
                AnimePlayer.gameObject.SetActive(true);
                _walkable = true;
            });
            AttrText.text = "LV.  " + Attr.Level + "\nATK: " + Attr.ATK + "\nDEF: " + Attr.DEF + "\nHP:  " + Attr.HP;
            _attr.OnAttrChange += (prop,val,attr) =>
            {
                if(prop=="HP") OnHpChange();
                if(prop=="SPD") _speed = _attr.SPD;
                AttrText.text = "LV.  " + Attr.Level + "\nATK: " + Attr.ATK + "\nDEF: " + Attr.DEF + "\nHP:  " + Attr.HP;
            };
        }
        
        // 绑定角色信息
        private PlayerSetting.PlayerInfo _playerInfo;
        public PlayerSetting.PlayerInfo PlayerInfo
        {
            get => _playerInfo;
            set
            {
                _playerInfo = value;
            }
        }
        
        // 移动速度
        public int Speed
        {
            get => (int)_speed;
            set => _speed = value;
        }
        
        public Rigidbody2D Rigidbody
        {
            get => _rigidbody;
            set => _rigidbody = value;
        }
        
        // 图档序列
        public uint Serial
        {
            get => AnimePlayer.Serial;
            set
            {
                AnimePlayer.Serial = value;
                AnimePlayer.DirectionType = _faceDirection;
                if (_IsEnemy) AnimePlayer.ActionType = Anime.ActionType.Walk;
            }
        }

        // 角色属性
        public Attr Attr
        {
            get => _attr;
            // set => _attr = value;
        }

        // 初始化属性基础值
        public void InitAttr()
        {
            _attr.InitBase(_playerInfo);
            _speed = _attr.SPD;
        }

        // 直接升级至指定等级
        public void LevelUpTo(int level)
        {
            while (_attr.Level < level)
            {
                LevelUp();
            }
        }

        // 是否为怪物
        public bool IsEnemy
        {
            get => _IsEnemy;
            set
            {
                _IsEnemy = value;
                if (_IsEnemy)
                {
                    LootRange.SetActive(false);
                    AttackRange.SetActive(true);
                    AnimePlayer.ActionType = Anime.ActionType.Walk;
                    GetComponent<Rigidbody2D>().mass = 100;
                }
                
            }
        }
        
        // 是否为BOSS
        public bool IsBoss
        {
            get => _IsBoss;
            set
            {
                _IsBoss = value;
                if (_IsBoss)
                {
                    //增大碰撞和质量
                    gameObject.transform.localScale = new Vector3(1.5f,1.5f,1);
                    GetComponent<Rigidbody2D>().mass = 200;
                    GetComponent<BoxCollider2D>().size = new Vector2(40,40);
                    // AnimePlayer.Serial = AnimePlayer.Serial;
                    GetComponent<BoxCollider2D>().offset = new Vector2(0,10);
                }
                
            }
        }

        // 角色移动方向,如果当前闪避中,则切换跑步动画
        public Anime.DirectionType MoveDirection
        {
            get => _moveDirection;
            set
            {
                _moveDirection = value;
                if (!_IsEnemy)
                {
                    if (_moveDirection == Anime.DirectionType.NULL)
                    {
                        AnimePlayer.ActionType = Anime.ActionType.Stand;
                    }
                    else
                    {
                        _rigidbody.velocity = Vector2.zero;
                        if (_isDashing)
                        {
                            AnimePlayer.ActionType = Anime.ActionType.Run;    
                        }
                        else
                        {
                            AnimePlayer.ActionType = Anime.ActionType.Walk;
                        }

                    }
                }
                
            }
        }
        
        // 角色面向方向
        public Anime.DirectionType FaceDirection
        {
            get => _faceDirection;
            set
            {
                _faceDirection = value;
                AnimePlayer.DirectionType = _faceDirection;
                TargetPosition = GlobalReference.UI.PlaygroundCamera.ScreenToWorldPoint(Input.mousePosition);
                TargetPosition.z = 0;

            }
        }

        // 获取学习的技能
        public SkillSetting.SkillBase GetLearnedSkill(int skillID)
        {
            if (SkillLevel.ContainsKey(skillID))
            {
                return SkillLevel[skillID];
            }
            return null;
        }
        
        // 学习技能
        public void LearSkill(int skillID,int level = 1)
        {
            if (SkillLevel.ContainsKey(skillID))
            {
                SkillLevel[skillID].LevelUp();
                return;
            }
            // 反射载入技能
            GameObject skillObject = UnityEngine.Resources.Load<GameObject>("BattleSkills/s" + skillID + "/Script");
            if (skillObject == null)
            {
                Debug.LogError("SkillScript " + skillID + " Not Found");
                return;
            }
            SkillSetting.SkillBase skill = Instantiate(skillObject, SkillContainer.transform).GetComponent<SkillSetting.SkillBase>();
            if (skill == null)
            {
                Debug.LogError("SkillBase " + skillID + " Not Found");
                return;
            }
            skill.Init(this, SkillSetting.GetSkillInfo(skillID));
            if (level > 0)
            {
                for (int i = 0; i < level; i++) skill.LevelUp();
            }
            
            
            if (skillID == _playerInfo.ATKSkill)
            {
                skill.IsAtkSkill = true;
                ATKSkill = skill;
            }

            if (skillID == _playerInfo.SpecialSkill)
            {
                skill.IsSpecialSkill = true;
                SpecialSkill = skill;
            }
            
            PlayerSkills.Add(skill);
            SkillLevel.Add(skillID, skill);
            
            // 如果是怪物则设定所有技能为自动技能
            if (IsEnemy)
            {
                skill.IsAudoSkill = true;
                // skill.StartCD();
                return;
            }
            
            // 玩家则在左下角显示当前学到的技能
            Sprite sprite = UnityEngine.Resources.Load<Sprite>("SkillIcons/" + skill.SkillInfo.Icon);
            SkillItem skillItem = Instantiate(GlobalReference.Prefeb.SkillItem_Prefeb, GlobalReference.UI.BattleUI.SkillList.transform);
            skillItem.Skill = skill;
            skillItem.Level = skill.Level;
            skillItem.CircleCD.SetItem(sprite);
            skill.SkillItem = skillItem;
            skill.CircleCDs.Add(skillItem.CircleCD);
            if(skill.IsAtkSkill) skill.IsAudoSkill = true;
            if (skill.IsSpecialSkill)
            {
                skill.CircleCDs.Add(GlobalReference.UI.BattleUI.SpecialSkill);
                GlobalReference.UI.BattleUI.SpecialSkill.SetItem(sprite);
            }
            // 初始化CD
            skill.StartCD();
            
        }
        
        // 闪避
        public void Dash()
        {
            if(_isDashing) return;
            if(_attr.Dodge <= 0) return;
            _attr.Dodge -= 1;
            GetComponent<GhostEffect>().showGhost(0.5f,0.1f,0.5f);
            _speed = _speed * 3f;
            _isDashing = true;
            // _boxCollider2D.enabled = false;
            gameObject.layer = 9;
            if (!IsEnemy) GlobalReference.UI.PlaygroundCamera.DOOrthoSize(Screen.height / 2+30, 0.3f);
            // GlobalReference.UI.PlaygroundCamera.DOOrthoSize(Screen.height / 2+30, 0.3f);
                // AnimePlayer.changeActionType(Anime.ActionType.Run);
            DOTween.Sequence().SetDelay(0.5f).onComplete = () =>
            {
                GlobalReference.UI.PlaygroundCamera.DOOrthoSize(Screen.height / 2, 0.3f);
                gameObject.layer = 7;
                _speed = _attr.SPD;
                _isDashing = false;
                DodgeCD();
                // _boxCollider2D.enabled = true;
            };


        }

        // 升级
        public void LevelUp()
        {
            // 计算各属性
            int ATK = _attr.ATK_Base;
            int DEF = _attr.DEF_Base;
            int SPD = _attr.SPD_Base;
            int MaxHP = _attr.MaxHP_Base;
            int MaxMP = _attr.MaxMP_Base;
            int Exp = _attr.NextLevelEXP;
            PlayerSetting.PlayerGrowBase growBase = IsEnemy ? EnemySetting.EnemyGrow : PlayerSetting.PlayerGrow;
            ATK = (int) Math.Ceiling(ATK + ATK * growBase.ATK);
            DEF = (int) Math.Ceiling(DEF + DEF * growBase.DEF);
            SPD = (int) Math.Ceiling(SPD + SPD * growBase.SPD);
            MaxHP = (int) Math.Ceiling(MaxHP + MaxHP * growBase.HP);
            MaxMP = (int) Math.Ceiling(MaxMP + MaxMP * growBase.MP);
            Exp = (int) Math.Ceiling(Exp + Exp * 0.5);
            _attr.ATK_Base = ATK;
            _attr.DEF_Base = DEF;
            _attr.SPD_Base = SPD;
            _attr.MaxHP_Base = MaxHP;
            _attr.MaxMP_Base = MaxMP;
            _attr.NextLevelEXP = Exp;
            _attr.HP = MaxHP;
            _attr.MP = MaxMP;
            _attr.Level++;
        }
        
        // 闪避CD
        private void DodgeCD()
        {
            if(_IsDodgeCD) return;
            if(_attr.Dodge < _attr.MaxDodge)
            {
                _IsDodgeCD = true;
                DOTween.Sequence().SetDelay(_attr.DodgeCD).onComplete = () =>
                {
                    _IsDodgeCD = false;
                    _attr.Dodge += 1;
                    DodgeCD();
                };
                
            }
        }
        
        
        private void FixedUpdate()
        {
            if(!_walkable) return;
            if(!IsWalkable) return;
            if (IsEnemy)
            {
                // 怪物移动
                if(!IsDead) EnemyMove();
            }
            else
            {
                // 人物移动
                UserMove();
            }

        }

        // 释放所有技能
        public void CaseSkills()
        {
            if (IsEnemy)
            {
                foreach (var skill in PlayerSkills)
                {
                    skill.IsAudoSkill = true;
                    if(!skill.IsAvailable) continue;
                    skill.CastSkill();
                }
            }
        }
        // 所有技能停止自动释放
        public void StopCastSkills()
        {
            if (IsEnemy)
            {
                foreach (var skill in PlayerSkills)
                {
                    skill.IsAudoSkill = false;
                }
            }
        }
        
        //怪物移动
        private void EnemyMove()
        {
            //朝向玩家
            Vector2 playerPos = GlobalReference.BattleData.PlayerUnit.transform.position;
            Vector2 enemyPos = transform.position;
            Anime.DirectionType directionType = Toolkit.GetTargetDirection(playerPos,enemyPos);
            FaceDirection = directionType;
            //移动
            Vector2 force = Vector2.zero;
            Vector2 moveDirection = Vector2.zero;
            if (!IsNearPlayer)
            {
                //延直线最小距离朝玩家移动而非方向移动
                // 计算朝向玩家的方向向量
                moveDirection = (playerPos - enemyPos).normalized;    
            }
            
            // 使用 Rigidbody2D 来移动对象
            _rigidbody.velocity = moveDirection * _speed;
            TargetPosition = playerPos;
            

        }
        
        //角色移动
        private void UserMove()
        {
            if (_moveDirection == Anime.DirectionType.NULL)
            {
                _rigidbody.velocity = Vector2.zero;
                return;
            };
            Vector2 force = Vector2.zero;
            //根据_moveDirection方向移动transform,其中 Anime.DirectionType.NorthEast 对应 上方,Anime.DirectionType.SouthWest 对应 下方
            switch (_moveDirection)
            {
                case Anime.DirectionType.NorthEast:
                    force = Vector2.up;
                    break;
                case Anime.DirectionType.NorthWest:
                    force = Vector2.left;
                    break;
                case Anime.DirectionType.SouthEast:
                    force = Vector2.right;
                    break;
                case Anime.DirectionType.SouthWest:
                    force = Vector2.down;
                    break;
                case Anime.DirectionType.East:
                    force = (Vector2.up + Vector2.right);
                    break;
                case Anime.DirectionType.West:
                    force = (Vector2.down + Vector2.left);
                    break;
                case Anime.DirectionType.North:
                    force = (Vector2.up + Vector2.left);
                    break;
                case Anime.DirectionType.South:
                    force = (Vector2.down + Vector2.right);
                    break;
            }
            
            _rigidbody.velocity = force.normalized * _speed;
        }

        // 碰撞检测
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (_IsEnemy)
            {
                PlayerUnit unit = other.gameObject.GetComponent<PlayerUnit>();
                if (unit == null) return;
                if(unit.IsEnemy) return;
                IsNearPlayer = true;
                //如果碰撞到地图物体或怪物则 自行移位
                if (other.gameObject.CompareTag("MapUnit") || other.gameObject.CompareTag("EnemyUnit"))
                {
                    EscapeFromMapUnit(other);
                }
            }
        }

        // 碰撞检测 持续
        private float _checkTimer = 1f;
        private float _lastCheckTime = 0;
        private void OnCollisionStay2D(Collision2D other)
        {
            if(_lastCheckTime + _checkTimer > Time.time) return;
            _lastCheckTime = Time.time;
            if (_IsEnemy)
            {
                if (other.gameObject.CompareTag("MapUnit") || other.gameObject.CompareTag("EnemyUnit"))
                {
                    EscapeFromMapUnit(other);
                }
            }
        }

        // 逃离地面单位
        private void EscapeFromMapUnit(Collision2D other)
        {
            _walkable = false;
            Vector2 force = Vector2.zero;
            switch (FaceDirection)
            {
                case Anime.DirectionType.SouthWest:
                case Anime.DirectionType.NorthEast:
                case Anime.DirectionType.East:
                case Anime.DirectionType.West:
                    if (transform.position.x > other.transform.position.x)
                    {
                        force = Vector2.right;
                    }
                    else
                    {
                        force = Vector2.left;
                    }
                    break;
                case Anime.DirectionType.SouthEast:
                case Anime.DirectionType.NorthWest:
                case Anime.DirectionType.North:
                case Anime.DirectionType.South:
                    if (transform.position.y > other.transform.position.y)
                    {
                        force = Vector2.up;
                    }
                    else
                    {
                        force = Vector2.down;
                    }
                    break;
            }
            // 根据force改变facedirection
            if (force.x > 0 && force.y > 0)
            {
                FaceDirection = Anime.DirectionType.NorthEast;
            }
            else if (force.x > 0 && force.y < 0)
            {
                FaceDirection = Anime.DirectionType.SouthEast;
            }
            else if (force.x < 0 && force.y > 0)
            {
                FaceDirection = Anime.DirectionType.NorthWest;
            }
            else if (force.x < 0 && force.y < 0)
            {
                FaceDirection = Anime.DirectionType.SouthWest;
            }
            // _rigidbody.mass = 0;
            _rigidbody.velocity = force.normalized * _speed;
            
            DOTween.Sequence().SetDelay(0.5f).onComplete = () =>
            {
                _walkable = true;
            };
        }

        // 碰撞检测
        private void OnCollisionExit2D(Collision2D other)
        {
            if (_IsEnemy)
            {
                PlayerUnit unit = other.gameObject.GetComponent<PlayerUnit>();
                if (unit == null) return;
                if(unit.IsEnemy) return;
                IsNearPlayer = false;
            }
        }

        // 直接死亡(调试)
        public void InstantDeath()
        {
            if(IsDead) return;
            Attr.HP = -Attr.HP;
        }

        // HP发生变化
        public void OnHpChange()
        {
            // 死亡
            if (Attr.HP <= 0)
            {
                if (!IsEnemy) GameController.IsGameStart = false;
                IsDead = true;
                _walkable = false;
                _rigidbody.velocity = Vector2.zero;
                _rigidbody.mass = 10000;
                GetComponent<BoxCollider2D>().enabled = false;
                if (!IsEnemy) GameController.IsGameStart = false;
                float speed = IsEnemy ? 2f : 1f;
                PlayerSkills.ForEach(skill => skill.IsAudoSkill = false);
                AnimePlayer.playOnce(FaceDirection, Anime.ActionType.Dead,speed,(action) =>
                {
                    _rigidbody.velocity = Vector2.zero;
                    // 如果死亡的是怪物,则生成掉落物
                    if (IsEnemy)
                    {
                        // 如果是BOSS或判定成功则掉落
                        if (IsBoss || Random.Range(0, 1) == 0)
                        {
                            int count = IsBoss ? 20 * (1 + Attr.Level / 10) : Random.Range(1, 2);
                            for (int i = 0; i < count; i++)
                            {
                                LootItem lootItem = Instantiate(GlobalReference.Prefeb.LootItem_Prefeb,
                                    GlobalReference.UI.MapUnitContainer.transform);
                                Vector3 pos = transform.position;
                                //掉落物品位置随机
                                // if (IsBoss)
                                // {
                                    pos.x += Random.Range(-50, 50);
                                    pos.y += Random.Range(-50, 50);    
                                // }
                                lootItem.transform.position = pos;
                                LootSetting.LootType lootType = LootSetting.GetRandomLootType();
                                int level = Attr.Level / 5 + 1;
                                lootItem.Init(lootType, level);
                            }
                        }
                        Destroy(gameObject);
                    }
                    OnDead?.Invoke(this);
                    
                    if (!IsEnemy)
                    {
                        GameController.IsGameStart = false;
                        DOTween.Sequence().SetDelay(1f).onComplete += () =>
                        {
                            GameController.EndGame(true); 
                            
                        };
                        
                    }
                });
            }
        }
        
        // 受到攻击
        public void TakeDamage(SkillState skillState,Vector3 damageFrom)
        {
            // 如果已被处理死亡，不再受到伤害
            if(IsDead) return;
            int damage = skillState.Damage;
            // 伤害修正随机
            damage = (int) (damage * Random.Range(0.8f, 1.2f));
            // 防御修正随机
            if(!skillState.IgnoreDefense) damage -= (int) (Attr.DEF * Random.Range(0.8f, 1.2f));
            // 暴击判定
            bool Critical = Random.Range(0, 100) < GlobalReference.BattleData.CriticalRate;
            if(!skillState.CriticalAble) Critical = false;
            // Debug.Log("暴击率:"+GlobalReference.BattleData.CriticalRate+" 伤害:"+damage+" 防御:"+Attr.DEF+" 暴击:"+Critical);
            // 暴击伤害
            if (Critical) damage *= 2;
            // damage = (int) (damage * Random.Range(0.8f, 1.2f));
            // 最低伤害修正
            if(damage<=0) damage = 1;
            
            // 伤害显示
            int size = 18;
            if(Critical) size = 36;
            GameController.AddFloatingText(transform.position + new Vector3(0, 30, 0), damage.ToString(),
                Color.red,size);
            
            // 受到伤害
            Attr.HP -= damage;
            // 播放受击音效
            if (skillState.TargetAudio>0)
            {
                _audioSource.clip = AudioTool.GetAudio(AudioTool.Type.EFFECT, skillState.TargetAudio);
                _audioSource.Play();
            }
            // 处理暴击击退
            if (Critical)
            {
                EffectPlayer.play(110135, Anime.PlayType.OnceAndDestroy,2f);
                skillState.HitBackDistance *= 5;
            }
            // 如果当前伤害造成死亡,则不再处理后续
            if(IsDead) return;

            if (Critical)
            {
                TakeAround(damageFrom,skillState.HitBackDistance);
            }
            else
            {
                TakeHitback(damageFrom,skillState.HitBackDistance);    
            }
            
            
            
        }

        //处理击退
        private void TakeHitback(Vector3 damageFrom,float distance)
        {
            //如果被外部技能设定为不可移动，则不受击退
            if (!IsWalkable) return;
            //如果击退距离为0，则不受击退
            if(distance<=0) return;
            //禁用移动
            _walkable = false;
            //停止移动
            _rigidbody.velocity = Vector2.zero;
            
            //计算击退方向
            Vector2 force = (transform.position - damageFrom).normalized;
            _rigidbody.velocity = force * distance * 5f;

            //击退后恢复移动
            float time = IsEnemy ? 0.5f : 0.2f;
            DOTween.Sequence().SetDelay(time).onComplete = () =>
            {
                _walkable = true;
            };
        }

        //处理被暴击后人物旋转效果
        private void TakeAround(Vector3 damageFrom,float distance)
        {
            _rigidbody.velocity = Vector2.zero;   
            TakeHitback(damageFrom,distance);
            IsWalkable = false;
            _walkable = false;
            float time = 0.05f;
            AnimePlayer.changeDirection(Anime.DirectionType.East);
            DOTween.Sequence().SetDelay(time).onComplete = () =>
            {
                AnimePlayer.changeDirection(Anime.DirectionType.SouthEast);
                DOTween.Sequence().SetDelay(time).onComplete = () =>
                {
                    AnimePlayer.changeDirection(Anime.DirectionType.South);
                    DOTween.Sequence().SetDelay(time).onComplete = () =>
                    {
                        AnimePlayer.changeDirection(Anime.DirectionType.SouthWest);
                        DOTween.Sequence().SetDelay(time).onComplete = () =>
                        {
                            AnimePlayer.changeDirection(Anime.DirectionType.West);
                            DOTween.Sequence().SetDelay(time).onComplete = () =>
                            {
                                AnimePlayer.changeDirection(Anime.DirectionType.NorthWest);
                                DOTween.Sequence().SetDelay(time).onComplete = () =>
                                {
                                    AnimePlayer.changeDirection(Anime.DirectionType.North);
                                    DOTween.Sequence().SetDelay(time).onComplete = () =>
                                    {
                                        AnimePlayer.changeDirection(Anime.DirectionType.NorthEast);
                                        IsWalkable = true;
                                        _walkable = true;
                                    };
                                };
                            };
                        };
                    };
                };
            };
        }
    }
}