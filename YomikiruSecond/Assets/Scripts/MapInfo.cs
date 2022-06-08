using UnityEngine;

namespace Yomikiru
{

    [CreateAssetMenu(menuName = "ScriptableObject/MapInfo")]
    public class MapInfo : ScriptableObject
    {

        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string SceneName { get; private set; }
        [field: SerializeField] public Sprite Banner { get; private set; }
        [field: SerializeField] public AreaBox AreaBox { get; private set; }
    }

}
