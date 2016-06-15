using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ValidationWindow : MonoBehaviour
{
    public Text m_outputDataLinePfb;
    public Transform m_messagesContainerTf;

    public void Populate(Level.ValidationData data)
    {
        if (data.m_success)
        {
            Text successMsg = Instantiate(m_outputDataLinePfb);
            successMsg.text = "Level validated successfully";

            successMsg.transform.SetParent(m_messagesContainerTf.transform, false);
        }
        else
        {
            Text failMsg = Instantiate(m_outputDataLinePfb);
            failMsg.text = "Level validation failed with following errors:";
            failMsg.transform.SetParent(m_messagesContainerTf.transform, false);

            //Create one line + separtion line for every error
            List<Text> errorMsgs = new List<Text>();
            if (!data.m_startTileSet)
                errorMsgs.Add(CreateErrorMessage("Start tile has not been set"));
            if (!data.m_finishTileSet)
                errorMsgs.Add(CreateErrorMessage("Finish tile has not been set"));
            if (data.m_solution == null)
                errorMsgs.Add(CreateErrorMessage("Could not find a path between start and finish tile"));
            if (data.m_bonusesAreReachable == false)
                errorMsgs.Add(CreateErrorMessage("Some bonuses are not reachable"));

            //add separation lines
            for (int i = 0; i != errorMsgs.Count; i++)
            {
                errorMsgs[i].transform.SetParent(m_messagesContainerTf, false);
                errorMsgs[i].color = Color.red;
            }
        }
}

    private Text CreateErrorMessage(string text)
    {
        Text errorMsg = Instantiate(m_outputDataLinePfb);
        errorMsg.text = text;

        return errorMsg;
    }

    public void OnClickClose()
    {
        Destroy(this.gameObject);
    }
}
