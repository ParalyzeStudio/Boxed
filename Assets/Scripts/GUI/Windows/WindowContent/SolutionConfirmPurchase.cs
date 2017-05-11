using UnityEngine;
using UnityEngine.UI;

public class SolutionConfirmPurchase : GameWindowElement
{
    public Text m_solutionConfirmPurchaseMessage;

    public override void Show()
    {
        m_solutionConfirmPurchaseMessage.text = "Confirm the purchase of the solution for 12 credits?";

        base.Show();
    }

    public void OnClickConfirmPurchaseSolution()
    {
        //close this window
        Dismiss(0.4f);
        StartCoroutine(ResetPositionAfterDelay(0.4f));
        StartCoroutine(DeactivateAfterDelay(0.4f));

        //update the level data file
        LevelData levelData = GameController.GetInstance().GetLevelManager().GetLevelDataForCurrentLevel();
        levelData.m_solutionPurchased = true;
        levelData.SaveToFile();

        //build and show the window solution after delay
        PauseMenu pauseMenu = this.transform.parent.GetComponent<PauseMenu>();
        pauseMenu.ShowSolution(0.2f);
    }

    public void OnClickCancel()
    {
        PauseMenu parentMenu = this.transform.parent.GetComponent<PauseMenu>();
        parentMenu.DismissDarkBackground();

        Dismiss(0.4f);
        StartCoroutine(ResetPositionAfterDelay(0.4f));
        StartCoroutine(DeactivateAfterDelay(0.4f));
    }
}
