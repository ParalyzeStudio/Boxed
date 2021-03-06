﻿using UnityEngine;
using UnityEngine.UI;

public class SolutionPanel : MonoBehaviour
{
    public GameObject m_solutionColumn; //one column per solution
    public Image m_arrowImagePfb;

    public void SetSolution(Brick.RollDirection[] solution)
    {
        //clear any previously displayed solution
        Transform[] childrenColumns = this.GetComponentsInChildren<Transform>();
        for (int i = 0; i != childrenColumns.Length; i++)
        {
            if (childrenColumns[i] != this.transform)
                Destroy(childrenColumns[i].gameObject);
        }

        //create a new one
        GameObject solutionColumn = Instantiate(m_solutionColumn);
        solutionColumn.transform.SetParent(this.transform, false);

        for (int i = 0; i != solution.Length; i++)
        {
            AddDirection(solutionColumn, solution[i]);
        }
    }

    private void AddDirection(GameObject holder, Brick.RollDirection direction)
    {
        Image arrowImage = Instantiate(m_arrowImagePfb);
        arrowImage.name = GetDirectionAsString(direction) + "Arrow";
        arrowImage.transform.SetParent(holder.transform, false);

        arrowImage.gameObject.transform.rotation = GetRotationForDirection(direction);
    }

    private Quaternion GetRotationForDirection(Brick.RollDirection direction)
    {
        if (direction == Brick.RollDirection.BOTTOM_LEFT)
            return Quaternion.AngleAxis(180, Vector3.forward);
        else if (direction == Brick.RollDirection.TOP_LEFT)
            return Quaternion.AngleAxis(90, Vector3.forward);
        else if (direction == Brick.RollDirection.BOTTOM_RIGHT)
            return Quaternion.AngleAxis(270, Vector3.forward);
        else
            return Quaternion.identity;
    }

    private string GetDirectionAsString(Brick.RollDirection direction)
    {
        if (direction == Brick.RollDirection.BOTTOM_LEFT)
            return "Left";
        else if (direction == Brick.RollDirection.TOP_RIGHT)
            return "Right";
        else if (direction == Brick.RollDirection.TOP_LEFT)
            return "Top";
        else
            return "Bottom";
    }
}
