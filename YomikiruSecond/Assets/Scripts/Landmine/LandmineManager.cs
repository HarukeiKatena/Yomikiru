using UnityEngine;
using Random = UnityEngine.Random;

namespace Yomikiru.Landmine
{
    public class LandmineManager : MonoBehaviour
    {

        [SerializeField] private Transform[] landminePositions;
        [SerializeField] private int landminesToCreate;
        [SerializeField] private LandmineLight landminePrefab;

        private void Awake()
        {
            // Selection sampling; (number needed) / (number left)
            // 40個の候補から5個選ぶ場合、最初の候補は (5 / 40) の確率で選ぶ
            // 選ばれたら次の候補は (4 / 39) 選ばれなかったら (5 / 39) で選ぶ
            // これを続けるといい感じにランダムで5個選ばれるらしい
            int numNeeded = Mathf.Min(landminesToCreate, landminePositions.Length);
            for (int i = 0; i < landminePositions.Length; i++)
            {
                var numLeft = landminePositions.Length - i;
                if (Random.Range(0, numLeft) < numNeeded)
                {
                    numNeeded--;
                    Instantiate(landminePrefab, landminePositions[i]);
                }
            }
        }

    }
}
