﻿using System.Collections.Generic;
using UnityEngine;

public class SolutionTree
{
    private Level m_levelToSolve;
        
    public int m_maximumHeight { get; set; }//maximum height (or depth) of the tree, i.e the maximum number of nodes allowed along a path
    public bool m_maximumHeightReached { get; set; } //did we reach a leaf node of this tree

    private List<SolutionNode> m_successNodes;

    public Tile[] m_startTiles; //the tile that serves as start point for or tree
    public Tile m_targetTile; //the tile that serves as target for the search of solutions

    public List<Tile> m_bonusTiles; //a copy of the tiles holding bonuses
    public HashSet<Tile> m_traversedBonusTiles;

    public Brick m_pawn; //the brick serving as a pawn
    public SolutionNode m_currentNode; //the node the pawn is currently on

    public bool m_oneCoveredTileOnly; //when reaching the target tile, decide if it must be the only covered tile or if another tile can be covered by the brick
    public bool m_stopWhenTreeIsSolved; //In case we just want to know if at least one solution exists, we can stop processing nodes as soon as we found one
    public bool m_isSolved; //is this tree solved (i.e contains at least one successful path to target)

    private long m_processedNodesCount;
    private long m_validNodesCount;

    //store here globally some variables to be able to use threads with delegates of type void functionName(void)
    private int m_filters;
    public SolutionNode[][] m_solutions { get; set; }
    private Level.ValidationData m_validationData;

    public SolutionTree(int height, Tile[] startTiles, Tile targetTile, bool oneCoveredTileOnly = true)
    {
        m_maximumHeight = height;
        m_maximumHeightReached = false;
        m_startTiles = startTiles;
        m_targetTile = targetTile;
        m_oneCoveredTileOnly = oneCoveredTileOnly;
        m_successNodes = new List<SolutionNode>();
        m_isSolved = false;        
        m_traversedBonusTiles = new HashSet<Tile>();

        m_processedNodesCount = 0;
        m_validNodesCount = 0;
    }

