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
                optionsBuilder.UseSqlServer(@"data source=(localdb)\local;initial catalog=DealerContract;Integrated Security=true;");
                //optionsbuilder.usesqlserver(@"data source=prd-vn-hdesk01\sqlexpress;
                //    initial catalog=dealercontract;
                //    persist security info=true;
                //    user id=sa_dev;password=760119;
                //    multipleactiveresultsets=true;
                //    app=entityframework");
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
                entity.Property(e => e.BussinessId)
                    .IsRequired()
                    .HasColumnType("nchar(20)");

                entity.Property(e => e.DealerName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Delegate).HasMaxLength(50);

                entity.Property(e => e.EndEffective).HasColumnType("date");

                entity.Property(e => e.Fax).HasColumnType("nchar(20)");

                entity.Property(e => e.GroupName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Hqaddress)
                    .IsRequired()
                    .HasColumnName("HQAddress")
                    .HasMaxLength(50);

                entity.Property(e => e.Owner)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasColumnType("nchar(20)");

                entity.Property(e => e.Position)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.RegisteredName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Representative)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.StartEffective).HasColumnType("date");

                entity.Property(e => e.SubDelegate).HasMaxLength(50);

                entity.Property(e => e.TaxId)
                    .IsRequired()
                    .HasColumnType("nchar(20)");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnType("nchar(50)");

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
                    .HasColumnType("nchar(20)");

                entity.Property(e => e.Brand).HasColumnType("nchar(20)");

                entity.Property(e => e.PosCode).HasColumnType("nchar(20)");

                entity.Property(e => e.PosName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Province)
                    .IsRequired()
                    .HasColumnType("nchar(20)");

                entity.Property(e => e.Region)
                    .IsRequired()
                    .HasColumnType("nchar(20)");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnType("nchar(20)");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnType("nchar(50)");

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
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.UploadDate).HasColumnType("date");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnType("nchar(50)");

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
                    .HasColumnType("nchar(50)")
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
