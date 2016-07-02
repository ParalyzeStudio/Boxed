using System.Collections.Generic;
using UnityEngine;

public class SolutionTree
{
    //maximum height (or depth) of the tree, i.e the maximum number of nodes allowed along a path
    public int m_maximumHeight { get; set; }

    private List<SolutionNode> m_successNodes;

    public Tile[] m_startTiles; //the tile that serves as start point for or tree
    public Tile m_targetTile; //the tile that serves as target for the search of solutions

    public List<Tile> m_bonusTiles;

    public bool m_oneCoveredTileOnly; //when reaching the target tile, decide if it must be the only covered tile or if another tile can be covered by the brick
    public bool m_stopWhenTreeIsSolved; //In case we just want to know if at least one solution exists, we can stop processing nodes as soon as we found one
    public bool m_isSolved; //is this tree solved (i.e contains at least one successful path to target)

    private long m_processedNodesCount;
    
    public SolutionTree(int height, Tile[] startTiles, Tile targetTile, bool oneCoveredTileOnly = true)
    {
        m_maximumHeight = height;
        m_startTiles = startTiles;
        m_targetTile = targetTile;
        m_oneCoveredTileOnly = oneCoveredTileOnly;
        m_successNodes = new List<SolutionNode>();
        m_isSolved = false;

        m_processedNodesCount = 0;
    }

    public SolutionTree(Level levelToSolve) : this(50, 
                                                   null,
                                                   levelToSolve.m_floor.GetFinishTile(),
                                                   true)
    {
        m_startTiles = new Tile[2];
        m_startTiles[0] = levelToSolve.m_floor.GetStartTile();
        m_startTiles[1] = null;
        m_bonusTiles = levelToSolve.m_floor.GetBonusTiles();
    }

    //filters to know which types of solutions we want
    public const int SHORTEST_SOLUTION = 1; //the solution that requires the less movements
    public const int ALL_SOLUTIONS = 2;

    /**
    * Traverse the floor for all directions possible at each step making the tree grow until each path has reached the maximum height of the tree or
    * found a success path
    **/
    public SolutionNode[][] SearchForSolutions(int bFilters = SHORTEST_SOLUTION)
    {
        //Construct 4 nodes and 4 bricks (1 for each rolling direction) starting from level start tile
        SolutionNode[] childNodes = new SolutionNode[4];
        childNodes[0] = new SolutionNode(this, Brick.RollDirection.LEFT, null, 0, new Brick(m_startTiles));
        childNodes[1] = new SolutionNode(this, Brick.RollDirection.RIGHT, null, 0, new Brick(m_startTiles));
        childNodes[2] = new SolutionNode(this, Brick.RollDirection.TOP, null, 0, new Brick(m_startTiles));
        childNodes[3] = new SolutionNode(this, Brick.RollDirection.BOTTOM, null, 0, new Brick(m_startTiles));

        //Process every of the 4 nodes declared above
        for (int i = 0; i != childNodes.Length; i++)
        {
            childNodes[i].Process();
        }

        Debug.Log("processed nodes count:" + GetProcessedNodesCount());

        //now search for paths that are marked as successful and return them
        Debug.Log("bFilters:" + bFilters);
        return ExtractSuccessPaths(bFilters);
    }

    public void AddSuccessNode(SolutionNode node)
    {
        m_isSolved = true;
        m_successNodes.Add(node);
    }

    public void IncrementProcessedNodesCount()
    {
        m_processedNodesCount++;
    }

    public long GetProcessedNodesCount()
    {
        return m_processedNodesCount;
    }

    private SolutionNode[][] ExtractSuccessPaths(int bFilters = SHORTEST_SOLUTION)
    {
        if (m_successNodes.Count == 0)
            return null;        

        if ((bFilters & ALL_SOLUTIONS) > 0)
        {
            SolutionNode[][] allSuccessPaths = new SolutionNode[m_successNodes.Count][];
            for (int i = 0; i != m_successNodes.Count; i++)
            {
                SolutionNode node = m_successNodes[i];
                allSuccessPaths[i] = ExtractPathFromNode(node);
            }

            return allSuccessPaths;
        }
        else if ((bFilters & SHORTEST_SOLUTION) > 0)
        {
            SolutionNode[][] successPath = new SolutionNode[1][];
            successPath[0] = ExtractPathFromNode(m_successNodes[m_successNodes.Count - 1]);

            return successPath;
        }

        return null;
    }

