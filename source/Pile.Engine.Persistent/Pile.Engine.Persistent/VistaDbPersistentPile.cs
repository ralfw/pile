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
        private IVistaDBTable tbRelations;

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

                    #region table structure
                    IVistaDBTableSchema tbSchema = this.db.NewTable("Relations");

                    tbSchema.AddColumn("id", VistaDBType.BigInt);
                    tbSchema.DefineColumnAttributes("id", false, false, false, false, null, null);

                    tbSchema.AddColumn("nParentId", VistaDBType.BigInt);
                    tbSchema.DefineColumnAttributes("nParentId", false, false, false, false, null, null);

                    tbSchema.AddColumn("aParentId", VistaDBType.BigInt);
                    tbSchema.DefineColumnAttributes("aParentId", false, false, false, false, null, null);

                    tbSchema.AddColumn("qualifierId", VistaDBType.BigInt);
                    tbSchema.DefineColumnAttributes("qualifierId", false, false, false, false, null, null);

                    tbSchema.AddColumn("terminalValue", VistaDBType.VarChar);
                    tbSchema.DefineColumnAttributes("terminalValue", false, false, false, false, null, null);

                    this.tbRelations = this.db.CreateTable(tbSchema, false, false);
                    #endregion

                    #region indexes
                    this.tbRelations.CreateIndex("findByParents", "nParentId;aParentId", false, false);

                    this.tbRelations.CreateIndex("findByAParent", "aParentId", false, false);

                    this.tbRelations.CreateIdentity("id", "1", "1");
                    this.tbRelations.CreateIndex("findById", "id", true, true);
                    
                    this.tbRelations.CreateIndex("findByQualifier", "qualifierId", false, false);
                    
                    this.tbRelations.CreateIndex("findByTerminalValue", "terminalValue", false, true);
                    #endregion
                }
                else
                {
                    // open existing database
                    this.db = this.dda.OpenDatabase(connectionString, VistaDBDatabaseOpenMode.ExclusiveReadWrite, null);
                    this.tbRelations = this.db.OpenTable("Relations", false, false);
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
                this.tbRelations.Close();
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
                this.tbRelations.Insert();
                this.tbRelations.PutInt64("nParentId", 0);
                this.tbRelations.PutInt64("aParentId", 0);
                this.tbRelations.PutInt64("qualifierId", 0);
                this.tbRelations.PutString("terminalValue", text);
                this.tbRelations.Post();

                isNew = true;
                return (long)this.tbRelations.Get("id").Value;
            }
            catch (VistaDB.Diagnostic.VistaDBException)
            {
                isNew = false;
                return this.Lookup(text);
            }
        }
        #endregion

        public long Lookup(string text)
        {
            this.tbRelations.ActiveIndex = "findByTerminalValue";
            IVistaDBRow template = this.tbRelations.CurrentKey;
            template.InitTop();
            template["terminalValue"].Value = text;
            if (this.tbRelations.Find(template, "findByTerminalValue", false, false))
                return (long)this.tbRelations.Get("id").Value;
            else
                throw new ApplicationException(string.Format("Unknown root relation '{0}'.", text.Length > 0 ? text.Substring(0, Math.Min(10, text.Length)) + (text.Length > 10 ? "..." : "") : ""));
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


        public int CountOfRelations
        {
            get 
            {
                return 0;
                //IVistaDBConnection conn = new VistaDBConnection(this.db);
                //IDbCommand cmd = 
            }
        }

        public int CountOfRoots
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }


        public long Create(long nParent, long aParent, long qualifier, out bool isNew)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public long Create(long nParent, long aParent, long qualifier)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public long Create(long nParent, long aParent, out bool isNew)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public long Create(long nParent, long aParent)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IEnumerable<long> GetChildren(long parent, ParentModes mode, long qualifier)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IEnumerable<long> GetChildren(long parent, ParentModes mode)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IEnumerable<long> GetQualified(long qualifier)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public long GetQualifier(long qualified)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool IsRoot(long relation)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public long Lookup(long nParent, long aParent)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool TryGetParents(long childRelation, out long nParent, out long aParent)
        {
            throw new Exception("The method or operation is not implemented.");
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
