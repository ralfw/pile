using System;
using System.Collections.Generic;
using System.Text;

namespace Pile.Contracts
{
    public interface IPersistentPile : IPile, IDisposable
    {
        void Open(string connectionString);
        void Close();

        long Create(string text);
        long Create(string text, out bool isNew);
        long Create(string text, long qualifierRelation);
        long Create(string text, long qualifierRelation, out bool isNew);

        long Lookup(string text);
    }
}
