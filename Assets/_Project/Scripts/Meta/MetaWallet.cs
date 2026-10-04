using Game.Core;

namespace Game.Meta
{
    public class MetaWallet
    {
        private readonly SaveData _saveData;

        public int Balance => _saveData.MetaCurrency;

        public MetaWallet(SaveData saveData)
        {
            _saveData = saveData;
        }

        public void Add(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            _saveData.MetaCurrency += amount;
            GameEvents.Publish(new MetaCurrencyChangedEvent(Balance));
        }

        public bool TrySpend(int amount)
        {
            if (amount <= 0 || amount > Balance)
            {
                return false;
            }

            _saveData.MetaCurrency -= amount;
            GameEvents.Publish(new MetaCurrencyChangedEvent(Balance));
            return true;
        }
    }
}
