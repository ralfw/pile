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
        long Create(long qualifier);
        bool IsRoot(long relation);

        long Create(long nParent, long aParent);
        long Create(long nParent, long aParent, out bool isNew);
        long Create(long nParent, long aParent, long qualifier);
        long Create(long nParent, long aParent, long qualifier, out bool isNew);

        long Lookup(long nParent, long aParent);

        bool TryGetParents(long childRelation, out long nParent, out long aParent);
        IEnumerable<long> GetChildren(long parent, ParentModes mode);
        IEnumerable<long> GetChildren(long parent, ParentModes mode, long qualifier);
        
        long GetQualifier(long qualified);
        IEnumerable<long> GetQualified(long qualifier);

        int CountOfRoots { get; }
        int CountOfRelations { get; }
    }
}
