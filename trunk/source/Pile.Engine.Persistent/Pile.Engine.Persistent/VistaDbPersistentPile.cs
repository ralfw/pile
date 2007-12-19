using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using Pile.Contracts;

using VistaDB;
using VistaDB.DDA;
using VistaDB.Provider;

namespace Pile.Engine.Persistent.VistaDb
{
    public class VistaDbPersistentPile : IPersistentPile
    {
        #region vars and ctor
        private string connectionString;

        private IVistaDBDDA dda;
        private IVistaDBDatabase db;
        private IVistaDBTable tbInnerRelations, tbRootRelations;

        public VistaDbPersistentPile()
        { }

        public VistaDbPersistentPile(string connectionString)
        {
            this.Open(connectionString);
        }
        #endregion

        #region IPersistentPile Members
        #region db connection management
        public void Open(string connectionString)
        {
            if (this.db == null)
            {
                this.connectionString = connectionString;

                this.dda = VistaDBEngine.Connections.OpenDDA();

                if (!System.IO.File.Exists(connectionString))
                {
                    // create database
                    this.db = this.dda.CreateDatabase(connectionString, true, null, 0, 0, false);

                    #region table structures
                    IVistaDBTableSchema tbSchema = this.db.NewTable("InnerRelations");
                    tbSchema.AddColumn("id", VistaDBType.BigInt);
                    tbSchema.DefineColumnAttributes("id", false, false, false, false, null, null);
                    tbSchema.AddColumn("nParentId", VistaDBType.BigInt);
                    tbSchema.DefineColumnAttributes("nParentId", false, false, false, false, null, null);
                    tbSchema.AddColumn("aParentId", VistaDBType.BigInt);
                    tbSchema.DefineColumnAttributes("aParentId", false, false, false, false, null, null);
                    tbSchema.AddColumn("qualifierId", VistaDBType.BigInt);
                    tbSchema.DefineColumnAttributes("qualifierId", false, false, false, false, null, null);
                    this.tbInnerRelations = this.db.CreateTable(tbSchema, false, false);

                    tbSchema = this.db.NewTable("RootRelations");
                    tbSchema.AddColumn("id", VistaDBType.BigInt);
                    tbSchema.DefineColumnAttributes("id", false, false, false, false, null, null);
                    tbSchema.AddColumn("terminalValue", VistaDBType.VarChar);
                    tbSchema.DefineColumnAttributes("terminalValue", false, false, false, false, null, null);
                    tbSchema.AddColumn("qualifierId", VistaDBType.BigInt);
                    tbSchema.DefineColumnAttributes("qualifierId", false, false, false, false, null, null);
                    this.tbRootRelations = this.db.CreateTable(tbSchema, false, false);
                    #endregion

                    #region indexes
                    this.tbInnerRelations.CreateIdentity("id", "1", "1");
                    this.tbInnerRelations.CreateIndex("findById", "id", true, true);
                    this.tbInnerRelations.CreateIndex("findByParents", "nParentId;aParentId", false, true);
                    this.tbInnerRelations.CreateIndex("findByAParent", "aParentId", false, false);
                    this.tbInnerRelations.CreateIndex("findByQualifier", "qualifierId", false, false);

                    this.tbRootRelations.CreateIdentity("id", "-1", "-1");
                    this.tbRootRelations.CreateIndex("findById", "id", true, true);
                    this.tbRootRelations.CreateIndex("findByTerminalValue", "terminalValue", false, true);
                    this.tbRootRelations.CreateIndex("findByQualifier", "qualifierId", false, false);
                    #endregion
                }
                else
                {
                    // open existing database
                    this.db = this.dda.OpenDatabase(connectionString, VistaDBDatabaseOpenMode.ExclusiveReadWrite, null);
                    this.tbInnerRelations = this.db.OpenTable("InnerRelations", false, false);
                    this.tbRootRelations = this.db.OpenTable("RootRelations", false, false);
                }
            }
        }

