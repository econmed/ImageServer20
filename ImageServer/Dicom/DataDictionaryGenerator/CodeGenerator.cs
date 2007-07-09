using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using System.Xml;

namespace ClearCanvas.ImageServer.Dicom.DataDictionaryGenerator
{
    public class CodeGenerator
    {
        SortedList<uint, Tag> _tagList = null;
        SortedList _tSyntaxList = null;
        SortedList _sopList = null;
        SortedList _metaSopList = null;
        XmlDocument _transferSyntaxDoc = null;

        public CodeGenerator(SortedList<uint,Tag> tags, SortedList tSyntax, SortedList sops, SortedList metaSops, XmlDocument transferSyntaxDoc)
        {
            _tagList = tags;
            _tSyntaxList = tSyntax;
            _sopList = sops;
            _metaSopList = metaSops;
            _transferSyntaxDoc = transferSyntaxDoc;
        }

        private void WriterHeader(StreamWriter writer)
        {
            writer.WriteLine("using System;");
            writer.WriteLine("using System.Collections;");
            writer.WriteLine("using System.Collections.Generic;");
            writer.WriteLine("");
            writer.WriteLine("namespace ClearCanvas.ImageServer.Dicom");
            writer.WriteLine("{");
        }

        private void WriterFooter(StreamWriter writer)
        {
             writer.WriteLine("}");
        }

        /// <summary>
        /// Create the DicomTags.cs file.
        /// </summary>
        /// <param name="tagFile"></param>
        public void WriteTags(String tagFile)
        {
            StreamWriter writer = new StreamWriter(tagFile);

            WriterHeader(writer);

            writer.WriteLine("    /// <summary>");
            writer.WriteLine("    /// This structure contains defines for all DICOM tags.");
            writer.WriteLine("    /// </summary>");
            writer.WriteLine("    public struct DicomTags");
            writer.WriteLine("    {");

            IEnumerator<Tag> iter = _tagList.Values.GetEnumerator();

            while (iter.MoveNext())
            {
                Tag tag = iter.Current;

                writer.WriteLine("        /// <summary>");
                writer.WriteLine("        /// <para>" + tag.tag + " " + tag.name + "</para>");
                writer.WriteLine("        /// <para> VR: " + tag.vr + " VM:" + tag.vm + "</para>");
                if (tag.retired != null && tag.retired.Equals("RET"))
                    writer.WriteLine("        /// <para>This tag has been retired.</para>");
                writer.WriteLine("        /// </summary>");
                writer.WriteLine("        public const uint " + tag.varName + " = " + tag.nTag + ";");
            }

            writer.WriteLine("    }");
            WriterFooter(writer);

            writer.Close();
        }

