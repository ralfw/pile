using System;
using System.Collections.Generic;
using System.Text;

using Pile.Contracts;

namespace Pile.Engine.Transient
{
    public class TransientPile : IPile
    {
        #region IPile Members

        public long Create(long nParentRelation, long aParentRelation, long qualifierRelation)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public long Create(long nParentRelation, long aParentRelation)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public long Create(long qualifierRelation)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public long Create()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IEnumerable<long> GetChildren(long parentRelation, ParentModes mode, long qualifierRelation)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IEnumerable<long> GetChildren(long parentRelation, ParentModes mode)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void GetParents(long childRelation, out long nParantRelation, out long aParentRelation)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IEnumerable<long> GetQualified(long qualifierRelation)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public long GetQualifier(long qualifiedRelation)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool IsRoot(long relation)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public long Lookup(long nParentRelation, long aParentRelation)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
