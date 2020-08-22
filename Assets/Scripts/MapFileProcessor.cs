using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public static class MapFileProcessor {
    public class MapDescription {
        public Vector2 Dimensions { get; }
        public Vector3 StartPosition { get;  }
        [NotNull] public List<Vector3> NodePositions { get; }
        public List<Vector2> Waypoints { get; }

        public MapDescription( Vector2 dimensions, Vector3 startPosition,
            [NotNull] List<Vector3> nodePositions, List<Vector2> waypoints) {
            Dimensions = dimensions;
            StartPosition = startPosition;
            NodePositions = nodePositions ?? throw new ArgumentNullException(nameof(nodePositions));
            Waypoints = waypoints;
        }
    }

    private enum MapSymbol { Start = '0', End = '1', Path = '.', Node = '*' }

    public static MapDescription CreateMapDescription(int level) {
        var startPosition = Vector3.zero;
        var startCoords = Vector2.zero;
        var endCoords = Vector2.zero;
        var nodePositions = new List<Vector3>();

        var mapLines = Resources.Load<TextAsset>($"Levels/{level}/map").text.Split('\n');
        var numRows = mapLines.Length;
        var numCols = mapLines[0].Length;

        for (var iRow = 0; iRow < numRows; iRow++) {
            for (var iCol = 0; iCol < numCols; iCol++) {
                var symbol = (MapSymbol) mapLines[iRow][iCol];
                if (symbol == MapSymbol.Path) continue;

                bool startOrEnd = symbol == MapSymbol.Start || symbol == MapSymbol.End;
                Vector3 nodeRaise = startOrEnd ? Vector3.up * .75f : Vector3.zero;
                Vector3 nodePos = Vector3.right * iCol + Vector3.forward * (numRows - iRow - 1) + nodeRaise;
                nodePositions.Add(nodePos);

                switch (symbol) {
                    case MapSymbol.Start:
                        startPosition = nodePos;
                        startCoords = new Vector2(iCol, iRow);
                        break;
                    case MapSymbol.End:
                        endCoords = new Vector2(iCol, iRow);
                        break;
                }
            }
        }

        var waypoints = WaypointFinder.CreateWaypoints(mapLines, startCoords, endCoords);
        var dimensions = new Vector2(numCols, numRows);
        
        return new MapDescription(dimensions, startPosition, nodePositions, waypoints);
    }
}