        /// <summary>
        /// Create the DicomTagDictionary.cs file.
        /// </summary>
        /// <param name="tagFile"></param>
        public void WriteTagDictionary(String tagFile)
        {
            StreamWriter writer = new StreamWriter(tagFile);

            WriterHeader(writer);

            writer.WriteLine("    /// <summary>");
            writer.WriteLine("    /// This class contains a dictionary of all DICOM tags.");
            writer.WriteLine("    /// </summary>");
            writer.WriteLine("    /// <remarks>");
            writer.WriteLine("    /// <para>This class is the Flyweight Factor for the DicomTag Flyweight class as defined in the Flyweight pattern.</para>");
            writer.WriteLine("    /// </remarks>");
            writer.WriteLine("    public class DicomTagDictionary");
            writer.WriteLine("    {");
            writer.WriteLine("        // Internal members");
            writer.WriteLine("        private static Dictionary<uint,DicomTag> _tags = new Dictionary<uint,DicomTag>();");
            writer.WriteLine("        private static DicomTagDictionary _instance;");
            writer.WriteLine("");    
            writer.WriteLine("        // Static constructor");
            writer.WriteLine("        static DicomTagDictionary()");
            writer.WriteLine("        {");
            writer.WriteLine("            _instance = new DicomTagDictionary();");
            writer.WriteLine("            InitStandardTags();");
            writer.WriteLine("        }");
            writer.WriteLine("");
            writer.WriteLine("        /// <summary>");
            writer.WriteLine("        /// Public instance of class, used to access the Indexer for the class");
            writer.WriteLine("        /// </summary>");
            writer.WriteLine("        public static DicomTagDictionary Instance");
            writer.WriteLine("        {");
            writer.WriteLine("            get { return _instance; }");
            writer.WriteLine("        }");
            writer.WriteLine("");
            writer.WriteLine("        /// <summary>");
            writer.WriteLine("        /// Indexer for retrieving DicomTag instances for specific DICOM attributes.");
            writer.WriteLine("        /// </summary>");
            writer.WriteLine("        public DicomTag this[uint tag]");
            writer.WriteLine("        {");
            writer.WriteLine("            get ");
            writer.WriteLine("            {");
            writer.WriteLine("                if (!_tags.ContainsKey(tag))");
            writer.WriteLine("                    return null;");
            writer.WriteLine("");        
            writer.WriteLine("                return _tags[tag]; ");
            writer.WriteLine("            }");
            writer.WriteLine("            set { _tags[tag] = value; }");
            writer.WriteLine("        }");
            writer.WriteLine("");
            writer.WriteLine("        /// <summary>");
            writer.WriteLine("        /// Indexer for retrieving DicomTag instances for specific DICOM attributes.");
            writer.WriteLine("        /// </summary>");
            writer.WriteLine("        public DicomTag this[ushort group, ushort element]");
            writer.WriteLine("        {");
            writer.WriteLine("            get ");
            writer.WriteLine("            {");
            writer.WriteLine("                if (!_tags.ContainsKey((uint)group << 16 | (uint)element))");
            writer.WriteLine("                    return null;");
            writer.WriteLine("");
            writer.WriteLine("                return _tags[(uint)group << 16 | (uint)element]; ");
            writer.WriteLine("            }");
            writer.WriteLine("            set { _tags[(uint)group << 16 | (uint)element] = value; }");
            writer.WriteLine("        }");
            writer.WriteLine("");
            writer.WriteLine("        /// <summary>");
            writer.WriteLine("        /// Initialize dictionary with standard tags.");
            writer.WriteLine("        /// </summary>");
            writer.WriteLine("        public static void InitStandardTags()");
            writer.WriteLine("        {");

            IEnumerator<Tag> iter = _tagList.Values.GetEnumerator();

            while (iter.MoveNext())
            {
                Tag tag = iter.Current;
                uint vmLow = 0;
                uint vmHigh = 0;
                char[] charSeparators = new char[] { '�', '-' };

                String[] nodes = tag.vm.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                if (nodes.Length == 1)
                {
                    vmLow = uint.Parse(nodes[0]);
                    vmHigh = vmLow;
                }
                else if (nodes.Length == 2)
                {
                    if (nodes[0].Contains("N") || nodes[0].Contains("n"))
                    {
                        vmLow = 1;
                    }
                    else
                    {
                        vmLow = uint.Parse(nodes[0]);
                    }
                    if (nodes[1].Contains("N") || nodes[1].Contains("n"))
                    {
                        vmHigh = UInt32.MaxValue;
                    }
                    else
                    {
                        vmHigh = uint.Parse(nodes[1]);
                    }


                }

                //public Tag(uint tag, String name, DicomVr vr, uint vmLow, uint vmHigh, bool isRetired)
                writer.WriteLine("            _tags.Add(DicomTags." + tag.varName + ",");
                writer.WriteLine("                      new DicomTag(");
                writer.WriteLine("                          DicomTags." + tag.varName + ",");
                writer.WriteLine("                          \"" + tag.name + "\",");
                if (tag.vr.Contains("or"))
                {
                    if (tag.varName.Equals("PixelData"))
                    {
                        writer.WriteLine("                          DicomVr.OWvr,");
                        writer.WriteLine("                          true, //isMultiVrTag");
                    }
                    else
                    {

                        // Just take the first VR listed
                        writer.WriteLine("                          DicomVr." + tag.vr.Substring(0,2) + "vr,");
                        writer.WriteLine("                          true, //isMultiVrTag");
                    }
                }
                else
                {
                    writer.WriteLine("                          DicomVr." + tag.vr + "vr,");
                    writer.WriteLine("                          false, //isMultiVrTag");
                }
                writer.WriteLine("                          " + vmLow + ", // vmLow");
                writer.WriteLine("                          " + vmHigh + ", // vmHigh");
                if (tag.retired != null && tag.retired.Equals("RET"))
                    writer.WriteLine("                          true // isRetired");
                else
                    writer.WriteLine("                          false // isRetired");
                writer.WriteLine("                          ));");
                    
            }
            writer.WriteLine("        }");

            writer.WriteLine("    }"); // end of class

            WriterFooter(writer);

            writer.Close();
        }

