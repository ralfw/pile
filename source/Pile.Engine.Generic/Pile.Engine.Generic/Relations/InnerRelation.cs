using System;
using System.Collections.Generic;
using System.Text;

namespace Pile.Engine.Generic
{
    public class InnerRelation : RelationBase
    {
        private RelationBase nParent, aParent;


        public InnerRelation() { }

        internal void Initialize(RelationBase nParent, RelationBase aParent)
        {
            this.nParent = nParent;

            this.aParent = aParent;
#if WITH_CHILDREN
            this.nParent.AddChild(this, true);
            this.aParent.AddChild(this, false);
#endif
        }


        public RelationBase NormParent
        {
            get { return this.nParent; }
        }

        public RelationBase AssocParent
        {
            get { return this.aParent; }
        }
    }
}
