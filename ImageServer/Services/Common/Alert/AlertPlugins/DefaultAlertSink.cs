using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Xml;
using System.Xml.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Services.Common.Alert.AlertPlugins
{
    /// <summary>
    /// Represents an alert service extension that stores <see cref="ClearCanvas.ImageServer.Common.Alert"/> in the database or log file, whichever available.
    /// </summary>
    [ExtensionOf(typeof(AlertServiceExtensionPoint))]
    class DefaultAlertSink : IAlertServiceExtension
    {
        #region IAlertServiceExtension Members

        public void OnAlert(ImageServer.Common.Alert alert)
        {
            AlertFilter filter = new AlertFilter(AlertCache.Instance);
            if (!filter.Filter(alert))
            {
                AlertCache.Instance.Add(alert);

                try
                {
                    WriteToDB(alert);
                }
                catch(Exception)
                {
                    WriteToLog(alert);
                }
               
            }

        }

        #endregion

        #region Private Methods
        private void WriteToLog(ImageServer.Common.Alert alert)
        {
            XmlDocument doc = new XmlDocument();

            if (alert.Data is string)
            {
                XmlNode content = doc.CreateElement("Message");
                XmlNode msg = doc.CreateTextNode(alert.Data.ToString());
                content.AppendChild(msg);
                doc.AppendChild(content);

            }
            else
            {
                MemoryStream ms = new MemoryStream();
                XmlSerializer serializer = new XmlSerializer(alert.Data.GetType());
                try
                {
                    serializer.Serialize(ms, alert.Data);
                    ms.Seek(0, SeekOrigin.Begin);
                    doc.Load(ms);
                }
                catch (Exception)
                {
                    // cannot be serialized as xml. Resort to string instead.
                    XmlNode content = doc.CreateElement("Message");
                    XmlNode msg = doc.CreateTextNode(alert.Data.ToString());
                    content.AppendChild(msg);
                    doc.AppendChild(content);
                }
            }

            using (StringWriter sw = new StringWriter())
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.NewLineOnAttributes = false;
                settings.OmitXmlDeclaration = true;
                settings.Encoding = Encoding.UTF8;

                XmlWriter writer = XmlWriter.Create(sw, settings);
                doc.WriteTo(writer);
                writer.Flush();

                String log = String.Format("ALERT: {0} : {1}", alert.Source.Name, sw);
                switch (alert.Level)
                {
                    case AlertLevel.Critical:
                    case AlertLevel.Error:
                        Platform.Log(LogLevel.Error, log);
                        break;

                    case AlertLevel.Informational:
                        Platform.Log(LogLevel.Info, log);
                        break;
                    case AlertLevel.Warning:
                        Platform.Log(LogLevel.Warn, log);
                        break;
                    default:
                        Platform.Log(LogLevel.Info, log);
                        break;
                }
            }

        }

        private void WriteToDB(ImageServer.Common.Alert alert)
        {
            IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();

            using (IUpdateContext ctx = store.OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IAlertEntityBroker broker = ctx.GetBroker<IAlertEntityBroker>();
                AlertUpdateColumns columns = new AlertUpdateColumns();
                columns.InsertTime = Platform.Time;

                switch (alert.Level)
                {
                    case AlertLevel.Informational: columns.AlertLevelEnum = AlertLevelEnum.Informational; break;
                    case AlertLevel.Warning: columns.AlertLevelEnum = AlertLevelEnum.Warning; break;
                    case AlertLevel.Error: columns.AlertLevelEnum = AlertLevelEnum.Error; break;
                    case AlertLevel.Critical: columns.AlertLevelEnum = AlertLevelEnum.Critical; break;
                    default:
                        columns.AlertLevelEnum = AlertLevelEnum.Informational; break;
                }

                switch (alert.Category)
                {
                    case AlertCategory.System: columns.AlertCategoryEnum = AlertCategoryEnum.System; break;
                    case AlertCategory.Application: columns.AlertCategoryEnum = AlertCategoryEnum.Application; break;
                    case AlertCategory.Security: columns.AlertCategoryEnum = AlertCategoryEnum.Security; break;
                    case AlertCategory.User: columns.AlertCategoryEnum = AlertCategoryEnum.User; break;
                    default:
                        columns.AlertCategoryEnum = AlertCategoryEnum.User; break;
                }

                columns.TypeCode = alert.Code;
                columns.Source = alert.Source.Host;
                columns.Component = alert.Source.Name;

                XmlDocument doc = new XmlDocument();

                if (alert.Data is string)
                {
                    XmlNode content = doc.CreateElement("Message");
                    XmlNode msg = doc.CreateTextNode(alert.Data.ToString());
                    content.AppendChild(msg);
                    doc.AppendChild(content);

                }
                else
                {
                    MemoryStream ms = new MemoryStream();
                    XmlSerializer serializer = new XmlSerializer(alert.Data.GetType());
                    try
                    {
                        serializer.Serialize(ms, alert.Data);
                        ms.Seek(0, SeekOrigin.Begin);
                        doc.Load(ms);
                    }
                    catch (Exception)
                    {
                        // cannot be serialized as xml. Resort to string instead.
                        XmlNode content = doc.CreateElement("Message");
                        XmlNode msg = doc.CreateTextNode(alert.Data.ToString());
                        content.AppendChild(msg);
                        doc.AppendChild(content);
                    }

                }

                columns.Content = doc;

                broker.Insert(columns);

                ctx.Commit();

            }
        }

        #endregion

    }
    
    /// <summary>
    /// Represent an alert cache
    /// </summary>
    internal class AlertCache
    {
        #region Private members
        private readonly Cache _cache = HttpRuntime.Cache;
        private readonly List<ImageServer.Common.Alert> _listAlerts = new List<ImageServer.Common.Alert>();
        #endregion

        #region Private Static Members
        static private AlertCache _instance;
        #endregion

        #region Public Static Properties
        
        /// <summary>
        /// Gets an instance of <see cref="AlertCache"/>
        /// </summary>
        static public AlertCache Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AlertCache();
                }

                return _instance;
            }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// ***** Internal use only. Use AlertCache.Instance instead ****
        /// </summary>
        private AlertCache()
        { }

        #endregion

        #region Private Methods
        static private string ResolveKey(ImageServer.Common.Alert alert)
        {
            string key = String.Format("{0}/{1}/{2}/{3}",
                                       alert.Source.Host, alert.Source.Name, alert.Code, alert.Data);

            return key;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds an alert into the cache.
        /// </summary>
        /// <param name="alert"></param>
        public void Add(ImageServer.Common.Alert alert)
        {
            _cache.Add(ResolveKey(alert), alert, null, alert.ExpirationTime, Cache.NoSlidingExpiration, CacheItemPriority.Normal,
                    delegate(string key, Object value, CacheItemRemovedReason reason)
                        {
                            _listAlerts.Remove( (ImageServer.Common.Alert) value);
                        });
            _listAlerts.Add(alert);
        }

        /// <summary>
        /// Gets a value indicating whether the specified alert or another alert that represents the same event is already in the cache.
        /// </summary>
        /// <param name="alert"></param>
        /// <returns></returns>
        public bool Contains(ImageServer.Common.Alert alert)
        {
            return _listAlerts.Contains(alert);
        }

        #endregion

    }


    /// <summary>
    /// Represents an alert filter
    /// </summary>
    internal class AlertFilter
    {
        #region Private Members
        private readonly AlertCache _cache;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of <see cref="AlertFilter"/> backed by the specified the alert cache.
        /// </summary>
        /// <param name="cache"></param>
        public AlertFilter(AlertCache cache)
        {
            _cache = cache;
        }
        #endregion

        #region Public Methods
        public bool Filter(ImageServer.Common.Alert alert)
        {
            return _cache.Contains(alert);
        }

        #endregion
    }

}