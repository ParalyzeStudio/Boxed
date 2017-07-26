using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CreditsAmount : GameWindowElement
{
    public Text m_amount;
    private const float ANIMATION_DURATION = 2.0f;

    //animating value
    private bool m_invalidating;
    private int m_oldAmount;
    private float m_currentAmount;
    private int m_newAmount;

    public void InvalidateAmount(bool bAnimated = false)
    {
        PersistentDataManager pDataManager = GameController.GetInstance().GetPersistentDataManager();
        m_newAmount = pDataManager.GetCreditsAmount();

        if (bAnimated)
        {
            m_invalidating = true;
            m_oldAmount = int.Parse(m_amount.text);
            m_currentAmount = m_oldAmount;
        }
        else
            m_amount.text = m_newAmount.ToString();
    }

    protected override void Update()
    {
        if (m_invalidating)
        {
            float dt = Time.deltaTime;
            float dAmount = dt / ANIMATION_DURATION * (m_newAmount - m_oldAmount);

            if (Math.Abs(m_newAmount - m_currentAmount) < Math.Abs(dAmount))
            {
                m_currentAmount = m_newAmount;
                m_invalidating = false;
            }
            else
            {
                m_currentAmount += dAmount;
            }

            m_amount.text = Math.Round(m_currentAmount).ToString();
        }

        base.Update();
    }
}