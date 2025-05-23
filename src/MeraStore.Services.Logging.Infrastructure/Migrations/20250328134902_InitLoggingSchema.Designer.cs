﻿// <auto-generated />
using System;
using MeraStore.Services.Logging.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MeraStore.Services.Logging.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250328134902_InitLoggingSchema")]
    partial class InitLoggingSchema
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("MeraStore.Services.Logging.Domain.Models.ApiRequestLog", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(26)
                        .HasColumnType("nvarchar(26)");

                    b.Property<string>("ContentType")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("CorrelationId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("HttpMethod")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<byte[]>("Payload")
                        .HasColumnType("VARBINARY(MAX)");

                    b.Property<DateTime>("Timestamp")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CorrelationId");

                    b.HasIndex("Timestamp");

                    b.HasIndex("HttpMethod", "Timestamp");

                    b.ToTable("Requests");
                });

            modelBuilder.Entity("MeraStore.Services.Logging.Domain.Models.ApiResponseLog", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(26)
                        .HasColumnType("nvarchar(26)");

                    b.Property<string>("ContentType")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("CorrelationId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<byte[]>("Payload")
                        .HasColumnType("VARBINARY(MAX)");

                    b.Property<Guid>("RequestId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("StatusCode")
                        .HasColumnType("int");

                    b.Property<DateTime>("Timestamp")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.HasKey("Id");

                    b.HasIndex("CorrelationId");

                    b.HasIndex("StatusCode");

                    b.HasIndex("Timestamp");

                    b.HasIndex("RequestId", "Timestamp");

                    b.ToTable("Responses");
                });
#pragma warning restore 612, 618
        }
    }
}