        public void Open()
        {
            if (this.db == null)
                this.Open("default.piledb");
        }


        public void Close()
        {
            if (this.db != null)
            {
                this.tbRootRelations.Close();
                this.tbInnerRelations.Close();
                this.db.Close();
                this.db = null;
                this.dda.Dispose();
                this.dda = null;
            }
        }
        #endregion

        #region IPile extensions
        #region creating root relations
        public long Create(string text)
        {
            bool isNew;
            return this.Create(text, 0, out isNew);
        }

        public long Create(string text, long qualifier)
        {
            bool isNew;
            return this.Create(text, qualifier, out isNew);
        }

        public long Create(string text, out bool isNew)
        {
            return this.Create(text, 0, out isNew);
        }

        public long Create(string text, long qualifier, out bool isNew)
        {
            try
            {
                this.tbRootRelations.Insert();
                this.tbRootRelations.PutString("terminalValue", text);
                this.tbRootRelations.PutInt64("qualifierId", qualifier);
                this.tbRootRelations.Post();

                isNew = true;
                return (long)this.tbRootRelations.Get("id").Value;
            }
            catch (VistaDB.Diagnostic.VistaDBException)
            {
                isNew = false;
                long r = this.Lookup(text);
                if (r != 0)
                    return r;
                else
                    throw new ApplicationException(string.Format("Unknown root relation '{0}'.", text.Length > 0 ? text.Substring(0, Math.Min(10, text.Length)) + (text.Length > 10 ? "..." : "") : ""));
            }
        }
        #endregion

        public long Lookup(string text)
        {
            this.tbRootRelations.ActiveIndex = "findByTerminalValue";
            IVistaDBRow template = this.tbRootRelations.CurrentKey;
            template.InitTop();
            template["terminalValue"].Value = text;
            if (this.tbRootRelations.Find(template, "findByTerminalValue", false, false))
                return (long)this.tbRootRelations.Get("id").Value;
            else
                return 0;
        }
        #endregion
        #endregion

        #region IPile Members
        #region create anonymous root relations
        public long Create()
        {
            return this.Create(Guid.NewGuid().ToString(), 0);
        }

        public long Create(long qualifier)
        {
            return this.Create(Guid.NewGuid().ToString(), qualifier);
        }
        #endregion

        #region informational functions
        public int CountOfRelations
        {
            get 
            {
                int count = this.CountOfRoots;

                this.tbInnerRelations.ActiveIndex = "findById";
                this.tbInnerRelations.Last();
                if (!this.tbInnerRelations.EndOfTable)
                    count += (int)(long)this.tbInnerRelations.Get("id").Value;

                return count;
            }
        }

        public int CountOfRoots
        {
            get 
            {
                this.tbRootRelations.ActiveIndex = "findById";
                this.tbRootRelations.First(); // "first", because the root ids are all negative, so the "highest" id is the smallest number (-3 < -1) and thus is at the top of an ASC index
                if (!this.tbRootRelations.EndOfTable)
                    return (int)-(long)this.tbRootRelations.Get("id").Value;
                else
                    return 0;
            }
        }

        public bool IsRoot(long relation)
        {
            return relation < 0;
        }
        #endregion

        #region creating inner relations
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
            try
            {
                this.tbInnerRelations.Insert();
                this.tbInnerRelations.PutInt64("nParentId", nParent);
                this.tbInnerRelations.PutInt64("aParentId", aParent);
                this.tbInnerRelations.PutInt64("qualifierId", qualifier);
                this.tbInnerRelations.Post();

                isNew = true;
                return (long)this.tbInnerRelations.Get("id").Value;
            }
            catch (VistaDB.Diagnostic.VistaDBException)
            {
                isNew = false;
                long r = this.Lookup(nParent, aParent);
                if (r != 0)
                    return r;
                else
                    throw new ApplicationException("Unknown inner relation!");
            }
        }
        #endregion


