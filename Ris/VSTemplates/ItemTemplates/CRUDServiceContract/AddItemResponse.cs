using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace $rootnamespace$
{
    [DataContract]
    public class Add$fileinputname$Response : DataContractBase
    {
        public Add$fileinputname$Response($fileinputname$Summary summary)
        {
            this.$fileinputname$ = summary;
        }

        [DataMember]
        public $fileinputname$Summary $fileinputname$;
    }
}
