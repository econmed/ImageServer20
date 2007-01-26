using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using ClearCanvas.Common;
using System.Drawing;
using ClearCanvas.Common.Utilities;
using System.Configuration;

namespace ClearCanvas.ImageViewer.Annotations
{
	public sealed class AnnotationLayoutStore
	{
		internal const string DefaultSettingsFile = "AnnotationLayoutStoreDefaults.xml";

		private static AnnotationLayoutStore _instance;

		private XmlDocument _document;
		private event EventHandler _storeChanged;
		private Dictionary<string, StoredAnnotationLayout> _layoutsInMemory;

		private AnnotationLayoutStore()
		{
			_layoutsInMemory = new Dictionary<string, StoredAnnotationLayout>();
		}

		public event EventHandler StoreChanged
		{
			add { _storeChanged += value; }
			remove { _storeChanged -= value; }
		}

		public static AnnotationLayoutStore Instance
		{
			get
			{
				if (_instance == null)
					_instance = new AnnotationLayoutStore();

				return _instance;
			}
		}

		public void Clear()
		{
			_layoutsInMemory.Clear();
			SaveSettings("");
			Initialize(true);
		}

		public void RemoveLayout(string identifier)
		{
			Platform.CheckForEmptyString(identifier, "identifier"); 
			Initialize(false);

			string xPath = "annotation-configuration/annotation-layouts";
			XmlElement layoutsNode = (XmlElement)_document.SelectSingleNode(xPath);
			if (layoutsNode == null)
				throw new InvalidDataException(String.Format(SR.ExceptionInvalidAnnotationLayoutXml, "'annotation-layouts' node does not exist"));

			xPath = String.Format("annotation-layout[@id='{0}']", identifier);
			XmlNodeList matchingNodes = layoutsNode.SelectNodes(xPath);
			foreach(XmlElement matchingNode in matchingNodes)
				layoutsNode.RemoveChild(matchingNode);

			if (_layoutsInMemory.ContainsKey(identifier))
				_layoutsInMemory.Remove(identifier);

			SaveSettings(_document.OuterXml);
		}

		public IList<StoredAnnotationLayout> GetLayouts(IList<IAnnotationItem> availableAnnotationItems)
		{
			Initialize(false);

			string xPath = "annotation-configuration/annotation-layouts/annotation-layout";
			XmlNodeList layoutNodes = _document.SelectNodes(xPath);
					
			StoredAnnotationLayoutDeserializer deserializer = new StoredAnnotationLayoutDeserializer(availableAnnotationItems);
			List<StoredAnnotationLayout> layouts = new List<StoredAnnotationLayout>();

			foreach (XmlElement layoutNode in layoutNodes)
			{
				StoredAnnotationLayout layout;
				string identifier = layoutNode.GetAttribute("id");
				if (_layoutsInMemory.ContainsKey(identifier))
				{
					layout = _layoutsInMemory[identifier];
				}
				else
				{
					layout = deserializer.DeserializeLayout(layoutNode);
					_layoutsInMemory[layout.Identifier] = layout;
				}

				layouts.Add(layout);
			}

			return layouts;
		}

		public StoredAnnotationLayout GetLayout(string identifier, IList<IAnnotationItem> availableAnnotationItems)
		{
			if (String.IsNullOrEmpty(identifier))
				return null;

			if (_layoutsInMemory.ContainsKey(identifier))
				return _layoutsInMemory[identifier];

			Initialize(false);
			
			string xPath = String.Format("annotation-configuration/annotation-layouts/annotation-layout[@id='{0}']", identifier);
			XmlElement layoutNode = (XmlElement)_document.SelectSingleNode(xPath);
			if (layoutNode == null)
				return null;

			StoredAnnotationLayout layout = new StoredAnnotationLayoutDeserializer(availableAnnotationItems).DeserializeLayout(layoutNode);
			_layoutsInMemory[layout.Identifier] = layout;
			return layout;
		}

		public void Update(StoredAnnotationLayout layout)
		{
			Platform.CheckForNullReference(layout, "layout");
			Platform.CheckForEmptyString(layout.Identifier, "layout.Identifier");

			Initialize(false);

			try
			{
				new StoredAnnotationLayoutSerializer().SerializeLayout(layout);
				_layoutsInMemory[layout.Identifier] = layout;
				SaveSettings(_document.OuterXml);
			}
			catch
			{
				//undo any changes you may have just made.
				Initialize(true);
				throw;
			}
		}

		public void Update(IEnumerable<StoredAnnotationLayout> layouts)
		{
			StoredAnnotationLayoutSerializer serializer = new StoredAnnotationLayoutSerializer();
			Initialize(false);

			try
			{
				foreach (StoredAnnotationLayout layout in layouts)
				{
					Platform.CheckForNullReference(layout, "layout");
					Platform.CheckForEmptyString(layout.Identifier, "layout.Identifier");

					serializer.SerializeLayout(layout);
					_layoutsInMemory[layout.Identifier] = layout;
				}

				SaveSettings(_document.OuterXml);
			}
			catch
			{
				//undo any changes you may have just made.
				Initialize(true);
				throw;
			}
		}

