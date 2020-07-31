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
    private List<Vector2> waypoints = new List<Vector2>();

    void Start() {
        enemyManager = GetComponent<EnemyManager>();
        enemyManager.enemyPrefab = enemyPrefab;
        ProcessMapFile(File.ReadAllLines("Assets/Levels/Level1.txt"));
        enemyManager.startPosition = startPosition;
        enemyManager.Waypoints = waypoints;
    }

    private void ProcessMapFile(string[] lines) {
        Vector2 startCoords = Vector2.zero;
        Vector2 endCoords = Vector2.zero;

        for (int iRow = 0; iRow < lines[0].Length; iRow++) {
            for (int iCol = 0; iCol < lines.Length; iCol++) {
                var pos = nodePrefab.position + Vector3.right * iRow + Vector3.forward * (lines.Length - iCol - 1);
                var symbol = lines[iCol][iRow];
                if (symbol == '.') continue;
                
                var node = Instantiate(nodePrefab, pos, nodePrefab.rotation);
                if (symbol == '*') continue;
                    
                var isStart = symbol == '0';
                var isEnd = symbol == '1';
                node.GetComponent<Renderer>().material.color = isStart ?
                    startColor : isEnd ? endColor : Color.blue;
                if (isStart) {
                    startPosition = pos;
                    startCoords = new Vector2(iRow, iCol);
                } else if (isEnd) {
                    endCoords = new Vector2(iRow, iCol);
                }

                if (symbol != 'g') continue;
                    
                var gunTransform = Instantiate(gunPrefab, pos, Quaternion.identity);
                var gun = gunTransform.GetComponent<Gun>();
                gun.EnemyManager = enemyManager;
            }
        }

        CreateWaypoints(lines, startCoords, endCoords);
    }

    private void CreateWaypoints(string[] lines, Vector2 startCoords, Vector2 endCoords) {
        Vector2 searchPos = startCoords;
        Vector2 searchDir = Vector2.up; // Up is down
        Vector2[] searchDirs = {Vector2.up, Vector2.right, Vector2.down, Vector2.left};

        List<Vector2> DirsExceptOpposite() => searchDirs.Where(d => d != searchDir * -1).ToList();

        Vector2 NextDir() => DirsExceptOpposite().Find(candidateDir => {
            Vector2 candidatePos = searchPos + candidateDir;
            int iLine = (int) candidatePos.y;
            int iCol = (int) candidatePos.x;
            bool inBounds = iLine >= 0 && iLine < lines.Length && iCol >= 0 && iCol < lines[0].Length;
            return inBounds && ".1".Contains(lines[iLine][iCol]);
        });

        while (searchPos != endCoords) {
            searchPos += searchDir;
            Vector2 nextDir = NextDir();
            if (nextDir != searchDir) {
                waypoints.Add(searchPos);
                searchDir = nextDir;
            }
        }
    }

    void Update() {
        enemyManager.UpdateEnemies();
    }
}