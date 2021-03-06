﻿#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.PresentationStates;

namespace ClearCanvas.ImageViewer
{
	#region Default

	[Cloneable(false)]
	internal class SeriesDisplaySetDescriptor : DicomDisplaySetDescriptor
	{
		public SeriesDisplaySetDescriptor(ISeriesIdentifier sourceSeries, IPresentationImageFactory presentationImageFactory)
			: base(sourceSeries, presentationImageFactory)
		{
			Platform.CheckForNullReference(presentationImageFactory, "presentationImageFactory");
		}

		protected SeriesDisplaySetDescriptor(SeriesDisplaySetDescriptor source, ICloningContext context)
			: base(source, context)
		{
		}

		protected override string GetName()
		{
			return String.Format("{0}: {1}", SourceSeries.SeriesNumber, SourceSeries.SeriesDescription);
		}

		protected override string GetDescription()
		{
			return SourceSeries.SeriesDescription;
		}

		protected override string GetUid()
		{
			return SourceSeries.SeriesInstanceUid;
		}

		internal override bool ShouldAddSop(Sop sop)
		{
			return (sop.SeriesInstanceUid == SourceSeries.SeriesInstanceUid && sop.IsImage);
		}
	}

	[Cloneable(false)]
	internal class SingleFrameDisplaySetDescriptor : DicomDisplaySetDescriptor
	{
		private readonly string _suffix;
		private readonly string _seriesInstanceUid;
		private readonly string _sopInstanceUid;
		private readonly int _frameNumber;
		private readonly int _position;

		public SingleFrameDisplaySetDescriptor(ISeriesIdentifier sourceSeries, Frame frame, int position)
			: base(sourceSeries)
		{
			_seriesInstanceUid = frame.SeriesInstanceUid;
			_sopInstanceUid = frame.SopInstanceUid;
			_frameNumber = frame.FrameNumber;
			_position = position;

			if (sourceSeries.SeriesInstanceUid == frame.SeriesInstanceUid)
			{
				_suffix = String.Format(SR.SuffixFormatSingleFrameDisplaySet, frame.ParentImageSop.InstanceNumber, _frameNumber);
			}
			else
			{
				//this is a referenced frame (e.g. key iamge).
				_suffix = String.Format(SR.SuffixFormatSingleReferencedFrameDisplaySet, 
					frame.ParentImageSop.SeriesNumber, frame.ParentImageSop.InstanceNumber, _frameNumber);
			}
		}

		protected SingleFrameDisplaySetDescriptor(SingleFrameDisplaySetDescriptor source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		protected override string GetName()
		{
			if (String.IsNullOrEmpty(SourceSeries.SeriesDescription))
				return String.Format("{0}: {1}", SourceSeries.SeriesNumber, _suffix);
			else
				return String.Format("{0}: {1} - {2}", SourceSeries.SeriesNumber, SourceSeries.SeriesDescription, _suffix);
		}

		protected override string GetDescription()
		{
			if (String.IsNullOrEmpty(SourceSeries.SeriesDescription))
				return _suffix;
			else
				return String.Format("{0} - {1}", SourceSeries.SeriesDescription, _suffix);
		}

		protected override string GetUid()
		{
			return String.Format("{0}:{1}:{2}:{3}:{4}", SourceSeries.SeriesInstanceUid, _seriesInstanceUid, _sopInstanceUid, _frameNumber, _position);
		}
	}

	[Cloneable(false)]
	internal class SingleImageDisplaySetDescriptor : DicomDisplaySetDescriptor
	{
		private readonly string _suffix;
		private readonly string _seriesInstanceUid;
		private readonly string _sopInstanceUid;
		private readonly int _position;

