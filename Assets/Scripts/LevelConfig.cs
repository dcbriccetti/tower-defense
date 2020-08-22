using UnityEngine;

[CreateAssetMenu(fileName = "Level.asset", menuName = "TowerDefense/Level Configuration", order = 1)]
public class LevelConfig : ScriptableObject {
    public int startingWaveIndex;
    public int secondsBetweenWaves = 10;
    public WaveConfig[] waveConfigs;
}