        /// <summary>
        /// Get transfer syntax details from the TransferSyntax.xml file for a specific transfer syntax.
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="littleEndian"></param>
        /// <param name="encapsulated"></param>
        /// <param name="explicitVR"></param>
        /// <param name="deflated"></param>
        public void GetTransferSyntaxDetails( String uid, ref String littleEndian, ref String encapsulated,
            ref String explicitVR, ref String deflated)
        {
            XmlNode syntaxNode = _transferSyntaxDoc.FirstChild;

            // I know the format, just do a quick traversal to the first TransferSyntax entry
            syntaxNode = syntaxNode.NextSibling;
            syntaxNode = syntaxNode.FirstChild;

            while (syntaxNode != null)
            {
                if (syntaxNode.Name.Equals("TransferSyntax"))
                {
                    String xmlUid = syntaxNode.Attributes["uid"].Value;

                    if (xmlUid.Equals(uid))
                    {
                        littleEndian = syntaxNode.Attributes["littleEndian"].Value;
                        encapsulated = syntaxNode.Attributes["encapsulated"].Value;
                        explicitVR = syntaxNode.Attributes["explicitVR"].Value;
                        deflated = syntaxNode.Attributes["deflated"].Value;
                        return;
                    }
                }
                syntaxNode = syntaxNode.NextSibling;
            }
        }

