using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Tracy.Frameworks.Common.Extends;
using Tracy.Frameworks.LogClient.Entity;
using Tracy.Frameworks.LogClient.Helper;

namespace Tracy.Proxy
{
    /// <summary>
    /// 外部接口代理
    /// 1，写xml日志
    /// 2，写性能日志
    /// </summary>
    public class WSAgent
    {
        public string SystemCode { get; set; }

        public string ClassName { get; set; }

        public string MethodName { get; set; }

        public string RequestXml { get; set; }

        public string ResponseXml { get; set; }

        /// <summary>
        /// xml日志
        /// </summary>
        private XmlLog XmlLog { get; set; }

        /// <summary>
        /// 性能日志
        /// </summary>
        private PerformanceLog PerfLog { get; set; }

        public WSAgent()
        {
            XmlLog = new XmlLog { Source = "SSharing.Proxy" };
            PerfLog = new PerformanceLog { Source = "SSharing.Proxy" };
        }

        public TResponse Request<TResponse>(Func<TResponse> func)
        {
            try
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                var result = func();
                stopWatch.Stop();
                PerfLog.Duration = stopWatch.ElapsedMilliseconds;
                ResponseXml = result.ToXml(isNeedFormat: true);
                return result;
            }
            catch
            {
                throw;
            }
            finally
            {
                //写xml日志和性能日志
                WriteXmlLog();
                WritePerfLog();
            }
        }

        /// <summary>
        /// 写xml日志
        /// </summary>
        private void WriteXmlLog()
        {
            XmlLog.SystemCode = SystemCode;
            XmlLog.ClassName = ClassName;
            XmlLog.MethodName = MethodName;
            XmlLog.RQ = RequestXml;
            XmlLog.RS = ResponseXml;
            XmlLog.Remark = "SSharing.Proxy";

            LogClientHelper.Xml(XmlLog);
        }

        /// <summary>
        /// 写性能日志
        /// </summary>
        private void WritePerfLog()
        {
            PerfLog.SystemCode = SystemCode;
            PerfLog.ClassName = ClassName;
            PerfLog.MethodName = MethodName;
            PerfLog.Remark = "SSharing.Proxy";

            LogClientHelper.Performance(PerfLog);
        }

    }
}
