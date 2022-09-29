using System;
using Manage.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BioTech.Models
{
    public partial class BioTechContext : DbContext
    {
        public BioTechContext()
        {
        }

        public BioTechContext(DbContextOptions<BioTechContext> options)
            : base(options)
        {
        }

        public virtual DbSet<BillingRole> BillingRole { get; set; }
        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<Contact> Contact { get; set; }
        public virtual DbSet<Proposal> Proposal { get; set; }
        public virtual DbSet<ProposalApplicationUser> ProposalApplicationUser { get; set; }        
        public virtual DbSet<ProjectTaskUserAssigned> ProjectTaskUserAssigned { get; set; }
        public virtual DbSet<TaskBioTech> TaskBioTech { get; set; }
        public virtual DbSet<TaskExtension> TaskExtension { get; set; }
        public virtual DbSet<TouchLog> TouchLog { get; set; }
        public virtual DbSet<TaskUser> TaskUser { get; set; }
        public virtual DbSet<TimesheetEntry> TimesheetEntry { get; set; }
        public virtual DbSet<TimesheetApprovedDateRange> TimesheetApprovedDateRange { get; set; }
        public virtual DbSet<InvoiceProject> InvoiceProject { get; set; }
        public virtual DbSet<ContractNumbers> ContractNumbers { get; set; }
        public virtual DbSet<InvoiceTimesheetEntry> InvoiceTimesheetEntry { get; set; }
        public virtual DbSet<PartialPayment> PartialPayment { get; set; }
        public virtual DbSet<ContactClub> ContactClub { get; set; }
        public virtual DbSet<Call> Call { get; set; }
        public virtual DbSet<Country> Country { get; set; }
        public virtual DbSet<League> League { get; set; }
        public virtual DbSet<Team> Team { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
//            if (!optionsBuilder.IsConfigured)
//            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
//                optionsBuilder.UseSqlServer("Server=biotech.bssdev.com\\MSSQLSERVER2012;Database=biotech;User ID=biotech;password=vus99w85");
//            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:DefaultSchema", "dbo");

            modelBuilder.Entity<Company>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Contact>(entity =>
            {
                entity.HasIndex(e => e.CompanyId);

                entity.Property(e => e.ContactType).IsRequired();

                entity.Property(e => e.EmailAddress).IsRequired();

                entity.Property(e => e.FirstName).IsRequired();

                entity.Property(e => e.LastName).IsRequired();

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Contact)
                    .HasForeignKey(d => d.CompanyId);
            });

            modelBuilder.Entity<Proposal>(entity =>
            {
                entity.HasIndex(e => e.ContactId);

                entity.Property(e => e.BTCOffice).HasColumnName("BTCOffice");

                entity.HasOne(d => d.Contact)
                    .WithMany(p => p.Proposal)
                    .HasForeignKey(d => d.ContactId);
            });

            modelBuilder.Entity<ProposalApplicationUser>(entity =>
            {
                entity.HasIndex(e => e.ApplicationUserId);

                entity.HasIndex(e => e.Id)
                    .HasName("AK_ProposalApplicationUser_Id")
                    .IsUnique();

                entity.Property(e => e.ApplicationUserId).IsRequired();
            });

            modelBuilder.Entity<TaskExtension>(entity =>
            {
                entity.HasIndex(e => e.ProposalId);

                entity.Property(e => e.FeeStructure).IsRequired();

                entity.Property(e => e.InstancePrice).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Proposal)
                    .WithMany(p => p.TaskExtension)
                    .HasForeignKey(d => d.ProposalId);
            });

            modelBuilder.Entity<TouchLog>(entity =>
            {
                entity.HasIndex(e => e.ContactId);

                entity.HasOne(d => d.Contact)
                    .WithMany(p => p.TouchLog)
                    .HasForeignKey(d => d.ContactId);
            });        
        }
    }
}
