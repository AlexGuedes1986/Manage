﻿// <auto-generated />
using System;
using BioTech.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BioTech.Migrations
{
    [DbContext(typeof(BioTechContext))]
    partial class BioTechContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("biotech")
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("BioTech.Models.BillingRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description");

                    b.Property<int>("HourlyRate");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("BillingRole");
                });

            modelBuilder.Entity("BioTech.Models.Company", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AddressLine1");

                    b.Property<string>("AddressLine2");

                    b.Property<string>("City");

                    b.Property<string>("CompanyFax");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<string>("Phone");

                    b.Property<string>("State");

                    b.Property<string>("ZipCode");

                    b.HasKey("Id");

                    b.ToTable("Company");
                });

            modelBuilder.Entity("BioTech.Models.Contact", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("BusinessPhone");

                    b.Property<int>("CompanyId");

                    b.Property<string>("ContactType")
                        .IsRequired();

                    b.Property<string>("EmailAddress")
                        .IsRequired();

                    b.Property<string>("EmailAddress2");

                    b.Property<string>("FirstName")
                        .IsRequired();

                    b.Property<string>("JobTitle");

                    b.Property<string>("LastName")
                        .IsRequired();

                    b.Property<string>("MiddleName");

                    b.Property<string>("MobilePhone");

                    b.Property<string>("Suffix");

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.ToTable("Contact");
                });

            modelBuilder.Entity("BioTech.Models.InvoiceProject", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("FromDate");

                    b.Property<string>("ProjectNumber");

                    b.Property<string>("ProjectTitle");

                    b.Property<int>("RecordCount");

                    b.Property<DateTime>("ToDate");

                    b.HasKey("Id");

                    b.ToTable("InvoiceProject");
                });

            modelBuilder.Entity("BioTech.Models.InvoiceTimesheetEntry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("Amount");

                    b.Property<decimal>("Contract");

                    b.Property<DateTime>("DateModified");

                    b.Property<string>("EmployeeId");

                    b.Property<int?>("InvoiceProjectId");

                    b.Property<string>("Note");

                    b.Property<decimal>("Prev");

                    b.Property<int>("Qty");

                    b.Property<decimal>("RateUsed");

                    b.Property<string>("TaskName");

                    b.Property<string>("TaskNumber");

                    b.Property<decimal>("TotalPercentage");

                    b.HasKey("Id");

                    b.HasIndex("InvoiceProjectId");

                    b.ToTable("InvoiceTimesheetEntry");
                });

            modelBuilder.Entity("BioTech.Models.Proposal", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AuthorId");

                    b.Property<string>("BTCOffice")
                        .HasColumnName("BTCOffice");

                    b.Property<int>("ContactId");

                    b.Property<DateTime?>("ContractDate");

                    b.Property<string>("ContractNumber");

                    b.Property<DateTime?>("DateActive");

                    b.Property<DateTime>("DateCreated");

                    b.Property<string>("ImageUrl");

                    b.Property<bool>("IsActive");

                    b.Property<bool>("PdfCreated");

                    b.Property<DateTime?>("PdfDateCreated");

                    b.Property<string>("ProjectCounty");

                    b.Property<string>("ProjectName");

                    b.Property<string>("ProjectNumber");

                    b.Property<string>("ProjectStatus");

                    b.Property<string>("SecondParagraphCoverLetter");

                    b.HasKey("Id");

                    b.HasIndex("ContactId");

                    b.ToTable("Proposal");
                });

            modelBuilder.Entity("BioTech.Models.ProposalApplicationUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ApplicationUserId")
                        .IsRequired();

                    b.Property<string>("FormattedName");

                    b.Property<int>("ProposalId");

                    b.Property<string>("Type");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId");

                    b.HasIndex("Id")
                        .IsUnique()
                        .HasName("AK_ProposalApplicationUser_Id");

                    b.ToTable("ProposalApplicationUser");
                });

            modelBuilder.Entity("BioTech.Models.ProposalTeamMember", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("FormattedName");

                    b.Property<int>("ProposalId");

                    b.Property<string>("TeamMemberId");

                    b.HasKey("Id");

                    b.ToTable("ProposalTeamMember");
                });

            modelBuilder.Entity("BioTech.Models.TaskBioTech", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Category");

                    b.Property<int>("TaskCodeParent");

                    b.Property<int>("TaskCodeSub");

                    b.Property<string>("TaskDescription");

                    b.Property<string>("TaskTitle");

                    b.HasKey("Id");

                    b.ToTable("TaskBioTech");
                });

            modelBuilder.Entity("BioTech.Models.TaskExtension", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("AlreadyAddedToTimesheet");

                    b.Property<string>("AssignedEmployeeId");

                    b.Property<string>("Category");

                    b.Property<DateTime?>("DueDate");

                    b.Property<string>("FeeStructure")
                        .IsRequired();

                    b.Property<string>("FormattedNameAssignedTo");

                    b.Property<decimal>("InstancePrice")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<string>("IntervalType");

                    b.Property<decimal>("NotToExceedTotalPrice");

                    b.Property<string>("Note");

                    b.Property<int>("NumberOfInstances");

                    b.Property<string>("PercentageCompleted");

                    b.Property<int>("ProposalId");

                    b.Property<bool>("RequireComment");

                    b.Property<string>("Status");

                    b.Property<int>("TaskCodeParent");

                    b.Property<int>("TaskCodeSub");

                    b.Property<bool>("TaskCompleted");

                    b.Property<string>("TaskDescription");

                    b.Property<string>("TaskTitle");

                    b.Property<decimal>("TotalPrice")
                        .HasColumnType("decimal(18, 2)");

                    b.HasKey("Id");

                    b.HasIndex("ProposalId");

                    b.ToTable("TaskExtension");
                });

            modelBuilder.Entity("BioTech.Models.TaskUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Active");

                    b.Property<bool>("DisplayInTimesheet");

                    b.Property<DateTime?>("EffectiveDate");

                    b.Property<int>("TaskExtensionId");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("TaskExtensionId");

                    b.ToTable("TaskUser");
                });

            modelBuilder.Entity("BioTech.Models.TimesheetApprovedDateRange", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("DateEnd");

                    b.Property<DateTime?>("DateStart");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.ToTable("TimesheetApprovedDateRange");
                });

            modelBuilder.Entity("BioTech.Models.TimesheetEntry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Comment");

                    b.Property<DateTime>("DateModified");

                    b.Property<int>("HourlyRate");

                    b.Property<float>("HoursWorked");

                    b.Property<bool>("IsApproved");

                    b.Property<int>("TaskUserId");

                    b.HasKey("Id");

                    b.HasIndex("TaskUserId");

                    b.ToTable("TimesheetEntry");
                });

            modelBuilder.Entity("BioTech.Models.TouchLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ContactId");

                    b.Property<DateTime>("Date");

                    b.Property<string>("Employee");

                    b.Property<string>("Log")
                        .IsRequired();

                    b.Property<string>("Type")
                        .IsRequired();

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("ContactId");

                    b.ToTable("TouchLog");
                });

            modelBuilder.Entity("BioTech.Models.Contact", b =>
                {
                    b.HasOne("BioTech.Models.Company", "Company")
                        .WithMany("Contact")
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("BioTech.Models.InvoiceTimesheetEntry", b =>
                {
                    b.HasOne("BioTech.Models.InvoiceProject")
                        .WithMany("InvoiceTimesheetEntries")
                        .HasForeignKey("InvoiceProjectId");
                });

            modelBuilder.Entity("BioTech.Models.Proposal", b =>
                {
                    b.HasOne("BioTech.Models.Contact", "Contact")
                        .WithMany("Proposal")
                        .HasForeignKey("ContactId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("BioTech.Models.TaskExtension", b =>
                {
                    b.HasOne("BioTech.Models.Proposal", "Proposal")
                        .WithMany("TaskExtension")
                        .HasForeignKey("ProposalId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("BioTech.Models.TaskUser", b =>
                {
                    b.HasOne("BioTech.Models.TaskExtension", "TaskExtension")
                        .WithMany()
                        .HasForeignKey("TaskExtensionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("BioTech.Models.TimesheetEntry", b =>
                {
                    b.HasOne("BioTech.Models.TaskUser", "TaskUser")
                        .WithMany("TimesheetEntries")
                        .HasForeignKey("TaskUserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("BioTech.Models.TouchLog", b =>
                {
                    b.HasOne("BioTech.Models.Contact", "Contact")
                        .WithMany("TouchLog")
                        .HasForeignKey("ContactId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
