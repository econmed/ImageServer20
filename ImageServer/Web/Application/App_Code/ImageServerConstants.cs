using System.Web.UI;

public class ImageServerConstants
{
    private static string _theme;

    /// <summary>
    /// Sets or gets the theme for the web application.
    /// </summary>
    public static string Theme
    {
        set { _theme = value; }
        get { return _theme; }
    }

    public const string High = "high";
    public const string Low = "low";
    public const string ImagePng = "image/png";
    public const string Pct = "pct";
    public const string Default = "Default";
    public const string First = "first";
    public const string Last = "last";
    public const string Next = "next";
    public const string Prev = "prev";
    public const string MMDDYYY = "MM/dd/yyyy";
    public const string DicomDate = "yyyyMMdd";
    public const string DicomDateTime = "YYYYMMDDHHMMSS.FFFFFF";
    public const string DicomSeparator = "^";
    public const string DefaultConfigurationXml = "<HsmArchive><RootDir>e:\\Archive</RootDir></HsmArchive>";
    
    public class DicomTags {
        public const string PatientsName = "00100010";
        public const string PatientID = "00100020";
        public const string PatientsBirthDate = "00100030";
        public const string PatientsSex = "00100040";
        public const string PatientsAge = "00101010";
        public const string ReferringPhysician = "00080090";
        public const string StudyDate = "00080020";
        public const string StudyTime = "00080030";
        public const string AccessionNumber = "00080050";
        public const string StudyDescription = "00081030";
        public const string StudyInstanceUID = "0020000D";
        public const string StudyID = "00200010";
    }

    public class PageURLs
    {
        public const string BarChartPage = "~/Pages/Common/BarChart.aspx?pct={0}&high={1}&low={2}";
        public const string MoveStudyPage = "~/Pages/Search/Move/Default.aspx";
        public const string SeriesDetailsPage = "~/Pages/SeriesDetails/Default.aspx";
        public const string StudyDetailsPage = "~/Pages/StudyDetails/Default.aspx";
		public const string SearchPage = "~/Pages/Search/Default.aspx";
        public const string WorkQueueItemDetailsPage = "~/Pages/Queues/WorkQueue/Edit/Default.aspx";
        public const string WebServices = "~/Services/webservice.htc";
        public const string NumberFormatScript = "~/Scripts/NumberFormat154.js";
        public const string ErrorPage = "~/Pages/Error/ErrorPage.aspx";
        
	}

    public class ImageURLs
    {
        public const string AddButtonDisabled = "images/Buttons/AddDisabled.png";
        public const string AddButtonEnabled = "images/Buttons/AddEnabled.png";
        public const string UpdateButtonDisabled = "images/Buttons/UpdateDisabled.png";
        public const string UpdateButtonEnabled = "images/Buttons/UpdateEnabled.png";
        public static readonly string AutoRouteFeature = string.Format("~/App_Themes/{0}/images/Indicators/AutoRouteFeature.png", Theme);
        public static readonly string Blank = string.Format("~/App_Themes/{0}/images/blank.gif", Theme);
        public static readonly string Checked = string.Format("~/App_Themes/{0}/images/Indicators/checked.png", Theme);
        public static readonly string QueryFeature = string.Format("~/App_Themes/{0}/images/Indicators/QueryFeature.png", Theme);
        public static readonly string RetrieveFeature = string.Format("~/App_Themes/{0}/images/Indicators/RetrieveFeature.png", Theme);
        public static readonly string StoreFeature = string.Format("~/App_Themes/{0}/images/Indicators/StoreFeature.png", Theme);
        public static readonly string Unchecked = string.Format("~/App_Themes/{0}/images/Indicators/unchecked.png", Theme);
        public static readonly string UsageBar = string.Format("~/App_Themes/{0}/images/Indicators/usage.png", Theme);
        public static readonly string Watermark = string.Format("~/App_Themes/{0}/images/Indicators/Watermark.gif", Theme);
    }

    public class QueryStrings
    {
        public const string ServerAE = "serverae";
        public const string StudyUID = "studyuid";
    }

    public class ContextKeys
    {
        public const string ErrorMessage = "ERROR_MESSAGE";
        public const string Description = "DESCRIPTION";
        public const string StackTrace = "STACK_TRACE";

    }

}
