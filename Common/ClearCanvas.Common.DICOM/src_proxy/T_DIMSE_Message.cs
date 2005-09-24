/* ----------------------------------------------------------------------------
 * This file was automatically generated by SWIG (http://www.swig.org).
 * Version 1.3.24
 *
 * Do not make changes to this file unless you know what you are doing--modify
 * the SWIG interface file instead.
 * ----------------------------------------------------------------------------- */

namespace ClearCanvas.Common.DICOM {

using System;
using System.Text;

public class T_DIMSE_Message : IDisposable {
  private IntPtr swigCPtr;
  protected bool swigCMemOwn;

  internal T_DIMSE_Message(IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = cPtr;
  }

  internal static IntPtr getCPtr(T_DIMSE_Message obj) {
    return (obj == null) ? IntPtr.Zero : obj.swigCPtr;
  }

  ~T_DIMSE_Message() {
    Dispose();
  }

  public virtual void Dispose() {
    if(swigCPtr != IntPtr.Zero && swigCMemOwn) {
      swigCMemOwn = false;
      DCMTKPINVOKE.delete_T_DIMSE_Message(swigCPtr);
    }
    swigCPtr = IntPtr.Zero;
    GC.SuppressFinalize(this);
  }

  public T_DIMSE_Command CommandField {
    set {
      DCMTKPINVOKE.set_T_DIMSE_Message_CommandField(swigCPtr, (int)value);
    } 
    get {
      return (T_DIMSE_Command)DCMTKPINVOKE.get_T_DIMSE_Message_CommandField(swigCPtr);
    } 
  }

  public T_DIMSE_Message_msg msg {
    get {
      IntPtr cPtr = DCMTKPINVOKE.get_T_DIMSE_Message_msg(swigCPtr);
      return (cPtr == IntPtr.Zero) ? null : new T_DIMSE_Message_msg(cPtr, false);
    } 
  }

  public T_DIMSE_Message() : this(DCMTKPINVOKE.new_T_DIMSE_Message(), true) {
  }

}

}
