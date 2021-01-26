using System;
using System.Collections.Generic;
using System.Text;

namespace GoogleAnalysis
{
    internal class Parameters
    {
        private Dictionary<string, string> _paramsDictionary = new Dictionary<string, string>();

        #region Field

        private static bool _isInit = false;

        /// <summary>
        /// 版本号
        /// </summary>
        private static int _version = 1;

        /// <summary>
        /// Tracking ID / Property ID
        /// </summary>
        private static string _tid = "UA-182501874-6";

        /// <summary>
        /// 客户端ID
        /// </summary>
        private static string _cid = AnalysisHelpers.MacAddress;

        /// <summary>
        /// Document hostname.
        /// </summary>
        private static string _dh = "pad.com";


        /// <summary>
        /// Application Name
        /// </summary>
        private static string _an = "";

        /// <summary>
        /// Application Version
        /// </summary>
        private static string _av = "";

        // unknow, but old solution has
        private static string _ds = "";

        /// <summary>
        /// document page
        /// </summary>
        private static string _dp = "";

        /// <summary>
        /// document title
        /// </summary>
        private static string _dt = "Home";

        /// <summary>
        /// Document encode? my guess
        /// </summary>
        private static string _de = Encoding.Default.BodyName;

        /// <summary>
        /// User Language? my guess
        /// </summary>
        private static string _ul = System.Globalization.CultureInfo.CurrentUICulture.Name.ToUpper();

        // unknow, but old solution has
        private static string _sc = "start";

        #endregion

        public Parameters(string applicationName, string applicationVersion, DataServicesPlatform ds)
        {
            if (_isInit)
            {
                return;
            }

            _an = applicationName;

            _av = applicationVersion;

            _dp = $"/{applicationName}";

            _ds = ds.GetDescription();

            _isInit = true;
        }

        public Parameters InitParams()
        {
            _paramsDictionary.Clear();

            _paramsDictionary.Add("v", _version.ToString());

            _paramsDictionary.Add("tid", _tid);

            _paramsDictionary.Add("cid", _cid);

            // Don't ask, ask just don't know
            _paramsDictionary.Add("uid", _cid);

            _paramsDictionary.Add("an", _an);

            _paramsDictionary.Add("av", _av);

            _paramsDictionary.Add("ds", _ds);

            return this;
        }

        /// <summary>
        /// 页面追踪
        /// </summary>
        /// <returns></returns>
        public Parameters AddPageTrackParams()
        {
            _paramsDictionary.Add("t", "pageview");

            _paramsDictionary.Add("dh", _dh);

            _paramsDictionary.Add("dp", _dp);

            _paramsDictionary.Add("dt", _dt);

            _paramsDictionary.Add("de", _de);

            return this;
        }

        /// <summary>
        /// 社交互动 
        /// </summary>
        /// <returns></returns>
        public Parameters AddSocialInteraction()
        {

            _paramsDictionary.Add("t", "social");
            // Social Action. Required.
            _paramsDictionary.Add("sa", Guid.NewGuid().ToString());
            // Social Network. Required.
            _paramsDictionary.Add("sn", Guid.NewGuid().ToString());
            // Social Target. Required.
            _paramsDictionary.Add("st", "");

            // unknow, but old solution has
            _paramsDictionary.Add("qt", "560");
            _paramsDictionary.Add("api", "1");

            return this;
        }

        /// <summary>
        /// 异常跟踪
        /// </summary>
        /// <param name="exceptionDesription">异常描述</param>
        /// <param name="isFatal">是否致命</param>
        /// <returns></returns>
        public Parameters AddExceptionTrack(string exceptionDesription, bool isFatal = false)
        {

            //& t = exception       // Exception hit type.
            //& exd = IOException   
            //& exf = 1             // Exception is fatal?

            _paramsDictionary.Add("t", "exception");
            // Exception description.
            _paramsDictionary.Add("exd", System.Web.HttpUtility.UrlEncode(exceptionDesription, Encoding.UTF8));
            // Exception is fatal?
            _paramsDictionary.Add("exf", isFatal ? "0" : "1");

            return this;
        }

        /// <summary>
        /// 增加事件跟踪
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public Parameters AddEventTrack(string description)
        {
            //&t =event         // Event hit type
            //&ec=video        // Event Category. Required.
            //&ea=play         // Event Action. Required.
            //&el=holiday      // Event label.
            //&ev=300          // Event value.

            _paramsDictionary.Add("dp", System.Web.HttpUtility.UrlEncode(_dp));
            _paramsDictionary.Add("t", "event");
            _paramsDictionary.Add("ec", $"exception");
            _paramsDictionary.Add("ea", System.Web.HttpUtility.UrlEncode(_av));
            _paramsDictionary.Add("el", System.Web.HttpUtility.UrlEncode($"{_ds}:{description}", Encoding.UTF8));
            _paramsDictionary.Add("ev", "100");

            return this;
        }

        /// <summary>
        /// Get Query Params 
        /// </summary>
        public string GetQueryParams
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_cid))
                {
                    return string.Empty;
                }
                StringBuilder qparams = new StringBuilder();
                foreach (var item in _paramsDictionary)
                {
                    qparams.Append($"{item.Key}={item.Value}&");
                }
                return $"{qparams.ToString().TrimEnd('&')}";
            }
        }
    }
}
