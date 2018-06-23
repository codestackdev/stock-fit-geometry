using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sw.Tests.Framework
{
    [DataContract]
    [KnownType(typeof(StartFromPathOptionDetails))]
    [KnownType(typeof(LoadEmbededOptionDetails))]
    [KnownType(typeof(ConnectToProcessOptionDetails))]
    public class SwUnitTestParameters
    {
        [DataMember]
        public SwLoadOption_e LoadOption { get; set; }

        [DataMember]
        public OptionDetails OptionDetails { get; set; }
    }

    [DataContract]
    public enum SwLoadOption_e
    {
        [EnumMember]
        StartFromPath,

        [EnumMember]
        LoadEmbeded,

        [EnumMember]
        ConnectToProcess
    }

    [DataContract]
    public abstract class OptionDetails
    {
    }

    [DataContract]
    public class StartFromPathOptionDetails : OptionDetails
    {
        [DataMember]
        public string ExecutablePath { get; set; }

        [DataMember]
        public int Timeout { get; set; } = 5;
    }

    [DataContract]
    public class LoadEmbededOptionDetails : OptionDetails
    {
        [DataMember]
        public bool UseAnyVersion { get; set; } = true;

        [DataMember]
        public int RevisionNumber { get; set; }
    }

    [DataContract]
    public class ConnectToProcessOptionDetails : OptionDetails
    {
        [DataMember]
        public int ProcessToConnect { get; set; }
    }
}
