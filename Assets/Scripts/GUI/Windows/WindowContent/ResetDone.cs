
public class ResetDone : GameWindowContent
{
    public void OnClickGoBack()
    {
        MainPageMenu parentWindow = this.transform.parent.GetComponent<MainPageMenu>();
        parentWindow.ShowBackButton();
        parentWindow.StartCoroutine(parentWindow.ShowContentForID(MainPageMenu.ContentID.SETTINGS));
    }
}