		private void SaveSettings(string settingsXml)
		{
			AnnotationLayoutStoreSettings settings = new AnnotationLayoutStoreSettings();
			settings.LayoutSettings = settingsXml;
			settings.Save();

			if (_storeChanged != null)
				_storeChanged(this, EventArgs.Empty);
		}

		private void Initialize(bool reloadSettings)
		{
			if (_document != null && !reloadSettings)
				return;

			_layoutsInMemory.Clear();

			try
			{
				AnnotationLayoutStoreSettings settings = new AnnotationLayoutStoreSettings();
				_document = new XmlDocument();

				if (!String.IsNullOrEmpty(settings.LayoutSettings))
				{
					_document.LoadXml(settings.LayoutSettings);
				}
				else if (!String.IsNullOrEmpty(settings.ApplicationDefaultLayoutSettings))
				{
					_document.LoadXml(settings.ApplicationDefaultLayoutSettings);

					SaveSettings(settings.ApplicationDefaultLayoutSettings);
				}
				else if (File.Exists(DefaultSettingsFile))
				{
					using (FileStream stream = new FileStream(DefaultSettingsFile, FileMode.Open))
					{
						XmlTextReader reader = new XmlTextReader(stream);
						_document.Load(reader);

						SaveSettings(_document.OuterXml);
					}
				}
				else
				{
					XmlElement root = _document.CreateElement("annotation-configuration");
					_document.AppendChild(root);
					root.AppendChild(_document.CreateElement("annotation-layouts"));

					SaveSettings(_document.OuterXml);
				}
			}
			catch
			{
				_document = null;
				throw;
			}
		}

		private class StoredAnnotationLayoutDeserializer
		{
			private IList<IAnnotationItem> _availableAnnotationItems;
			
			public StoredAnnotationLayoutDeserializer(IList<IAnnotationItem> availableAnnotationItems)
			{
				Platform.CheckForNullReference(availableAnnotationItems, "availableAnnotationItems");

				_availableAnnotationItems = availableAnnotationItems;
			}

			public StoredAnnotationLayout DeserializeLayout(XmlElement layoutNode)
			{
				Platform.CheckForNullReference(layoutNode, "layoutNode");

				StoredAnnotationLayout layout = new StoredAnnotationLayout(layoutNode.GetAttribute("id"));
				XmlNodeList annotationBoxGroupNodes = layoutNode.SelectNodes("annotation-box-groups/annotation-box-group");
				if (annotationBoxGroupNodes != null)
					DeserializeAnnotationBoxGroups(layout, annotationBoxGroupNodes);

				return layout;
			}

			private void DeserializeAnnotationBoxGroups(StoredAnnotationLayout layout, XmlNodeList groupNodes)
			{
				foreach (XmlElement groupNode in groupNodes)
				{
					string newGroupId = groupNode.GetAttribute("id");
					StoredAnnotationBoxGroup newGroup = new StoredAnnotationBoxGroup(newGroupId);

					XmlElement defaultBoxSettingsNode = (XmlElement)groupNode.SelectSingleNode("default-box-settings");

					if (defaultBoxSettingsNode != null)
						DeserializeBoxSettings(newGroup.DefaultBoxSettings, defaultBoxSettingsNode);

					XmlNodeList annotationBoxNodes = groupNode.SelectNodes("annotation-boxes/annotation-box");
					if (annotationBoxNodes != null)
						DeserializeAnnotationBoxes(newGroup, annotationBoxNodes);

					layout.AnnotationBoxGroups.Add(newGroup);
				}
			}

			private void DeserializeAnnotationBoxes(StoredAnnotationBoxGroup group, XmlNodeList annotationBoxNodes)
			{
				foreach (XmlElement annotationBoxNode in annotationBoxNodes)
				{
					string normalizedRectangleString = annotationBoxNode.GetAttribute("normalized-rectangle");

					RectangleF normalizedRectangle;
					if (!DeserializeNormalizedRectangle(normalizedRectangleString, out normalizedRectangle))
						continue;

					XmlElement boxSettingsNode = (XmlElement)annotationBoxNode.SelectSingleNode("box-settings");

					AnnotationBox newBox = group.GetNewBox();
					newBox.NormalizedRectangle = normalizedRectangle;

					if (boxSettingsNode != null)
						DeserializeBoxSettings(newBox, boxSettingsNode);

					string annotationItemIdentifier = annotationBoxNode.GetAttribute("annotation-item-id");
					foreach (IAnnotationItem item in _availableAnnotationItems)
					{
						if (item.GetIdentifier() == annotationItemIdentifier)
						{
							newBox.AnnotationItem = item;
							break;
						}
					}

					group.AnnotationBoxes.Add(newBox);
				}
			}

