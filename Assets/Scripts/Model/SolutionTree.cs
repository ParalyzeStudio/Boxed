using System.Collections.Generic;
using UnityEngine;

public class SolutionTree
{
    //maximum height (or depth) of the tree, i.e the maximum number of nodes allowed along a path
    private int m_maximumHeight;
    public int MaximumHeight
    {
        get
        {
            return m_maximumHeight;
        }
    }

    private List<SolutionNode> m_successNodes;

    public Tile m_startTile; //the tile that serves as start point for or tree
    public Tile m_targetTile; //the tile that serves as target for the search of solutions
    
    public bool m_stopWhenTreeIsSolved; //In case we just want to know if at least one solution exists, we can stop processing nodes as soon as we found one
    public bool m_isSolved; //is this tree solved (i.e contains at least one successful path to target)

    public SolutionTree(int height, Tile startTile, Tile targetTile, bool stopWhenTreeIsSolved = false)
    {
        m_maximumHeight = height;
        m_startTile = startTile;
        m_targetTile = targetTile;
        m_stopWhenTreeIsSolved = stopWhenTreeIsSolved;
        m_successNodes = new List<SolutionNode>();
        m_isSolved = false;
    }

    /**
    * Traverse the floor for all directions possible at each step making the tree grow until each path has reached the maximum height of the tree or
    * found a success path
    **/
    public SolutionNode[][] SearchForSolutions()
    {
        //Construct 4 nodes and 4 bricks (1 for each rolling direction) starting from level start tile
        SolutionNode[] childNodes = new SolutionNode[4];
        childNodes[0] = new SolutionNode(this, Brick.RollDirection.LEFT, null, 0, new Brick(m_startTile));
        childNodes[1] = new SolutionNode(this, Brick.RollDirection.RIGHT, null, 0, new Brick(m_startTile));
        childNodes[2] = new SolutionNode(this, Brick.RollDirection.TOP, null, 0, new Brick(m_startTile));
        childNodes[3] = new SolutionNode(this, Brick.RollDirection.BOTTOM, null, 0, new Brick(m_startTile));

        //Process every of the 4 nodes declared above
        for (int i = 0; i != childNodes.Length; i++)
        {
            childNodes[i].Process();
        }

        //now search for paths that are marked as successful and return them
        return ExtractSuccessPaths();
    }

    public void AddSuccessNode(SolutionNode node)
    {
        m_isSolved = true;
        m_successNodes.Add(node);
    }

    private SolutionNode[][] ExtractSuccessPaths()
    {
        if (m_successNodes.Count == 0)
            return null;

        SolutionNode[][] successPaths = new SolutionNode[m_successNodes.Count][];

        for (int i = 0; i != m_successNodes.Count; i++)
        {
            SolutionNode node = m_successNodes[i];
            int pathLength = node.m_distanceFromRoot + 1;

            SolutionNode[] successPath = new SolutionNode[pathLength];
            
            while (node != null)
            {
                successPath[pathLength - 1] = node;
                pathLength--;
                node = node.m_parentNode;
            }

            successPaths[i] = successPath;
        }

        return successPaths;
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

    public SolutionTree m_parentTree;

    public SolutionNode(SolutionTree parentTree, Brick.RollDirection direction, SolutionNode parentNode, int distanceFromRoot, Brick brick)
    {
        m_parentTree = parentTree;
        m_direction = direction;
        m_parentNode = parentNode;
        m_distanceFromRoot = distanceFromRoot;
        m_brick = brick;
    }

    public void Process()
    {
        if (m_parentTree.m_stopWhenTreeIsSolved && m_parentTree.m_isSolved)
            return;

        //we reach the maximum height of the tree
        if (m_distanceFromRoot == m_parentTree.MaximumHeight)
            return;

        //try to make the brick roll
        Brick.RollResult rollResult;
        Brick.BrickEdge rotationEdge;
        m_brick.Roll(m_direction, out rollResult, out rotationEdge);

        if (rollResult == Brick.RollResult.VALID)
        {
            //set the state of the brick to IDLE so it can roll at once
            m_brick.m_state = Brick.BrickState.IDLE;

            if (m_brick.GetCoveredTilesCount() == 1 && m_brick.CoveredTiles[0] == m_parentTree.m_targetTile)
            {
                m_parentTree.AddSuccessNode(this);
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
}

