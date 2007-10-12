#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageServer.Streaming
{
    public class SeriesStream : IEnumerable<InstanceStream>
    {
        #region Private members

        private Dictionary<string, InstanceStream> _instanceList = new Dictionary<string, InstanceStream>();
        private String _seriesInstanceUid = null;
        private InstanceStream _seriesTagsStream = null;

        #endregion

        #region Public Properties

        public String SeriesInstanceUid
        {
            get
            {
                if (_seriesInstanceUid == null)
                    return "";
                return _seriesInstanceUid;
            }
        }

        #endregion

        #region Constructors

        public SeriesStream(String seriesInstanceUid)
        {
            _seriesInstanceUid = seriesInstanceUid;
        }

        public SeriesStream()
        {
        }

        #endregion

        #region Public Methods

        public InstanceStream this[String sopInstanceUid]
        {
            get
            {
                if (_instanceList.ContainsKey(sopInstanceUid))
                    return  _instanceList[sopInstanceUid];

                return null;
            }
            set
            {
                if (value == null)
                    _instanceList.Remove(sopInstanceUid);
                else
                {
                    _instanceList[sopInstanceUid] = value;
                }
            }
        }

        #endregion

		#region Internal Methods

        internal void CalculateBaseCollectionForSeries()
        {
            if (_instanceList.Count < 2)
                return;

            // Optimization:  a base collection has already been created, just return.
            if (_seriesTagsStream != null)
                return;

            IEnumerator<InstanceStream> iterator = GetEnumerator();

            if (false == iterator.MoveNext())
                return;

            DicomAttributeCollection collect1 = iterator.Current.Collection;

            if (false == iterator.MoveNext())
                return;

            DicomAttributeCollection collect2 = iterator.Current.Collection;

            _seriesTagsStream = new InstanceStream(new DicomAttributeCollection(),null,TransferSyntax.ExplicitVrLittleEndian);

            foreach (DicomAttribute attrib1 in collect1)
            {
                if ((attrib1 is DicomAttributeOB)
                    || (attrib1 is DicomAttributeOW)
                    || (attrib1 is DicomAttributeOF))
                    continue;

                DicomAttribute attrib2 = collect2[attrib1.Tag];
                if (attrib2 == null)
                    continue;

                if (attrib1.Equals(attrib2))
                {
                    _seriesTagsStream.Collection[attrib1.Tag] = attrib1.Copy();
                }
            }
        }

		internal XmlElement GetMomento(XmlDocument theDocument)
		{
            // Calc the base attributes
            CalculateBaseCollectionForSeries();

			XmlElement series = theDocument.CreateElement("Series");

			XmlAttribute seriesInstanceUid = theDocument.CreateAttribute("UID");
			seriesInstanceUid.Value = _seriesInstanceUid;
			series.Attributes.Append(seriesInstanceUid);

            XmlElement baseElement = theDocument.CreateElement("BaseInstance");

            // If there's only 1 total image in the series, leave an empty base instance
            // and just have the entire image be stored.
            if (_instanceList.Count > 1)
            {
                XmlElement baseInstance = _seriesTagsStream.GetMomento(theDocument, null);

                baseElement.AppendChild(baseInstance);
            }
            series.AppendChild(baseElement);

			foreach (InstanceStream instance in _instanceList.Values)
			{
				XmlElement instanceElement = instance.GetMomento(theDocument, _seriesTagsStream);

				series.AppendChild(instanceElement);
			}

			return series;
		}

        internal void SetMemento(XmlNode theSeriesNode)
        {


            _seriesInstanceUid = theSeriesNode.Attributes["UID"].Value;

            if (!theSeriesNode.HasChildNodes)
                return;

            XmlNode childNode = theSeriesNode.FirstChild;

            while (childNode != null)
            {
                // Just search for the first study node, parse it, then break
                if (childNode.Name.Equals("BaseInstance"))
                {
                    if (childNode.HasChildNodes)
                    {
                        XmlNode instanceNode = childNode.FirstChild;
                        if (instanceNode.Name.Equals("Instance"))
                        {
                            _seriesTagsStream = new InstanceStream(instanceNode,null);
                        }
                    }
                }
                else if (childNode.Name.Equals("Instance"))
                {
                    // This assumes the BaseInstance is in the xml ahead of the actual instances, note, however,
                    // that if there is only 1 instance in the series, there will be no base instance value
                
                    InstanceStream instanceStream;

                    if (_seriesTagsStream == null)
                        instanceStream = new InstanceStream(childNode, null);
                    else
                        instanceStream = new InstanceStream(childNode, _seriesTagsStream.Collection);
                
                    _instanceList.Add(instanceStream.SopInstanceUid, instanceStream);
                }

                childNode = childNode.NextSibling;
            }


        }


		#endregion

		#region IEnumerator Implementation
		public IEnumerator<InstanceStream> GetEnumerator()
        {
            return _instanceList.Values.GetEnumerator();   
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
