namespace TowerFall.ModLoader.mm
{
    public static class Events
    {
        public static class QuestSpawnPortal
        {
            public delegate bool SpawnEnemyHandler(string enemyName);
            /// <summary>
            /// Return true if an enemy spawned
            /// </summary>
            public static event SpawnEnemyHandler OnSpawnEnemy;
            internal static bool SpawnEnemy(string enemyName)
            {
                if (OnSpawnEnemy == null)
                {
                    return false;
                }
                return OnSpawnEnemy.Invoke(enemyName);
            }
        }
    }
}