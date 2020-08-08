using UnityEngine;

[CreateAssetMenu(fileName = "Wave.asset", menuName = "TowerDefense/Wave Configuration", order = 1)]
public class WaveConfig : ScriptableObject {
    public int numEnemies;
    public float secondsBetweenEnemies;
    public Transform[] enemyPrefabs;
}
