using System.Collections.Generic;

public class PanelManager
{
    private UIManager UIManager;
    private BasePanel BasePanel;

    public Stack<BasePanel> panel_Stack;
    public PanelManager()
    {
        panel_Stack = new Stack<BasePanel>();
        UIManager = new UIManager();
    }

    public void PushPanel(BasePanel nextPanel)
    {
        if (panel_Stack.Count > 0)
        { 
            panel_Stack.Peek().OnPause();
        }
        panel_Stack.Push(nextPanel);
        UIManager.GetUI(nextPanel.UIType);
    }

    public void PopPanel()
    {
        if (panel_Stack.Count > 0)
        { 
            panel_Stack.Peek().OnExit();
            panel_Stack.Pop();
        }

        if(panel_Stack.Count > 0)
            panel_Stack.Peek().OnResume();

    }
}
