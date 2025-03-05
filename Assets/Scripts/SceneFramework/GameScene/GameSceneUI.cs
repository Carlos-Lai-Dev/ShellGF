using UnityEngine;

public class GameSceneUI : MonoBehaviour
{
  
    private void Start()
    {
        UIManager.GetInstance().OpenPanel(new GamePanel());
    }
}
