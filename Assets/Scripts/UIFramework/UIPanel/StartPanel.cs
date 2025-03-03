using System;
using UnityEngine.UI;

public class StartPanel : BasePanel
{
    private Button startBtn;
    private static readonly string path = resource + PanelName.StartPanel.ToString();
    public StartPanel() : base(new UIType(path))
    {
    }
    private void LoadGame()
    {
        GameScene gameScene = new GameScene();
        SceneController.GetInstance().LoadScene(gameScene.sceneName, gameScene);
    }
    public override void OnEnter()
    {
        base.OnEnter();
        startBtn = UIMethod.GetInstance().GetOrAddComponentInChildren<Button>(activeTransform, BtnName.StartBtn.ToString());
        startBtn.onClick.AddListener(LoadGame);
    }
    public override void OnEnable()
    {
        base.OnEnable();
    }
    public override void OnDisable()
    {
        base.OnDisable();
    }

    public override void OnExit()
    {
        base.OnExit();
        startBtn.onClick.RemoveListener(LoadGame);
    }
}
