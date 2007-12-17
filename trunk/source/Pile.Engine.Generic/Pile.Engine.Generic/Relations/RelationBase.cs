using System;
using System.Collections.Generic;
using System.Text;

namespace Pile.Engine.Generic
{
    public abstract class RelationBase
    {
        #region Id Management
        private static long idCounter = 0;

        private static long GenerateId()
        {
            return System.Threading.Interlocked.Increment(ref idCounter);
        }
        #endregion


        private long id;
#if WITH_CHILDREN
        private List<InnerRelation> nChildren, aChildren;
#endif

        public RelationBase()
        {
            this.id = RelationBase.GenerateId();
#if WITH_CHILDREN
            this.nChildren = new List<InnerRelation>();
            this.aChildren = new List<InnerRelation>();
#endif
        }


        public long Id
        {
            get { return this.id; }
        }

#if WITH_CHILDREN
        #region Children management
        internal void AddChild(InnerRelation child, bool isNormChild)
        {
            if (isNormChild)
                lock (this.nChildren)
                {
                    this.nChildren.Add(child);
                }
            else
                lock (this.aChildren)
                {
                    this.aChildren.Add(child);
                }
        }


        public InnerRelation[] NormChildren
        {
            get { lock (this.nChildren) { return this.nChildren.ToArray(); } }
        }

        public InnerRelation[] AssocChildren
        {
            get { lock (this.aChildren) { return this.aChildren.ToArray(); } }
        }
        #endregion
#endif
    }
}
