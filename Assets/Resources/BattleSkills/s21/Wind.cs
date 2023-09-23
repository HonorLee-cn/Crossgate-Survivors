using System;
using CGTool;
using GameData;
using Prefeb;
using UnityEngine;

namespace Resources.BattleSkills.s21
{
    public class Wind:MonoBehaviour
    {
        [SerializeField,Header("风")] public AnimePlayer WindAnime;
        private SkillState _skillState;
        private AudioSource _audioSource;
        private void Start()
        {
            _skillState = GetComponent<SkillState>();
            _audioSource = GetComponent<AudioSource>();
            _audioSource.clip = AudioTool.GetAudio(AudioTool.Type.EFFECT,268);
            _audioSource.Play();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerUnit playerUnit = other.GetComponent<PlayerUnit>();
            if (playerUnit == null)
            {
                // Destroy(this);
                return;;
            }
            if (playerUnit.IsEnemy != _skillState.IsFromEnemy)
            {
                playerUnit.TakeDamage(_skillState, transform.position);
            }
        }
        
    }
}