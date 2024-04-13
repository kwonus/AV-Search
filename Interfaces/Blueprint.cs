using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AVSearch.Interfaces
{
    [DataContract]
    public class Parsed
    {
        [DataMember]
        public string rule { set; get; }
        [DataMember]
        public string text { set; get; }
        [DataMember]
        public Parsed[] children { set; get; }

        public Parsed()
        {
            this.rule = string.Empty;
            this.text = string.Empty;
            this.children = new Parsed[0];
        }
    }
    public class ParsedExpression
    {
        public string Text { get; set; }
        public bool Ordered { get; set; }
        public Parsed Blueprint { get; set; }
    }
}
