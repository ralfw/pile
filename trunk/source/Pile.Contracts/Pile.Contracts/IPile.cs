using System;
using System.Collections.Generic;
using System.Text;

namespace Pile.Contracts
{
    public enum ParentModes
    {
        normative,
        associative,
        both
    }

    public interface IPile
    {
        long Create();
        long Create(long qualifierRelation);
        bool IsRoot(long relation);

        long Create(long nParentRelation, long aParentRelation);
        long Create(long nParentRelation, long aParentRelation, long qualifierRelation);

        long Lookup(long nParentRelation, long aParentRelation);

        void GetParents(long childRelation, out long nParantRelation, out long aParentRelation);
        IEnumerable<long> GetChildren(long parentRelation, ParentModes mode);
        IEnumerable<long> GetChildren(long parentRelation, ParentModes mode, long qualifierRelation);
        
        long GetQualifier(long qualifiedRelation);
        IEnumerable<long> GetQualified(long qualifierRelation);
    }
}
