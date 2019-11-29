using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MTFrame;
using MTFrame.MTEvent;
using RenderHeads.Media.AVProVideo;
using UnityEngine.UI;

public class WaitPanel : BasePanel
{
    public MediaPlayer mediaPlayer;
    public Button button;

    public override void InitFind()
    {
        base.InitFind();
        mediaPlayer = FindTool.FindChildComponent<MediaPlayer>(transform, "VideoPlayer");
        button = FindTool.FindChildComponent<Button>(transform, "VideoPlayer");
    }

    public override void InitEvent()
    {
        base.InitEvent();
        button.onClick.AddListener(() => {
            ZiBoState.SentToState(PanelName.ChanyefenbuPanel);
        });
    }

    public override void Open()
    {
        base.Open();
        //mediaPlayer.Play();
        mediaPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder, "AVProVideoSamples/三屏联动-首页.mp4");
    }

    public override void Hide()
    {
        base.Hide();
        //mediaPlayer.Pause();
        mediaPlayer.CloseVideo();
    }
}