		public SingleImageDisplaySetDescriptor(ISeriesIdentifier sourceSeries, ImageSop imageSop, int position)
			: base(sourceSeries)
		{
			_sopInstanceUid = imageSop.SopInstanceUid;
			_seriesInstanceUid = imageSop.SeriesInstanceUid;
			_position = position;

			string laterality = imageSop.ImageLaterality;
			string viewPosition = imageSop.ViewPosition;
			if (string.IsNullOrEmpty(viewPosition))
			{
				DicomAttributeSQ codeSequence = imageSop[DicomTags.ViewCodeSequence] as DicomAttributeSQ;
				if (codeSequence != null && codeSequence.Count > 0)
					viewPosition = codeSequence[0][DicomTags.CodeMeaning].GetString(0, null);
			}

			string lateralityViewPosition = null;
			if (!String.IsNullOrEmpty(laterality) && !String.IsNullOrEmpty(viewPosition))
				lateralityViewPosition = String.Format("{0}/{1}", laterality, viewPosition);
			else if (!String.IsNullOrEmpty(laterality))
				lateralityViewPosition = laterality;
			else if (!String.IsNullOrEmpty(viewPosition))
				lateralityViewPosition = viewPosition;

			if (sourceSeries.SeriesInstanceUid == imageSop.SeriesInstanceUid)
			{
				if (lateralityViewPosition != null)
					_suffix = String.Format(SR.SuffixFormatSingleImageDisplaySetWithLateralityViewPosition, lateralityViewPosition, imageSop.InstanceNumber);
				else
					_suffix = String.Format(SR.SuffixFormatSingleImageDisplaySet, imageSop.InstanceNumber);
			}
			else
			{
				//this is a referenced image (e.g. key iamge).
				if (lateralityViewPosition != null)
					_suffix = String.Format(SR.SuffixFormatSingleReferencedImageDisplaySetWithLateralityViewPosition, 
						lateralityViewPosition, imageSop.SeriesNumber, imageSop.InstanceNumber);
				else
					_suffix = String.Format(SR.SuffixFormatSingleReferencedImageDisplaySet,
						imageSop.SeriesNumber, imageSop.InstanceNumber);
			}
		}

		protected SingleImageDisplaySetDescriptor(SingleImageDisplaySetDescriptor source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		protected override string GetName()
		{
			if (String.IsNullOrEmpty(SourceSeries.SeriesDescription))
				return String.Format("{0}: {1}", SourceSeries.SeriesNumber, _suffix);
			else
				return String.Format("{0}: {1} - {2}", SourceSeries.SeriesNumber, SourceSeries.SeriesDescription, _suffix);
		}

		protected override string GetDescription()
		{
			if (String.IsNullOrEmpty(SourceSeries.SeriesDescription))
				return _suffix;
			else
				return String.Format("{0} - {1}", SourceSeries.SeriesDescription, _suffix);
		}

		protected override string GetUid()
		{
			return String.Format("{0}:{1}:{2}:{3}", SourceSeries.SeriesInstanceUid, _seriesInstanceUid, _sopInstanceUid, _position);
		}
	}

	/// <summary>
	/// A <see cref="DisplaySetFactory"/> for the most typical cases; creating a <see cref="IDisplaySet"/> that
	/// contains <see cref="IPresentationImage"/>s for the entire series, and creating a single <see cref="IDisplaySet"/> for
	/// each image in the series.
	/// </summary>
	public class BasicDisplaySetFactory : DisplaySetFactory
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public BasicDisplaySetFactory()
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="presentationImageFactory">The <see cref="IPresentationImageFactory"/>
		/// used to create the <see cref="IPresentationImage"/>s that populate the constructed <see cref="IDisplaySet"/>s.</param>
		public BasicDisplaySetFactory(IPresentationImageFactory presentationImageFactory)
			: base(presentationImageFactory)
		{
		}

		public bool CreateSingleImageDisplaySets { get; set; }

		/// <summary>
		/// Creates <see cref="IDisplaySet"/>s from the given <see cref="Series"/>.
		/// </summary>
		public override List<IDisplaySet> CreateDisplaySets(Series series)
		{
			if (CreateSingleImageDisplaySets)
			{
				return DoCreateSingleImageDisplaySets(series);
			}
			else
			{
				List<IDisplaySet> displaySets = new List<IDisplaySet>();
				IDisplaySet displaySet = CreateSeriesDisplaySet(series);
				if (displaySet != null)
					displaySets.Add(displaySet);
				return displaySets;
			}
		}

