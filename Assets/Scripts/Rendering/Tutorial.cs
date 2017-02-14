using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public int m_levelNumber; //number of the level this tutorial is related to  

    private Text[] m_instructions; //the list of instructions in this tutorial with eventually an event attach to them
    private int m_instructionIndex;

    public void Start()
    {
        m_instructions = this.GetComponentsInChildren<Text>();
        m_instructionIndex = -1;

        StartCoroutine("Run");
    }

    /**
    * Show the next instruction if any
    **/
    public bool ShowNextInstruction()
    {
        if (++m_instructionIndex < m_instructions.Length)
        {
            Text currentInstruction = m_instructions[m_instructionIndex];
            //fade in and translate by a certain amount the current instruction
            float animationDuration = 2.0f;
            GUITextAnimator instructionAnimator = currentInstruction.GetComponent<GUITextAnimator>();
            instructionAnimator.SetOpacity(0);
            instructionAnimator.FadeTo(1.0f, 0.5f * animationDuration, 1.0f);
            instructionAnimator.SetPosition(instructionAnimator.GetPosition() - new Vector3(0, 100, 0));
            instructionAnimator.TranslateBy(new Vector3(0, 100, 0), animationDuration);
            return true;
        }
        else
            return false;
    }

    /**
    * Dismiss the current instruction
    **/
    private void DismissCurrentInstruction()
    {
        Text currentInstruction = m_instructions[m_instructionIndex];
        //fade out and translate by a certain amount the current instruction
        float animationDuration = 2.0f;
        GUITextAnimator instructionAnimator = currentInstruction.GetComponent<GUITextAnimator>();
        instructionAnimator.FadeTo(0.0f, 0.5f * animationDuration);
        instructionAnimator.TranslateBy(new Vector3(0, 100, 0), animationDuration);
    }

    /**
    * Run the event associated to this instruction if any
    **/
    private void InvokeCurrentInstructionEvent()
    {
        Text currentInstruction = m_instructions[m_instructionIndex];
        TutorialEvent tEvent = currentInstruction.GetComponent<TutorialEvent>();
        if (tEvent != null)
        {
            tEvent.Run();
        }
    }

    /**
    * Run the tutorial by displaying instructions one after another
    **/
    private IEnumerator Run()
    {
        while (ShowNextInstruction())
        {
            yield return new WaitForSeconds(2.0f);

            InvokeCurrentInstructionEvent();

            yield return new WaitForSeconds(2.0f);

            DismissCurrentInstruction();

            yield return new WaitForSeconds(2.0f);
        }

        Dismiss();

        yield return null;
    }

    /**
    * Remove this tutorial from sight
    **/
    private void Dismiss()
    {
        Debug.Log("Dismiss tutorial:" + GameController.GetInstance().m_gameStatus);
        Destroy(this.gameObject);

        GameGUI gameGUI = this.transform.parent.GetComponent<GameGUI>();
        if (!gameGUI.ShowNextTutorial())
            GameController.GetInstance().m_gameStatus = GameController.GameStatus.RUNNING;
    }

    public void OnClickScreen()
    {

    }
}
