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

public class DcmDictEntryListIterator : IDisposable {
  private HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal DcmDictEntryListIterator(IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new HandleRef(this, cPtr);
  }

  internal static HandleRef getCPtr(DcmDictEntryListIterator obj) {
    return (obj == null) ? new HandleRef(null, IntPtr.Zero) : obj.swigCPtr;
  }

  ~DcmDictEntryListIterator() {
    Dispose();
  }

  public virtual void Dispose() {
    if(swigCPtr.Handle != IntPtr.Zero && swigCMemOwn) {
      swigCMemOwn = false;
      DCMTKPINVOKE.delete_DcmDictEntryListIterator(swigCPtr);
    }
    swigCPtr = new HandleRef(null, IntPtr.Zero);
    GC.SuppressFinalize(this);
  }

  public DcmDictEntryListIterator() : this(DCMTKPINVOKE.new_DcmDictEntryListIterator__SWIG_0(), true) {
  }

  public DcmDictEntryListIterator(SWIGTYPE_p_OFIteratorTDcmDictEntry_p_t iter) : this(DCMTKPINVOKE.new_DcmDictEntryListIterator__SWIG_1(SWIGTYPE_p_OFIteratorTDcmDictEntry_p_t.getCPtr(iter)), true) {
    if (DCMTKPINVOKE.SWIGPendingException.Pending) throw DCMTKPINVOKE.SWIGPendingException.Retrieve();
  }

}

}
