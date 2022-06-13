using UnityEngine;

namespace Yomikiru
{

    [CreateAssetMenu(menuName = "ScriptableObject/CharacterInfo")]
    public class CharacterInfo : ScriptableObject
    {

        [field: SerializeField] public string Name { get; private set; }
        [field: TextArea]
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public Sprite CharacterImage { get; private set; }

    }

}
