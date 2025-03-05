using UnityEngine;

public class StartSceneUI : MonoBehaviour
{
    private void Start()
    {
        UIManager.GetInstance().OpenPanel(new StartPanel());
    }
}