		private IDisplaySet CreateSeriesDisplaySet(Series series)
		{
			IDisplaySet displaySet = null;
			List<IPresentationImage> images = new List<IPresentationImage>();
			foreach (Sop sop in series.Sops)
				images.AddRange(PresentationImageFactory.CreateImages(sop));

			if (images.Count > 0)
			{
				DisplaySetDescriptor descriptor = new SeriesDisplaySetDescriptor(series.GetIdentifier(), PresentationImageFactory);
				displaySet = new DisplaySet(descriptor);
				foreach (IPresentationImage image in images)
					displaySet.PresentationImages.Add(image);
			}

			return displaySet;
		}

		private List<IDisplaySet> DoCreateSingleImageDisplaySets(Series series)
		{
			List<IDisplaySet> displaySets = new List<IDisplaySet>();

			int position = 0;
			foreach (Sop sop in series.Sops)
			{
				List<IPresentationImage> images = PresentationImageFactory.CreateImages(sop);
				if (images.Count == 0)
					continue;

				if (series.Sops.Count == 1 && images.Count == 1)
				{
					DisplaySetDescriptor descriptor = new SeriesDisplaySetDescriptor(series.GetIdentifier(), PresentationImageFactory);
					DisplaySet displaySet = new DisplaySet(descriptor);
					displaySet.PresentationImages.Add(images[0]);
					displaySets.Add(displaySet);
				}
				else if (sop.IsImage)
				{
					ImageSop imageSop = (ImageSop)sop;
					DisplaySetDescriptor descriptor;

					if (imageSop.NumberOfFrames == 1)
						descriptor = new SingleImageDisplaySetDescriptor(series.GetIdentifier(), imageSop, position++);
					else
						descriptor = new MultiframeDisplaySetDescriptor(series.GetIdentifier(), sop.SopInstanceUid, sop.InstanceNumber);

					DisplaySet displaySet = new DisplaySet(descriptor);
					foreach (IPresentationImage image in images)
						displaySet.PresentationImages.Add(image);

					displaySets.Add(displaySet);
				}
				else
				{
					//The sop is actually a container for other referenced sops, like key images.
					foreach (IPresentationImage image in images)
					{
						DisplaySetDescriptor descriptor = null;
						if (image is IImageSopProvider)
						{
							IImageSopProvider provider = (IImageSopProvider) image;
							if (provider.ImageSop.NumberOfFrames == 1)
								descriptor = new SingleImageDisplaySetDescriptor(series.GetIdentifier(), provider.ImageSop, position++);
							else
								descriptor = new SingleFrameDisplaySetDescriptor(series.GetIdentifier(), provider.Frame, position++);
						}
						else
						{
							//TODO (CR Jan 2010): this because the design here is funny... the factory here should actually know something about the key object series it is building for
							ISeriesIdentifier sourceSeries = series.GetIdentifier();
							descriptor = new BasicDisplaySetDescriptor();
							descriptor.Description = sourceSeries.SeriesDescription;
							descriptor.Name = string.Format("{0}: {1}", sourceSeries.SeriesNumber, sourceSeries.SeriesDescription);
							descriptor.Number = sourceSeries.SeriesNumber.GetValueOrDefault(0);
							descriptor.Uid = sourceSeries.SeriesInstanceUid;
						}

						DisplaySet displaySet = new DisplaySet(descriptor);
						displaySet.PresentationImages.Add(image);
						displaySets.Add(displaySet);
					}
				}
			}

			return displaySets;
		}

		internal static IEnumerable<IDisplaySet> CreateSeriesDisplaySets(Series series, StudyTree studyTree)
		{
			BasicDisplaySetFactory factory = new BasicDisplaySetFactory();
			factory.SetStudyTree(studyTree);
			return factory.CreateDisplaySets(series);
		}
	}
	
