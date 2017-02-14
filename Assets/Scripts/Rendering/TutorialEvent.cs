using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class TutorialEvent : MonoBehaviour
{
    //public string m_methodToExecute; //the name of the method to execute
    public int m_methodToExecuteIndex; // the index of the method to execute

    public static string[] m_classMethods;
    private static string[] m_ignoreMethods = new string[]{ "Run", "Show", "Dismiss"};

    public static void RetrievePublicClassMethods()
    {
        m_classMethods =
        typeof(TutorialEvent)
        .GetMethods(BindingFlags.Instance | BindingFlags.Public)
        .Where(x => x.DeclaringType == typeof(TutorialEvent)) // Only list methods defined in our own class
        .Where(x => !m_ignoreMethods.Any(n => n == x.Name)) // Don't list methods in the ignoreMethods array (so we can exclude Unity specific methods, etc.)
        .Where(x => x.GetParameters().Length == 0) // Make sure we only get methods with zero arguments
        .Select(x => x.Name)
        .ToArray();
    }

    public void Run()
    {
        if (m_classMethods == null)
            RetrievePublicClassMethods();

        //run the job
        typeof(TutorialEvent)
        .GetMethod(m_classMethods[m_methodToExecuteIndex], BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
        .Invoke(this, new object[0]);
    }

    /**
    * Different events that are called during tutorials
    **/
    public void HighlightQUBE()
    {
        Debug.Log("Highlight cube");
    }

    public void HighlightDestinationTile()
    {
        Debug.Log("Highlight destination tile");
    }

    /**
    * Indicates which areas of the area are clickable and what they do
    **/
    public void ShowMoveButtons()
    {
        Debug.Log("ShowMoveButtons");
    }

    /**
    * Indicates what is the purpose of the PAR score
    **/
    public void ShowPARScore()
    {
        Debug.Log("ShowPARScore");
    }
}