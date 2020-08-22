using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class WaypointFinder {
    public static List<Vector2> CreateWaypoints(IReadOnlyList<string> lines, Vector2 startCoords, Vector2 endCoords) {
        var waypoints = new List<Vector2>();
        var searchPosition = startCoords;
        var searchDirection = new Vector2(0, 1); // “Down”
        Vector2[] directions = {Vector2.up, Vector2.right, Vector2.down, Vector2.left};

        List<Vector2> AllDirectionsExceptOpposite() => directions.Where(d => d != searchDirection * -1).ToList();

        Vector2 NextDirection() =>
            AllDirectionsExceptOpposite().Find(candidateDir => {
                var candidatePos = searchPosition + candidateDir;
                var iLine = (int) candidatePos.y;
                var iCol  = (int) candidatePos.x;
                var inBounds = iLine >= 0 && iLine < lines.Count && iCol >= 0 && iCol < lines[0].Length;
                if (! inBounds) return false;
                var cellCode = lines[iLine][iCol];
                return ".1".Contains(cellCode);
            });

        int moves = 0;
        const int maxMoves = 100;
        while (searchPosition != endCoords && ++moves <= maxMoves) {
            searchPosition += searchDirection;
            var nextDirection = NextDirection();
            if (nextDirection == searchDirection) continue;
            waypoints.Add(new Vector2(searchPosition.x, lines.Count - 1 - searchPosition.y));
            searchDirection = nextDirection;
        }

        if (moves == maxMoves) 
            Debug.LogError("Too many moves");

        return waypoints;
    }
}