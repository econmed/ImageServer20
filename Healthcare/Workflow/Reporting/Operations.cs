using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Workflow;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Healthcare.Brokers;
using System.Collections;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Workflow.Reporting
{
    public class Operations
    {
        public abstract class ReportingOperation
        {
            public abstract bool CanExecute(ReportingProcedureStep step, Staff currentUserStaff);
        }

        public class ClaimInterpretation : ReportingOperation
        {
            public void Execute(InterpretationStep step, Staff currentUserStaff, IWorkflow workflow)
            {
                step.Assign(currentUserStaff);
            }

            public override bool CanExecute(ReportingProcedureStep step, Staff currentUserStaff)
            {
                if (step.Is<InterpretationStep>() == false)
                    return false;

                // step already started
                if (step.State != ActivityStatus.SC)
                    return false;

                // step already claim by the same staff
                if (Equals(step.AssignedStaff, currentUserStaff))
                    return false;

                return true;
            }
        }

        public class StartInterpretation : ReportingOperation
        {
            public void Execute(InterpretationStep step, Staff currentUserStaff, IWorkflow workflow)
            {
                // if not assigned, assign
                if (step.AssignedStaff == null)
                {
                    step.Assign(currentUserStaff);
                }

                if (step.State == ActivityStatus.SC)
                {
                    step.Start(currentUserStaff);
                }
            }

            public override bool CanExecute(ReportingProcedureStep step, Staff currentUserStaff)
            {
                if (step.Is<InterpretationStep>() == false)
                    return false;

                // step is completed/cancelled
                if (step.State != ActivityStatus.SC && step.State != ActivityStatus.IP)
                    return false;

                // step is not claimed or is assigned to someone else
                if (Equals(step.AssignedStaff, currentUserStaff) == false)
                    return false;

                return true;
            }
        }

        public abstract class CompleteInterpretationBase : ReportingOperation
        {
            public virtual void Execute(InterpretationStep step, Staff currentUserStaff, IWorkflow workflow)
            {
                step.Complete();
            }

            public override bool CanExecute(ReportingProcedureStep step, Staff currentUserStaff)
            {
                if (step.Is<InterpretationStep>() == false)
                    return false;

                // step is not started or has been cancelled/completed
                if (step.State != ActivityStatus.IP)
                    return false;

                // step is assigned to someone else
                if (Equals(step.AssignedStaff, currentUserStaff) == false)
                    return false;

                return true;
            }
        }

        public class CompleteInterpretationForTranscription : CompleteInterpretationBase
        {
            public new TranscriptionStep Execute(InterpretationStep step, Staff currentUserStaff, IWorkflow workflow)
            {
                base.Execute(step, currentUserStaff, workflow);

                TranscriptionStep transcription = new TranscriptionStep(step);
                workflow.AddActivity(transcription);
                return transcription;
            }
        }

        public class CompleteInterpretationForVerification : CompleteInterpretationBase
        {
            public new VerificationStep Execute(InterpretationStep step, Staff currentUserStaff, IWorkflow workflow)
            {
                base.Execute(step, currentUserStaff, workflow);

                VerificationStep verification = new VerificationStep(step);
                verification.Assign(step.PerformingStaff);
                workflow.AddActivity(verification);
                return verification;
            }
        }

        public class CompleteInterpretationAndVerify : CompleteInterpretationBase
        {
            public new VerificationStep Execute(InterpretationStep step, Staff currentUserStaff, IWorkflow workflow)
            {
                base.Execute(step, currentUserStaff, workflow);

                VerificationStep verification = new VerificationStep(step);
                verification.Assign(step.PerformingStaff);
                verification.Complete(step.PerformingStaff);
                workflow.AddActivity(verification);
                return verification;
            }
        }

        public class CancelPendingTranscription : ReportingOperation
        {
            public InterpretationStep Execute(TranscriptionStep step, Staff currentUserStaff, IWorkflow workflow)
            {
                step.Discontinue();

                InterpretationStep interpretation = new InterpretationStep(step);

                // TODO assign the new interpretation back to the dictating physician, from the Report object
                //interpretation.Assign();

                workflow.AddActivity(interpretation);
                return interpretation;
            }

            public override bool CanExecute(ReportingProcedureStep step, Staff currentUserStaff)
            {
                if (step.Is<TranscriptionStep>() == false)
                    return false;

                // step already completed or cancelled
                if (step.State == ActivityStatus.CM || step.State == ActivityStatus.DC)
                    return false;

                // step already claim by the same staff
                if (Equals(step.AssignedStaff, currentUserStaff))
                    return false;

                return true;
            }
        }

        public class StartVerification : ReportingOperation
        {
            public void Execute(VerificationStep step, Staff currentUserStaff, IWorkflow workflow)
            {
                // if not assigned, assign
                if (step.AssignedStaff == null)
                {
                    step.Assign(currentUserStaff);
                }

                if (step.State == ActivityStatus.SC)
                {
                    step.Start(currentUserStaff);
                }
            }

            public override bool CanExecute(ReportingProcedureStep step, Staff currentUserStaff)
            {
                if (step.Is<VerificationStep>() == false)
                    return false;

                // step is completed/cancelled
                if (step.State != ActivityStatus.SC && step.State != ActivityStatus.IP)
                    return false;

                // step is assigned to someone else
                if (Equals(step.AssignedStaff, currentUserStaff) == false)
                    return false;

                return true;
            }
        }

        public class CompleteVerification : ReportingOperation
        {
            public void Execute(VerificationStep step, Staff currentUserStaff, IWorkflow workflow)
            {
                // this operation is legal even if the step was never started, therefore need to supply the performer
                step.Complete(currentUserStaff);
            }

            public override bool CanExecute(ReportingProcedureStep step, Staff currentUserStaff)
            {
                if (step.Is<VerificationStep>() == false)
                    return false;

                // step already completed or cancelled
                if (step.State == ActivityStatus.CM || step.State == ActivityStatus.DC)
                    return false;

                // step is assigned to someone else
                if (Equals(step.AssignedStaff, currentUserStaff) == false)
                    return false;

                return true;
            }
        }

        public class CreateAddendum : ReportingOperation
        {
            public InterpretationStep Execute(VerificationStep step, Staff currentUserStaff, IWorkflow workflow)
            {
                InterpretationStep interpretation = new InterpretationStep(step.RequestedProcedure);
                interpretation.Assign(step.PerformingStaff);
                interpretation.Start(step.PerformingStaff);
                workflow.AddActivity(interpretation);
                return interpretation;
            }

            public override bool CanExecute(ReportingProcedureStep step, Staff currentUserStaff)
            {
                // can only create an addendum for a completed verification step
                if (step.Is<VerificationStep>() == false)
                    return false;

                if (step.State != ActivityStatus.CM)
                    return false;

                return true;
            }
        }
    }
}
