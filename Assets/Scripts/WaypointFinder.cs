using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class WaypointFinder {
    public static List<Vector2> CreateWaypoints(IReadOnlyList<string> lines, Vector2 startCoords, Vector2 endCoords) {
        List<Vector2> waypoints = new List<Vector2>();
        var searchPos = startCoords;
        var searchDir = new Vector2(0, 1); // “Down”
        Vector2[] searchDirs = {Vector2.up, Vector2.right, Vector2.down, Vector2.left};

        List<Vector2> DirsExceptOpposite() => searchDirs.Where(d => d != searchDir * -1).ToList();

        Vector2 NextDir() =>
            DirsExceptOpposite().Find(candidateDir => {
                var candidatePos = searchPos + candidateDir;
                var iLine = (int) candidatePos.y;
                var iCol = (int) candidatePos.x;
                var inBounds = iLine >= 0 && iLine < lines.Count && iCol >= 0 && iCol < lines[0].Length;
                return inBounds && ".1".Contains(lines[iLine][iCol]);
            });

        int moves = 0;
        const int maxMoves = 100;
        while (searchPos != endCoords && ++moves <= maxMoves) {
            searchPos += searchDir;
            var nextDir = NextDir();
            if (nextDir != searchDir) {
                waypoints.Add(new Vector2(searchPos.x, lines.Count - 1 - searchPos.y));
                searchDir = nextDir;
            }
        }

        if (moves == maxMoves) 
            Debug.LogError("Too many moves");

        return waypoints;
    }
}