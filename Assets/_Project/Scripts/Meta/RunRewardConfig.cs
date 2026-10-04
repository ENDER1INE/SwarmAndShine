using UnityEngine;

namespace Game.Meta
{
    [CreateAssetMenu(fileName = "RunRewardConfig", menuName = "SAS/Meta/Run Reward Config")]
    public class RunRewardConfig : ScriptableObject
    {
        [SerializeField] private int _baseReward = 10;
        [SerializeField] private int _perEnemyKilled = 1;
        [SerializeField] private int _perWaveCompleted = 5;
        [SerializeField] private int _perMinuteSurvived = 5;
        [Tooltip("Granted when the run ends in victory. Becomes the mini-boss bonus once the boss exists.")]
        [SerializeField] private int _victoryBonus = 50;

        public RunRewardRates ToRates()
        {
            return new RunRewardRates(_baseReward, _perEnemyKilled, _perWaveCompleted, _perMinuteSurvived, _victoryBonus);
        }
    }
}
