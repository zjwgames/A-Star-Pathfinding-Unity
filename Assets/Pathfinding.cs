using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinding : MonoBehaviour {

    public Transform seeker, target;

    Grid grid;

    void Awake()
    {
        grid = GetComponent<Grid>();
    }

    void Update()
    {
        FindPath(seeker.position, target.position);
    }

    // A* pathfinding algorithm
    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.GetNodeFromWorldPoint(startPos);
        Node targetNode = grid.GetNodeFromWorldPoint(targetPos);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            // Set current node to the node with the lowest f cost in open set
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            // Remove current node from open set and add current node to closed set
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            // If current node is the target node then we have found our path
            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            // Loop through each neighbour of the current node and calculate their f values
            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                // If cannot be traversed or is in closed set then skip to next neighbour
                if (!neighbour.walkable || closedSet.Contains(neighbour)) continue;

                // Check new path to neighbour is shorter or neighbour is not in the open set
                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    // Set g, h costs
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);

                    // Set the parent of the neighbour as the current node
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }
    }

    // Trace path back to start, based on path found
    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        // Add nodes to path
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent; // update current node to parent
        }

        path.Reverse();

        grid.path = path;
    }

    // Calculate grid based distance between two nodes
    int GetDistance(Node nodeA, Node nodeB)
    {
        // Functions for distance to get from A to B through a grid as follows:
        // 14y + 10(x-y) for y < x, or, 14x + 10(y-x) for x < y
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY) return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}
