using UniRx;
using UnityEngine;
using UnityExtensions;
using Yomikiru.Effect;
using Yomikiru.Sound;

namespace Player
{
    public class PlayerPropSetting : MonoBehaviour
    {
        [Header("設定")]
        [InspectInline(canEditRemoteTarget = true)]
        public PlayerProperty playerProperty;
        public InputRecord inputRecord;
        public Animator Animator;
        public PlayerCollision collision;
        public AudioSource audioSource;

        [Header("自動設定される奴")]
        public int PlayerIndex = 0;

        [HideInInspector]
        public GameObject CinemachineCameraTarget;//シネマシーンのターゲット
        [HideInInspector]
        public PlayerManagement playerMana;//プレイヤーマネジメント
        [HideInInspector]
        public IntroSequence intro;//イントロ
        [HideInInspector]
        public SoundManager Sound;//サウンド
        [HideInInspector]
        public EffectManager effectNamager;//エフェクト

        //攻撃したとき
        public Subject<Unit> OnAttack = new Subject<Unit>();

        public bool EnableKeybodeMouse = false;
    }
}