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
        long Create(string text, long qualifier);
        long Create(string text, long qualifier, out bool isNew);

        long Lookup(string text);
    }
}
