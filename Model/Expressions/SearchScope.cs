using AVSearch.Model.Expressions;
using AVXLib.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVSearch.Model.Expressions
{
    public class SearchScope : Dictionary<byte, ScopingFilter>
    {
        public SearchScope() : base()
        {
        }
        public bool InScope(byte book, byte chapter)
        {
            if (this.Count == 0)
            {
                return true;
            }
            if (this.ContainsKey(book))
            {
                return this[book].InScope(chapter);
            }
            return false;
        }
        public bool InScope(byte book)
        {
            return this.Count == 0 || this.ContainsKey(book);
        }
    }
}