        /// <summary>
        /// Create the TransferSyntax.cs file.
        /// </summary>
        /// <param name="syntaxFile"></param>
        public void WriteTransferSyntaxes(String syntaxFile)
        {
            StreamWriter writer = new StreamWriter(syntaxFile);

            WriterHeader(writer);
            writer.WriteLine("    /// <summary>");
            writer.WriteLine("    /// Enumerated value to differentiate between little and big endian.");
            writer.WriteLine("    /// </summary>");
            writer.WriteLine("    public enum Endian");
            writer.WriteLine("    {");
            writer.WriteLine("        Little,");
            writer.WriteLine("        Big");
            writer.WriteLine("    }");
            writer.WriteLine("");

            writer.WriteLine("    /// <summary>");
            writer.WriteLine("    /// This class contains transfer syntax definitions.");
            writer.WriteLine("    /// </summary>");
            writer.WriteLine("    public class TransferSyntax");
            writer.WriteLine("    {");

            IEnumerator iter = _tSyntaxList.GetEnumerator();

            while (iter.MoveNext())
            {
                SopClass tSyntax = (SopClass)((DictionaryEntry)iter.Current).Value;

                writer.WriteLine("        /// <summary>");
                writer.WriteLine("        /// <para>" + tSyntax.name + "</para>");
                writer.WriteLine("        /// <para>UID: " + tSyntax.uid + "</para>");
                writer.WriteLine("        /// </summary>");
                writer.WriteLine("        public static readonly String " + tSyntax.varName + " = \"" + tSyntax.uid + "\";");
                writer.WriteLine("");
            }

            writer.WriteLine("        // Internal members");
            writer.WriteLine("        private static Dictionary<String,TransferSyntax> _transferSyntaxes = new Dictionary<String,TransferSyntax>();");
            writer.WriteLine("        private static bool _listInit = false;");
            writer.WriteLine("        private bool _littleEndian;");
            writer.WriteLine("        private bool _encapsulated;");
            writer.WriteLine("        private bool _explicitVr;");
            writer.WriteLine("        private bool _deflate;");
            writer.WriteLine("        private String _name;");
            writer.WriteLine("        private String _uid;");
            writer.WriteLine("");
            writer.WriteLine("        ///<summary>");
            writer.WriteLine("        /// Constructor for transfer syntax objects");
            writer.WriteLine("        ///</summary>");
            writer.WriteLine("        public TransferSyntax(String name, String uid, bool bLittleEndian, bool bEncapsulated, bool bExplicitVr, bool bDeflate)");
            writer.WriteLine("        {");
            writer.WriteLine("            this._uid = uid;");
            writer.WriteLine("            this._name = name;");
            writer.WriteLine("            this._littleEndian = bLittleEndian;");
            writer.WriteLine("            this._encapsulated = bEncapsulated;");
            writer.WriteLine("            this._explicitVr = bExplicitVr;");
            writer.WriteLine("            this._deflate = bDeflate;");
            writer.WriteLine("        }");
            writer.WriteLine("");
            writer.WriteLine("        ///<summary>Override to the ToString() method, returns the name of the transfer syntax.</summary>");
            writer.WriteLine("        public override String ToString()");
            writer.WriteLine("        {");
            writer.WriteLine("            return _name;");
            writer.WriteLine("        }");
            writer.WriteLine("");
            writer.WriteLine("        ///<summary>Property representing the UID string of transfer syntax.</summary>");
            writer.WriteLine("        public String UidString");
            writer.WriteLine("        {");
            writer.WriteLine("            get { return _uid; }");
            writer.WriteLine("        }");
            writer.WriteLine("");
            writer.WriteLine("        ///<summary>Property representing the UID of transfer syntax.</summary>");
            writer.WriteLine("        public DicomUid UID");
            writer.WriteLine("        {");
            writer.WriteLine("            get");
            writer.WriteLine("            {");
            writer.WriteLine("                return new DicomUid(_uid, _name, UidType.TransferSyntax);");
            writer.WriteLine("            }");
            writer.WriteLine("        }");
            writer.WriteLine("");
            writer.WriteLine("        ///<summary>Property representing the name of the transfer syntax.</summary>");
            writer.WriteLine("        public String Name");
            writer.WriteLine("        {");
            writer.WriteLine("            get { return _name; }");
            writer.WriteLine("        }");
            writer.WriteLine("");
            writer.WriteLine("        ///<summary>Property representing if the transfer syntax is encoded as little endian.</summary>");
            writer.WriteLine("        public bool LittleEndian");
            writer.WriteLine("        {");
            writer.WriteLine("            get { return _littleEndian; }");
            writer.WriteLine("        }");
            writer.WriteLine("");
            writer.WriteLine("        ///<summary>Property representing the Endian enumerated value for the transfer syntax.</summary>");
            writer.WriteLine("          public Endian Endian");
            writer.WriteLine("        {");
            writer.WriteLine("            get");
            writer.WriteLine("            {");
            writer.WriteLine("                if (_littleEndian)");
            writer.WriteLine("                    return Endian.Little;");
            writer.WriteLine("");
            writer.WriteLine("                return Endian.Big;");
            writer.WriteLine("            }");
            writer.WriteLine("        }");
            writer.WriteLine("");
            writer.WriteLine("        ///<summary>Property representing if the transfer syntax is encoded as encapsulated.</summary>");
            writer.WriteLine("        public bool Encapsulated");
            writer.WriteLine("        {");
            writer.WriteLine("            get { return _encapsulated; }");
            writer.WriteLine("        }");
            writer.WriteLine("");
            writer.WriteLine("        ///<summary>Property representing if the transfer syntax is encoded as explicit Value Representation.</summary>");
            writer.WriteLine("        public bool ExplicitVr");
            writer.WriteLine("        {");
            writer.WriteLine("            get { return _explicitVr; }");
            writer.WriteLine("        }");
            writer.WriteLine("");
            writer.WriteLine("        ///<summary>Property representing if the transfer syntax is encoded in deflate format.</summary>");
            writer.WriteLine("        public bool Deflate");
            writer.WriteLine("        {");
            writer.WriteLine("            get { return _deflate; }");
            writer.WriteLine("        }");
            writer.WriteLine("");
            writer.WriteLine("        /// <summary>");
            writer.WriteLine("        /// Get a TransferSyntax object for a specific transfer syntax UID.");
            writer.WriteLine("        /// </summary>");
            writer.WriteLine("        public static TransferSyntax GetTransferSyntax(String uid)");
            writer.WriteLine("        {");
            writer.WriteLine("            if (_listInit == false)");
            writer.WriteLine("            {");
            writer.WriteLine("                _listInit = true;");
            writer.WriteLine("");

            iter = _tSyntaxList.GetEnumerator();

            while (iter.MoveNext())
            {
                SopClass tSyntax = (SopClass)((DictionaryEntry)iter.Current).Value;
                String littleEndian = "";
                String encapsulated = "";
                String explicitVR = "";
                String deflated = "";

                GetTransferSyntaxDetails(tSyntax.uid, ref littleEndian, ref encapsulated, ref explicitVR, ref deflated);

                writer.WriteLine("            _transferSyntaxes.Add(TransferSyntax." + tSyntax.varName + ",");
                writer.WriteLine("                    new TransferSyntax(\"" + tSyntax.name + "\",");
                writer.WriteLine("                                 TransferSyntax." + tSyntax.varName + ",");
                writer.WriteLine("                                 " + littleEndian + ", // Little Endian?");
                writer.WriteLine("                                 " + encapsulated+ ", // Encapsulated?");
                writer.WriteLine("                                 " + explicitVR + ", // Explicit VR?");
                writer.WriteLine("                                 " + deflated + " // Deflated?");
                writer.WriteLine("                                 ));");
                writer.WriteLine("");
            }

            writer.WriteLine("            }");
            writer.WriteLine("");
            writer.WriteLine("            if (!_transferSyntaxes.ContainsKey(uid))");
            writer.WriteLine("                return null;");
            writer.WriteLine("");
            writer.WriteLine("            return _transferSyntaxes[uid];");
            writer.WriteLine("        }");

            writer.WriteLine("    }");
            WriterFooter(writer);

            writer.Close();
        }

