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

public class DcmAgeString : DcmByteString {
  private IntPtr swigCPtr;

  internal DcmAgeString(IntPtr cPtr, bool cMemoryOwn) : base(DCMTKPINVOKE.DcmAgeStringUpcast(cPtr), cMemoryOwn) {
    swigCPtr = cPtr;
  }

  internal static IntPtr getCPtr(DcmAgeString obj) {
    return (obj == null) ? IntPtr.Zero : obj.swigCPtr;
  }

  protected DcmAgeString() : this(IntPtr.Zero, false) {
  }

  ~DcmAgeString() {
    Dispose();
  }

  public override void Dispose() {
    if(swigCPtr != IntPtr.Zero && swigCMemOwn) {
      swigCMemOwn = false;
      DCMTKPINVOKE.delete_DcmAgeString(swigCPtr);
    }
    swigCPtr = IntPtr.Zero;
    GC.SuppressFinalize(this);
    base.Dispose();
  }

  public DcmAgeString(DcmTag tag, uint len) : this(DCMTKPINVOKE.new_DcmAgeString__SWIG_0(DcmTag.getCPtr(tag), len), true) {
  }

  public DcmAgeString(DcmTag tag) : this(DCMTKPINVOKE.new_DcmAgeString__SWIG_1(DcmTag.getCPtr(tag)), true) {
  }

  public DcmAgeString(DcmAgeString old) : this(DCMTKPINVOKE.new_DcmAgeString__SWIG_2(DcmAgeString.getCPtr(old)), true) {
  }

  public override DcmEVR ident() {
    return (DcmEVR)DCMTKPINVOKE.DcmAgeString_ident(swigCPtr);
  }

}

}
