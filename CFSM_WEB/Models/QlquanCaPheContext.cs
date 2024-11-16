using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CFSM_WEB.Models;

public partial class QlquanCaPheContext : DbContext
{
    public QlquanCaPheContext()
    {
    }

    public QlquanCaPheContext(DbContextOptions<QlquanCaPheContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TBlog> TBlogs { get; set; }

    public virtual DbSet<TChiTietHd> TChiTietHds { get; set; }

    public virtual DbSet<TDoAn> TDoAns { get; set; }

    public virtual DbSet<THoaDon> THoaDons { get; set; }

    public virtual DbSet<TKhachHang> TKhachHangs { get; set; }

    public virtual DbSet<TMenu> TMenus { get; set; }

    public virtual DbSet<TNhanVien> TNhanViens { get; set; }

    public virtual DbSet<TTaiKhoan> TTaiKhoans { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-8N1GJPH;Initial Catalog=QLQuanCaPhe;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TBlog>(entity =>
        {
            entity.HasKey(e => e.MaBlog).HasName("PK__tBlog__86827C7362E86398");

            entity.ToTable("tBlog");

            entity.Property(e => e.AnhTieuDe).HasMaxLength(100);
            entity.Property(e => e.NoiDungBlog).HasMaxLength(2000);
            entity.Property(e => e.TieuDeBlog).HasMaxLength(100);
        });

        modelBuilder.Entity<TChiTietHd>(entity =>
        {
            entity.HasKey(e => e.MaCthd).HasName("PK__tChiTiet__1E4FA771450B6B9C");

            entity.ToTable("tChiTietHD", tb => tb.HasTrigger("trg_UpdateThanhTien"));

            entity.Property(e => e.MaCthd).HasColumnName("MaCTHD");
            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TongTien)
                .HasComputedColumnSql("([SoLuong]*[DonGia])", true)
                .HasColumnType("decimal(29, 2)");

            entity.HasOne(d => d.MaDoAnNavigation).WithMany(p => p.TChiTietHds)
                .HasForeignKey(d => d.MaDoAn)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tChiTietH__MaDoA__4BAC3F29");

            entity.HasOne(d => d.MaHoaDonNavigation).WithMany(p => p.TChiTietHds)
                .HasForeignKey(d => d.MaHoaDon)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tChiTietH__MaHoa__4AB81AF0");
        });

        modelBuilder.Entity<TDoAn>(entity =>
        {
            entity.HasKey(e => e.MaDoAn).HasName("PK__tDoAn__2DCF106702CC7ED2");

            entity.ToTable("tDoAn");

            entity.Property(e => e.AnhDoAn).HasMaxLength(100);
            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MoTaDoAn).HasMaxLength(100);
            entity.Property(e => e.TenDoAn).HasMaxLength(100);

            entity.HasOne(d => d.MaMenuNavigation).WithMany(p => p.TDoAns)
                .HasForeignKey(d => d.MaMenu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tDoAn__MaMenu__440B1D61");
        });

        modelBuilder.Entity<THoaDon>(entity =>
        {
            entity.HasKey(e => e.MaHoaDon).HasName("PK__tHoaDon__835ED13B45CA6CBD");

            entity.ToTable("tHoaDon");

            entity.Property(e => e.NgayLap).HasColumnType("datetime");
            entity.Property(e => e.ThanhTien).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.MaKhachHangNavigation).WithMany(p => p.THoaDons)
                .HasForeignKey(d => d.MaKhachHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tHoaDon__MaKhach__46E78A0C");

            entity.HasOne(d => d.MaNhanVienNavigation).WithMany(p => p.THoaDons)
                .HasForeignKey(d => d.MaNhanVien)
                .HasConstraintName("FK__tHoaDon__MaNhanV__47DBAE45");
        });

        modelBuilder.Entity<TKhachHang>(entity =>
        {
            entity.HasKey(e => e.MaKhachHang).HasName("PK__tKhachHa__88D2F0E5100C3B69");

            entity.ToTable("tKhachHang");

            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.HoTen).HasMaxLength(255);
            entity.Property(e => e.SoDienThoai).HasMaxLength(15);
            entity.Property(e => e.TenDangNhap).HasMaxLength(50);
            entity.Property(e => e.TenHienThi).HasMaxLength(100);

            entity.HasOne(d => d.TenDangNhapNavigation).WithMany(p => p.TKhachHangs)
                .HasForeignKey(d => d.TenDangNhap)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tKhachHan__TenDa__3A81B327");
        });

        modelBuilder.Entity<TMenu>(entity =>
        {
            entity.HasKey(e => e.MaMenu).HasName("PK__tMenu__0EBABE42FD0E97A5");

            entity.ToTable("tMenu");

            entity.Property(e => e.TenMenu).HasMaxLength(100);
        });

        modelBuilder.Entity<TNhanVien>(entity =>
        {
            entity.HasKey(e => e.MaNhanVien).HasName("PK__tNhanVie__77B2CA47F26E4D76");

            entity.ToTable("tNhanVien");

            entity.Property(e => e.ChucVu).HasMaxLength(100);
            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.HoTen).HasMaxLength(255);
            entity.Property(e => e.SoDienThoai).HasMaxLength(15);
            entity.Property(e => e.TenDangNhap).HasMaxLength(50);
            entity.Property(e => e.TenHienThi).HasMaxLength(100);

            entity.HasOne(d => d.TenDangNhapNavigation).WithMany(p => p.TNhanViens)
                .HasForeignKey(d => d.TenDangNhap)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tNhanVien__TenDa__3D5E1FD2");
        });

        modelBuilder.Entity<TTaiKhoan>(entity =>
        {
            entity.HasKey(e => e.TenDangNhap).HasName("PK__tTaiKhoa__55F68FC1DDCF9B25");

            entity.ToTable("tTaiKhoan");

            entity.Property(e => e.TenDangNhap).HasMaxLength(50);
            entity.Property(e => e.MatKhau).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
