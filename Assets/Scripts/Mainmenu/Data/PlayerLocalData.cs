public static class PlayerLocalData
{
    public static UserData userData;
    public static PlayerStats playerStats;
    public static InventoryData inventoryData;

    public static bool IsPlayerStatsLoaded => playerStats != null;
}