	#endregion

	#region MR Echo

	[Cloneable(false)]
	internal class MREchoDisplaySetDescriptor : DicomDisplaySetDescriptor
	{
		private readonly int _echoNumber;
		private readonly string _suffix;

		public MREchoDisplaySetDescriptor(ISeriesIdentifier sourceSeries, int echoNumber, IPresentationImageFactory presentationImageFactory)
			: base(sourceSeries, presentationImageFactory)
		{
			Platform.CheckForNullReference(presentationImageFactory, "presentationImageFactory");
			_echoNumber = echoNumber;
			_suffix = String.Format(SR.SuffixFormatMREchoDisplaySet, echoNumber);
		}

		protected MREchoDisplaySetDescriptor(MREchoDisplaySetDescriptor source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		protected override string GetName()
		{
			if (String.IsNullOrEmpty(base.SourceSeries.SeriesDescription))
				return String.Format("{0}: {1}", SourceSeries.SeriesNumber, _suffix);
			else
				return String.Format("{0}: {1} - {2}", SourceSeries.SeriesNumber, SourceSeries.SeriesDescription, _suffix);
		
		}

		protected override string GetDescription()
		{
			if (String.IsNullOrEmpty(base.SourceSeries.SeriesDescription))
				return _suffix;
			else
				return String.Format("{0} - {1}", SourceSeries.SeriesDescription, _suffix);
		}

		protected override string GetUid()
		{
			return String.Format("{0}:Echo{1}", SourceSeries.SeriesInstanceUid, _echoNumber);
		}

		internal override bool ShouldAddSop(Sop sop)
		{
			if (sop.IsImage)
			{
				DicomAttribute echoAttribute = sop.DataSource[DicomTags.EchoNumbers];
				if (!echoAttribute.IsEmpty)
				{
					int echoNumber = echoAttribute.GetInt32(0, 0);
					return echoNumber == _echoNumber;
				}
			}

			return false;
		}
	}

	/// <summary>
	/// A <see cref="DisplaySetFactory"/> that splits MR series with multiple echoes into multiple <see cref="IDisplaySet"/>s; one per echo.
	/// </summary>
	public class MREchoDisplaySetFactory : DisplaySetFactory
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public MREchoDisplaySetFactory()
		{}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="presentationImageFactory">The <see cref="IPresentationImageFactory"/>
		/// used to create the <see cref="IPresentationImage"/>s that populate the constructed <see cref="IDisplaySet"/>s.</param>
		public MREchoDisplaySetFactory(IPresentationImageFactory presentationImageFactory)
			: base(presentationImageFactory)
		{ }

		/// <summary>
		/// Creates zero or more <see cref="IDisplaySet"/>s from the given <see cref="Series"/>.
		/// </summary>
		/// <remarks>
		/// When the input <see cref="Series"/> does not have multiple echoes, no <see cref="IDisplaySet"/>s will be returned.
		/// Otherwise, at least 2 <see cref="IDisplaySet"/>s will be returned.
		/// </remarks>
		public override List<IDisplaySet> CreateDisplaySets(Series series)
		{
			List<IDisplaySet> displaySets = new List<IDisplaySet>();

			if (series.Modality == "MR")
			{
				SortedDictionary<int, List<Sop>> imagesByEchoNumber = SplitMREchos(series.Sops);
				if (imagesByEchoNumber.Count > 1)
				{
					foreach (KeyValuePair<int, List<Sop>> echoImages in imagesByEchoNumber)
					{
						List<IPresentationImage> images = new List<IPresentationImage>();
						foreach (ImageSop sop in echoImages.Value)
							images.AddRange(PresentationImageFactory.CreateImages(sop));

						if (images.Count > 0)
						{
							IDisplaySet displaySet = new DisplaySet(new MREchoDisplaySetDescriptor(series.GetIdentifier(), echoImages.Key, PresentationImageFactory));
							foreach (IPresentationImage image in images)
								displaySet.PresentationImages.Add(image);

							displaySets.Add(displaySet);
						}
					}
				}
			}

			return displaySets;
		}

