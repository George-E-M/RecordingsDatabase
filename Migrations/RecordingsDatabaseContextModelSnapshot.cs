﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RecordingsDatabase.Models;

namespace RecordingsDatabase.Migrations
{
    [DbContext(typeof(RecordingsDatabaseContext))]
    partial class RecordingsDatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024");

            modelBuilder.Entity("RecordingsDatabase.Models.Recording", b =>
                {
                    b.Property<string>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Rating");

                    b.Property<string>("RecordingLength");

                    b.Property<string>("Syllables");

                    b.Property<string>("Tag");

                    b.Property<string>("Uploaded");

                    b.Property<string>("Url");

                    b.Property<string>("Word");

                    b.HasKey("ID");

                    b.ToTable("Recording");
                });
#pragma warning restore 612, 618
        }
    }
}
