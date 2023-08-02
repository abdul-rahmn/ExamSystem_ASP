using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ExamsSystem.Models
{
    public partial class ExamsSystemContext : DbContext
    {
        public ExamsSystemContext()
        {
        }

        public ExamsSystemContext(DbContextOptions<ExamsSystemContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AspNetRole> AspNetRoles { get; set; } = null!;
        public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; } = null!;
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; } = null!;
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; } = null!;
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; } = null!;
        public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; } = null!;
        public virtual DbSet<Certificate> Certificates { get; set; } = null!;
        public virtual DbSet<Course> Courses { get; set; } = null!;
        public virtual DbSet<Exam> Exams { get; set; } = null!;
        public virtual DbSet<ExamStudent> ExamStudents { get; set; } = null!;
        public virtual DbSet<Question> Questions { get; set; } = null!;
        public virtual DbSet<Result> Results { get; set; } = null!;
        public virtual DbSet<UserAnswer> UserAnswers { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                //optionsBuilder.UseSqlServer(@"Data Source=DESKTOP-GG26D4D;Initial Catalog=ExamsSystem;Integrated Security=True;TrustServerCertificate=True");
                optionsBuilder.UseSqlServer("Data Source=abdulrahman;Initial Catalog=ExamsSystem;Integrated Security=True;Trust Server Certificate=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRole>(entity =>
            {
                entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedName] IS NOT NULL)");

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetRoleClaim>(entity =>
            {
                entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AspNetUser>(entity =>
            {
                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.UserName).HasMaxLength(256);

                entity.HasMany(d => d.Roles)
                    .WithMany(p => p.Users)
                    .UsingEntity<Dictionary<string, object>>(
                        "AspNetUserRole",
                        l => l.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                        r => r.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_AspNetUserRoles_AspNetUsers"),
                        j =>
                        {
                            j.HasKey("UserId", "RoleId");

                            j.ToTable("AspNetUserRoles");

                            j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                        });
            });

            modelBuilder.Entity<AspNetUserClaim>(entity =>
            {
                entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AspNetUserClaims_AspNetUsers");
            });

            modelBuilder.Entity<AspNetUserLogin>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.ProviderKey).HasMaxLength(128);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AspNetUserLogins_AspNetUsers");
            });

            modelBuilder.Entity<AspNetUserToken>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.Name).HasMaxLength(128);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserTokens)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AspNetUserTokens_AspNetUsers");
            });

            modelBuilder.Entity<Certificate>(entity =>
            {
                entity.ToTable("Certificate");

                entity.Property(e => e.Date).HasColumnType("datetime");
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.Property(e => e.CourseName).HasMaxLength(100);
            });

            modelBuilder.Entity<Exam>(entity =>
            {
                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Exams)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Exams_Courses");
            });

            modelBuilder.Entity<ExamStudent>(entity =>
            {
                entity.Property(e => e.UserId).HasMaxLength(450);

                entity.HasOne(d => d.Exam)
                    .WithMany(p => p.ExamStudents)
                    .HasForeignKey(d => d.ExamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ExamStudents_Exams");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ExamStudents)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ExamStudents_AspNetUsers");
            });

            modelBuilder.Entity<Question>(entity =>
            {
                entity.Property(e => e.Answer).HasColumnType("text");

                entity.Property(e => e.Option1).HasColumnType("text");

                entity.Property(e => e.Option2).HasColumnType("text");

                entity.Property(e => e.Option3).HasColumnType("text");

                entity.Property(e => e.Option4).HasColumnType("text");

                entity.Property(e => e.Question1)
                    .HasColumnType("text")
                    .HasColumnName("Question");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Questions)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Questions_Courses");
            });

            modelBuilder.Entity<Result>(entity =>
            {
                entity.Property(e => e.UserId).HasMaxLength(450);

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Results)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Results_Courses");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Results)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Results_AspNetUsers");
            });

            modelBuilder.Entity<UserAnswer>(entity =>
            {
                entity.Property(e => e.UserAnswer1)
                    .HasColumnType("text")
                    .HasColumnName("UserAnswer");

                entity.Property(e => e.UserId).HasMaxLength(450);

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.UserAnswers)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserAnswers_Questions");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserAnswers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserAnswers_AspNetUsers");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
