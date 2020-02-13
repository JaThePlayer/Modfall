using Microsoft.Xna.Framework;

namespace TowerFall.ModLoader.mm
{
    public static class Events
    {
        public static class QuestSpawnPortal
        {
            public delegate bool SpawnEnemyHandler(Level level, string enemyName, Vector2 position, Facing facing, Vector2[] nodes);
            /// <summary>
            /// Return true if an enemy spawned
            /// </summary>
            public static event SpawnEnemyHandler OnSpawnEnemy;
            internal static bool SpawnEnemy(Level level, string enemyName, Vector2 position, Facing facing, Vector2[] nodes)
            {
                if (OnSpawnEnemy == null)
                {
                    return false;
                }
                return OnSpawnEnemy.Invoke(level, enemyName, position, facing, nodes);
            }
        }
    }
}