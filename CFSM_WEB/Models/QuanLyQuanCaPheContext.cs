using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CFSM_WEB.Models;

public partial class QuanLyQuanCaPheContext : DbContext
{
    public QuanLyQuanCaPheContext()
    {
    }

    public QuanLyQuanCaPheContext(DbContextOptions<QuanLyQuanCaPheContext> options)
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
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-8N1GJPH;Initial Catalog=QuanLyQuanCaPhe;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TBlog>(entity =>
        {
            entity.HasKey(e => e.MaBlog).HasName("PK__tBlog__86827C730E44E304");

            entity.ToTable("tBlog");

            entity.Property(e => e.AnhTieuDe).HasMaxLength(100);
            entity.Property(e => e.NoiDungBlog).HasMaxLength(2000);
            entity.Property(e => e.TieuDeBlog).HasMaxLength(100);
        });

        modelBuilder.Entity<TChiTietHd>(entity =>
        {
            entity.HasKey(e => e.MaCthd).HasName("PK__tChiTiet__1E4FA771A7713AFC");

            entity.ToTable("tChiTietHD", tb => tb.HasTrigger("trg_UpdateThanhTien"));

            entity.Property(e => e.MaCthd).HasColumnName("MaCTHD");
            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TongTien)
                .HasComputedColumnSql("([SoLuong]*[DonGia])", true)
                .HasColumnType("decimal(29, 2)");

            entity.HasOne(d => d.MaDoAnNavigation).WithMany(p => p.TChiTietHds)
                .HasForeignKey(d => d.MaDoAn)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tChiTietH__MaDoA__4E88ABD4");

            entity.HasOne(d => d.MaHoaDonNavigation).WithMany(p => p.TChiTietHds)
                .HasForeignKey(d => d.MaHoaDon)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tChiTietH__MaHoa__4D94879B");
        });

        modelBuilder.Entity<TDoAn>(entity =>
        {
            entity.HasKey(e => e.MaDoAn).HasName("PK__tDoAn__2DCF1067FD098BB8");

            entity.ToTable("tDoAn");

            entity.Property(e => e.AnhDoAn).HasMaxLength(100);
            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MoTaDoAn).HasMaxLength(100);
            entity.Property(e => e.TenDoAn).HasMaxLength(100);

            entity.HasOne(d => d.MaMenuNavigation).WithMany(p => p.TDoAns)
                .HasForeignKey(d => d.MaMenu)
                .HasConstraintName("FK__tDoAn__MaMenu__45F365D3");
        });

        modelBuilder.Entity<THoaDon>(entity =>
        {
            entity.HasKey(e => e.MaHoaDon).HasName("PK__tHoaDon__835ED13B19894BD9");

            entity.ToTable("tHoaDon");

            entity.Property(e => e.CachThanhToan).HasMaxLength(50);
            entity.Property(e => e.DiaChi).HasMaxLength(100);
            entity.Property(e => e.GhiChu).HasMaxLength(100);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.NgayLap)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SoDienThoai).HasMaxLength(15);
            entity.Property(e => e.ThanhTien).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TrangThaiHoaDon).HasMaxLength(50);

            entity.HasOne(d => d.MaKhachHangNavigation).WithMany(p => p.THoaDons)
                .HasForeignKey(d => d.MaKhachHang)
                .HasConstraintName("FK__tHoaDon__GhiChu__49C3F6B7");

            entity.HasOne(d => d.MaNhanVienNavigation).WithMany(p => p.THoaDons)
                .HasForeignKey(d => d.MaNhanVien)
                .HasConstraintName("FK__tHoaDon__MaNhanV__4AB81AF0");
        });

        modelBuilder.Entity<TKhachHang>(entity =>
        {
            entity.HasKey(e => e.MaKhachHang).HasName("PK__tKhachHa__88D2F0E5C94B3477");

            entity.ToTable("tKhachHang");

            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.HoTen).HasMaxLength(255);
            entity.Property(e => e.SoDienThoai).HasMaxLength(15);
            entity.Property(e => e.TenDangNhap).HasMaxLength(50);
            entity.Property(e => e.TenHienThi).HasMaxLength(100);
            entity.Property(e => e.TrangThai).HasDefaultValue(1);

            entity.HasOne(d => d.TenDangNhapNavigation).WithMany(p => p.TKhachHangs)
                .HasForeignKey(d => d.TenDangNhap)
                .HasConstraintName("FK__tKhachHan__TenDa__3B75D760");
        });

        modelBuilder.Entity<TMenu>(entity =>
        {
            entity.HasKey(e => e.MaMenu).HasName("PK__tMenu__0EBABE42FB03A8FC");

            entity.ToTable("tMenu");

            entity.Property(e => e.TenMenu).HasMaxLength(100);
        });

        modelBuilder.Entity<TNhanVien>(entity =>
        {
            entity.HasKey(e => e.MaNhanVien).HasName("PK__tNhanVie__77B2CA47E9CA04ED");

            entity.ToTable("tNhanVien");

            entity.Property(e => e.ChucVu).HasMaxLength(100);
            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.HoTen).HasMaxLength(255);
            entity.Property(e => e.SoDienThoai).HasMaxLength(15);
            entity.Property(e => e.TenDangNhap).HasMaxLength(50);
            entity.Property(e => e.TenHienThi).HasMaxLength(100);
            entity.Property(e => e.TrangThai).HasDefaultValue(1);

            entity.HasOne(d => d.TenDangNhapNavigation).WithMany(p => p.TNhanViens)
                .HasForeignKey(d => d.TenDangNhap)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tNhanVien__TenDa__3F466844");
        });

        modelBuilder.Entity<TTaiKhoan>(entity =>
        {
            entity.HasKey(e => e.TenDangNhap).HasName("PK__tTaiKhoa__55F68FC11C16BB11");

            entity.ToTable("tTaiKhoan");

            entity.Property(e => e.TenDangNhap).HasMaxLength(50);
            entity.Property(e => e.MatKhau).HasMaxLength(100);
            entity.Property(e => e.Salt).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
