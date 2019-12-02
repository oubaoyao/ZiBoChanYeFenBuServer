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

    public Slider[] SliderGroup;

    private string[] VideoName = { "AVProVideoSamples/AlphaLeftRight.mp4", "AVProVideoSamples/SampleSphere.mp4", "AVProVideoSamples/BigBuckBunny_360p30.mp4", "AVProVideoSamples/BigBuckBunny_720p30.mp4" };

    public MediaPlayer Current_MediaPlayer = null;
    public Slider Current_Slider = null;

    private bool _wasPlayingOnScrub;
    private float _setVideoSeekSliderValue;

    public override void InitFind()
    {
        base.InitFind();
        BackButton = FindTool.FindChildComponent<Button>(transform, "backButton");
        MediaPlayerGroup = FindTool.FindChildNode(transform, "VideoGroup").GetComponentsInChildren<MediaPlayer>();
        ButtonGroup = FindTool.FindChildNode(transform, "VideoGroup").GetComponentsInChildren<Button>();
        VideoButtonSprite = Resources.LoadAll<Sprite>("ZiBoUi/VideoButton");
        SliderGroup = FindTool.FindChildNode(transform, "VideoGroup").GetComponentsInChildren<Slider>();
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

        foreach (MediaPlayer item in MediaPlayerGroup)
        {
            item.Events.AddListener(OnMediaPlayerEvent);
        }
    }

    public void OnVideoSeekSlider()
    {
        if (Current_MediaPlayer && Current_Slider && Current_Slider.value != _setVideoSeekSliderValue)
        {
            Current_MediaPlayer.Control.Seek(Current_Slider.value * Current_MediaPlayer.Info.GetDurationMs());
            UdpSeverLink.Instance.SendMsgToClient(Current_Slider.value);
        }
    }

    public void OnVideoSliderDown()
    {
        if (Current_MediaPlayer)
        {
            _wasPlayingOnScrub = Current_MediaPlayer.Control.IsPlaying();
            if (_wasPlayingOnScrub)
            {
                Current_MediaPlayer.Control.Pause();
            }
            OnVideoSeekSlider();
        }
    }
    public void OnVideoSliderUp()
    {
        if (Current_MediaPlayer && _wasPlayingOnScrub)
        {
            Current_MediaPlayer.Control.Play();
            _wasPlayingOnScrub = false;
        }
    }

    private void OnMediaPlayerEvent(MediaPlayer arg0, MediaPlayerEvent.EventType arg1, ErrorCode arg2)
    {
        switch (arg1)
        {
            case MediaPlayerEvent.EventType.ReadyToPlay:
                break;
            case MediaPlayerEvent.EventType.Started:
                break;
            case MediaPlayerEvent.EventType.FinishedPlaying:
                VideoPlayComplete(arg1);
                break;
            default:
                break;
        }
    }

    private void VideoPlayComplete(MediaPlayerEvent.EventType arg1)
    {
        Current_MediaPlayer = null;
        Current_Slider = null;
        InitVideo(true);
    }

    public override void Open()
    {
        base.Open();
        EventManager.AddUpdateListener(UpdateEventEnumType.Update, "Aupdate", Aupdate);
        TimeTool.Instance.AddDelayed(TimeDownType.NoUnityTimeLineImpact, 120, ReturnWaitPanel);
    }

    private void Aupdate(float timeProcess)
    {
        if(Current_MediaPlayer!=null&& Current_Slider!=null&& Current_MediaPlayer.Info!=null&& Current_MediaPlayer.Info.GetDurationMs()>0f)
        {
            float time = Current_MediaPlayer.Control.GetCurrentTimeMs();
            float duration = Current_MediaPlayer.Info.GetDurationMs();
            float d = Mathf.Clamp(time / duration, 0.0f, 1.0f);

            _setVideoSeekSliderValue = d;
            Current_Slider.value = d;
        }

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
        Current_MediaPlayer = null;
        Current_Slider = null;
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
                if(Current_MediaPlayer==null || Current_MediaPlayer.name!= MediaPlayerGroup[num].name)
                {
                    Current_MediaPlayer = MediaPlayerGroup[num];
                    Current_Slider = SliderGroup[num];

                    UdpSeverLink.Instance.SendMsgToClient(VideoName[num]);
                    InitVideo();
                    Current_MediaPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder, VideoName[num]);        
                }
                else
                {
                    Current_MediaPlayer.Play();
                    UdpSeverLink.Instance.SendMsgToClient(Control_Order.play.ToString());
                }
                Current_Slider.transform.GetComponent<CanvasGroup>().alpha = 1;
                Current_Slider.transform.GetComponent<CanvasGroup>().blocksRaycasts = true;
                button.gameObject.GetComponent<Image>().sprite = VideoButtonSprite[1];
            }
            else
            {
                UdpSeverLink.Instance.SendMsgToClient(Control_Order.pause.ToString());
                Current_MediaPlayer.Pause();
                //SetButtonSprite();
                button.gameObject.GetComponent<Image>().sprite = VideoButtonSprite[0];
                Current_Slider.transform.GetComponent<CanvasGroup>().DOFillAlpha(0, 0.5f);
                Current_Slider.transform.GetComponent<CanvasGroup>().blocksRaycasts = false;
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
        foreach (Slider item in SliderGroup)
        {
            item.value = 0;
            item.transform.GetComponent<CanvasGroup>().alpha = 0;
            item.transform.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }

        if(Issent)
        {
            UdpSeverLink.Instance.SendMsgToClient(Control_Order.back.ToString());
        }
        
    }
}
