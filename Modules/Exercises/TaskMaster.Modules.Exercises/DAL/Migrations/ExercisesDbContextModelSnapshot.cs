﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TaskMaster.Modules.Exercises.DAL;

#nullable disable

namespace TaskMaster.Modules.Exercises.DAL.Migrations
{
    [DbContext(typeof(ExercisesDbContext))]
    partial class ExercisesDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("Exercises")
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("TaskMaster.Modules.Exercises.Entities.Essay", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("ExerciseHeaderInMotherLanguage")
                        .HasColumnType("boolean");

                    b.Property<string>("GrammarSection")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("MotherLanguage")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("TargetLanguage")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("TargetLanguageLevel")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("TopicsOfSentences")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<bool>("VerifiedByTeacher")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("Essays", "Exercises");
                });

            modelBuilder.Entity("TaskMaster.Modules.Exercises.Entities.Mail", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("ExerciseHeaderInMotherLanguage")
                        .HasColumnType("boolean");

                    b.Property<string>("GrammarSection")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("MotherLanguage")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("TargetLanguage")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("TargetLanguageLevel")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("TopicsOfSentences")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<bool>("VerifiedByTeacher")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("Mails", "Exercises");
                });

            modelBuilder.Entity("TaskMaster.Modules.Exercises.Entities.SummaryOfText", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("ExerciseHeaderInMotherLanguage")
                        .HasColumnType("boolean");

                    b.Property<string>("GrammarSection")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("MotherLanguage")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("TargetLanguage")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("TargetLanguageLevel")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("TopicsOfSentences")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<bool>("VerifiedByTeacher")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("SummariesOfText", "Exercises");
                });

            modelBuilder.Entity("TaskMaster.Modules.Exercises.Entities.Essay", b =>
                {
                    b.OwnsOne("TaskMaster.Models.Exercises.OpenForm.Essay", "Exercise", b1 =>
                        {
                            b1.Property<Guid>("EssayId")
                                .HasColumnType("uuid");

                            b1.HasKey("EssayId");

                            b1.ToTable("Essays", "Exercises");

                            b1.ToJson("Exercise");

                            b1.WithOwner()
                                .HasForeignKey("EssayId");

                            b1.OwnsOne("TaskMaster.Models.Exercises.Base.Exercise+ExerciseHeader", "Header", b2 =>
                                {
                                    b2.Property<Guid>("EssayId")
                                        .HasColumnType("uuid");

                                    b2.Property<string>("Example")
                                        .IsRequired()
                                        .HasColumnType("text");

                                    b2.Property<string>("Instruction")
                                        .IsRequired()
                                        .HasColumnType("text");

                                    b2.Property<string>("SupportMaterial")
                                        .IsRequired()
                                        .HasColumnType("text");

                                    b2.Property<string>("TaskDescription")
                                        .IsRequired()
                                        .HasColumnType("text");

                                    b2.Property<string>("Title")
                                        .IsRequired()
                                        .HasColumnType("text");

                                    b2.HasKey("EssayId");

                                    b2.ToTable("Essays", "Exercises");

                                    b2.WithOwner()
                                        .HasForeignKey("EssayId");
                                });

                            b1.Navigation("Header")
                                .IsRequired();
                        });

                    b.Navigation("Exercise")
                        .IsRequired();
                });

            modelBuilder.Entity("TaskMaster.Modules.Exercises.Entities.Mail", b =>
                {
                    b.OwnsOne("TaskMaster.Models.Exercises.OpenForm.Mail", "Exercise", b1 =>
                        {
                            b1.Property<Guid>("MailId")
                                .HasColumnType("uuid");

                            b1.HasKey("MailId");

                            b1.ToTable("Mails", "Exercises");

                            b1.ToJson("Exercise");

                            b1.WithOwner()
                                .HasForeignKey("MailId");

                            b1.OwnsOne("TaskMaster.Models.Exercises.Base.Exercise+ExerciseHeader", "Header", b2 =>
                                {
                                    b2.Property<Guid>("MailId")
                                        .HasColumnType("uuid");

                                    b2.Property<string>("Example")
                                        .IsRequired()
                                        .HasColumnType("text");

                                    b2.Property<string>("Instruction")
                                        .IsRequired()
                                        .HasColumnType("text");

                                    b2.Property<string>("SupportMaterial")
                                        .IsRequired()
                                        .HasColumnType("text");

                                    b2.Property<string>("TaskDescription")
                                        .IsRequired()
                                        .HasColumnType("text");

                                    b2.Property<string>("Title")
                                        .IsRequired()
                                        .HasColumnType("text");

                                    b2.HasKey("MailId");

                                    b2.ToTable("Mails", "Exercises");

                                    b2.WithOwner()
                                        .HasForeignKey("MailId");
                                });

                            b1.Navigation("Header")
                                .IsRequired();
                        });

                    b.Navigation("Exercise")
                        .IsRequired();
                });

            modelBuilder.Entity("TaskMaster.Modules.Exercises.Entities.SummaryOfText", b =>
                {
                    b.OwnsOne("TaskMaster.Models.Exercises.OpenForm.SummaryOfText", "Exercise", b1 =>
                        {
                            b1.Property<Guid>("SummaryOfTextId")
                                .HasColumnType("uuid");

                            b1.Property<string>("TextToSummary")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.HasKey("SummaryOfTextId");

                            b1.ToTable("SummariesOfText", "Exercises");

                            b1.ToJson("Exercise");

                            b1.WithOwner()
                                .HasForeignKey("SummaryOfTextId");

                            b1.OwnsOne("TaskMaster.Models.Exercises.Base.Exercise+ExerciseHeader", "Header", b2 =>
                                {
                                    b2.Property<Guid>("SummaryOfTextId")
                                        .HasColumnType("uuid");

                                    b2.Property<string>("Example")
                                        .IsRequired()
                                        .HasColumnType("text");

                                    b2.Property<string>("Instruction")
                                        .IsRequired()
                                        .HasColumnType("text");

                                    b2.Property<string>("SupportMaterial")
                                        .IsRequired()
                                        .HasColumnType("text");

                                    b2.Property<string>("TaskDescription")
                                        .IsRequired()
                                        .HasColumnType("text");

                                    b2.Property<string>("Title")
                                        .IsRequired()
                                        .HasColumnType("text");

                                    b2.HasKey("SummaryOfTextId");

                                    b2.ToTable("SummariesOfText", "Exercises");

                                    b2.WithOwner()
                                        .HasForeignKey("SummaryOfTextId");
                                });

                            b1.Navigation("Header")
                                .IsRequired();
                        });

                    b.Navigation("Exercise")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
