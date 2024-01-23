using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ImportData.Models;

public partial class ImportDataContext : DbContext
{
    public ImportDataContext()
    {
    }

    public ImportDataContext(DbContextOptions<ImportDataContext> options)
        : base(options)
    {
    }

    public virtual DbSet<SchoolYear> SchoolYears { get; set; }

    public virtual DbSet<Score> Scores { get; set; }

    public virtual DbSet<Stdatum> Stdata { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=MSI\\EVE;Database=ImportData;User=sa;Password=12345;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SchoolYear>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SchoolYe__3214EC075051FC9E");

            entity.ToTable("SchoolYear");

            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Score>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Score__3214EC0701CEFCAD");

            entity.ToTable("Score");

            entity.Property(e => e.Score1).HasColumnName("Score");

            entity.HasOne(d => d.Student).WithMany(p => p.Scores)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK__Score__StudentId__5070F446");

            entity.HasOne(d => d.Subject).WithMany(p => p.Scores)
                .HasForeignKey(d => d.SubjectId)
                .HasConstraintName("FK__Score__SubjectId__5165187F");
        });

        modelBuilder.Entity<Stdatum>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__STData__32C52B99CD9D4654");

            entity.ToTable("STData");

            entity.Property(e => e.StudentId).HasMaxLength(50);
            entity.Property(e => e.Biology).HasMaxLength(50);
            entity.Property(e => e.Chemistry).HasMaxLength(50);
            entity.Property(e => e.CivicEducation).HasMaxLength(50);
            entity.Property(e => e.English).HasMaxLength(50);
            entity.Property(e => e.Geography).HasMaxLength(50);
            entity.Property(e => e.History).HasMaxLength(50);
            entity.Property(e => e.Literature).HasMaxLength(50);
            entity.Property(e => e.Mathematics).HasMaxLength(50);
            entity.Property(e => e.Physics).HasMaxLength(50);
            entity.Property(e => e.Province).HasMaxLength(50);
            entity.Property(e => e.Year).HasMaxLength(50);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Student__3214EC07346310E2");

            entity.ToTable("Student");

            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.StudentCode)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.SchoolYear).WithMany(p => p.Students)
                .HasForeignKey(d => d.SchoolYearId)
                .HasConstraintName("FK__Student__SchoolY__4D94879B");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Subject__3214EC07F13C5686");

            entity.ToTable("Subject");

            entity.Property(e => e.Code)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
