using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MTFrame;
using MTFrame.MTEvent;
using UnityEngine.UI;

public class WaitPanel : BasePanel
{
    public Button button;

    protected override void Awake()
    {
        base.Awake();
        Input.multiTouchEnabled = false;
    }

    public override void InitFind()
    {
        base.InitFind();
        button = FindTool.FindChildComponent<Button>(transform, "Button");
    }

    public override void InitEvent()
    {
        base.InitEvent();
        button.onClick.AddListener(() => {
            ZiBoState.SentToState(PanelName.ChanyefenbuPanel);
        });
    }
}
