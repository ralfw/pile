using System;
using System.Collections.Generic;
using System.Text;

namespace dnppv.pile
{
    public class TerminalValueBase : RelationBase
    {
        private string key;


        public TerminalValueBase() { }

        internal void Initialize(string key)
        {
            this.key = key;
        }


        public string Key
        {
            get { return this.key; }
        }
    }
}