    /**
    * Return the full path with 'node' parameter a leaf node
    **/
    private SolutionNode[] ExtractPathFromNode(SolutionNode node)
    {
        int pathLength = node.m_distanceFromRoot + 1;

        SolutionNode[] path = new SolutionNode[pathLength];

        while (node != null)
        {
            path[pathLength - 1] = node;
            pathLength--;
            node = node.m_parentNode;
        }

        return path;
    }

    /**
    * Tell if the path ending with the parameter 'leafNode' goes through all bonuses
    **/
    private bool PathContainsAllBonuses(SolutionNode[] path)
    {
        int bonusTilesCount = m_bonusTiles.Count;
        bool[] bonusTilesCoveredState = new bool[bonusTilesCount];
        int bonusTilesCoveredCount = 0;

        for (int i = 0; i != path.Length; i++)
        {
            SolutionNode node = path[i];
            for (int p = 0; p != bonusTilesCount; p++)
            {
                if (!bonusTilesCoveredState[p] && node.CoversTile(m_bonusTiles[p]))
                {
                    bonusTilesCoveredState[p] = true;
                    bonusTilesCoveredCount++;
                }
            }
        }

        return bonusTilesCoveredCount == bonusTilesCount;
    }
}

/**
* Node of the above tree that contains various informations such as:
* -a reference to a brick that will simulate the rolling operation over the currently edited level floor
* -the direction of the rolling operation to perform
* -the height of the node inside the tree, tree base nodes have height = 0
**/
public class SolutionNode
{
    public Brick.RollDirection m_direction;
    public SolutionNode m_parentNode; //store the parent node to so we can navigate inside the tree along a path from bottom to top
    public int m_distanceFromRoot; //the distance from root of this node, must be between 0 and (parentTree.MaximumHeight - 1)
    public Brick m_brick; //a brick object that will simulate the rolling operation
    public Tile[] m_coveredTiles; //as a brick object can be reused across several nodes, store here the tiles that are covered at that exact moment

    public SolutionTree m_parentTree;

    public SolutionNode(SolutionTree parentTree, Brick.RollDirection direction, SolutionNode parentNode, int distanceFromRoot, Brick brick)
    {
        m_parentTree = parentTree;
        m_direction = direction;
        m_parentNode = parentNode;
        m_distanceFromRoot = distanceFromRoot;
        m_brick = brick;
        m_coveredTiles = new Tile[2];
        m_coveredTiles[0] = brick.CoveredTiles[0];
        m_coveredTiles[1] = brick.CoveredTiles[1];
    }

    public void Process()
    {
        if (m_parentTree.m_stopWhenTreeIsSolved && m_parentTree.m_isSolved)
            return;

        //we reach the maximum height of the tree
        if (m_distanceFromRoot >= m_parentTree.m_maximumHeight)
            return;

        if (IsCycling())
            return;

        //try to make the brick roll
        Brick.RollResult rollResult;
        Geometry.Edge rotationEdge;
        m_brick.Roll(m_direction, out rollResult, out rotationEdge);

        m_parentTree.IncrementProcessedNodesCount();

        if (rollResult == Brick.RollResult.VALID)
        {
            m_coveredTiles = m_brick.CoveredTiles;

            //set the state of the brick to IDLE so it can roll at once
            m_brick.m_state = Brick.BrickState.IDLE;

            //find the condition telling us that the target tile has been reached
            bool targetTileHasBeenReached = false;
            if (m_parentTree.m_oneCoveredTileOnly)
            {
                if (m_brick.GetCoveredTilesCount() == 1 && m_brick.CoveredTiles[0] == m_parentTree.m_targetTile)
                    targetTileHasBeenReached = true;
            }
            else
            {
                if ((m_brick.CoveredTiles[0] == m_parentTree.m_targetTile || m_brick.CoveredTiles[1] == m_parentTree.m_targetTile))
                    targetTileHasBeenReached = true;
            }            

            if (targetTileHasBeenReached)
            {                
                m_parentTree.AddSuccessNode(this);
                m_parentTree.m_maximumHeight = this.m_distanceFromRoot + 1;
            }
            else //keep processing child nodes
            {
                SolutionNode[] childNodes = Split();
                for (int i = 0; i != childNodes.Length; i++)
                {
                    childNodes[i].Process();
                }
            }
        }
    }

