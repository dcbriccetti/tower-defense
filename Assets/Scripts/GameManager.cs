using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public Transform nodePrefab;
    public Transform enemyPrefab;
    public Transform gunPrefab;
    public Color startColor;
    public Color endColor;
    public Vector3 startPosition;
    private EnemyManager enemyManager;
    private List<Vector2> waypoints;

    private void Start() {
        enemyManager = GetComponent<EnemyManager>();
        enemyManager.enemyPrefab = enemyPrefab;
        ProcessMapFile(File.ReadAllLines("Assets/Levels/Level1.txt"));
        enemyManager.startPosition = startPosition;
        enemyManager.Waypoints = waypoints;
    }

    private void ProcessMapFile(IReadOnlyList<string> lines) {
        var startCoords = Vector2.zero;
        var endCoords = Vector2.zero;

        for (var iRow = 0; iRow < lines[0].Length; iRow++) {
            for (var iCol = 0; iCol < lines.Count; iCol++) {
                var symbol = lines[iCol][iRow];
                if (symbol == '.') continue;
                var raise = "01".Contains(symbol) ? Vector3.up / 2 : Vector3.zero; 
                var pos = nodePrefab.position + Vector3.right * iRow + Vector3.forward * (lines.Count - iCol - 1) + raise;
                var node = Instantiate(nodePrefab, pos, nodePrefab.rotation);
                if (symbol == '*') continue;

                var material = node.GetComponent<Renderer>().material;
                if (symbol == '0') {
                    material.color = startColor;
                    startPosition = pos;
                    startCoords = new Vector2(iRow, iCol);
                } else if (symbol == '1') {
                    material.color = endColor;
                    endCoords = new Vector2(iRow, iCol);
                }

                if (symbol != 'g') continue;
                    
                var gunTransform = Instantiate(gunPrefab, pos, Quaternion.identity);
                var gun = gunTransform.GetComponent<Gun>();
                gun.EnemyManager = enemyManager;
            }
        }

        waypoints = WaypointFinder.CreateWaypoints(lines, startCoords, endCoords);
    }

}