		private static SortedDictionary<int, List<Sop>> SplitMREchos(IEnumerable<Sop> sops)
		{
			SortedDictionary<int, List<Sop>> imagesByEchoNumber = new SortedDictionary<int, List<Sop>>();

			foreach (Sop sop in sops)
			{
				if (sop.IsImage)
				{
					DicomAttribute echoAttribute = sop.DataSource[DicomTags.EchoNumbers];
					if (!echoAttribute.IsEmpty)
					{
						int echoNumber = echoAttribute.GetInt32(0, 0);
						if (!imagesByEchoNumber.ContainsKey(echoNumber))
							imagesByEchoNumber[echoNumber] = new List<Sop>();

						imagesByEchoNumber[echoNumber].Add(sop);
					}
				}
			}

			return imagesByEchoNumber;
		}
	}

	#endregion

	#region Mixed Multi-frame

	[Cloneable(false)]
	internal class MultiframeDisplaySetDescriptor : DicomDisplaySetDescriptor
	{
		private readonly string _sopInstanceUid;
		private readonly string _suffix;

		public MultiframeDisplaySetDescriptor(ISeriesIdentifier sourceSeries, string sopInstanceUid, int instanceNumber)
			: base(sourceSeries)
		{
			_sopInstanceUid = sopInstanceUid;
			_suffix = String.Format(SR.SuffixFormatMultiframeDisplaySet, instanceNumber);
		}

		protected MultiframeDisplaySetDescriptor(MultiframeDisplaySetDescriptor source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		protected override string GetName()
		{
			if (String.IsNullOrEmpty(base.SourceSeries.SeriesDescription))
				return String.Format("{0}: {1}", SourceSeries.SeriesNumber, _suffix);
			else
				return String.Format("{0}: {1} - {2}", SourceSeries.SeriesNumber, SourceSeries.SeriesDescription, _suffix);
		}
		
		protected override string GetDescription()
		{
			if (String.IsNullOrEmpty(base.SourceSeries.SeriesDescription))
				return _suffix;
			else
				return String.Format("{0} - {1}", SourceSeries.SeriesDescription, _suffix);
		}
		
		protected override string GetUid()
		{
			return _sopInstanceUid;
		}
	}

	[Cloneable(false)]
	internal class SingleImagesDisplaySetDescriptor : DicomDisplaySetDescriptor
	{
		private readonly string _suffix;

		public SingleImagesDisplaySetDescriptor(ISeriesIdentifier sourceSeries, IPresentationImageFactory presentationImageFactory)
			: base(sourceSeries, presentationImageFactory)
		{
			_suffix = SR.SuffixSingleImagesDisplaySet;
		}

		protected SingleImagesDisplaySetDescriptor(SingleImagesDisplaySetDescriptor source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		protected override string GetName()
		{
			if (String.IsNullOrEmpty(base.SourceSeries.SeriesDescription))
				return String.Format("{0}: {1}", SourceSeries.SeriesNumber, _suffix);
			else
				return String.Format("{0}: {1} - {2}", SourceSeries.SeriesNumber, SourceSeries.SeriesDescription, _suffix);
		}
	
		protected override string GetDescription()
		{
			if (String.IsNullOrEmpty(base.SourceSeries.SeriesDescription))
				return _suffix;
			else
				return String.Format("{0} - {1}", SourceSeries.SeriesDescription, _suffix);
		}

		protected override string GetUid()
		{
			return String.Format("{0}:SingleImages", SourceSeries.SeriesInstanceUid);
		}

		internal override bool ShouldAddSop(Sop sop)
		{
			return sop.SeriesInstanceUid == SourceSeries.SeriesInstanceUid && sop.IsImage && ((ImageSop)sop).NumberOfFrames == 1;
		}
	}

