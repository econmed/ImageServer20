/* ----------------------------------------------------------------------------
 * This file was automatically generated by SWIG (http://www.swig.org).
 * Version 1.3.25
 *
 * Do not make changes to this file unless you know what you are doing--modify
 * the SWIG interface file instead.
 * ----------------------------------------------------------------------------- */

namespace ClearCanvas.Common.Dicom {

using System;
using System.Text;
using System.Runtime.InteropServices;

public class DcmTime : DcmByteString {
  private HandleRef swigCPtr;

  internal DcmTime(IntPtr cPtr, bool cMemoryOwn) : base(DCMTKPINVOKE.DcmTimeUpcast(cPtr), cMemoryOwn) {
    swigCPtr = new HandleRef(this, cPtr);
  }

  internal static HandleRef getCPtr(DcmTime obj) {
    return (obj == null) ? new HandleRef(null, IntPtr.Zero) : obj.swigCPtr;
  }

  ~DcmTime() {
    Dispose();
  }

  public override void Dispose() {
    if(swigCPtr.Handle != IntPtr.Zero && swigCMemOwn) {
      swigCMemOwn = false;
      DCMTKPINVOKE.delete_DcmTime(swigCPtr);
    }
    swigCPtr = new HandleRef(null, IntPtr.Zero);
    GC.SuppressFinalize(this);
    base.Dispose();
  }

  public DcmTime(DcmTag tag, uint len) : this(DCMTKPINVOKE.new_DcmTime__SWIG_0(DcmTag.getCPtr(tag), len), true) {
    if (DCMTKPINVOKE.SWIGPendingException.Pending) throw DCMTKPINVOKE.SWIGPendingException.Retrieve();
  }

  public DcmTime(DcmTag tag) : this(DCMTKPINVOKE.new_DcmTime__SWIG_1(DcmTag.getCPtr(tag)), true) {
    if (DCMTKPINVOKE.SWIGPendingException.Pending) throw DCMTKPINVOKE.SWIGPendingException.Retrieve();
  }

  public DcmTime(DcmTime old) : this(DCMTKPINVOKE.new_DcmTime__SWIG_2(DcmTime.getCPtr(old)), true) {
    if (DCMTKPINVOKE.SWIGPendingException.Pending) throw DCMTKPINVOKE.SWIGPendingException.Retrieve();
  }

  public override DcmEVR ident() {
    DcmEVR ret = (DcmEVR)DCMTKPINVOKE.DcmTime_ident(swigCPtr);
    return ret;
  }

  public override OFCondition getOFString(StringBuilder stringValue, uint pos, bool normalize) {
    OFCondition ret = new OFCondition(DCMTKPINVOKE.DcmTime_getOFString__SWIG_0(swigCPtr, stringValue, pos, normalize), true);
    return ret;
  }

  public override OFCondition getOFString(StringBuilder stringValue, uint pos) {
    OFCondition ret = new OFCondition(DCMTKPINVOKE.DcmTime_getOFString__SWIG_1(swigCPtr, stringValue, pos), true);
    return ret;
  }

  public OFCondition setCurrentTime(bool seconds, bool fraction) {
    OFCondition ret = new OFCondition(DCMTKPINVOKE.DcmTime_setCurrentTime__SWIG_0(swigCPtr, seconds, fraction), true);
    return ret;
  }

  public OFCondition setCurrentTime(bool seconds) {
    OFCondition ret = new OFCondition(DCMTKPINVOKE.DcmTime_setCurrentTime__SWIG_1(swigCPtr, seconds), true);
    return ret;
  }

  public OFCondition setCurrentTime() {
    OFCondition ret = new OFCondition(DCMTKPINVOKE.DcmTime_setCurrentTime__SWIG_2(swigCPtr), true);
    return ret;
  }

