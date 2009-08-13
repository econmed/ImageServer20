using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebDeleteStudy.Extensions.LogHistory
{
    [ExtensionOf(typeof(WebDeleteProcessorExtensionPoint))]
    class LogHistoryExtension:IWebDeleteProcessorExtension
    {
        private StudyInformation _studyInfo;
        
        #region IWebDeleteProcessorExtension Members

        public void OnSeriesDeleting(WebDeleteProcessorContext context, Series series)
        {
            _studyInfo = StudyInformation.CreateFrom(context.StorageLocation.Study);
        }

        public void OnSeriesDeleted(WebDeleteProcessorContext context, Series _series)
        {
            
        }

        #endregion

        #region IWebDeleteProcessorExtension Members


        public void OnCompleted(WebDeleteProcessorContext context, IList<Series> series)
        {
            if (series.Count > 0)
            {
                Platform.Log(LogLevel.Info, "Logging history..");
                DateTime now = Platform.Time;
                using(IUpdateContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
                {
                    IStudyHistoryEntityBroker broker = ctx.GetBroker<IStudyHistoryEntityBroker>();
                    StudyHistoryUpdateColumns columns = new StudyHistoryUpdateColumns();
                    columns.InsertTime = Platform.Time;
                    columns.StudyHistoryTypeEnum = StudyHistoryTypeEnum.SeriesDeleted;
                    columns.StudyStorageKey = context.StorageLocation.Key;
                    columns.DestStudyStorageKey = context.StorageLocation.Key;
                    columns.StudyData = XmlUtils.SerializeAsXmlDoc(_studyInfo);
                    SeriesDeletionChangeLog changeLog =  new SeriesDeletionChangeLog();
                    changeLog.TimeStamp = now;
                    changeLog.Reason = context.Reason;
                    changeLog.UserId = context.UserName;
                    changeLog.Series = CollectionUtils.Map<Series, SeriesInformation>(series,
                                      delegate(Series ser)
                                          {
                                              ServerEntityAttributeProvider seriesWrapper = new ServerEntityAttributeProvider(ser);
                                              return new SeriesInformation(seriesWrapper);
                                          });
                    columns.ChangeDescription = XmlUtils.SerializeAsXmlDoc(changeLog);
                    StudyHistory history = broker.Insert(columns);
                    if (history!=null)
                        ctx.Commit();
                    
                }
            }
            
        }

        #endregion
    }

    public class SeriesDeletionChangeLog
    {
        private DateTime _timeStamp;
        private string _userId;
        private string reason;
        private List<SeriesInformation> _seriesInfo;

        public DateTime TimeStamp
        {
            get { return _timeStamp; }
            set { _timeStamp = value; }
        }

        public string UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        public string Reason
        {
            get { return reason; }
            set { reason = value; }
        }
        [XmlArray("DeletedSeries")]
        public List<SeriesInformation> Series
        {
            get { return _seriesInfo; }
            set { _seriesInfo = value; }
        }
    }
}