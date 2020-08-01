using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaypointFinder {
    public static List<Vector2> CreateWaypoints(IReadOnlyList<string> lines, Vector2 startCoords, Vector2 endCoords) {
        List<Vector2> waypoints = new List<Vector2>();
        var searchPos = startCoords;
        var searchDir = Vector2.up; // Up is down
        Vector2[] searchDirs = {Vector2.up, Vector2.right, Vector2.down, Vector2.left};

        List<Vector2> DirsExceptOpposite() => searchDirs.Where(d => d != searchDir * -1).ToList();

        Vector2 NextDir() => DirsExceptOpposite().Find(candidateDir => {
            var candidatePos = searchPos + candidateDir;
            var iLine = (int) candidatePos.y;
            var iCol = (int) candidatePos.x;
            var inBounds = iLine >= 0 && iLine < lines.Count && iCol >= 0 && iCol < lines[0].Length;
            return inBounds && ".1".Contains(lines[iLine][iCol]);
        });

        while (searchPos != endCoords) {
            searchPos += searchDir;
            var nextDir = NextDir();
            if (nextDir != searchDir) {
                waypoints.Add(searchPos);
                searchDir = nextDir;
            }
        }

        return waypoints;
    }
}