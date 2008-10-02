using System;
using System.Collections.Generic;
using System.Xml;
using ClearCanvas.ImageServer.Common.CommandProcessor;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.MergeStudy
{
    /// <summary>
    /// "MergeStudy" xml parser.
    /// </summary>
    class MergeStudyCommandXmlParser
    {
        /// <summary>
        /// Retrieves the list of <see cref="IImageLevelUpdateCommand"/> specified in the xml.
        /// </summary>
        /// <param name="createStudyNode"></param>
        /// <returns></returns>
        public IList<IImageLevelUpdateCommand> ParseImageLevelCommands(XmlNode createStudyNode)
        {
            List<IImageLevelUpdateCommand> _commands = new List<IImageLevelUpdateCommand>();

            foreach (XmlNode subNode in createStudyNode.ChildNodes)
            {
                if (! (subNode is XmlComment))
                {
                    //TODO: Use plugin?
                    if (subNode.Name == "SetTag")
                    {
                        UpdateTagCommandParser parser = new UpdateTagCommandParser();
                        _commands.Add(parser.Parse(subNode));
                    }
                    else
                    {
                        throw new NotSupportedException(String.Format("Unsupported operator {0}", subNode.Name));
                    }
                }
                
            }

            return _commands;
        }

    }
}
