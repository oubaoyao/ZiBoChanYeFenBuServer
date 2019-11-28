using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MTFrame;

public class ChanyefenbuTask : BaseTask
{
    public ChanyefenbuTask(BaseState state) : base(state)
    {
    }

    public override void Enter()
    {
        base.Enter();
        UIManager.CreatePanel<ChanyefenbuPanel>(WindowTypeEnum.ForegroundScreen);
    }

    public override void Exit()
    {
        base.Exit();
        UIManager.ChangePanelState<ChanyefenbuPanel>(WindowTypeEnum.ForegroundScreen, UIPanelStateEnum.Hide);
    }
}
