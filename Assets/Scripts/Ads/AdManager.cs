using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour
{
#if !UNITY_ADS
    public string gameId;
    public bool enableTestMode = true;
#endif

    private int m_lastAdCount;
    private const int AD_FREQUENCY = 1;

    /**
    * Call that method to play an ad
    **/ 
    public void TryToPlayAd()
    {        
        if (++m_lastAdCount >= AD_FREQUENCY)
        {
            m_lastAdCount = 0;
            StartCoroutine("Play");
        }
    }

    private IEnumerator Play()
    {
#if !UNITY_ADS
        if (Advertisement.isSupported)
        { 
            Advertisement.Initialize(gameId, enableTestMode);
        }
#endif

        while (!Advertisement.isInitialized || !Advertisement.IsReady())
        {
            yield return new WaitForSeconds(0.5f);
        }
        Advertisement.Show();
    }
}