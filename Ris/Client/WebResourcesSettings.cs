#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System.Configuration;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Provides application settings for all core RIS web content URLss
	/// </summary>
	/// <remarks>
	/// This code is adapted from the Visual Studio generated template code;  the generated code has been removed from the project.  Additional 
	/// settings need to be manually added to this class.
	/// </remarks>
	[SettingsGroupDescription("Provides application settings for all core RIS web content URLs.")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	public sealed class WebResourcesSettings : global::System.Configuration.ApplicationSettingsBase
	{
		private static WebResourcesSettings defaultInstance = ((WebResourcesSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new WebResourcesSettings())));

		public WebResourcesSettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}

		public static WebResourcesSettings Default
		{
			get
			{
				return defaultInstance;
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("http://localhost/RIS")]
		[global::System.Configuration.SettingsDescription(" Provides base URL for HtmlApplicationComponent web resources.  URL should specify protocol (i.e. http://server, file:///C:/directory, etc.)")]
		public string BaseUrl
		{
			get
			{
				return ((string)(this["BaseUrl"]));
			}
		}

		#region ClearCanvas.Ris.Client component settings

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("banner.htm")]
		public string BannerPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["BannerPageUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("forms/technologist/pre-exam.htm")]
		public string OrderAdditionalInfoPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["OrderAdditionalInfoPageUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("orderdetail.htm")]
		public string OrderDetailPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["OrderDetailPageUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("biographyorderdetail.htm")]
		public string BiographyOrderDetailPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["BiographyOrderDetailPageUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("visitdetail.htm")]
		public string VisitDetailPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["VisitDetailPageUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("technologistdocumentation-ordersummary.htm")]
		public string OrderSummaryUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["OrderSummaryUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("biographypatientprofile.htm")]
		public string BiographyPatientProfilePageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["BiographyPatientProfilePageUrl"]));
			}
		}

		#endregion

		#region ClearCanvas.Ris.Client.Adt component settings

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("bookingpreview.htm")]
		public string BookingFolderSystemUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["BookingFolderSystemUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("forms/technologist/mpps.htm")]
		public string DetailsPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["DetailsPageUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("protocolsummary.htm")]
		public string ProtocolSummaryUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["ProtocolSummaryUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("registrationpreview.htm")]
		public string RegistrationFolderSystemUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["RegistrationFolderSystemUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("technologistpreview.htm")]
		public string TechnologistFolderSystemUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["TechnologistFolderSystemUrl"]));
			}
		}

		#endregion

		#region ClearCanvas.Ris.Client.Reporting component settings

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("addendumeditor.htm")]
		public string AddendumEditorPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["AddendumEditorPageUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("protocollingpreview.htm")]
		public string ProtocollingFolderSystemUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["ProtocollingFolderSystemUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("protocollingorderdetail.htm")]
		public string ProtocollingOrderDetailPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["ProtocollingOrderDetailPageUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("radiologistpreview.htm")]
		public string RadiologistFolderSystemUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["RadiologistFolderSystemUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("reporteditor.htm")]
		public string ReportEditorPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["ReportEditorPageUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("reportpreview.htm")]
		public string ReportPreviewPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["ReportPreviewPageUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("priorreport.htm")]
		public string ReportViewPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["ReportViewPageUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("emergencyphysicianpreview.htm")]
		public string EmergencyPhysicianFolderSystemUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["EmergencyPhysicianFolderSystemUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("noteboxpreview.htm")]
		public string EmergencyPhysicianOrderNoteboxFolderSystemUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["EmergencyPhysicianOrderNoteboxFolderSystemUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("noteboxpreview.htm")]
		public string ReportingOrderNoteboxFolderSystemUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["ReportingOrderNoteboxFolderSystemUrl"]));
			}
		}
		#endregion
	}

	public static class WebResourceAbsoluteUrlHelper
	{
		private static readonly char[] _slash = new char[] {'/'};

		public static string FromRelative(string relativeUrl)
		{
			return WebResourcesSettings.Default.BaseUrl.TrimEnd(_slash) + '/' + relativeUrl.TrimStart(_slash);
		}
	}
}
