using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Model.Brokers
{
    public interface IInsertFilesystem : IProcedureQueryBroker<FilesystemInsertParameters, Filesystem>
    {
    }
}
