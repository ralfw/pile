using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

//TODO: Create generisch auslegen. Create<T>() mit T als Ableitung von TTerminalValue oder TRelation. So würden dann unterschiedliche Relationentypen im Pile möglich.
namespace Pile.Engine.Generic
{
    public class MemoryPile<TRootRelation, TInnerRelation> 
        where TRootRelation : RootRelation, new()
        where TInnerRelation : InnerRelation, new()
    {
        private Dictionary<string, TRootRelation> terminalValues;
        private Hashtable innerRelations;

        public MemoryPile()
        {
            this.terminalValues = new Dictionary<string, TRootRelation>();
            this.innerRelations = new Hashtable();
        }


        #region Terminal Values

        public TRootRelation Create()
        {
            return this.Create(Guid.NewGuid().ToString());
        }

        public TRootRelation Create(string key)
        {
            bool isNew;
            return this.Create(key, out isNew);
        }

        public TRootRelation Create(string key, out bool isNew)
        {
            //lock (this.terminalValues)
            //{
                TRootRelation tv;
                isNew = !this.terminalValues.TryGetValue(key, out tv);
                if (isNew)
                {
                    tv = new TRootRelation();
                    tv.Initialize(key);
                    this.terminalValues.Add(key, tv);
                }
                return tv;
            //}
        }


        public int CountTV
        {
            get { return this.terminalValues.Count; }
        }
        #endregion


        #region Create relation
        public TInnerRelation Create(RelationBase nParent, RelationBase aParent)
        {
            bool isNew;
            return this.Create(nParent, aParent, out isNew);
        }

        public TInnerRelation Create(RelationBase nParent, RelationBase aParent, out bool isNew)
        {
            //lock (this.innerRelations)
            //{
                TInnerRelation child = this.Get(nParent, aParent);
                isNew = child == null;

                if (isNew)
                {
                    child = new TInnerRelation();
                    child.Initialize(nParent, aParent);

                    Hashtable assocRelations;
                    if (this.innerRelations.ContainsKey(nParent.Id))
                        assocRelations = (Hashtable)this.innerRelations[nParent.Id];
                    else
                    {
                        assocRelations = new Hashtable();
                        this.innerRelations.Add(nParent.Id, assocRelations);
                    }
                    assocRelations.Add(aParent.Id, child);
                }

                return child;
            //}
        }
        #endregion


        #region Get relation
        public TRootRelation Get(string key)
        {
            TRootRelation tv;
            if (this.terminalValues.TryGetValue(key, out tv))
                return tv;
            else
                return null;
        }


        public TInnerRelation Get(RelationBase nParent, RelationBase aParent)
        {
            TInnerRelation child = null;
            Hashtable assocRelations;
            if (this.innerRelations.ContainsKey(nParent.Id))
            {
                assocRelations = (Hashtable)this.innerRelations[nParent.Id];
                if (assocRelations.ContainsKey(aParent.Id))
                    child = (TInnerRelation)assocRelations[aParent.Id];
            }
            return child;            
        }


        public int CountInnerRelation
        {
            get { return this.innerRelations.Count; }
        }
        #endregion
    }
}
