using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MTFrame;
using MTFrame.MTEvent;

public class ZiBoState : BaseState
{
    //注意state一定要在get里面监听事件，没有的话就写成下面样子
    //这里一般用来监听Panel切换
    private Queue<string> GetVs = new Queue<string>();
    public override string[] ListenerMessageID
    {
        get
        {
            return new string[]
            {
                //事件名string类型
                Contact.UDP.ToString(),
                Contact.SwitchPanel.ToString(),
            };
        }
        set { }
    }

    public override void OnListenerMessage(EventParamete parameteData)
    {

        //接收监听事件的数据，然后用swich判断做处理

        //除此之外，也可以在这里监听UDP传输的数据，但是接收的数据是子线程数据，要通过队列接收，
        //然后在update转换成主线程数据，才能对数据进行处理

        if (parameteData.EvendName == Contact.UDP.ToString())
        {
            //获取数据parameteData.GetParameter<string>()[0]
            GetVs.Enqueue(parameteData.GetParameter<string>()[0]);
        }

        if(parameteData.EvendName == Contact.SwitchPanel.ToString())
        {
            PanelName panelName = parameteData.GetParameter<PanelName>()[0];
            switch (panelName)
            {
                case PanelName.WaitPanel:
                    CurrentTask.ChangeTask(new WaitTask(this));
                    break;
                case PanelName.ChanyefenbuPanel:
                    CurrentTask.ChangeTask(new ChanyefenbuTask(this));
                    break;
                default:
                    break;
            }
        }
    }

    public override void Enter()
    {
        base.Enter();
        CurrentTask.ChangeTask(new WaitTask(this));
        EventManager.AddUpdateListener(UpdateEventEnumType.Update, "Onupdate", Onupdate);
    }

    public override void Exit()
    {
        base.Exit();
        EventManager.RemoveUpdateListener(UpdateEventEnumType.Update, "Onupdate", Onupdate);
    }

    private void Onupdate(float timeProcess)
    {
        //数据在这里转换
        lock (GetVs)
        {
            if (GetVs.Count > 0)
            {
                string st = GetVs.Dequeue();
                Debug.Log("状态类里接收到的数据：" + st);
                EventParamete eventParamete = new EventParamete();
                eventParamete.AddParameter(st);
                EventManager.TriggerEvent(GenericEventEnumType.Message, Contact.Panel.ToString(), eventParamete);
                //在这里进行switch对数据进行处理
            }
        }
    }

    public static void SentToState(PanelName panelName)
    {
        EventParamete eventParamete = new EventParamete();
        eventParamete.AddParameter<PanelName>(panelName);
        EventManager.TriggerEvent(GenericEventEnumType.Message, Contact.SwitchPanel.ToString(), eventParamete);
    }
}
