namespace ClearCanvas.Dicom.Network
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using ClearCanvas.Dicom.Exceptions;
    using ClearCanvas.Dicom.OffisWrapper;

    internal class OffisConditionParser
    {
        internal static string GetTextString(ApplicationEntity ae, DicomRuntimeApplicationException e)
        {
            UInt16 module = e.Module; // module in the OFFIS library 0x06 is dcmnet
            UInt16 code = e.Code;
            UInt16 submodule = (UInt16)(code & 0x0f00);
            UInt16 subcode = (UInt16)(code & 0x00ff);

            if (0x06 != module)
                throw new System.ArgumentOutOfRangeException("Encountered a non-dcmnet module error condition in the network error condition parser");

            StringBuilder exceptionMessage = new StringBuilder(256);

            string submoduleName = null;
            string errorText = null;

            switch (submodule)
            {
                case 0x0100:
                    submoduleName = SR.ExceptionOffisModuleAssociation;

                    switch (subcode)
                    {
                        case 0x01:
                            errorText = SR.ExceptionOffisAssociationBadPresentationContextID;
                            break;
                        case 0x05:
                            errorText = SR.ExceptionOffisAssociationMissingTransferSyntax;
                            break;
                        case 0x06:
                            errorText = SR.ExceptionOffisAssociationNullKey;
                            break;
                        case 0x07:
                            errorText = SR.ExceptionOffisAssociationShutdownApplication;
                            break;
                        default:
                            throw new System.Exception(SR.ExceptionOffisUnknownError);
                    }
                    break;
                case 0x0200:
                    submoduleName = SR.ExceptionOffisModuleDimse;

                    switch (subcode)
                    {
                        case 0x01:
                            errorText = SR.ExceptionOffisDimseBadCommandType;
                            break;
                        case 0x02:
                            errorText = SR.ExceptionOffisDimseBadData;
                            break;
                        case 0x03:
                            errorText = SR.ExceptionOffisDimseBadMessage;
                            break;
                        case 0x05:
                            errorText = SR.ExceptionOffisDimseIllegalAssociation;
                            break;
                        case 0x07:
                            errorText = SR.ExceptionOffisDimseNoDataAvailable;
                            break;
                        case 0x08:
                            errorText = SR.ExceptionOffisDimseNoValidPresentationContextID;
                            break;
                        case 0x09:
                            errorText = SR.ExceptionOffisDimseNullKey;
                            break;
                        case 0x0a:
                            errorText = SR.ExceptionOffisDimseOutOfResources;
                            break;
                        case 0x0b:
                            errorText = SR.ExceptionOffisDimseParseFailed;
                            break;
                        case 0x0c:
                            errorText = SR.ExceptionOffisDimseReadPDVFailed;
                            break;
                        case 0x0d:
                            errorText = SR.ExceptionOffisDimseReceiveFailed;
                            break;
                        case 0x0e:
                            errorText = SR.ExceptionOffisDimseSendFailed;
                            break;
                        case 0x0f:
                            errorText = SR.ExceptionOffisDimseUnexpectedPDVType;
                            break;
                        case 0x13:
                            errorText = SR.ExceptionOffisDimseNoDataDictionary;
                            break;
                        default:
                            throw new System.Exception(SR.ExceptionOffisUnknownError);
                    }

                    break;
                case 0x0300:
                    submoduleName = SR.ExceptionOffisModuleDul;

                    switch (subcode)
                    {
                        case 0x01:
                            errorText = SR.ExceptionOffisDulAssociationRejected;
                            break;
                        case 0x04:
                            errorText = SR.ExceptionOffisDulIllegalAccept;
                            break;
                        case 0x05:
                            errorText = SR.ExceptionOffisDulIllegalKey;
                            break;
                        case 0x07:
                            errorText = SR.ExceptionOffisDulIllegalPdu;
                            break;
                        case 0x08:
                            errorText = SR.ExceptionOffisDulIllegalPduLength;
                            break;
                        case 0x0b:
                            errorText = SR.ExceptionOffisDulIllegalRequest;
                            break;
                        case 0x0d:
                            errorText = SR.ExceptionOffisDulIncorrectBufferLength;
                            break;
                        case 0xe:
                            errorText = SR.ExceptionOffisDulInsufficientBufferLength;
                            break;
                        case 0x0f:
                            errorText = SR.ExceptionOffisDulListError;
                            break;
                        case 0x10:
                            errorText = SR.ExceptionOffisDulNetworkClosed;
                            break;
                        case 0x12:
                            errorText = SR.ExceptionOffisDulNoAssociationRequest;
                            break;
                        case 0x13:
                            errorText = SR.ExceptionOffisDulNoPdvs;
                            break;
                        case 0x14:
                            errorText = SR.ExceptionOffisDulNullKey;
                            break;
                        case 0x15:
                            errorText = SR.ExceptionOffisDulPCTranslationFailure;
                            break;
                        case 0x16:
                            errorText = SR.ExceptionOffisDulPDataPduArrived;
                            break;
                        case 0x17:
                            errorText = SR.ExceptionOffisDulPeerAbortedAssociation;
                            break;
                        case 0x19:
                            errorText = SR.ExceptionOffisDulPeerRequestedRelease;
                            break;
                        case 0x1a:
                            errorText = SR.ExceptionOffisDulReadTimeout;
                            break;
                        case 0x1b:
                            errorText = SR.ExceptionOffisDulRequestAssociationFailed;
                            break;
                        case 0x1f:
                            errorText = SR.ExceptionOffisDulUnexpectedPdu;
                            break;
                        case 0x22:
                            errorText = SR.ExceptionOffisDulUnsupportedPeerProtocol;
                            break;
                        case 0x23:
                            errorText = SR.ExceptionOffisDulWrongDataType;
                            break;
                        default:
                            throw new System.Exception(SR.ExceptionOffisUnknownError);
                    }

                    break;
                default:
                    throw new System.ArgumentOutOfRangeException(SR.ExceptionDecodingBadModuleNumber);
            }

            exceptionMessage.AppendFormat(SR.ExceptionOffisFormatNetwork, submoduleName, errorText, ae.AE, ae.Host + ":" + ae.Port, e.Code);
            return exceptionMessage.ToString();
        }
    }
}
