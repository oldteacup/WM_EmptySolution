using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GoogleAnalysis
{

    public static class AnalysisServices
    {

        /// <summary>
        /// 域
        /// </summary>
        private static string _host = "http://www.google-analytics.com/collect";

        private static readonly object _paramQueueLocker = new object();

        private static Queue<string> _paramQueue = new Queue<string>();

        private static Thread _queueThread = null;

        private static RestClient _restClient = new RestClient();

        private static Method _requestMethod = Method.POST;

        private static Parameters _parameters;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="applicationName"></param>
        /// <param name="applicationVersion"></param>
        /// <param name="dataServicesPlatform"></param>
        public static void Init(string applicationName, string applicationVersion, DataServicesPlatform dataServicesPlatform)
        {
            _parameters = new Parameters(applicationName, applicationVersion, dataServicesPlatform);
        }

        /// <summary>
        /// 开始
        /// </summary>
        /// <param name="applicationName"></param>
        /// <param name="applicationVersion"></param>
        /// <param name="dataServicesPlatform"></param>
        public static void Start()
        {
            string uri = $"{_host}?{_parameters.InitParams().AddPageTrackParams().GetQueryParams}";
            RestRequest request = new RestRequest(uri, _requestMethod);
            new RestClient().Execute(request);
            //发送完上面的请求，后面的就交给线程内自己添加发送命令。
            //后续请求在此方法内
            //StartSendPostRequest();
        }

        /// <summary>
        /// 发送异常跟踪信息
        /// </summary>
        /// <param name="description">描述</param>
        /// <param name="isFatal">是否致命异常</param>
        public static void SendExceptionTrack(string description, bool isFatal = false)
        {
            try
            {
                string uri = $"{_host}?{_parameters.InitParams().AddExceptionTrack(description, isFatal).GetQueryParams}";
                RestRequest request = new RestRequest(uri, _requestMethod);
                new RestClient().Execute(request);
            }
            catch
            { }
        }

        /// <summary>
        /// 发送事件跟踪信息
        /// </summary>
        /// <param name="description">描述</param>
        public static void SendEventTrack(string description)
        {
            try
            {
                string uri = $"{_host}?{_parameters.InitParams().AddEventTrack(description).GetQueryParams}";
                RestRequest request = new RestRequest(uri, _requestMethod);
                new RestClient().Execute(request);
            }
            catch
            { }
        }


        private static void SendTask(string param)
        {
            lock (_paramQueueLocker)
            {
                _paramQueue.Enqueue(param);
            }
        }

        private static void StartSendPostRequest()
        {
            _queueThread = new Thread(() =>
            {
                while (true)
                {
                    lock (_paramQueueLocker)
                    {
                        if (_paramQueue.Count > 0)
                        {
                            try
                            {
                                RestRequest request = new RestRequest(_paramQueue.Dequeue(), _requestMethod);
                                _restClient.Execute(request);
                            }
                            catch
                            {

                            }
                        }
                    }
                    Thread.Sleep(2 * 60 * 1000);
                    SendTask($"{_host}?{_parameters.InitParams().AddSocialInteraction().GetQueryParams}");
                }
            });
            _queueThread.IsBackground = true;
            _queueThread.Start();
        }
    }

}
