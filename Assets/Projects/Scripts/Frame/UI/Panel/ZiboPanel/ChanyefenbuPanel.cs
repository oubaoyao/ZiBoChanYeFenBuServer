using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MTFrame;
using MTFrame.MTEvent;
using UnityEngine.UI;
using System;
using RenderHeads.Media.AVProVideo;

public class ChanyefenbuPanel : BasePanel
{
    public Button BackButton;

    public MediaPlayer[] MediaPlayerGroup;

    public Button[] ButtonGroup;

    public Sprite[] VideoButtonSprite;

    private string[] VideoName = { "AVProVideoSamples/AlphaLeftRight.mp4", "AVProVideoSamples/SampleSphere.mp4", "AVProVideoSamples/BigBuckBunny_360p30.mp4", "AVProVideoSamples/BigBuckBunny_720p30.mp4" };

    public override void InitFind()
    {
        base.InitFind();
        BackButton = FindTool.FindChildComponent<Button>(transform, "backButton");
        MediaPlayerGroup = FindTool.FindChildNode(transform, "VideoGroup").GetComponentsInChildren<MediaPlayer>();
        ButtonGroup = FindTool.FindChildNode(transform, "VideoGroup").GetComponentsInChildren<Button>();
        VideoButtonSprite = Resources.LoadAll<Sprite>("ZiBoUi/VideoButton");
    }

    public override void InitEvent()
    {
        base.InitEvent();
        BackButton.onClick.AddListener(() => {
            ReturnWaitPanel();
        });

        for (int i = 0; i < ButtonGroup.Length; i++)
        {
            InitButton(ButtonGroup[i],i);
        }
    }

    public override void Open()
    {
        base.Open();
        EventManager.AddUpdateListener(UpdateEventEnumType.Update, "Aupdate", Aupdate);
        TimeTool.Instance.AddDelayed(TimeDownType.NoUnityTimeLineImpact, 120, ReturnWaitPanel);
    }

    private void Aupdate(float timeProcess)
    {
        if(Input.GetMouseButtonDown(0))
        {
            TimeTool.Instance.Remove(TimeDownType.NoUnityTimeLineImpact, ReturnWaitPanel);
        }

        if (Input.GetMouseButtonUp(0))
        {
            TimeTool.Instance.AddDelayed(TimeDownType.NoUnityTimeLineImpact, 120, ReturnWaitPanel);
        }
    }

    public override void Hide()
    {
        base.Hide();
        TimeTool.Instance.Remove(TimeDownType.NoUnityTimeLineImpact, ReturnWaitPanel);
        EventManager.RemoveUpdateListener(UpdateEventEnumType.Update, "Aupdate", Aupdate);
        InitVideo(true);
    }

    private void ReturnWaitPanel()
    {
        ZiBoState.SentToState(PanelName.WaitPanel);
    }

    private void InitButton(Button button,int num)
    {
        button.onClick.AddListener(() => {
            if(button.gameObject.GetComponent<Image>().sprite == VideoButtonSprite[0])
            {
                UdpSeverLink.Instance.SendMsgToClient(VideoName[num]);
                InitVideo();
                MediaPlayerGroup[num].OpenVideoFromFile(MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder, VideoName[num]);
                button.gameObject.GetComponent<Image>().sprite = VideoButtonSprite[1];
            }
            else
            {
                UdpSeverLink.Instance.SendMsgToClient(Control_Order.back.ToString());
                MediaPlayerGroup[num].CloseVideo();
                SetButtonSprite();
                button.gameObject.GetComponent<Image>().sprite = VideoButtonSprite[0];
            }   
        });
    }

    private void SetButtonSprite()
    {
        foreach (Button item in ButtonGroup)
        {
            item.gameObject.GetComponent<Image>().sprite = VideoButtonSprite[0];
        }
    }

    private void InitVideo(bool Issent = false)
    {
        SetButtonSprite();
        foreach (MediaPlayer item in MediaPlayerGroup)
        {
            item.CloseVideo();
        }

        if(Issent)
        {
            UdpSeverLink.Instance.SendMsgToClient(Control_Order.back.ToString());
        }
        
    }
}
