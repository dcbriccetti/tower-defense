using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public Transform nodePrefab;
    public Transform enemyPrefab;
    public Transform turnerPrefab;
    public Color startColor;
    public Color endColor;
    public Vector3 startPosition;
    private float launchTime;

    void Start() {
        PlaceTiles(File.ReadAllLines("Assets/Levels/Level1.txt"));
        launchTime = Time.time + 1;
    }

    private void PlaceTiles(string[] lines) {
        for (int x = 0; x < lines[0].Length; x++) {
            for (int z = 0; z < lines.Length; z++) {
                var pos = nodePrefab.position + Vector3.right * x + Vector3.forward * (lines.Length - z - 1);
                var symbol = lines[z][x];
                if ("nsew".Contains(symbol.ToString())) {
                    var turnerTransform = Instantiate(turnerPrefab, pos, Quaternion.identity);
                    Turner turner = turnerTransform.GetComponent<Turner>();
                    turner.direction = symbol;
                } else if (symbol != '.') {
                    var node = Instantiate(nodePrefab, pos, nodePrefab.rotation);
                    if (symbol == '*') 
                        continue;
                    var isStart = symbol == '0';
                    var isEnd = symbol == '1';
                    node.GetComponent<Renderer>().material.color = isStart ? 
                        startColor : isEnd ? endColor : Color.blue;
                    if (isStart) {
                        startPosition = pos;
                    }
                }
            }
        }
    }

    void Update() {
        if (Time.time > launchTime) {
            var pos = new Vector3(startPosition.x, enemyPrefab.position.y, startPosition.z);
            Instantiate(enemyPrefab, pos, enemyPrefab.localRotation);
            launchTime = Time.time + 10;
        }
    }
}