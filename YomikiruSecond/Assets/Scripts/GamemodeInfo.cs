using UnityEngine;

namespace Yomikiru
{

    [CreateAssetMenu(menuName = "ScriptableObject/GamemodeInfo")]
    public class GamemodeInfo : ScriptableObject
    {

        [field: SerializeField] public Gamemode Gamemode { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: TextArea]
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public Sprite Banner { get; private set; }

    }

}