			private void DeserializeBoxSettings(AnnotationBox boxSettings, XmlElement boxSettingsNode)
			{
				string font = boxSettingsNode.GetAttribute("font");
				string color = boxSettingsNode.GetAttribute("color");
				string italics = boxSettingsNode.GetAttribute("italics");
				string bold = boxSettingsNode.GetAttribute("bold");
				string numberOfLines = boxSettingsNode.GetAttribute("number-of-lines");
				string truncation = boxSettingsNode.GetAttribute("truncation");
				string justification = boxSettingsNode.GetAttribute("justification");

				if (!String.IsNullOrEmpty(font))
					boxSettings.Font = font;
				if (!String.IsNullOrEmpty(color))
					boxSettings.Color = color;
				if (!String.IsNullOrEmpty(italics))
					boxSettings.Italics = (String.Compare("True", italics, true) == 0);
				if (!String.IsNullOrEmpty(bold))
					boxSettings.Bold = (String.Compare("True", bold, true) == 0);
				if (!String.IsNullOrEmpty(numberOfLines))
				{
					byte result;
					if (!byte.TryParse(numberOfLines, out result))
						result = 1;

					boxSettings.NumberOfLines = result;
				}

				if (!String.IsNullOrEmpty(truncation))
				{
					AnnotationBox.TruncationBehaviour fromString = boxSettings.Truncation;
					if (EnumUtilities.StringToEnum(truncation, ref fromString))
						boxSettings.Truncation = fromString;
				}

				if (!String.IsNullOrEmpty(justification))
				{
					AnnotationBox.JustificationBehaviour fromString = boxSettings.Justification;
					if (EnumUtilities.StringToEnum(justification, ref fromString))
						boxSettings.Justification = fromString;
				}

				XmlElement configurationSettings = (XmlElement)boxSettingsNode.SelectSingleNode("configuration-settings");
				if (configurationSettings != null)
				{
					string showLabel = configurationSettings.GetAttribute("show-label");
					string showLabelIfEmpty = configurationSettings.GetAttribute("show-label-if-empty");
					if (!String.IsNullOrEmpty(showLabel))
						boxSettings.ConfigurationOptions.ShowLabel = (String.Compare("True", showLabel, true) == 0);
					if (!String.IsNullOrEmpty(showLabelIfEmpty))
						boxSettings.ConfigurationOptions.ShowLabelIfValueEmpty = (String.Compare("True", showLabelIfEmpty, true) == 0);
				}
			}

			private bool DeserializeNormalizedRectangle(string normalizedRectangleString, out RectangleF normalizedRectangle)
			{
				normalizedRectangle = new RectangleF();

				string[] rectangleComponents = normalizedRectangleString.Split('\\');
				if (rectangleComponents.Length != 4)
					return false;

				float left, right, top, bottom;
				if (!float.TryParse(rectangleComponents[0], out left))
					return false;
				if (!float.TryParse(rectangleComponents[1], out top))
					return false;
				if (!float.TryParse(rectangleComponents[2], out right))
					return false;
				if (!float.TryParse(rectangleComponents[3], out bottom))
					return false;

				if (left >= right)
					return false;
				if (top >= bottom)
					return false;
				if (left < 0F || left > 1.0F)
					return false;
				if (top < 0F || top > 1.0F)
					return false;
				if (right < 0F || right > 1.0F)
					return false;
				if (bottom < 0F || bottom > 1.0F)
					return false;

				normalizedRectangle = RectangleF.FromLTRB(left, top, right, bottom);
				return true;
			}
		}

		private class StoredAnnotationLayoutSerializer
		{
			public StoredAnnotationLayoutSerializer()
			{
			}

			private XmlDocument Document
			{
				get { return AnnotationLayoutStore._instance._document; }
			}

			public void SerializeLayout(StoredAnnotationLayout layout)
			{
				string xPath = "annotation-configuration/annotation-layouts";
				XmlElement layoutsNode = (XmlElement)this.Document.SelectSingleNode(xPath);
				if (layoutsNode == null)
					throw new InvalidDataException(String.Format(SR.ExceptionInvalidAnnotationLayoutXml, "'annotation-layouts' node does not exist"));

				XmlElement newLayoutNode = this.Document.CreateElement("annotation-layout");
				newLayoutNode.SetAttribute("id", layout.Identifier);

				XmlElement groupsNode = this.Document.CreateElement("annotation-box-groups");
				newLayoutNode.AppendChild(groupsNode);

				SerializeAnnotationBoxGroups(groupsNode, layout.AnnotationBoxGroups);

				xPath = String.Format("annotation-layout[@id='{0}']", layout.Identifier);
				XmlElement existingLayoutNode = (XmlElement)layoutsNode.SelectSingleNode(xPath);

				if (existingLayoutNode != null)
					layoutsNode.ReplaceChild(newLayoutNode, existingLayoutNode);
				else
					layoutsNode.AppendChild(newLayoutNode);
			}

