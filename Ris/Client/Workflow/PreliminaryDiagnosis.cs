using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.OrderNotes;

namespace ClearCanvas.Ris.Client.Workflow
{
    static class PreliminaryDiagnosis
    {
        /// <summary>
        /// Gets a value indicating whether a preliminary diagnosis conversation exists for the specified order.
        /// </summary>
        /// <param name="orderRef"></param>
        /// <returns></returns>
        public static bool ConversationExists(EntityRef orderRef)
        {
            bool exists = false;
            List<string> filters = new List<string>(new string[] {OrderNoteCategory.PreliminaryDiagnosis.Key});
            Platform.GetService<IOrderNoteService>(
                delegate(IOrderNoteService service)
                {
                    exists = service.GetConversation(new GetConversationRequest(orderRef, filters, true)).NoteCount > 0;
                });
            return exists;
        }

        /// <summary>
        /// Displays the prelminary diagnosis conversation dialog.
        /// </summary>
        /// <param name="orderRef"></param>
        /// <param name="desktopWindow"></param>
        /// <returns></returns>
        public static ApplicationComponentExitCode ShowConversationDialog(EntityRef orderRef, IDesktopWindow desktopWindow)
        {
            OrderNoteConversationComponent component = new OrderNoteConversationComponent(orderRef, OrderNoteCategory.PreliminaryDiagnosis.Key);
            return ApplicationComponent.LaunchAsDialog(desktopWindow, component, "Review Preliminary Diagnosis");
        }
    }
}
