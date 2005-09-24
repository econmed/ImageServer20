/* ----------------------------------------------------------------------------
 * This file was automatically generated by SWIG (http://www.swig.org).
 * Version 1.3.24
 *
 * Do not make changes to this file unless you know what you are doing--modify
 * the SWIG interface file instead.
 * ----------------------------------------------------------------------------- */

namespace ClearCanvas.Common.Dicom {

using System;
using System.Text;

public class OFTime : IDisposable {
  private IntPtr swigCPtr;
  protected bool swigCMemOwn;

  internal OFTime(IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = cPtr;
  }

  internal static IntPtr getCPtr(OFTime obj) {
    return (obj == null) ? IntPtr.Zero : obj.swigCPtr;
  }

  ~OFTime() {
    Dispose();
  }

  public virtual void Dispose() {
    if(swigCPtr != IntPtr.Zero && swigCMemOwn) {
      swigCMemOwn = false;
      DCMTKPINVOKE.delete_OFTime(swigCPtr);
    }
    swigCPtr = IntPtr.Zero;
    GC.SuppressFinalize(this);
  }

  public OFTime() : this(DCMTKPINVOKE.new_OFTime__SWIG_0(), true) {
  }

  public OFTime(OFTime timeVal) : this(DCMTKPINVOKE.new_OFTime__SWIG_1(OFTime.getCPtr(timeVal)), true) {
  }

  public OFTime(uint hour, uint minute, double second, double timeZone) : this(DCMTKPINVOKE.new_OFTime__SWIG_2(hour, minute, second, timeZone), true) {
  }

  public OFTime(uint hour, uint minute, double second) : this(DCMTKPINVOKE.new_OFTime__SWIG_3(hour, minute, second), true) {
  }

  public virtual void clear() {
    DCMTKPINVOKE.OFTime_clear(swigCPtr);
  }

  public virtual bool isValid() {
    return DCMTKPINVOKE.OFTime_isValid(swigCPtr);
  }

  public bool setTime(uint hour, uint minute, double second, double timeZone) {
    return DCMTKPINVOKE.OFTime_setTime__SWIG_0(swigCPtr, hour, minute, second, timeZone);
  }

  public bool setTime(uint hour, uint minute, double second) {
    return DCMTKPINVOKE.OFTime_setTime__SWIG_1(swigCPtr, hour, minute, second);
  }

  public bool setHour(uint hour) {
    return DCMTKPINVOKE.OFTime_setHour(swigCPtr, hour);
  }

  public bool setMinute(uint minute) {
    return DCMTKPINVOKE.OFTime_setMinute(swigCPtr, minute);
  }

  public bool setSecond(double second) {
    return DCMTKPINVOKE.OFTime_setSecond(swigCPtr, second);
  }

  public bool setTimeZone(double timeZone) {
    return DCMTKPINVOKE.OFTime_setTimeZone__SWIG_0(swigCPtr, timeZone);
  }

  public bool setTimeZone(int hour, uint minute) {
    return DCMTKPINVOKE.OFTime_setTimeZone__SWIG_1(swigCPtr, hour, minute);
  }

  public bool setTimeInSeconds(double seconds, double timeZone, bool normalize) {
    return DCMTKPINVOKE.OFTime_setTimeInSeconds__SWIG_0(swigCPtr, seconds, timeZone, normalize);
  }

  public bool setTimeInSeconds(double seconds, double timeZone) {
    return DCMTKPINVOKE.OFTime_setTimeInSeconds__SWIG_1(swigCPtr, seconds, timeZone);
  }

  public bool setTimeInSeconds(double seconds) {
    return DCMTKPINVOKE.OFTime_setTimeInSeconds__SWIG_2(swigCPtr, seconds);
  }

  public bool setTimeInHours(double hours, double timeZone, bool normalize) {
    return DCMTKPINVOKE.OFTime_setTimeInHours__SWIG_0(swigCPtr, hours, timeZone, normalize);
  }

  public bool setTimeInHours(double hours, double timeZone) {
    return DCMTKPINVOKE.OFTime_setTimeInHours__SWIG_1(swigCPtr, hours, timeZone);
  }

  public bool setTimeInHours(double hours) {
    return DCMTKPINVOKE.OFTime_setTimeInHours__SWIG_2(swigCPtr, hours);
  }

