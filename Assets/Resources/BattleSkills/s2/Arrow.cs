using System;
using CGTool;
using GameData;
using Prefeb;
using UnityEngine;

namespace Resources.BattleSkills.s2
{
    public class Arrow:MonoBehaviour
    {
        private SkillState _skillState;
        private AudioSource _audioSource;
        private void Start()
        {
            _skillState = GetComponent<SkillState>();
            _audioSource = GetComponent<AudioSource>();
            _audioSource.clip = AudioTool.GetAudio(AudioTool.Type.EFFECT,101);
            _audioSource.Play();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerUnit playerUnit = other.GetComponent<PlayerUnit>();
            if (playerUnit == null)
            {
                return;;
            }
            if (playerUnit.IsEnemy != _skillState.IsFromEnemy)
            {
                playerUnit.TakeDamage(_skillState, transform.position);
                Destroy(gameObject);
            }
        }
        
    }
}