        public long Lookup(long nParent, long aParent)
        {
            this.tbInnerRelations.ActiveIndex = "findByParents";
            IVistaDBRow template = this.tbInnerRelations.CurrentKey;
            template.InitTop();
            template["nParentId"].Value = nParent;
            template["aParentId"].Value = aParent;
            if (this.tbInnerRelations.Find(template, "findByParents", false, false))
                return (long)this.tbInnerRelations.Get("id").Value;
            else
                return 0;
        }


        #region parent-child axis
        public bool TryGetParents(long child, out long nParent, out long aParent)
        {
            if (PositionOnInnerRelation(child))
            {
                nParent = (long)this.tbInnerRelations.Get("nParentId").Value;
                aParent = (long)this.tbInnerRelations.Get("aParentId").Value;
                return true;
            }
            else
            {
                nParent = 0; aParent = 0;
                return false;
            }
        }


        public IEnumerable<long> GetChildren(long parent, ParentModes mode)
        {
            return this.GetChildren(parent, mode, 0);
        }

        public IEnumerable<long> GetChildren(long parent, ParentModes mode, long qualifier)
        {
            List<long> children = new List<long>();

            using (VistaDBConnection conn = new VistaDBConnection(this.db))
            {
                VistaDBCommand cmd;
                cmd = new VistaDBCommand("select id from InnerRelations where nParentId=@parentId", conn);
                cmd.Parameters.AddWithValue("@parentId", parent);
                if (qualifier != 0)
                {
                    cmd.CommandText += " and qualifierId=@qualifierId";
                    cmd.Parameters.AddWithValue("@qualifierId", qualifier);
                }

                if (mode == ParentModes.normative || mode == ParentModes.both)
                {
                    using (VistaDBDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                            children.Add(rd.GetInt64(0));
                    }
                }

                if (mode == ParentModes.associative || mode == ParentModes.both)
                {
                    cmd.CommandText = cmd.CommandText.Replace("nParentId", "aParentId");
                    using (VistaDBDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                            children.Add(rd.GetInt64(0));
                    }
                }
            }

            return children;
        }
        #endregion

        #region type axis
        public long GetQualifier(long qualified)
        {
            if (qualified < 0)
            {
                this.tbRootRelations.ActiveIndex = "findById";
                IVistaDBRow template = this.tbRootRelations.CurrentKey;
                template.InitTop();
                template["id"].Value = qualified;
                if (this.tbRootRelations.Find(template, "findById", false, false))
                    return (long)this.tbRootRelations.Get("qualifierId").Value;
                else
                    return 0;
            }
            else
                if (PositionOnInnerRelation(qualified))
                    return (long)this.tbInnerRelations.Get("qualifierId").Value;
                else
                    return 0;
        }


        public IEnumerable<long> GetQualified(long qualifier)
        {
            List<long> qualifiedRelations = new List<long>();

            using (VistaDBConnection conn = new VistaDBConnection(this.db))
            {
                VistaDBCommand cmd;

                // get qualified root relations
                cmd = new VistaDBCommand("select id from RootRelations where qualifierId=@qualifierId", conn);
                cmd.Parameters.AddWithValue("@qualifierId", qualifier);
                using (VistaDBDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        qualifiedRelations.Add(rd.GetInt64(0));
                }

                // get qualified inner relations
                cmd.CommandText = "select id from InnerRelations where qualifierId=@qualifierId";
                using (VistaDBDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        qualifiedRelations.Add(rd.GetInt64(0));
                }
            }

            return qualifiedRelations;
        }
        #endregion

        private bool PositionOnInnerRelation(long relation)
        {
            this.tbInnerRelations.ActiveIndex = "findById";
            IVistaDBRow template = this.tbInnerRelations.CurrentKey;
            template.InitTop();
            template["id"].Value = relation;
            if (this.tbInnerRelations.Find(template, "findById", false, false))
                return true;
            else
                return false;
        }
        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            this.Close();
        }
        #endregion
    }
}