    public SolutionTree(int height, Level levelToSolve) : this(height,
                                                          null,
                                                          levelToSolve.m_floor.GetFinishTile(),
                                                          true)
    {
        m_levelToSolve = levelToSolve;
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
    //public SolutionNode[][] SearchForSolutions(int bFilters = SHORTEST_SOLUTION)
    //{
    //    //Construct 4 nodes and 4 bricks (1 for each rolling direction) starting from level start tile
    //    Brick b1 = new Brick();
    //    Brick b2 = new Brick();
    //    Brick b3 = new Brick();
    //    Brick b4 = new Brick();
    //    b1.PlaceOnTiles(m_startTiles);
    //    b2.PlaceOnTiles(m_startTiles);
    //    b3.PlaceOnTiles(m_startTiles);
    //    b4.PlaceOnTiles(m_startTiles);

    //    SolutionNode[] childNodes = new SolutionNode[4];
    //    childNodes[0] = new SolutionNode(this, Brick.RollDirection.LEFT, null, 0, b1);
    //    childNodes[1] = new SolutionNode(this, Brick.RollDirection.RIGHT, null, 0, b2);
    //    childNodes[2] = new SolutionNode(this, Brick.RollDirection.TOP, null, 0, b3);
    //    childNodes[3] = new SolutionNode(this, Brick.RollDirection.BOTTOM, null, 0, b4);

    //    //Process every of the 4 nodes declared above
    //    for (int i = 0; i != childNodes.Length; i++)
    //    {
    //        childNodes[i].Process();
    //    }
        
    //    //now search for paths that are marked as successful and return them
    //    return ExtractSuccessPaths(bFilters);
    //}

    public void SearchForSolutions(Level.ValidationData validationData, int bFilters = SHORTEST_SOLUTION)
    {
        m_filters = bFilters;
        m_validationData = validationData;

        //QueuedThreadedJobsManager threadManager = GameController.GetInstance().GetComponent<QueuedThreadedJobsManager>();
        //ThreadedJob job = new ThreadedJob(RunSolutionSearchingThread, null, OnFinishComputingSolutions);
        //threadManager.AddJob(job);

        RunSolutionSearchingThread();
        OnFinishComputingSolutions();
    }

    private void RunSolutionSearchingThread()
    {
        //Construct 1 brick that will serve as a pawn moving across the tree exploring paths
        m_pawn = new Brick();
        m_pawn.PlaceOnTiles(m_startTiles);

        m_pawn.AddCurrentCoveredTilesToRolledOnTiles();

        //construct a root node with arbitrary direction (does not matter there)
        m_currentNode = new SolutionNode(Brick.RollDirection.BOTTOM_LEFT, null, 0);

        int maxMoves = 100000;
        int movesCount = 0;
        while (!(m_stopWhenTreeIsSolved && m_isSolved))
        {
            if (!MovePawn())
                break;

            if (movesCount >= maxMoves)
            {
                Debug.Log("Reached max move count");
                break;
            }
            else
                movesCount++;
        }

        //now search for paths that are marked as successful and return them
        m_solutions = ExtractSuccessPaths(m_filters);
    }

    public void OnFinishComputingSolutions()
    {
        m_levelToSolve.OnFinishComputingSolutionsForTree(this, m_validationData);
    }

    public void AddSuccessNode(SolutionNode node)
    {
        m_successNodes.Add(node);
    }

    public void IncrementProcessedNodesCount()
    {
        m_processedNodesCount++;
    }

    public void IncrementValidNodesCount()
    {
        m_validNodesCount++;
    }

    public long GetProcessedNodesCount()
    {
        return m_processedNodesCount;
    }

    public long GetValidNodesCount()
    {
        return m_validNodesCount;
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
        int pathLength = node.m_distanceFromRoot;

        SolutionNode[] path = new SolutionNode[pathLength];

        while (node.m_parentNode != null) //do not take into account the root node
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
    public bool PathContainsAllBonuses(SolutionNode leafNode)
    {
        SolutionNode[] path = ExtractPathFromNode(leafNode);

        //build a dummy brick that will roll on this path
        //Brick pawn = new Brick();
        //pawn.PlaceOnTiles(m_startTiles);

        //count how many bonuses are traversed
        int bonusTilesCount = m_bonusTiles.Count;
        bool[] bonusTilesCoveredState = new bool[bonusTilesCount];
        int bonusTilesCoveredCount = 0;

        Brick.CoveredTiles coveredTiles = new Brick.CoveredTiles(m_startTiles);

        //check the start tiles
        for (int p = 0; p != bonusTilesCount; p++)
        {
            if (!bonusTilesCoveredState[p] && coveredTiles.ContainsBonus(m_bonusTiles[p].AttachedBonus))
            {
                bonusTilesCoveredState[p] = true;
                bonusTilesCoveredCount++;
            }
        }        

        //check other tiles
        for (int i = 0; i != path.Length; i++)
        {
            SolutionNode node = path[i];
            coveredTiles = coveredTiles.GetNextCoveredTilesForDirection(m_levelToSolve.m_floor, node.m_direction);

            for (int p = 0; p != bonusTilesCount; p++)
            {
                if (!bonusTilesCoveredState[p] && coveredTiles.ContainsBonus(m_bonusTiles[p].AttachedBonus))
                {
                    bonusTilesCoveredState[p] = true;
                    bonusTilesCoveredCount++;
                }
            }
        }

        return bonusTilesCoveredCount == bonusTilesCount;

        //int bonusTilesCount = m_bonusTiles.Count;
        //bool[] bonusTilesCoveredState = new bool[bonusTilesCount];
        //int bonusTilesCoveredCount = 0;

        //SolutionNode node = leafNode;
        //while (node != null)
        //{
        //    for (int p = 0; p != bonusTilesCount; p++)
        //    {
        //        if (!bonusTilesCoveredState[p] && node.CoversTile(m_bonusTiles[p]))
        //        {
        //            bonusTilesCoveredState[p] = true;
        //            bonusTilesCoveredCount++;
        //        }
        //    }

        //    node = node.m_parentNode;
        //}

        //return bonusTilesCoveredCount == bonusTilesCount;
    }

    /**
    * Try to move the pawn forward. If not possible, roll back until a forward movement is possible
    **/
    public bool MovePawn()
    {
        Brick.RollDirection pawnNextMoveDirection = CanMovePawnForward();

        if (pawnNextMoveDirection != Brick.RollDirection.NONE)
        {
            MovePawnForward(pawnNextMoveDirection);
            return true;
        }
        else
        {
            if (m_currentNode.m_parentNode == null) //root node, we finished traversing the tree
            {
                //Debug.Log("Cannot move anymore");
                return false;
            }
            else
            {
                MovePawnBackward();
                return true;
            }
        }
    }

    /**
    * Tell if the pawn can move forward on one of the 4 child nodes of this node
    **/
    private Brick.RollDirection CanMovePawnForward()
    {
        for (int i = 0; i != m_currentNode.m_remainingMoves.Length; i++)
        {
            if (m_currentNode.m_remainingMoves[i])
            {
                return m_currentNode.GetDirectionForRemainingForwardMove(i);
            }
        }

        return Brick.RollDirection.NONE;
    }

    private void MovePawnForward(Brick.RollDirection direction)
    {
        if (direction == Brick.RollDirection.BOTTOM_LEFT)
            m_currentNode.m_remainingMoves[0] = false;
        else if (direction == Brick.RollDirection.TOP_LEFT)
            m_currentNode.m_remainingMoves[1] = false;
        else if (direction == Brick.RollDirection.TOP_RIGHT)
            m_currentNode.m_remainingMoves[2] = false;
        else if (direction == Brick.RollDirection.BOTTOM_RIGHT)
            m_currentNode.m_remainingMoves[3] = false;
                
        int nextNodeDistanceFromRoot = m_currentNode.m_distanceFromRoot + 1;
        SolutionNode nextNode = new SolutionNode(direction, m_currentNode, nextNodeDistanceFromRoot);
        if (nextNodeDistanceFromRoot == m_maximumHeight - 1) //leaf node
            nextNode.SetAsLeaf();

        SolutionNode previousNode = m_currentNode;
        m_currentNode = nextNode;
        
        if (!PerformRolling(direction)) //dead node, revert the m_currentNode to the previous node and reset the state of the pawn to IDLE
        {
            //Debug.Log("FAILED MOVING PAWN FORWARD in direction " + direction);

            m_pawn.m_state = Brick.BrickState.IDLE;
            m_currentNode = previousNode;
        }
        else
        {
            //Debug.Log("MOVED PAWN FORWARD with success in direction " + direction);

            //add the covered tiles to the pawn list
            m_pawn.AddCurrentCoveredTilesToRolledOnTiles();
           
            //test if we ended up on a cycle
            List<Brick.CoveredTiles> cycle;
            bool isCycling = m_pawn.IsCycling(m_currentNode.m_distanceFromRoot, out cycle);
            if (isCycling)
            {
                //There is an exception to this rule, as we are allowed to cycle if we rolled on a tile that adds something to the gameplay (bonus, switch...)
                //So we need to check if the covered tiles inside a cycle contains such a tile
                for (int i = 0; i != cycle.Count; i++)
                {
                    Tile[] cycleTiles = cycle[i].GetAsTruncatedArray();
                    for (int j = 0; j != cycleTiles.Length; j++)
                    {
                        //check bonuses
                        if (cycleTiles[j].AttachedBonus != null && !m_traversedBonusTiles.Contains(cycleTiles[j]))
                        {
                            isCycling = false;
                            break;
                        }
                    }

                    if (!isCycling)
                        break;
                }

                if (isCycling)
                {
                    //if we end up on a cycle set the node as a leaf node
                    m_currentNode.SetAsLeaf();
                }
            }

            //check if the covered tiles contains some bonus
            Tile[] coveredTiles = m_pawn.m_coveredTiles.GetAsTruncatedArray();
            for (int i = 0; i != coveredTiles.Length; i++)
            {
                if (coveredTiles[i].AttachedBonus != null)
                    m_traversedBonusTiles.Add(coveredTiles[i]);
            }
        }
    }

    private void MovePawnBackward()
    {
        //Debug.Log("MovePawnBackward");

        Brick.RollDirection currentNodeDirection = m_currentNode.m_direction;

        //Find the opposite direction
        Brick.RollDirection oppDirection = Brick.RollDirection.NONE;
        if (currentNodeDirection == Brick.RollDirection.BOTTOM_LEFT)
            oppDirection = Brick.RollDirection.TOP_RIGHT;
        else if (currentNodeDirection == Brick.RollDirection.TOP_LEFT)
            oppDirection = Brick.RollDirection.BOTTOM_RIGHT;
        else if (currentNodeDirection == Brick.RollDirection.TOP_RIGHT)
            oppDirection = Brick.RollDirection.BOTTOM_LEFT;
        else if (currentNodeDirection == Brick.RollDirection.BOTTOM_RIGHT)
            oppDirection = Brick.RollDirection.TOP_LEFT;



        //Debug.Log("MOVED PAWN BACKWARD in direction " + oppDirection);


        //rollback actions
        Tile[] currentCoveredTiles = m_pawn.m_coveredTiles.GetAsTruncatedArray();
        //on switches and bonuses
        for (int i = 0; i != currentCoveredTiles.Length; i++)
        {
            if (currentCoveredTiles[i].CurrentState == Tile.State.SWITCH)
                ((SwitchTile)currentCoveredTiles[i]).Toggle();
            if (currentCoveredTiles[i].AttachedBonus != null)
                m_traversedBonusTiles.Remove(currentCoveredTiles[i]);
        }

        //on ice tiles
        Brick.CoveredTiles previousRolledOnTiles = m_pawn.GetRolledOnTilesAtDistanceFromLast(1);
        if (previousRolledOnTiles != null)
        {
            Tile[] previousCoveredTiles = previousRolledOnTiles.GetAsTruncatedArray();
            for (int i = 0; i != previousCoveredTiles.Length; i++)
            {
                if (previousCoveredTiles[i].CurrentState == Tile.State.ICE)
                    ((IceTile)previousCoveredTiles[i]).m_blocked = false;
            }
        }

        //make the brick roll
        Brick.RollResult rollResult;
        Geometry.Edge rotationEdge;
        m_pawn.Roll(oppDirection, out rollResult, out rotationEdge);
        m_pawn.m_state = Brick.BrickState.IDLE;

        m_currentNode = m_currentNode.m_parentNode;

        m_pawn.RemoveLastRolledOnTiles();
    }
    
    /**
    * Process the current node by performing a rolling movement of the brick
    * Return true if the movement of the brick is valid, false otherwise
    **/
    private bool PerformRolling(Brick.RollDirection direction)
    {
        IncrementProcessedNodesCount();

        Brick.CoveredTiles previousCoveredTiles = m_pawn.m_coveredTiles;

        //try to make the brick roll
        Brick.RollResult rollResult;
        Geometry.Edge rotationEdge;
        m_pawn.Roll(direction, out rollResult, out rotationEdge);

        if (rollResult == Brick.RollResult.VALID)
        {
            m_pawn.OnFinishRolling();

            IncrementValidNodesCount();
            //m_coveredTiles = m_brick.CoveredTiles;

            //find the condition telling us that the target tile has been reached
            bool targetTileHasBeenReached = false;
            if (m_oneCoveredTileOnly)
            {
                if (m_pawn.m_coveredTiles.GetCount() == 1 && m_pawn.m_coveredTiles.GetTileAtIndex(0).CurrentState == Tile.State.FINISH)
                    targetTileHasBeenReached = true;
            }
            else
            {
                if ((m_pawn.m_coveredTiles.GetTileAtIndex(0) == m_targetTile || m_pawn.m_coveredTiles.GetTileAtIndex(1) == m_targetTile))
                    targetTileHasBeenReached = true;
            }

            if (targetTileHasBeenReached)
            {
                //we want to check if this path contains all bonuses
                if (PathContainsAllBonuses(m_currentNode))
                {
                    Debug.Log("addsuccessnode");
                    AddSuccessNode(m_currentNode);
                    m_isSolved = true;
                    m_maximumHeight = m_currentNode.m_distanceFromRoot + 1;
                }
            }
            else
            {
                //Did we just reach a leaf node?
                if (m_currentNode.m_distanceFromRoot == m_maximumHeight - 1)
                {
                    m_maximumHeightReached = true;
                }
            }

            return true;
        }
        else if (rollResult == Brick.RollResult.FALL) //roll back some operations
        {
            m_pawn.m_coveredTiles = previousCoveredTiles;
        }

        return false;
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
    public bool[] m_remainingMoves; //child nodes that have been visited or not in this order [LEFT, TOP, RIGHT, BOTTOM]
                                    //public Brick m_brick; //a brick object that will simulate the rolling operation
                                    //public Tile[] m_coveredTiles; //as a brick object can be reused across several nodes, store here the tiles that are covered before the brick rolled

    //public SolutionTree m_parentTree;

    private bool m_leaf;

    public SolutionNode(/*SolutionTree parentTree, */Brick.RollDirection direction, SolutionNode parentNode, int distanceFromRoot /*, Brick brick*/)
    {
        //m_parentTree = parentTree;
        m_direction = direction;
        m_parentNode = parentNode;
        m_distanceFromRoot = distanceFromRoot;
        m_remainingMoves = new bool[4];
        for (int i = 0; i != m_remainingMoves.Length; i++)
        {
            m_remainingMoves[i] = true;
        }

        m_leaf = false;

        //m_brick = brick;
        //m_coveredTiles = new Tile[2];
        //m_coveredTiles[0] = brick.CoveredTiles[0];
        //m_coveredTiles[1] = brick.CoveredTiles[1];
    }

    public void SetAsLeaf()
    {
        m_leaf = true;
        for (int i = 0; i != m_remainingMoves.Length; i++)
        {
            m_remainingMoves[i] = false;
        }
    }

    //public void Process()
    //{
    //    if (m_parentTree.m_stopWhenTreeIsSolved && m_parentTree.m_isSolved)
    //        return;

    //    m_parentTree.IncrementProcessedNodesCount();

    //    //try to make the brick roll
    //    Brick.RollResult rollResult;
    //    Geometry.Edge rotationEdge;
    //    m_parentTree.m_pawn.Roll(m_direction, out rollResult, out rotationEdge);
    //    m_parentTree.m_pawn.OnFinishRolling();

    //    if (rollResult == Brick.RollResult.VALID)
    //    {
    //        m_parentTree.IncrementValidNodesCount();
    //        //m_coveredTiles = m_brick.CoveredTiles;

    //        //set the state of the brick to IDLE so it can roll at once
    //        m_parentTree.m_pawn.m_state = Brick.BrickState.IDLE;

    //        //find the condition telling us that the target tile has been reached
    //        bool targetTileHasBeenReached = false;
    //        if (m_parentTree.m_oneCoveredTileOnly)
    //        {
    //            if (m_parentTree.m_pawn.GetCoveredTilesCount() == 1 && m_parentTree.m_pawn.CoveredTiles[0] == m_parentTree.m_targetTile)
    //                targetTileHasBeenReached = true;
    //        }
    //        else
    //        {
    //            if ((m_parentTree.m_pawn.CoveredTiles[0] == m_parentTree.m_targetTile || m_parentTree.m_pawn.CoveredTiles[1] == m_parentTree.m_targetTile))
    //                targetTileHasBeenReached = true;
    //        }

    //        if (targetTileHasBeenReached)
    //        {
    //            //we want to check if this path contains all bonuses
    //            if (m_parentTree.PathContainsAllBonuses(this))
    //            {
    //                m_parentTree.AddSuccessNode(this);
    //                m_parentTree.m_isSolved = true;
    //                m_parentTree.m_maximumHeight = this.m_distanceFromRoot + 1;
    //            }
    //        }            
    //        else //keep processing child nodes
    //        {
    //            //Did we just reach a leaf node?
    //            if (m_distanceFromRoot == m_parentTree.m_maximumHeight - 1)
    //            {
    //                m_parentTree.m_maximumHeightReached = true;
    //                return;
    //            }

    //            //is the current path cycling?
    //            //if (IsCycling())
    //            //    return;

    //            //any other case, we keep processing child nodes
    //            SolutionNode[] childNodes = Split();
    //            for (int i = 0; i != childNodes.Length; i++)
    //            {
    //                childNodes[i].Process();
    //            }
    //        }
    //    }
    //}

    /**
    * Build child nodes for every relevant direction
    * Reuse the brick from parent node for the first rolling direction
    **/
    //public SolutionNode[] Split()
    //{
    //    SolutionNode[] childNodes = new SolutionNode[4];

    //    childNodes[0] = new SolutionNode(m_parentTree, Brick.RollDirection.LEFT, this, m_distanceFromRoot + 1, m_brick);
    //    childNodes[1] = new SolutionNode(m_parentTree, Brick.RollDirection.TOP, this, m_distanceFromRoot + 1, new Brick(m_brick));
    //    childNodes[2] = new SolutionNode(m_parentTree, Brick.RollDirection.RIGHT, this, m_distanceFromRoot + 1, new Brick(m_brick));
    //    childNodes[3] = new SolutionNode(m_parentTree, Brick.RollDirection.BOTTOM, this, m_distanceFromRoot + 1, new Brick(m_brick));

    //    //SolutionNode[] childNodes = new SolutionNode[3];

    //    //if (m_direction == Brick.RollDirection.LEFT)
    //    //{
    //    //    childNodes[0] = new SolutionNode(m_parentTree, Brick.RollDirection.LEFT, this, m_distanceFromRoot + 1, m_brick);
    //    //    childNodes[1] = new SolutionNode(m_parentTree, Brick.RollDirection.TOP, this, m_distanceFromRoot + 1, new Brick(m_brick));
    //    //    childNodes[2] = new SolutionNode(m_parentTree, Brick.RollDirection.BOTTOM, this, m_distanceFromRoot + 1, new Brick(m_brick));
    //    //}
    //    //else if (m_direction == Brick.RollDirection.RIGHT)
    //    //{
    //    //    childNodes[0] = new SolutionNode(m_parentTree, Brick.RollDirection.RIGHT, this, m_distanceFromRoot + 1, m_brick);
    //    //    childNodes[1] = new SolutionNode(m_parentTree, Brick.RollDirection.TOP, this, m_distanceFromRoot + 1, new Brick(m_brick));
    //    //    childNodes[2] = new SolutionNode(m_parentTree, Brick.RollDirection.BOTTOM, this, m_distanceFromRoot + 1, new Brick(m_brick));
    //    //}
    //    //else if (m_direction == Brick.RollDirection.TOP)
    //    //{
    //    //    childNodes[0] = new SolutionNode(m_parentTree, Brick.RollDirection.LEFT, this, m_distanceFromRoot + 1, m_brick);
    //    //    childNodes[1] = new SolutionNode(m_parentTree, Brick.RollDirection.RIGHT, this, m_distanceFromRoot + 1, new Brick(m_brick));
    //    //    childNodes[2] = new SolutionNode(m_parentTree, Brick.RollDirection.TOP, this, m_distanceFromRoot + 1, new Brick(m_brick));
    //    //}
    //    //else
    //    //{
    //    //    childNodes[0] = new SolutionNode(m_parentTree, Brick.RollDirection.LEFT, this, m_distanceFromRoot + 1, m_brick);
    //    //    childNodes[1] = new SolutionNode(m_parentTree, Brick.RollDirection.RIGHT, this, m_distanceFromRoot + 1, new Brick(m_brick));
    //    //    childNodes[2] = new SolutionNode(m_parentTree, Brick.RollDirection.BOTTOM, this, m_distanceFromRoot + 1, new Brick(m_brick));
    //    //}

    //    return childNodes;
    //}

    

    public Brick.RollDirection GetDirectionForRemainingForwardMove(int moveIndex)
    {
        switch (moveIndex)
        {
            case 0:
                return Brick.RollDirection.BOTTOM_LEFT;
            case 1:
                return Brick.RollDirection.TOP_LEFT;
            case 2:
                return Brick.RollDirection.TOP_RIGHT;
            case 3:
                return Brick.RollDirection.BOTTOM_RIGHT;
            default:
                return Brick.RollDirection.NONE;
        }
    }

    /**
    * To optimize the algorithm we can skip paths that contain cycles in it.
    * There is an exception to this rule, as we are allowed to cycle to get a bonus.
    * So we need to check if the covered tiles inside a cycle contains at least one bonus before deciding whether to keep it or not.
    **/
    //public bool IsCycling()
    //{
    //    if (m_distanceFromRoot < 2)
    //        return false;

    //    SolutionNode node = this.m_parentNode;
    //    int minDistanceForCycling = 1; //we need at least to move the brick twice to obtain a minimum cycle
    //    bool bCycleContainsBonuses = false;
    //    while (node != null)
    //    {
    //        if (minDistanceForCycling > 0)
    //        {
    //            minDistanceForCycling--;
    //            node = node.m_parentNode;
    //            continue;
    //        }

    //        if (!bCycleContainsBonuses && node.ContainsBonus()) //no need to check if this node contains a bonus if we already checked it positively before
    //            bCycleContainsBonuses = true;

    //        bool bCycling = this.CoversSameTiles(node);
    //        if (bCycling)
    //        {
    //            //check if it contains bonus. If not we consider that the cycle is useless and the path containing this cycle can be removed
    //            return !bCycleContainsBonuses;
    //        }

    //        node = node.m_parentNode;
    //    }

    //    return false;
    //}

    //public bool CoversTile(Tile tile)
    //{
    //    if (m_coveredTiles[0].HasSameFloorPosition(tile))
    //        return true;
    //    if (m_coveredTiles[1] != null && m_coveredTiles[1].HasSameFloorPosition(tile))
    //        return true;

    //    return false;
    //}

    //private bool CoversSameTiles(SolutionNode node)
    //{
    //    int coveredTilesCount = this.GetCoveredTilesCount();
    //    if (coveredTilesCount != node.GetCoveredTilesCount())
    //        return false;

    //    if (coveredTilesCount == 1)
    //    {
    //        if (CoversTile(node.m_coveredTiles[0]))
    //            return true;
    //    }
    //    else
    //    {
    //        if (CoversTile(node.m_coveredTiles[0]) && CoversTile(node.m_coveredTiles[1]))
    //            return true;
    //    }

    //    return false;
    //}

    //private bool ContainsBonus()
    //{
    //    if (m_coveredTiles[0].AttachedBonus != null)
    //        return true;
    //    if (m_coveredTiles[1] != null && m_coveredTiles[1].AttachedBonus != null)
    //        return true;

    //    return false;
    //}

    //private int GetCoveredTilesCount()
    //{
    //    if (m_coveredTiles[1] == null)
    //        return 1;

    //    return 2;
    //}
}