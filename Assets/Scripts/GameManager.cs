using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public Transform groundPrefab;
    public Transform nodePrefab;
    public new Transform camera;
    public Color startColor;
    public Color endColor;
    public Vector3 startPosition;
    private EnemyManager enemyManager;
    private List<Vector2> waypoints;
    private int numRows, numCols;

    private void Start() {
        enemyManager = GetComponent<EnemyManager>();
        var lines = File.ReadAllLines("Assets/Levels/Level1.txt");
        ProcessMapFile(lines);
        var ground = Instantiate(groundPrefab);
        ground.transform.localScale += Vector3.right * (numCols - 1) + Vector3.forward * (numRows - 1);
        ground.position += Vector3.right * (numCols / 2f - .5f) + Vector3.forward * (numRows / 2f - .5f);
        PositionCamera();
        enemyManager.startPosition = startPosition;
        enemyManager.Waypoints = waypoints;
    }

    private void PositionCamera() {
        var p = camera.position;
        camera.position = new Vector3(numCols / 2f, p.y, p.z);
    }

    private void ProcessMapFile(IReadOnlyList<string> lines) {
        var startCoords = Vector2.zero;
        var endCoords = Vector2.zero;

        numRows = lines.Count;
        numCols = lines[0].Length;
        for (var iRow = 0; iRow < numRows; iRow++) {
            for (var iCol = 0; iCol < numCols; iCol++) {
                var symbol = lines[iRow][iCol];
                if (symbol == '.') continue;
                var raise = "01".Contains(symbol) ? Vector3.up / 2 : Vector3.zero; 
                var pos = nodePrefab.position + Vector3.right * iCol + Vector3.forward * (lines.Count - iRow - 1) + raise;
                var nodeObject = Instantiate(nodePrefab, pos, nodePrefab.rotation);
                if (symbol == '*') continue;

                var material = nodeObject.GetComponent<Renderer>().material;
                if (symbol == '0') {
                    material.color = startColor;
                    startPosition = pos;
                    startCoords = new Vector2(iCol, iRow);
                } else if (symbol == '1') {
                    material.color = endColor;
                    endCoords = new Vector2(iCol, iRow);
                }

                if (symbol == 'g') {
                    nodeObject.GetComponent<Node>().AddGun();
                }
            }
        }

        waypoints = WaypointFinder.CreateWaypoints(lines, startCoords, endCoords);
    }

}