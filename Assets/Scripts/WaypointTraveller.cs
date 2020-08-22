using System.Collections.Generic;
using UnityEngine;

public class WaypointTraveller {
    private readonly List<Vector2> waypoints;

    public WaypointTraveller(List<Vector2> waypoints) {
        this.waypoints = waypoints;
    }

    /// <summary>
    /// Returns the position of a point moving a distance through waypoints
    /// </summary>
    /// <param name="position">the starting position</param>
    /// <param name="waypointIndex">the index of the next waypoint</param>
    /// <param name="distance">the distance to travel</param>
    /// <returns>the position after travel</returns>
    public Vector3 PositionAfterTravel(Vector3 position, int waypointIndex, float distance) {
        Vector3 waypoint = GetAdjustedWaypoint(waypointIndex, position.y);
        var distanceToWaypoint = (waypoint - position).magnitude;
        if (distance <= distanceToWaypoint) {
            var dir = (waypoint - position).normalized;
            return position + dir * distance;
        }

        // Travel continues towards the next waypoint (or stops if at the last waypoint)
        var lastWaypointIndex = waypoints.Count - 1;
        return waypointIndex == lastWaypointIndex ? 
            GetAdjustedWaypoint(lastWaypointIndex, position.y) : 
            PositionAfterTravel(waypoint, waypointIndex + 1, distance - distanceToWaypoint);
    }

    public Vector3 GetAdjustedWaypoint( int waypointIndex, float height) {
        var waypoint = waypoints[waypointIndex];
        return new Vector3(waypoint.x, height, waypoint.y);
    }
}
