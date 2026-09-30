namespace Game.Shop
{
    public interface IShopPoolRule
    {
        float GetWeight(ShopItemData item, int waveIndex);
    }
}
