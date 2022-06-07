using UniRx;
using UnityEngine;
using UnityExtensions;
using Yomikiru.Effect;
using Yomikiru.Sound;

namespace Player
{
    public class PlayerPropSetting : MonoBehaviour
    {
        [Header("�ݒ�")]
        [InspectInline(canEditRemoteTarget = true)]
        public PlayerProperty playerProperty;
        public InputRecord inputRecord;
        public Animator Animator;
        public PlayerCollision collision;
        public AudioSource audioSource;

        [Header("�����ݒ肳���z")]
        public int PlayerIndex = 0;

        [HideInInspector]
        public GameObject CinemachineCameraTarget;//�V�l�}�V�[���̃^�[�Q�b�g
        [HideInInspector]
        public PlayerManagement playerMana;//�v���C���[�}�l�W�����g
        [HideInInspector]
        public IntroSequence intro;//�C���g��
        [HideInInspector]
        public SoundManager Sound;//�T�E���h
        [HideInInspector]
        public EffectManager effectNamager;//�G�t�F�N�g

        //�U�������Ƃ�
        public Subject<Unit> OnAttack = new Subject<Unit>();

        public bool EnableKeybodeMouse = false;
    }
}