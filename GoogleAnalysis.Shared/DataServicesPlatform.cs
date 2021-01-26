using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace GoogleAnalysis
{
    public enum DataServicesPlatform
    {
        /// <summary>
        /// 微软应用商店
        /// </summary>
        [Description("微软应用商店")]
        MicrosoftStore,
        /// <summary>
        /// 华军软件园
        /// </summary>
        [Description("华军软件园")]
        Onlinedown,
        /// <summary>
        /// 360软件管家
        /// </summary>
        [Description("360软件管家")]
        _360,
        /// <summary>
        /// 腾讯软件中心
        /// </summary>
        [Description("腾讯软件中心")]
        Tencent,
        /// <summary>
        /// 联想软件中心
        /// </summary>
        [Description("联想软件中心")]
        Lenovo,
        /// <summary>
        /// 多特软件站
        /// </summary>
        [Description("多特软件站")]
        Duote,
        /// <summary>
        /// 金山软件管家
        /// </summary>
        [Description("金山软件管家")]
        IJinShan,
        /// <summary>
        /// 太平洋下载
        /// </summary>
        [Description("太平洋下载")]
        PCOnline,
        /// <summary>
        /// ZOL下载
        /// </summary>
        [Description("ZOL下载")]
        ZOL,
        /// <summary>
        /// PC6下载
        /// </summary>
        [Description("PC6下载")]
        PC6,
        /// <summary>
        /// 非凡软件站
        /// </summary>
        [Description("非凡软件站")]
        CRSKY,
        /// <summary>
        /// Softpedia
        /// </summary>
        [Description("Softpedia")]
        Softpedia,
        /// <summary>
        /// CNET
        /// </summary>
        [Description("CNET")]
        CNET,
        /// <summary>
        /// Soft32
        /// </summary>
        [Description("Soft32")]
        Soft32,
        /// <summary>
        /// UptoDown
        /// </summary>
        [Description("UptoDown")]
        UptoDown,
        /// <summary>
        /// 官网
        /// </summary>
        [Description("官网")]
        OfficialWebsite
    }
}
