using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public Transform nodePrefab;
    public Transform enemyPrefab;
    public Transform turnerPrefab;
    public Transform gunPrefab;
    public Color startColor;
    public Color endColor;
    public Vector3 startPosition;
    private EnemyManager enemyManager;

    void Start() {
        enemyManager = GetComponent<EnemyManager>();
        enemyManager.enemyPrefab = enemyPrefab;
        PlaceTiles(File.ReadAllLines("Assets/Levels/Level1.txt"));
        enemyManager.startPosition = startPosition;
    }

    private void PlaceTiles(string[] lines) {
        for (int x = 0; x < lines[0].Length; x++) {
            for (int z = 0; z < lines.Length; z++) {
                var pos = nodePrefab.position + Vector3.right * x + Vector3.forward * (lines.Length - z - 1);
                var symbol = lines[z][x];
                if ("nesw".Contains(symbol.ToString())) {
                    CreateTurner(pos, symbol);
                } else if (symbol != '.') {
                    var node = Instantiate(nodePrefab, pos, nodePrefab.rotation);
                    if (symbol != '*') {
                        var isStart = symbol == '0';
                        var isEnd = symbol == '1';
                        node.GetComponent<Renderer>().material.color = isStart ?
                            startColor : isEnd ? endColor : Color.blue;
                        if (isStart) {
                            startPosition = pos;
                        }

                        if (symbol == 'g') {
                            var gunTransform = Instantiate(gunPrefab, pos, Quaternion.identity);
                            var gun = gunTransform.GetComponent<Gun>();
                            gun.setEnemyManager(enemyManager);
                        }
                    }
                }
            }
        }
    }

    private void CreateTurner(Vector3 pos, char symbol) {
        var turnerPos = new Vector3(pos.x, 0f, pos.z);
        var turnerTransform = Instantiate(turnerPrefab, turnerPos, Quaternion.identity);
        Turner turner = turnerTransform.GetComponent<Turner>();
        turner.Direction = symbol;
    }

    void Update() {
        enemyManager.UpdateEnemies();
    }
}