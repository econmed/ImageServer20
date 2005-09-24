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

public class T_ASC_PresentationContext : IDisposable {
  private IntPtr swigCPtr;
  protected bool swigCMemOwn;

  internal T_ASC_PresentationContext(IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = cPtr;
  }

  internal static IntPtr getCPtr(T_ASC_PresentationContext obj) {
    return (obj == null) ? IntPtr.Zero : obj.swigCPtr;
  }

  ~T_ASC_PresentationContext() {
    Dispose();
  }

  public virtual void Dispose() {
    if(swigCPtr != IntPtr.Zero && swigCMemOwn) {
      swigCMemOwn = false;
      DCMTKPINVOKE.delete_T_ASC_PresentationContext(swigCPtr);
    }
    swigCPtr = IntPtr.Zero;
    GC.SuppressFinalize(this);
  }

  public SWIGTYPE_p_DUL_PRESENTATIONCONTEXTID presentationContextID {
    set {
      DCMTKPINVOKE.set_T_ASC_PresentationContext_presentationContextID(swigCPtr, SWIGTYPE_p_DUL_PRESENTATIONCONTEXTID.getCPtr(value));
    } 
    get {
      return new SWIGTYPE_p_DUL_PRESENTATIONCONTEXTID(DCMTKPINVOKE.get_T_ASC_PresentationContext_presentationContextID(swigCPtr), true);
    } 
  }

  public string abstractSyntax {
    set {
      DCMTKPINVOKE.set_T_ASC_PresentationContext_abstractSyntax(swigCPtr, value);
    } 
    get {
      return DCMTKPINVOKE.get_T_ASC_PresentationContext_abstractSyntax(swigCPtr);
    } 
  }

  public byte transferSyntaxCount {
    set {
      DCMTKPINVOKE.set_T_ASC_PresentationContext_transferSyntaxCount(swigCPtr, value);
    } 
    get {
      return DCMTKPINVOKE.get_T_ASC_PresentationContext_transferSyntaxCount(swigCPtr);
    } 
  }

  public SWIGTYPE_p_a_64_1__char proposedTransferSyntaxes {
    set {
      DCMTKPINVOKE.set_T_ASC_PresentationContext_proposedTransferSyntaxes(swigCPtr, SWIGTYPE_p_a_64_1__char.getCPtr(value));
    } 
    get {
      IntPtr cPtr = DCMTKPINVOKE.get_T_ASC_PresentationContext_proposedTransferSyntaxes(swigCPtr);
      return (cPtr == IntPtr.Zero) ? null : new SWIGTYPE_p_a_64_1__char(cPtr, false);
    } 
  }

  public string acceptedTransferSyntax {
    set {
      DCMTKPINVOKE.set_T_ASC_PresentationContext_acceptedTransferSyntax(swigCPtr, value);
    } 
    get {
      return DCMTKPINVOKE.get_T_ASC_PresentationContext_acceptedTransferSyntax(swigCPtr);
    } 
  }

  public T_ASC_P_ResultReason resultReason {
    set {
      DCMTKPINVOKE.set_T_ASC_PresentationContext_resultReason(swigCPtr, (int)value);
    } 
    get {
      return (T_ASC_P_ResultReason)DCMTKPINVOKE.get_T_ASC_PresentationContext_resultReason(swigCPtr);
    } 
  }

  public T_ASC_SC_ROLE proposedRole {
    set {
      DCMTKPINVOKE.set_T_ASC_PresentationContext_proposedRole(swigCPtr, (int)value);
    } 
    get {
      return (T_ASC_SC_ROLE)DCMTKPINVOKE.get_T_ASC_PresentationContext_proposedRole(swigCPtr);
    } 
  }

  public T_ASC_SC_ROLE acceptedRole {
    set {
      DCMTKPINVOKE.set_T_ASC_PresentationContext_acceptedRole(swigCPtr, (int)value);
    } 
    get {
      return (T_ASC_SC_ROLE)DCMTKPINVOKE.get_T_ASC_PresentationContext_acceptedRole(swigCPtr);
    } 
  }

  public T_ASC_PresentationContext() : this(DCMTKPINVOKE.new_T_ASC_PresentationContext(), true) {
  }

}

}
