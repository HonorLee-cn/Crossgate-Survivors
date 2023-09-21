using CGTool;
using DG.Tweening;
using Reference;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    // 回合导航条
    public class RoundPanel:MonoBehaviour
    {
        [SerializeField,Header("回合Slider")] public Slider CurrenRound;
        [SerializeField,Header("回合角色Player")] public AnimePlayer Player;
        [SerializeField,Header("BOSS框架")] public GameObject BOSSContainer;
        [SerializeField,Header("BOSS盒")] public GameObject BOSSBox_Prefeb;
        [SerializeField,Header("BOSSPlayer")] public RoundBOSS BOSS_Prefeb;
        
        private void OnEnable()
        {
            GetComponent<CanvasGroup>().alpha = 0;
        }

        public void Show()
        {
            Player.Serial = GlobalReference.BattleData.PlayerUnit.Serial;
            Player.changeDirection(Anime.DirectionType.South);
            Player.changeActionType(Anime.ActionType.Walk);
            GetComponent<CanvasGroup>().DOFade(0.6f,0.5f);
        }
    }
}