			private void SerializeAnnotationBoxGroups(XmlElement groupsNode, IEnumerable<StoredAnnotationBoxGroup> annotationBoxGroups)
			{
				foreach (StoredAnnotationBoxGroup group in annotationBoxGroups)
				{
					XmlElement groupNode = this.Document.CreateElement("annotation-box-group");
					groupsNode.AppendChild(groupNode);

					groupNode.SetAttribute("id", group.Identifier);

					XmlElement defaultBoxSettingsNode = this.Document.CreateElement("default-box-settings");
					SerializeAnnotationBoxSettings(group.DefaultBoxSettings, new AnnotationBox(), defaultBoxSettingsNode);

					if (defaultBoxSettingsNode.ChildNodes.Count > 0 || defaultBoxSettingsNode.Attributes.Count > 0) 
						groupNode.AppendChild(defaultBoxSettingsNode);

					SerializeAnnotationBoxes(group.AnnotationBoxes, group.DefaultBoxSettings, groupNode);
				}
			}

			private void SerializeAnnotationBoxes(IList<AnnotationBox> annotationBoxes, AnnotationBox defaultBoxSettings, XmlElement groupNode)
			{
				XmlElement boxesNode = this.Document.CreateElement("annotation-boxes");
				groupNode.AppendChild(boxesNode);

				foreach (AnnotationBox box in annotationBoxes)
				{
					XmlElement boxNode = this.Document.CreateElement("annotation-box");
					boxesNode.AppendChild(boxNode);

					string normalizedRectangle = String.Format("{0:F6}\\{1:F6}\\{2:F6}\\{3:F6}", box.NormalizedRectangle.Left, box.NormalizedRectangle.Top, box.NormalizedRectangle.Right, box.NormalizedRectangle.Bottom);

					boxNode.SetAttribute("normalized-rectangle", normalizedRectangle);
					boxNode.SetAttribute("annotation-item-id", (box.AnnotationItem == null) ? "" : box.AnnotationItem.GetIdentifier());

					XmlElement settingsNode = this.Document.CreateElement("box-settings");
					SerializeAnnotationBoxSettings(box, defaultBoxSettings, settingsNode);

					if (settingsNode.ChildNodes.Count > 0 || settingsNode.Attributes.Count > 0)
						boxNode.AppendChild(settingsNode);
				}
			}

			private void SerializeAnnotationBoxSettings(AnnotationBox annotationBox, AnnotationBox defaultSettings, XmlElement boxSettingsNode)
			{
				//only save values that are different from the defaults.
				if (annotationBox.Bold != defaultSettings.Bold)
					boxSettingsNode.SetAttribute("bold", annotationBox.Bold ? "True" : "False");
				if (annotationBox.Italics != defaultSettings.Italics)
					boxSettingsNode.SetAttribute("italics", annotationBox.Italics ? "True" : "False");
				if (annotationBox.Font != defaultSettings.Font)
					boxSettingsNode.SetAttribute("font", annotationBox.Font);
				if (annotationBox.Color != defaultSettings.Color)
					boxSettingsNode.SetAttribute("color", annotationBox.Color);
				if (annotationBox.NumberOfLines != defaultSettings.NumberOfLines)
					boxSettingsNode.SetAttribute("number-of-lines", annotationBox.NumberOfLines.ToString());
				if (annotationBox.Truncation != defaultSettings.Truncation)
					boxSettingsNode.SetAttribute("truncation", annotationBox.Truncation.ToString());
				if (annotationBox.Justification != defaultSettings.Justification)
					boxSettingsNode.SetAttribute("justification", annotationBox.Justification.ToString());

				XmlElement configurationSettingsNode = this.Document.CreateElement("configuration-settings");
				if (annotationBox.ConfigurationOptions.ShowLabel != defaultSettings.ConfigurationOptions.ShowLabel)
					configurationSettingsNode.SetAttribute("show-label", annotationBox.ConfigurationOptions.ShowLabel ? "True" : "False");

				if (annotationBox.ConfigurationOptions.ShowLabelIfValueEmpty != defaultSettings.ConfigurationOptions.ShowLabelIfValueEmpty)
					configurationSettingsNode.SetAttribute("show-label-if-empty", annotationBox.ConfigurationOptions.ShowLabelIfValueEmpty ? "True" : "False");

				if (configurationSettingsNode.Attributes.Count > 0)
					boxSettingsNode.AppendChild(configurationSettingsNode);
			}
		}
	}
}