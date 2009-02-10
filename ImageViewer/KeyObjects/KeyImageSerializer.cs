using System;
using System.Collections.Generic;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Iod.ContextGroups;
using ClearCanvas.Dicom.Iod.Iods;
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.Dicom.Iod.Macros.DocumentRelationship;
using ClearCanvas.Dicom.Iod.Macros.HierarchicalSeriesInstanceReference;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.ImageViewer.PresentationStates;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.KeyObjects
{
	public class KeyImageSerializer
	{
		private readonly List<KeyValuePair<Frame, DicomSoftcopyPresentationState>> _frames;
		//private readonly List<string> _docTitleMods;
		private DateTime _datetime;
		private string _description;
		private string _seriesDescription;
		private KeyObjectSelectionDocumentTitle _docTitle = KeyObjectSelectionDocumentTitleContextGroup.OfInterest;

		public KeyImageSerializer()
		{
			_frames = new List<KeyValuePair<Frame, DicomSoftcopyPresentationState>>();
			_datetime = Platform.Time;
		}

		public IList<KeyValuePair<Frame, DicomSoftcopyPresentationState>> Frames
		{
			get { return _frames; }
		}

		//public IList<string> DocumentTitleModifiers
		//{
		//    get { return _docTitleMods; }
		//}

		public DateTime DateTime
		{
			get { return _datetime; }
			set { _datetime = value; }
		}

		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		public string SeriesDescription
		{
			get { return _seriesDescription; }
			set { _seriesDescription = value; }
		}

		public KeyObjectSelectionDocumentTitle DocumentTitle
		{
			get { return _docTitle; }
			set { _docTitle = value; }
		}

		public List<DicomFile> Serialize()
		{
			if (_frames.Count == 0)
				throw new InvalidOperationException("Key object selection cannot be empty.");

			List<DicomFile> dicomFiles = new List<DicomFile>();
			List<IHierarchicalSopInstanceReferenceMacro> identicalDocuments = new List<IHierarchicalSopInstanceReferenceMacro>();
			Dictionary<string, KeyObjectSelectionDocumentIod> koDocumentsByStudy = new Dictionary<string, KeyObjectSelectionDocumentIod>();
			foreach (KeyValuePair<Frame, DicomSoftcopyPresentationState> pair in _frames)
			{
				Frame frame = pair.Key;
				DicomSoftcopyPresentationState presentationState = pair.Value;

				string studyInstanceUid = frame.StudyInstanceUID;
				if (!koDocumentsByStudy.ContainsKey(studyInstanceUid))
				{
					DicomFile dcf = new DicomFile();
					KeyObjectSelectionDocumentIod iod = CreatePrototypeDocument(frame.ParentImageSop.DataSource, dcf.DataSet);

					iod.KeyObjectDocumentSeries.InitializeAttributes();
					iod.KeyObjectDocumentSeries.Modality = Modality.KO;
					iod.KeyObjectDocumentSeries.SeriesDateTime = _datetime;
					iod.KeyObjectDocumentSeries.SeriesDescription = _seriesDescription;
					iod.KeyObjectDocumentSeries.SeriesInstanceUid = DicomUid.GenerateUid().UID;
					iod.KeyObjectDocumentSeries.SeriesNumber = CalculateSeriesNumber(frame);
					iod.KeyObjectDocumentSeries.ReferencedPerformedProcedureStepSequence = null;
					iod.SopCommon.SopClass = SopClass.KeyObjectSelectionDocumentStorage;
					iod.SopCommon.SopInstanceUid = DicomUid.GenerateUid().UID;

					identicalDocuments.Add(iod.KeyObjectDocument.CreateIdenticalDocumentsSequence(
						studyInstanceUid,
						iod.KeyObjectDocumentSeries.SeriesInstanceUid,
						iod.SopCommon.SopClassUid,
						iod.SopCommon.SopInstanceUid));

					koDocumentsByStudy.Add(studyInstanceUid, iod);
					dicomFiles.Add(dcf);
				}
			}

			foreach (KeyObjectSelectionDocumentIod iod in koDocumentsByStudy.Values)
			{
				iod.KeyObjectDocument.InitializeAttributes();
				iod.KeyObjectDocument.InstanceNumber = 1;
				iod.KeyObjectDocument.ContentDateTime = _datetime;
				iod.KeyObjectDocument.ReferencedRequestSequence = null;

				iod.KeyObjectDocument.IdenticalDocumentsSequence = identicalDocuments.ToArray();

				iod.SrDocumentContent.InitializeContainerAttributes();
				iod.SrDocumentContent.ConceptNameCodeSequence = _docTitle;

				List<IContentSequence> contentList = new List<IContentSequence>();
				List<IHierarchicalSopInstanceReferenceMacro> currentRequestedProcedureEvidenceList = new List<IHierarchicalSopInstanceReferenceMacro>();

				Dictionary<ImageSop, List<int>> frameMap = new Dictionary<ImageSop, List<int>>();
				foreach (KeyValuePair<Frame,DicomSoftcopyPresentationState> framePRPair in _frames)
				{
					Frame frame = framePRPair.Key;
					ImageSop sop = frame.ParentImageSop;

					// build frame map by unique sop - used to make the evidence sequence less verbose
					if (!frameMap.ContainsKey(frame.ParentImageSop))
						frameMap.Add(frame.ParentImageSop, new List<int>());
					List<int> frames = frameMap[frame.ParentImageSop];
					if (!frames.Contains(frame.FrameNumber))
						frames.Add(frame.FrameNumber);

					// content sequence must still list all content as it was given, including any repeats
					IContentSequence content = iod.SrDocumentContent.CreateContentSequence();
					{
						content.RelationshipType = RelationshipType.Contains;
						content.ReferencedContentItemIdentifier = new uint[] { 1 };

						IImageReferenceMacro imgMac = content.InitializeImageReferenceAttributes();
						imgMac.ReferencedSopSequence.InitializeAttributes();
						imgMac.ReferencedSopSequence.ReferencedSopClassUid = sop.SopClassUID;
						imgMac.ReferencedSopSequence.ReferencedSopInstanceUid = sop.SopInstanceUID;
						if (sop.NumberOfFrames > 1)
							imgMac.ReferencedSopSequence.ReferencedFrameNumber = frame.FrameNumber.ToString();
						else
							imgMac.ReferencedSopSequence.ReferencedFrameNumber = null;

						// save the presentation state
						if(framePRPair.Value!=null)
						{
							DicomSoftcopyPresentationState presentationState = framePRPair.Value;
							imgMac.ReferencedSopSequence.CreateReferencedSopSequence();
							imgMac.ReferencedSopSequence.ReferencedSopSequence.InitializeAttributes();
							imgMac.ReferencedSopSequence.ReferencedSopSequence.ReferencedSopClassUid = presentationState.PresentationSopClass.Uid;
							imgMac.ReferencedSopSequence.ReferencedSopSequence.ReferencedSopInstanceUid = presentationState.PresentationInstanceUid;
						}
					}
					contentList.Add(content);
				}

				// add the description
				if (!string.IsNullOrEmpty(_description))
				{
					IContentSequence koDescription = iod.SrDocumentContent.CreateContentSequence();
					koDescription.InitializeAttributes();
					koDescription.ConceptNameCodeSequence = KeyObjectSelectionCodeSequences.DocumentTitleModifier;
					koDescription.TextValue = _description;
					koDescription.RelationshipType = RelationshipType.Contains;
					koDescription.ReferencedContentItemIdentifier = new uint[] {1};
					contentList.Add(koDescription);
				}

				// create evidence sequence using the map built earlier
				foreach (ImageSop sop in frameMap.Keys)
				{
					IHierarchicalSopInstanceReferenceMacro currentRequestedProcedureEvidence = iod.KeyObjectDocument.CreateCurrentRequestedProcedureEvidenceSequence();
					{
						currentRequestedProcedureEvidence.StudyInstanceUid = sop.StudyInstanceUID;

						IHierarchicalSeriesInstanceReferenceMacro referencedSeries = currentRequestedProcedureEvidence.CreateReferencedSeriesSequence();
						{
							referencedSeries.InitializeAttributes();
							referencedSeries.SeriesInstanceUid = sop.SeriesInstanceUID;
							referencedSeries.RetrieveAeTitle = sop[DicomTags.RetrieveAeTitle].ToString();
							referencedSeries.StorageMediaFileSetId = sop[DicomTags.StorageMediaFileSetId].GetString(0, string.Empty);
							referencedSeries.StorageMediaFileSetUid = sop[DicomTags.StorageMediaFileSetUid].GetString(0, string.Empty);

							IReferencedSopSequence referencedSop = referencedSeries.CreateReferencedSopSequence();
							{
								referencedSop.InitializeAttributes();
								referencedSop.ReferencedSopClassUid = sop.SopClassUID;
								referencedSop.ReferencedSopInstanceUid = sop.SopInstanceUID;
							}
							referencedSeries.ReferencedSopSequence = new IReferencedSopSequence[] {referencedSop};
						}
						currentRequestedProcedureEvidence.ReferencedSeriesSequence = new IHierarchicalSeriesInstanceReferenceMacro[] {referencedSeries};
					}
					currentRequestedProcedureEvidenceList.Add(currentRequestedProcedureEvidence);
				}

				// set the content and the evidence sequences
				iod.SrDocumentContent.ContentSequence = contentList.ToArray();
				iod.KeyObjectDocument.CurrentRequestedProcedureEvidenceSequence = currentRequestedProcedureEvidenceList.ToArray();
			}

			// set meta for the files
			foreach (DicomFile dcf in dicomFiles) {
				dcf.MediaStorageSopClassUid = dcf.DataSet[DicomTags.SopClassUid].ToString();
				dcf.MediaStorageSopInstanceUid = dcf.DataSet[DicomTags.SopInstanceUid].ToString();
			}

			return dicomFiles;
		}

		private int CalculateSeriesNumber(Frame frame)
		{
			if (frame.ParentImageSop == null || frame.ParentImageSop.ParentSeries == null || frame.ParentImageSop.ParentSeries.ParentStudy == null)
				return 1;

			int maxValue = 0;
			foreach (Series series in frame.ParentImageSop.ParentSeries.ParentStudy.Series)
			{
				if (series.SeriesNumber > maxValue)
					maxValue = series.SeriesNumber;
			}

			return maxValue + 1;
		}

		private static KeyObjectSelectionDocumentIod CreatePrototypeDocument(IDicomAttributeProvider source, IDicomAttributeProvider target)
		{
			KeyObjectSelectionDocumentIod iod = new KeyObjectSelectionDocumentIod(target);

			PatientModuleIod srcPatient = new PatientModuleIod(source);
			if (true) // patient module is always required
			{
				iod.Patient.BreedRegistrationSequence = srcPatient.BreedRegistrationSequence;
				iod.Patient.DeIdentificationMethod = srcPatient.DeIdentificationMethod;
				iod.Patient.DeIdentificationMethodCodeSequence = srcPatient.DeIdentificationMethodCodeSequence;
				iod.Patient.EthnicGroup = srcPatient.EthnicGroup;
				iod.Patient.IssuerOfPatientId = srcPatient.IssuerOfPatientId;
				iod.Patient.OtherPatientIds = srcPatient.OtherPatientIds;
				iod.Patient.OtherPatientIdsSequence = srcPatient.OtherPatientIdsSequence;
				iod.Patient.OtherPatientNames = srcPatient.OtherPatientNames;
				iod.Patient.PatientBreedCodeSequence = srcPatient.PatientBreedCodeSequence;
				iod.Patient.PatientBreedDescription = srcPatient.PatientBreedDescription;
				iod.Patient.PatientComments = srcPatient.PatientComments;
				iod.Patient.PatientId = srcPatient.PatientId;
				iod.Patient.PatientIdentityRemoved = srcPatient.PatientIdentityRemoved;
				iod.Patient.PatientsBirthDateTime = srcPatient.PatientsBirthDateTime;
				iod.Patient.PatientsName = srcPatient.PatientsName;
				iod.Patient.PatientSpeciesCodeSequence = srcPatient.PatientSpeciesCodeSequence;
				iod.Patient.PatientSpeciesDescription = srcPatient.PatientSpeciesDescription;
				iod.Patient.PatientsSex = srcPatient.PatientsSex;
				iod.Patient.ReferencedPatientSequence = srcPatient.ReferencedPatientSequence;
				iod.Patient.ResponsibleOrganization = srcPatient.ResponsibleOrganization;
				iod.Patient.ResponsiblePerson = srcPatient.ResponsiblePerson;
				iod.Patient.ResponsiblePersonRole = srcPatient.ResponsiblePersonRole;
			}

			SpecimenIdentificationModuleIod srcSpecimen = new SpecimenIdentificationModuleIod(source);
			if (srcSpecimen.HasValues()) // specimen module is required only if subject is a specimen
			{
				iod.SpecimenIdentification.SpecimenAccessionNumber = srcSpecimen.SpecimenAccessionNumber;
				iod.SpecimenIdentification.SpecimenSequence = srcSpecimen.SpecimenSequence;
			}

			ClinicalTrialSubjectModuleIod srcTrialSubject = new ClinicalTrialSubjectModuleIod(source);
			if (srcTrialSubject.HasValues()) // clinical trial subkect module is user optional
			{
				iod.ClinicalTrialSubject.ClinicalTrialProtocolId = srcTrialSubject.ClinicalTrialProtocolId;
				iod.ClinicalTrialSubject.ClinicalTrialProtocolName = srcTrialSubject.ClinicalTrialProtocolName;
				iod.ClinicalTrialSubject.ClinicalTrialSiteId = srcTrialSubject.ClinicalTrialSiteId;
				iod.ClinicalTrialSubject.ClinicalTrialSiteName = srcTrialSubject.ClinicalTrialSiteName;
				iod.ClinicalTrialSubject.ClinicalTrialSponsorName = srcTrialSubject.ClinicalTrialSponsorName;
				iod.ClinicalTrialSubject.ClinicalTrialSubjectId = srcTrialSubject.ClinicalTrialSubjectId;
				iod.ClinicalTrialSubject.ClinicalTrialSubjectReadingId = srcTrialSubject.ClinicalTrialSubjectReadingId;
			}

			GeneralStudyModuleIod srcGeneralStudy = new GeneralStudyModuleIod(source);
			if (true) // general study module is always required
			{
				iod.GeneralStudy.AccessionNumber = srcGeneralStudy.AccessionNumber;
				iod.GeneralStudy.NameOfPhysiciansReadingStudy = srcGeneralStudy.NameOfPhysiciansReadingStudy;
				iod.GeneralStudy.PhysiciansOfRecord = srcGeneralStudy.PhysiciansOfRecord;
				iod.GeneralStudy.PhysiciansOfRecordIdentificationSequence = srcGeneralStudy.PhysiciansOfRecordIdentificationSequence;
				iod.GeneralStudy.PhysiciansReadingStudyIdentificationSequence = srcGeneralStudy.PhysiciansReadingStudyIdentificationSequence;
				iod.GeneralStudy.ProcedureCodeSequence = srcGeneralStudy.ProcedureCodeSequence;
				iod.GeneralStudy.ReferencedStudySequence = srcGeneralStudy.ReferencedStudySequence;
				iod.GeneralStudy.ReferringPhysicianIdentificationSequence = srcGeneralStudy.ReferringPhysicianIdentificationSequence;
				iod.GeneralStudy.ReferringPhysiciansName = srcGeneralStudy.ReferringPhysiciansName;
				iod.GeneralStudy.StudyDateTime = srcGeneralStudy.StudyDateTime;
				iod.GeneralStudy.StudyDescription = srcGeneralStudy.StudyDescription;
				iod.GeneralStudy.StudyId = srcGeneralStudy.StudyId;
				iod.GeneralStudy.StudyInstanceUid = srcGeneralStudy.StudyInstanceUid;
			}

			PatientStudyModuleIod srcPatientStudy = new PatientStudyModuleIod(source);
			if (srcPatientStudy.HasValues()) // patient study module is user optional
			{
				iod.PatientStudy.AdditionalPatientHistory = srcPatientStudy.AdditionalPatientHistory;
				iod.PatientStudy.AdmissionId = srcPatientStudy.AdmissionId;
				iod.PatientStudy.AdmittingDiagnosesCodeSequence = srcPatientStudy.AdmittingDiagnosesCodeSequence;
				iod.PatientStudy.AdmittingDiagnosesDescription = srcPatientStudy.AdmittingDiagnosesDescription;
				iod.PatientStudy.IssuerOfAdmissionId = srcPatientStudy.IssuerOfAdmissionId;
				iod.PatientStudy.IssuerOfServiceEpisodeId = srcPatientStudy.IssuerOfServiceEpisodeId;
				iod.PatientStudy.Occupation = srcPatientStudy.Occupation;
				iod.PatientStudy.PatientsAge = srcPatientStudy.PatientsAge;
				iod.PatientStudy.PatientsSexNeutered = srcPatientStudy.PatientsSexNeutered;
				iod.PatientStudy.PatientsSize = srcPatientStudy.PatientsSize;
				iod.PatientStudy.PatientsWeight = srcPatientStudy.PatientsWeight;
				iod.PatientStudy.ServiceEpisodeDescription = srcPatientStudy.ServiceEpisodeDescription;
				iod.PatientStudy.ServiceEpisodeId = srcPatientStudy.ServiceEpisodeId;
			}

			ClinicalTrialStudyModuleIod srcTrialStudy = new ClinicalTrialStudyModuleIod(source);
			if (srcTrialStudy.HasValues()) // clinical trial study module is user optional
			{
				iod.ClinicalTrialStudy.ClinicalTrialTimePointDescription = srcTrialStudy.ClinicalTrialTimePointDescription;
				iod.ClinicalTrialStudy.ClinicalTrialTimePointId = srcTrialStudy.ClinicalTrialTimePointId;
			}

			ClinicalTrialSeriesModuleIod srcTrialSeries = new ClinicalTrialSeriesModuleIod(source);
			if (srcTrialSeries.HasValues()) // clinical trial series module is user optional
			{
				iod.ClinicalTrialSeries.ClinicalTrialCoordinatingCenterName = srcTrialSeries.ClinicalTrialCoordinatingCenterName;
				iod.ClinicalTrialSeries.ClinicalTrialSeriesDescription = srcTrialSeries.ClinicalTrialSeriesDescription;
				iod.ClinicalTrialSeries.ClinicalTrialSeriesId = srcTrialSeries.ClinicalTrialSeriesId;
			}

			GeneralEquipmentModuleIod srcGeneralEquipment = new GeneralEquipmentModuleIod(source);
			if (true) // general equipment module is always required
			{
				iod.GeneralEquipment.DateTimeOfLastCalibrationDateTime = srcGeneralEquipment.DateTimeOfLastCalibrationDateTime;
				iod.GeneralEquipment.DeviceSerialNumber = srcGeneralEquipment.DeviceSerialNumber;
				iod.GeneralEquipment.GantryId = srcGeneralEquipment.GantryId;
				iod.GeneralEquipment.InstitutionAddress = srcGeneralEquipment.InstitutionAddress;
				iod.GeneralEquipment.InstitutionalDepartmentName = srcGeneralEquipment.InstitutionalDepartmentName;
				iod.GeneralEquipment.InstitutionName = srcGeneralEquipment.InstitutionName;
				iod.GeneralEquipment.Manufacturer = srcGeneralEquipment.Manufacturer;
				iod.GeneralEquipment.ManufacturersModelName = srcGeneralEquipment.ManufacturersModelName;
				iod.GeneralEquipment.PixelPaddingValue = srcGeneralEquipment.PixelPaddingValue;
				iod.GeneralEquipment.SoftwareVersions = srcGeneralEquipment.SoftwareVersions;
				iod.GeneralEquipment.SpatialResolution = srcGeneralEquipment.SpatialResolution;
				iod.GeneralEquipment.StationName = srcGeneralEquipment.StationName;
			}

			return iod;
		}
	}
}