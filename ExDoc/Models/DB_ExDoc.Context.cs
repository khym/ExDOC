﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ExDoc.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class EX_DOCEntities : DbContext
    {
        public EX_DOCEntities()
            : base("name=EX_DOCEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<Action> Action { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<DocFile> DocFile { get; set; }
        public DbSet<DocFileBeforeAppr> DocFileBeforeAppr { get; set; }
        public DbSet<DocType> DocType { get; set; }
        public DbSet<GroupReview> GroupReview { get; set; }
        public DbSet<Issue> Issue { get; set; }
        public DbSet<Relation_Issue_Cust> Relation_Issue_Cust { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<Transaction> Transaction { get; set; }
        public DbSet<User_level> User_level { get; set; }
        public DbSet<V_ControlledDocument> V_ControlledDocument { get; set; }
        public DbSet<V_ExDoc_Employee_Info> V_ExDoc_Employee_Info { get; set; }
        public DbSet<V_ExDoc_Organization> V_ExDoc_Organization { get; set; }
        public DbSet<V_TransactionEmp> V_TransactionEmp { get; set; }
    }
}