        /// <summary>
        /// Create the SopClass.cs file.
        /// </summary>
        /// <param name="sopsFile"></param>
        public void WriteSopClasses(String sopsFile)
        {
            StreamWriter writer = new StreamWriter(sopsFile);

            WriterHeader(writer);

            writer.WriteLine("    /// <summary>");
            writer.WriteLine("    /// This class contains defines for all DICOM SOP Classes.");
            writer.WriteLine("    /// </summary>");
            writer.WriteLine("    public class SopClass");
            writer.WriteLine("    {");

            IEnumerator iter = _sopList.GetEnumerator();

            while (iter.MoveNext())
            {
                SopClass sopClass = (SopClass)((DictionaryEntry)iter.Current).Value;

                writer.WriteLine("        /// <summary>");
                writer.WriteLine("        /// <para>" + sopClass.name + "</para>");
                writer.WriteLine("        /// <para>UID: " + sopClass.uid + "</para>");
                writer.WriteLine("        /// </summary>");
                writer.WriteLine("        public static readonly String " + sopClass.varName + " = \"" + sopClass.uid + "\";");
                writer.WriteLine("");
            }

            iter = _metaSopList.GetEnumerator();

            while (iter.MoveNext())
            {
                SopClass sopClass = (SopClass)((DictionaryEntry)iter.Current).Value;

                writer.WriteLine("        /// <summary>");
                writer.WriteLine("        /// <para>" + sopClass.name + "</para>");
                writer.WriteLine("        /// <para>UID: " + sopClass.uid + "</para>");
                writer.WriteLine("        /// </summary>");
                writer.WriteLine("        public static readonly String " + sopClass.varName + " = \"" + sopClass.uid + "\";");
                writer.WriteLine("");
            }


            /*
             * Now, write out the constructor and the actual class
             */
            writer.WriteLine(""); 
            writer.WriteLine("        private String _sopName;");
            writer.WriteLine("        private String _sopUid;");
            writer.WriteLine("        private bool _bIsMeta;");
            writer.WriteLine(""); 
            writer.WriteLine("        /// <summary> Property that represents the Name of the SOP Class. </summary>");
            writer.WriteLine("        public String Name");
            writer.WriteLine("        {");
            writer.WriteLine("            get { return _sopName; }");
            writer.WriteLine("        }");
            writer.WriteLine("        /// <summary> Property that represents the Uid for the SOP Class. </summary>"); 
            writer.WriteLine("        public String Uid");
            writer.WriteLine("        {");
            writer.WriteLine("            get { return _sopUid; }");
            writer.WriteLine("        }");
            writer.WriteLine("        /// <summary> Property that represents the Uid for the SOP Class. </summary>");
            writer.WriteLine("        public bool Meta");
            writer.WriteLine("        {");
            writer.WriteLine("            get { return _bIsMeta; }");
            writer.WriteLine("        }");
            writer.WriteLine("        /// <summary> Constructor to create SopClass object. </summary>"); 
            writer.WriteLine("        public SopClass(String name,");
            writer.WriteLine("                           String uid,");
            writer.WriteLine("                           bool isMeta)");
            writer.WriteLine("        {");
            writer.WriteLine("            _sopName = name;");
            writer.WriteLine("            _sopUid = uid;");
            writer.WriteLine("            _bIsMeta = isMeta;");
            writer.WriteLine("        }");
            writer.WriteLine("");
            writer.WriteLine("        private static Dictionary<String,SopClass> _sopList = new Dictionary<String,SopClass>();");
            writer.WriteLine("        private static bool _bIsFirst = true;");
            writer.WriteLine("");
            writer.WriteLine("        /// <summary>Retrieve a SopClass object associated with the Uid.</summary>");
            writer.WriteLine("        public static SopClass GetSopClass(String uid)");
            writer.WriteLine("        {");
            writer.WriteLine("            if (_bIsFirst)");
            writer.WriteLine("            {");
            writer.WriteLine("                _bIsFirst = false;");

            // Standard Sops
            iter = _sopList.GetEnumerator();

            while (iter.MoveNext())
            {
                SopClass sopClass = (SopClass)((DictionaryEntry)iter.Current).Value;

                writer.WriteLine("                _sopList.Add(SopClass." + sopClass.varName + ", ");
                writer.WriteLine("                             new SopClass(\"" + sopClass.name + "\", ");
                writer.WriteLine("                                          SopClass." + sopClass.varName + ", ");
                writer.WriteLine("                                          false));");
                writer.WriteLine("");
            }

            // Now, Meta Sops
            iter = _metaSopList.GetEnumerator();

            while (iter.MoveNext())
            {
                SopClass sopClass = (SopClass)((DictionaryEntry)iter.Current).Value;

                writer.WriteLine("                _sopList.Add(SopClass." + sopClass.varName + ", ");
                writer.WriteLine("                             new SopClass(\"" + sopClass.name + "\", ");
                writer.WriteLine("                                          SopClass." + sopClass.varName + ", ");
                writer.WriteLine("                                          true));");
                writer.WriteLine("");
            }
            writer.WriteLine("            }");
            writer.WriteLine("");
            writer.WriteLine("            if (!_sopList.ContainsKey(uid))");
            writer.WriteLine("                return null;");
            writer.WriteLine("");
            writer.WriteLine("            return _sopList[uid];");
            writer.WriteLine("        }");
            
            writer.WriteLine("    }");
            WriterFooter(writer);

            writer.Close();
        }
    }
}