    /**
    * Build child nodes for every relevant direction
    * Reuse the brick from parent node for the first rolling direction
    **/
    public SolutionNode[] Split()
    {
        SolutionNode[] childNodes = new SolutionNode[3];

        if (m_direction == Brick.RollDirection.LEFT)
        {
            childNodes[0] = new SolutionNode(m_parentTree, Brick.RollDirection.LEFT, this, m_distanceFromRoot + 1, m_brick);
            childNodes[1] = new SolutionNode(m_parentTree, Brick.RollDirection.TOP, this, m_distanceFromRoot + 1, new Brick(m_brick));
            childNodes[2] = new SolutionNode(m_parentTree, Brick.RollDirection.BOTTOM, this, m_distanceFromRoot + 1, new Brick(m_brick));
        }
        else if (m_direction == Brick.RollDirection.RIGHT)
        {
            childNodes[0] = new SolutionNode(m_parentTree, Brick.RollDirection.RIGHT, this, m_distanceFromRoot + 1, m_brick);
            childNodes[1] = new SolutionNode(m_parentTree, Brick.RollDirection.TOP, this, m_distanceFromRoot + 1, new Brick(m_brick));
            childNodes[2] = new SolutionNode(m_parentTree, Brick.RollDirection.BOTTOM, this, m_distanceFromRoot + 1, new Brick(m_brick));
        }
        else if (m_direction == Brick.RollDirection.TOP)
        {
            childNodes[0] = new SolutionNode(m_parentTree, Brick.RollDirection.RIGHT, this, m_distanceFromRoot + 1, m_brick);
            childNodes[1] = new SolutionNode(m_parentTree, Brick.RollDirection.LEFT, this, m_distanceFromRoot + 1, new Brick(m_brick));
            childNodes[2] = new SolutionNode(m_parentTree, Brick.RollDirection.TOP, this, m_distanceFromRoot + 1, new Brick(m_brick));
        }
        else
        {
            childNodes[0] = new SolutionNode(m_parentTree, Brick.RollDirection.RIGHT, this, m_distanceFromRoot + 1, m_brick);
            childNodes[1] = new SolutionNode(m_parentTree, Brick.RollDirection.BOTTOM, this, m_distanceFromRoot + 1, new Brick(m_brick));
            childNodes[2] = new SolutionNode(m_parentTree, Brick.RollDirection.LEFT, this, m_distanceFromRoot + 1, new Brick(m_brick));
        }

        return childNodes;
    }

    /**
    * In case of this node underlying data is equal to one of its parent node, we can consider the brick is cycling and we can abort the process
    **/
    public bool IsCycling()
    {
        SolutionNode node = this.m_parentNode;
        int minDistanceForCycling = 5;
        while (node != null)
        {
            if (minDistanceForCycling > 0)
            {
                minDistanceForCycling--;
                node = node.m_parentNode;
                continue;
            }

            if (this.CoversSameTiles(node))
            {
                //Debug.Log("same tiles between node at depth:" + this.m_distanceFromRoot + " and node at depth:" + node.m_distanceFromRoot);
                //Debug.Log("this coveredTiles[0]:" + this.m_brick.CoveredTiles[0].m_columnIndex + " - " + this.m_brick.CoveredTiles[0].m_lineIndex);
                //Debug.Log("node coveredTiles[0]:" + node.m_brick.CoveredTiles[0].m_columnIndex + " - " + node.m_brick.CoveredTiles[0].m_lineIndex);
                return true;
            }

            node = node.m_parentNode;            
        }

        return false;
    }

    public bool CoversTile(Tile tile)
    {
        if (m_coveredTiles[0].HasSameFloorPosition(tile))
            return true;
        if (m_coveredTiles[1] != null && m_coveredTiles[1].HasSameFloorPosition(tile))
            return true;

        return false;
    }

    private bool CoversSameTiles(SolutionNode node)
    {
        int coveredTilesCount = this.GetCoveredTilesCount();
        if (coveredTilesCount != node.GetCoveredTilesCount())
            return false;

        if (coveredTilesCount == 1)
        {
            if (CoversTile(node.m_coveredTiles[0]))
                return true;
        }
        else
        {
            if (CoversTile(node.m_coveredTiles[0]) && CoversTile(node.m_coveredTiles[1]))
                return true;
        }

        return false;
    }

    private int GetCoveredTilesCount()
    {
        if (m_coveredTiles[1] == null)
            return 1;

        return 2;
    }
}