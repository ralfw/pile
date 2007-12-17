using System;
using System.Collections.Generic;
using System.Text;

using Pile.Contracts;

namespace Pile.Engine.Transient
{
    public class TransientPile : IPile
    {
        #region Relation type
        private class Relation
        {
            public long id, nParent, aParent, qualifier;

            public Relation(long id, long nParent, long aParent, long qualifier)
            {
                this.id = id;
                this.nParent = nParent;
                this.aParent = aParent;
                this.qualifier = qualifier;
            }
        }
        #endregion

        #region vars & ctor
        private List<Relation> relations;
        private int countRoots;
        private Dictionary<long, List<long>> qualifierIndex;
        private Dictionary<long, Dictionary<long, long>> parentIndex;
        private Dictionary<long, List<long>> aParentIndex;

        public TransientPile()
        {
            this.relations = new List<Relation>();
            this.relations.Add(null); // there is no relation with id==0
            this.countRoots = 0;

            this.qualifierIndex = new Dictionary<long, List<long>>();
            this.parentIndex = new Dictionary<long, Dictionary<long, long>>();
            this.aParentIndex = new Dictionary<long, List<long>>();
        }
        #endregion

        #region IPile Members

        #region creation
        #region Root relations
        public long Create()
        {
            Relation r = new Relation(this.relations.Count, 0, 0, 0);
            this.relations.Add(r);
            this.countRoots++;
            return r.id;
        }

        public long Create(long qualifier)
        {
            if (qualifier < 0 || qualifier >= this.relations.Count) throw new ArgumentOutOfRangeException("Invalid qualifier relation!");

            Relation r = new Relation(this.relations.Count, 0, 0, qualifier);
            this.relations.Add(r);
            this.countRoots++;

            RegisterWithQualifier(qualifier, r.id);

            return r.id;
        }
        #endregion

        #region Inner relations
        public long Create(long nParent, long aParent)
        {
            bool isNew;
            return this.Create(nParent, aParent, 0, out isNew);
        }

        public long Create(long nParent, long aParent, out bool isNew)
        {
            return this.Create(nParent, aParent, 0, out isNew);
        }

        public long Create(long nParent, long aParent, long qualifier)
        {
            bool isNew;
            return this.Create(nParent, aParent, qualifier, out isNew);
        }

        public long Create(long nParent, long aParent, long qualifier, out bool isNew)
        {
            if (qualifier < 0 || qualifier >= this.relations.Count) throw new ArgumentOutOfRangeException("Invalid qualifier relation!");

            isNew = false;
            long rId = this.Lookup(nParent, aParent);

            if (rId == 0)
            {
                Relation r = new Relation(this.relations.Count, nParent, aParent, qualifier);
                this.relations.Add(r);

                // register with nParent+aParent
                Dictionary<long, long> relationsWithNParent;
                if (!this.parentIndex.TryGetValue(nParent, out relationsWithNParent))
                {
                    relationsWithNParent = new Dictionary<long, long>();
                    this.parentIndex.Add(nParent, relationsWithNParent);
                }
                relationsWithNParent.Add(aParent, r.id);

                // register with aParent
                List<long> relationsWithAParent;
                if (!this.aParentIndex.TryGetValue(aParent, out relationsWithAParent))
                {
                    relationsWithAParent = new List<long>();
                    this.aParentIndex.Add(aParent, relationsWithAParent);
                }
                relationsWithAParent.Add(r.id);

                RegisterWithQualifier(qualifier, r.id);

                isNew = true;
                return r.id;
            }
            else
                return rId;
        }
        #endregion

        private void RegisterWithQualifier(long qualifier, long relation)
        {
            if (qualifier > 0)
            {
                List<long> qualifiedList;
                if (!this.qualifierIndex.TryGetValue(qualifier, out qualifiedList))
                {
                    qualifiedList = new List<long>();
                    this.qualifierIndex.Add(qualifier, qualifiedList);
                }
                qualifiedList.Add(relation);
            }
        }
        #endregion

        #region relatives axis
        public long Lookup(long nParent, long aParent)
        {
            if (nParent < 1 || nParent >= this.relations.Count) throw new ArgumentOutOfRangeException("Invalid normative parent relation!");
            if (aParent < 1 || aParent >= this.relations.Count) throw new ArgumentOutOfRangeException("Invalid associative parent relation!");

            Dictionary<long, long> relationsWithNParent;
            if (this.parentIndex.TryGetValue(nParent, out relationsWithNParent))
            {
                long rId;
                if (relationsWithNParent.TryGetValue(aParent, out rId))
                    return rId;
                else
                    return 0;
            }
            else
                return 0;
        }

        public bool TryGetParents(long child, out long nParent, out long aParent)
        {
            if (child < 1 || child >= this.relations.Count) throw new ArgumentOutOfRangeException("Invalid child relation!");

            Relation r = this.relations[(int)child];
            nParent = r.nParent;
            aParent = r.aParent;

            return nParent != 0;
        }

        public IEnumerable<long> GetChildren(long parent, ParentModes mode)
        {
            return this.GetChildren(parent, mode, 0);
        }

        public IEnumerable<long> GetChildren(long parent, ParentModes mode, long qualifier)
        {
            if (parent < 1 || parent >= this.relations.Count) throw new ArgumentOutOfRangeException("Invalid parent relation!");
            if (qualifier < 0 || qualifier >= this.relations.Count) throw new ArgumentOutOfRangeException("Invalid qualifier!");

            List<long> children = new List<long>();

            // select all normative children
            if (mode == ParentModes.normative || mode == ParentModes.both)
            {
                Dictionary<long, long> nChildren;
                if (this.parentIndex.TryGetValue(parent, out nChildren))
                    children.AddRange(nChildren.Values);
            }

            // select all associative children
            if (mode == ParentModes.associative || mode == ParentModes.both)
            {
                List<long> aChildren;
                if (this.aParentIndex.TryGetValue(parent, out aChildren))
                    children.AddRange(aChildren);
            }

            // delete all children from the selection not matching the qualifier
            if (qualifier > 0)
                for (int i = children.Count - 1; i >= 0; i--)
                    if (this.relations[(int)children[i]].qualifier != qualifier)
                        children.RemoveAt(i);

            return children;
        }
        #endregion

        #region qualification axis
        public IEnumerable<long> GetQualified(long qualifier)
        {
            if (qualifier < 1 || qualifier >= this.relations.Count) throw new ArgumentOutOfRangeException("Invalid qualifier relation!");

            List<long> qualifiedList;
            if (!this.qualifierIndex.TryGetValue(qualifier, out qualifiedList))
                qualifiedList = new List<long>();
            return qualifiedList;
        }

        public long GetQualifier(long qualified)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region information
        public bool IsRoot(long relation)
        {
            if (relation >= 1 && relation < this.relations.Count)
                return this.relations[(int)relation].nParent == 0;
            else
                return false;
        }

        public int CountOfRelations
        {
            get { return this.relations.Count-1; }
        }

        public int CountOfRoots
        {
            get { return this.countRoots; }
        }
        #endregion

        #endregion
    }
}
