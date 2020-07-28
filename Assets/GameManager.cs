using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public Transform nodePrefab;
    public Transform enemyPrefab;
    public Transform turnerPrefab;
    public Color startColor;
    public Color endColor;
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
                if (symbol == 'l' || symbol == 'r') {
                    var turnerTransform = Instantiate(turnerPrefab, pos, Quaternion.identity);
                    Turner turner = turnerTransform.GetComponent<Turner>();
                    turner.direction = symbol;
                } else if (symbol != '.') {
                    var node = Instantiate(nodePrefab, pos, nodePrefab.rotation);
                    if (symbol != '*') {
                        var color = symbol == 's' ? startColor : symbol == 'e' ? endColor : Color.blue;
                        node.GetComponent<Renderer>().material.color = color;
                    }
                }
            }
        }
    }

    void Update() {
        if (Time.time > launchTime) {
            Instantiate(enemyPrefab);
            launchTime = Time.time + 1;
        }
    }
}