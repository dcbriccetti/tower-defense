using System.Collections.Generic;
using UnityEngine;

public class WaypointTraveller {
    private readonly List<Vector2> waypoints;

    public WaypointTraveller(List<Vector2> waypoints) {
        this.waypoints = waypoints;
    }

    public Vector3 PositionThroughWaypoints(Vector3 pos, int waypointIndex, float distanceRemaining) {
        Vector3 waypoint = GetAdjustedWaypoint(waypointIndex, pos.y);
        var distToWp = (waypoint - pos).magnitude;
        if (distanceRemaining <= distToWp) {
            var dir = (waypoint - pos).normalized;
            return pos + dir * distanceRemaining;
        }

        var lastWaypointIndex = waypoints.Count - 1;
        return waypointIndex == lastWaypointIndex ? 
            GetAdjustedWaypoint(lastWaypointIndex, pos.y) : 
            PositionThroughWaypoints(waypoint, waypointIndex + 1, distanceRemaining - distToWp);
    }

    public Vector3 GetAdjustedWaypoint( int waypointIndex, float height) {
        var waypoint = waypoints[waypointIndex];
        return new Vector3(waypoint.x, height, waypoint.y);
    }
}
