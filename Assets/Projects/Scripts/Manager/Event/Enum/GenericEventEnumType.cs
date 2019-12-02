

namespace MTFrame.MTEvent
{
    /// <summary>
    /// 事件枚举
    /// </summary>
    public enum GenericEventEnumType
    {
        /// <summary>
        /// 常用
        /// </summary>
        Generic,
        /// <summary>
        /// 信息
        /// </summary>
        Message,
    }

    public enum Contact
    {
        /// <summary>
        /// UDP接收数据传给状态类
        /// </summary>
        UDP,
        /// <summary>
        /// 状态类转换后传给Panel层
        /// </summary>
        Panel,
        /// <summary>
        /// 切换Panel
        /// </summary>
        SwitchPanel,


    }

    public enum PanelName
    {
        WaitPanel,
        ChanyefenbuPanel,
    }

    public enum Control_Order
    {
        /// <summary>
        /// 返回待机视频
        /// </summary>
        back,
        /// <summary>
        /// 暂停视频
        /// </summary>
        pause,
        /// <summary>
        /// 播放视频
        /// </summary>
        play,
    }
}