	/// <summary>
	/// A <see cref="DisplaySetFactory"/> that splits series with multiple single or multiframe images into
	/// separate <see cref="IDisplaySet"/>s.
	/// </summary>
	/// <remarks>
	/// This factory will only create <see cref="IDisplaySet"/>s when the following is true.
	/// <list type="bullet">
	/// <item>The input series contains more than one multiframe image.</item>
	/// <item>The input series contains at least one multiframe image and at least one single frame image.</item>
	/// </list>
	/// For typical series, consisting only of single frame images, no <see cref="IDisplaySet"/>s will be created.
	/// The <see cref="IDisplaySet"/>s that are created are:
	/// <list type="bullet">
	/// <item>One <see cref="IDisplaySet"/> per multiframe image.</item>
	/// <item>One <see cref="IDisplaySet"/> containing all the single frame images, if any.</item>
	/// </list>
	/// </remarks>
	public class MixedMultiFrameDisplaySetFactory : DisplaySetFactory
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public MixedMultiFrameDisplaySetFactory()
		{}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="presentationImageFactory">The <see cref="IPresentationImageFactory"/>
		/// used to create the <see cref="IPresentationImage"/>s that populate the constructed <see cref="IDisplaySet"/>s.</param>
		public MixedMultiFrameDisplaySetFactory(IPresentationImageFactory presentationImageFactory)
			: base(presentationImageFactory)
		{ }

		/// <summary>
		/// Creates zero or more <see cref="IDisplaySet"/>s from the given <see cref="Series"/>.
		/// </summary>
		/// <remarks>When the input series does not contain a mixture of single and multiframe
		/// images, no <see cref="IDisplaySet"/>s will be returned.</remarks>
		public override List<IDisplaySet> CreateDisplaySets(Series series)
		{
			List<IDisplaySet> displaySets = new List<IDisplaySet>();

			List<ImageSop> singleFrames = new List<ImageSop>();
			List<ImageSop> multiFrames = new List<ImageSop>();

			foreach (Sop sop in series.Sops)
			{
				if (sop.IsImage)
				{
					ImageSop imageSop = (ImageSop)sop;
					if (imageSop.NumberOfFrames > 1)
						multiFrames.Add(imageSop);
					else
						singleFrames.Add(imageSop);
				}
			}

			if (multiFrames.Count > 1 || (singleFrames.Count > 0 && multiFrames.Count > 0))
			{
				if (singleFrames.Count > 0)
				{
					List<IPresentationImage> singleFrameImages = new List<IPresentationImage>();
					foreach (ImageSop singleFrame in singleFrames)
						singleFrameImages.AddRange(PresentationImageFactory.CreateImages(singleFrame));

					if (singleFrameImages.Count > 0)
					{
						SingleImagesDisplaySetDescriptor descriptor =
							new SingleImagesDisplaySetDescriptor(series.GetIdentifier(), PresentationImageFactory);
						DisplaySet singleImagesDisplaySet = new DisplaySet(descriptor);

						foreach (IPresentationImage singleFrameImage in singleFrameImages)
							singleImagesDisplaySet.PresentationImages.Add(singleFrameImage);

						displaySets.Add(singleImagesDisplaySet);
					}
				}

				foreach (ImageSop multiFrame in multiFrames)
				{
					List<IPresentationImage> multiFrameImages = PresentationImageFactory.CreateImages(multiFrame);
					if (multiFrameImages.Count > 0)
					{
						MultiframeDisplaySetDescriptor descriptor =
							new MultiframeDisplaySetDescriptor(multiFrame.ParentSeries.GetIdentifier(), multiFrame.SopInstanceUid, multiFrame.InstanceNumber);
						DisplaySet displaySet = new DisplaySet(descriptor);

						foreach (IPresentationImage multiFrameImage in multiFrameImages)
							displaySet.PresentationImages.Add(multiFrameImage);

						displaySets.Add(displaySet);
					}
				}
			}

			return displaySets;
		}
	}
	
	#endregion
}