  public OFCondition setOFTime(OFTime timeValue) {
    OFCondition ret = new OFCondition(DCMTKPINVOKE.DcmTime_setOFTime(swigCPtr, OFTime.getCPtr(timeValue)), true);
    if (DCMTKPINVOKE.SWIGPendingException.Pending) throw DCMTKPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public OFCondition getOFTime(OFTime timeValue, uint pos, bool supportOldFormat) {
    OFCondition ret = new OFCondition(DCMTKPINVOKE.DcmTime_getOFTime__SWIG_0(swigCPtr, OFTime.getCPtr(timeValue), pos, supportOldFormat), true);
    if (DCMTKPINVOKE.SWIGPendingException.Pending) throw DCMTKPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public OFCondition getOFTime(OFTime timeValue, uint pos) {
    OFCondition ret = new OFCondition(DCMTKPINVOKE.DcmTime_getOFTime__SWIG_1(swigCPtr, OFTime.getCPtr(timeValue), pos), true);
    if (DCMTKPINVOKE.SWIGPendingException.Pending) throw DCMTKPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public OFCondition getOFTime(OFTime timeValue) {
    OFCondition ret = new OFCondition(DCMTKPINVOKE.DcmTime_getOFTime__SWIG_2(swigCPtr, OFTime.getCPtr(timeValue)), true);
    if (DCMTKPINVOKE.SWIGPendingException.Pending) throw DCMTKPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public OFCondition getISOFormattedTime(StringBuilder formattedTime, uint pos, bool seconds, bool fraction, bool createMissingPart, bool supportOldFormat) {
    OFCondition ret = new OFCondition(DCMTKPINVOKE.DcmTime_getISOFormattedTime__SWIG_0(swigCPtr, formattedTime, pos, seconds, fraction, createMissingPart, supportOldFormat), true);
    return ret;
  }

  public OFCondition getISOFormattedTime(StringBuilder formattedTime, uint pos, bool seconds, bool fraction, bool createMissingPart) {
    OFCondition ret = new OFCondition(DCMTKPINVOKE.DcmTime_getISOFormattedTime__SWIG_1(swigCPtr, formattedTime, pos, seconds, fraction, createMissingPart), true);
    return ret;
  }

  public OFCondition getISOFormattedTime(StringBuilder formattedTime, uint pos, bool seconds, bool fraction) {
    OFCondition ret = new OFCondition(DCMTKPINVOKE.DcmTime_getISOFormattedTime__SWIG_2(swigCPtr, formattedTime, pos, seconds, fraction), true);
    return ret;
  }

  public OFCondition getISOFormattedTime(StringBuilder formattedTime, uint pos, bool seconds) {
    OFCondition ret = new OFCondition(DCMTKPINVOKE.DcmTime_getISOFormattedTime__SWIG_3(swigCPtr, formattedTime, pos, seconds), true);
    return ret;
  }

  public OFCondition getISOFormattedTime(StringBuilder formattedTime, uint pos) {
    OFCondition ret = new OFCondition(DCMTKPINVOKE.DcmTime_getISOFormattedTime__SWIG_4(swigCPtr, formattedTime, pos), true);
    return ret;
  }

  public OFCondition getISOFormattedTime(StringBuilder formattedTime) {
    OFCondition ret = new OFCondition(DCMTKPINVOKE.DcmTime_getISOFormattedTime__SWIG_5(swigCPtr, formattedTime), true);
    return ret;
  }

  public static OFCondition getCurrentTime(StringBuilder dicomTime, bool seconds, bool fraction) {
    OFCondition ret = new OFCondition(DCMTKPINVOKE.DcmTime_getCurrentTime__SWIG_0(dicomTime, seconds, fraction), true);
    return ret;
  }

  public static OFCondition getCurrentTime(StringBuilder dicomTime, bool seconds) {
    OFCondition ret = new OFCondition(DCMTKPINVOKE.DcmTime_getCurrentTime__SWIG_1(dicomTime, seconds), true);
    return ret;
  }

  public static OFCondition getCurrentTime(StringBuilder dicomTime) {
    OFCondition ret = new OFCondition(DCMTKPINVOKE.DcmTime_getCurrentTime__SWIG_2(dicomTime), true);
    return ret;
  }

  public static OFCondition getDicomTimeFromOFTime(OFTime timeValue, StringBuilder dicomTime, bool seconds, bool fraction) {
    OFCondition ret = new OFCondition(DCMTKPINVOKE.DcmTime_getDicomTimeFromOFTime__SWIG_0(OFTime.getCPtr(timeValue), dicomTime, seconds, fraction), true);
    if (DCMTKPINVOKE.SWIGPendingException.Pending) throw DCMTKPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static OFCondition getDicomTimeFromOFTime(OFTime timeValue, StringBuilder dicomTime, bool seconds) {
    OFCondition ret = new OFCondition(DCMTKPINVOKE.DcmTime_getDicomTimeFromOFTime__SWIG_1(OFTime.getCPtr(timeValue), dicomTime, seconds), true);
    if (DCMTKPINVOKE.SWIGPendingException.Pending) throw DCMTKPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static OFCondition getDicomTimeFromOFTime(OFTime timeValue, StringBuilder dicomTime) {
    OFCondition ret = new OFCondition(DCMTKPINVOKE.DcmTime_getDicomTimeFromOFTime__SWIG_2(OFTime.getCPtr(timeValue), dicomTime), true);
    if (DCMTKPINVOKE.SWIGPendingException.Pending) throw DCMTKPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static OFCondition getOFTimeFromString(string dicomTime, OFTime timeValue, bool supportOldFormat) {
    OFCondition ret = new OFCondition(DCMTKPINVOKE.DcmTime_getOFTimeFromString__SWIG_0(dicomTime, OFTime.getCPtr(timeValue), supportOldFormat), true);
    if (DCMTKPINVOKE.SWIGPendingException.Pending) throw DCMTKPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static OFCondition getOFTimeFromString(string dicomTime, OFTime timeValue) {
    OFCondition ret = new OFCondition(DCMTKPINVOKE.DcmTime_getOFTimeFromString__SWIG_1(dicomTime, OFTime.getCPtr(timeValue)), true);
    if (DCMTKPINVOKE.SWIGPendingException.Pending) throw DCMTKPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static OFCondition getISOFormattedTimeFromString(string dicomTime, StringBuilder formattedTime, bool seconds, bool fraction, bool createMissingPart, bool supportOldFormat) {
    OFCondition ret = new OFCondition(DCMTKPINVOKE.DcmTime_getISOFormattedTimeFromString__SWIG_0(dicomTime, formattedTime, seconds, fraction, createMissingPart, supportOldFormat), true);
    if (DCMTKPINVOKE.SWIGPendingException.Pending) throw DCMTKPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static OFCondition getISOFormattedTimeFromString(string dicomTime, StringBuilder formattedTime, bool seconds, bool fraction, bool createMissingPart) {
    OFCondition ret = new OFCondition(DCMTKPINVOKE.DcmTime_getISOFormattedTimeFromString__SWIG_1(dicomTime, formattedTime, seconds, fraction, createMissingPart), true);
    if (DCMTKPINVOKE.SWIGPendingException.Pending) throw DCMTKPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static OFCondition getISOFormattedTimeFromString(string dicomTime, StringBuilder formattedTime, bool seconds, bool fraction) {
    OFCondition ret = new OFCondition(DCMTKPINVOKE.DcmTime_getISOFormattedTimeFromString__SWIG_2(dicomTime, formattedTime, seconds, fraction), true);
    if (DCMTKPINVOKE.SWIGPendingException.Pending) throw DCMTKPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static OFCondition getISOFormattedTimeFromString(string dicomTime, StringBuilder formattedTime, bool seconds) {
    OFCondition ret = new OFCondition(DCMTKPINVOKE.DcmTime_getISOFormattedTimeFromString__SWIG_3(dicomTime, formattedTime, seconds), true);
    if (DCMTKPINVOKE.SWIGPendingException.Pending) throw DCMTKPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static OFCondition getISOFormattedTimeFromString(string dicomTime, StringBuilder formattedTime) {
    OFCondition ret = new OFCondition(DCMTKPINVOKE.DcmTime_getISOFormattedTimeFromString__SWIG_4(dicomTime, formattedTime), true);
    if (DCMTKPINVOKE.SWIGPendingException.Pending) throw DCMTKPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static OFCondition getTimeZoneFromString(string dicomTimeZone, out double timeZone) {
    OFCondition ret = new OFCondition(DCMTKPINVOKE.DcmTime_getTimeZoneFromString(dicomTimeZone, out timeZone), true);
    if (DCMTKPINVOKE.SWIGPendingException.Pending) throw DCMTKPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

}

}