  public bool setCurrentTime() {
    return DCMTKPINVOKE.OFTime_setCurrentTime(swigCPtr);
  }

  public bool setISOFormattedTime(string formattedTime) {
    return DCMTKPINVOKE.OFTime_setISOFormattedTime(swigCPtr, formattedTime);
  }

  public uint getHour() {
    return DCMTKPINVOKE.OFTime_getHour(swigCPtr);
  }

  public uint getMinute() {
    return DCMTKPINVOKE.OFTime_getMinute(swigCPtr);
  }

  public double getSecond() {
    return DCMTKPINVOKE.OFTime_getSecond(swigCPtr);
  }

  public uint getIntSecond() {
    return DCMTKPINVOKE.OFTime_getIntSecond(swigCPtr);
  }

  public uint getMilliSecond() {
    return DCMTKPINVOKE.OFTime_getMilliSecond(swigCPtr);
  }

  public uint getMicroSecond() {
    return DCMTKPINVOKE.OFTime_getMicroSecond(swigCPtr);
  }

  public double getTimeZone() {
    return DCMTKPINVOKE.OFTime_getTimeZone(swigCPtr);
  }

  public double getTimeInSeconds(bool useTimeZone, bool normalize) {
    return DCMTKPINVOKE.OFTime_getTimeInSeconds__SWIG_0(swigCPtr, useTimeZone, normalize);
  }

  public double getTimeInSeconds(bool useTimeZone) {
    return DCMTKPINVOKE.OFTime_getTimeInSeconds__SWIG_1(swigCPtr, useTimeZone);
  }

  public double getTimeInSeconds() {
    return DCMTKPINVOKE.OFTime_getTimeInSeconds__SWIG_2(swigCPtr);
  }

  public double getTimeInHours(bool useTimeZone, bool normalize) {
    return DCMTKPINVOKE.OFTime_getTimeInHours__SWIG_0(swigCPtr, useTimeZone, normalize);
  }

  public double getTimeInHours(bool useTimeZone) {
    return DCMTKPINVOKE.OFTime_getTimeInHours__SWIG_1(swigCPtr, useTimeZone);
  }

  public double getTimeInHours() {
    return DCMTKPINVOKE.OFTime_getTimeInHours__SWIG_2(swigCPtr);
  }

  public OFTime getCoordinatedUniversalTime() {
    return new OFTime(DCMTKPINVOKE.OFTime_getCoordinatedUniversalTime(swigCPtr), true);
  }

  public OFTime getLocalTime() {
    return new OFTime(DCMTKPINVOKE.OFTime_getLocalTime(swigCPtr), true);
  }

  public bool getISOFormattedTime(StringBuilder formattedTime, bool showSeconds, bool showFraction, bool showTimeZone, bool showDelimiter) {
    return DCMTKPINVOKE.OFTime_getISOFormattedTime__SWIG_0(swigCPtr, formattedTime, showSeconds, showFraction, showTimeZone, showDelimiter);
  }

  public bool getISOFormattedTime(StringBuilder formattedTime, bool showSeconds, bool showFraction, bool showTimeZone) {
    return DCMTKPINVOKE.OFTime_getISOFormattedTime__SWIG_1(swigCPtr, formattedTime, showSeconds, showFraction, showTimeZone);
  }

  public bool getISOFormattedTime(StringBuilder formattedTime, bool showSeconds, bool showFraction) {
    return DCMTKPINVOKE.OFTime_getISOFormattedTime__SWIG_2(swigCPtr, formattedTime, showSeconds, showFraction);
  }

  public bool getISOFormattedTime(StringBuilder formattedTime, bool showSeconds) {
    return DCMTKPINVOKE.OFTime_getISOFormattedTime__SWIG_3(swigCPtr, formattedTime, showSeconds);
  }

  public bool getISOFormattedTime(StringBuilder formattedTime) {
    return DCMTKPINVOKE.OFTime_getISOFormattedTime__SWIG_4(swigCPtr, formattedTime);
  }

  public static OFTime getCurrentTime() {
    return new OFTime(DCMTKPINVOKE.OFTime_getCurrentTime(), true);
  }

  public static double getLocalTimeZone() {
    return DCMTKPINVOKE.OFTime_getLocalTimeZone();
  }

}

}
