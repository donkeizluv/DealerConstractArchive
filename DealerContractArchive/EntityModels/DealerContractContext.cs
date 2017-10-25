using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DealerContractArchive.EntityModels
{
    public partial class DealerContractContext : DbContext
    {
        public virtual DbSet<AccountType> AccountType { get; set; }
        public virtual DbSet<Dealer> Dealer { get; set; }
        public virtual DbSet<DealerGroup> DealerGroup { get; set; }
        public virtual DbSet<Document> Document { get; set; }
        public virtual DbSet<Pos> Pos { get; set; }
        public virtual DbSet<Scan> Scan { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=(LocalDb)\local;Database=DealerContract;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountType>(entity =>
            {
                entity.HasKey(e => e.Type);

                entity.Property(e => e.Type)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Dealer>(entity =>
            {
                entity.HasIndex(e => e.DealerId)
                    .HasName("IX_DealerId")
                    .IsUnique();

                entity.HasIndex(e => e.DealerName)
                    .HasName("IX_DealerName");

                entity.HasIndex(e => e.TaxId)
                    .HasName("IX_DealerTaxId");

                entity.Property(e => e.BussinessId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ContractNo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.DealerName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Delegate).HasMaxLength(50);

                entity.Property(e => e.EndEffective).HasColumnType("date");

                entity.Property(e => e.Fax)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.GroupName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Hqaddress)
                    .IsRequired()
                    .HasColumnName("HQAddress")
                    .HasMaxLength(100);

                entity.Property(e => e.Owner).HasMaxLength(50);

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Position)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.RegisteredName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Representative)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.SignDate).HasColumnType("date");

                entity.Property(e => e.StartEffective).HasColumnType("date");

                entity.Property(e => e.SubDelegate).HasMaxLength(50);

                entity.Property(e => e.TaxId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.GroupNameNavigation)
                    .WithMany(p => p.Dealer)
                    .HasForeignKey(d => d.GroupName)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Dealer_DealerGroup");

                entity.HasOne(d => d.UsernameNavigation)
                    .WithMany(p => p.Dealer)
                    .HasForeignKey(d => d.Username)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Dealer_Users");
            });

            modelBuilder.Entity<DealerGroup>(entity =>
            {
                entity.HasKey(e => e.GroupName);

                entity.HasIndex(e => e.GroupName)
                    .HasName("IX_DealerGroup")
                    .IsUnique();

                entity.Property(e => e.GroupName)
                    .HasMaxLength(50)
                    .ValueGeneratedNever();
            });

            modelBuilder.Entity<Document>(entity =>
            {
                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.Filename)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Pos>(entity =>
            {
                entity.ToTable("POS");

                entity.Property(e => e.Address).HasMaxLength(100);

                entity.Property(e => e.Bl)
                    .IsRequired()
                    .HasColumnName("BL")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Brand)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PosCode)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.PosName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Province)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Region)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Dealer)
                    .WithMany(p => p.Pos)
                    .HasForeignKey(d => d.DealerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_POS_Dealer");

                entity.HasOne(d => d.UsernameNavigation)
                    .WithMany(p => p.Pos)
                    .HasForeignKey(d => d.Username)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_POS_Users");
            });

            modelBuilder.Entity<Scan>(entity =>
            {
                entity.Property(e => e.FilePath)
                    .HasMaxLength(200);

                entity.Property(e => e.UploadDate).HasColumnType("date");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Dealer)
                    .WithMany(p => p.Scan)
                    .HasForeignKey(d => d.DealerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Scan_Dealer");

                entity.HasOne(d => d.UsernameNavigation)
                    .WithMany(p => p.Scan)
                    .HasForeignKey(d => d.Username)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Scan_Users");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(e => e.Username);

                entity.Property(e => e.Username)
                    .HasMaxLength(50)
                    .ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.TypeNavigation)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.Type)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Users_AccountType");
            });
        }
    }
}
