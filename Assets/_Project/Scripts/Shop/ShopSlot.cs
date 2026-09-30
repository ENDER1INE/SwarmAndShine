namespace Game.Shop
{
    public class ShopSlot
    {
        public ShopItemData Item { get; private set; }
        public bool IsLocked { get; private set; }
        public bool IsEmpty => Item == null;

        internal void SetItem(ShopItemData item)
        {
            Item = item;
        }

        internal void SetLocked(bool isLocked)
        {
            IsLocked = isLocked;
        }
    }
}
