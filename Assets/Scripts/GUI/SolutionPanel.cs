using UnityEngine;
using UnityEngine.UI;

public class SolutionPanel : MonoBehaviour
{
    public GameObject m_solutionColumn; //one column per solution
    public Image m_arrowImagePfb;

    public void AddSolution(Brick.RollDirection[] solution)
    {
        GameObject solutionColumn = (GameObject)Instantiate(m_solutionColumn);
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
        if (direction == Brick.RollDirection.LEFT)
            return Quaternion.AngleAxis(180, Vector3.forward);
        else if (direction == Brick.RollDirection.TOP)
            return Quaternion.AngleAxis(90, Vector3.forward);
        else if (direction == Brick.RollDirection.BOTTOM)
            return Quaternion.AngleAxis(270, Vector3.forward);
        else
            return Quaternion.identity;
    }

    private string GetDirectionAsString(Brick.RollDirection direction)
    {
        if (direction == Brick.RollDirection.LEFT)
            return "Left";
        else if (direction == Brick.RollDirection.RIGHT)
            return "Right";
        else if (direction == Brick.RollDirection.TOP)
            return "Top";
        else
            return "Bottom";
    }
}
