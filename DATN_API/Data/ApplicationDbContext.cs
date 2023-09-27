﻿using System.Data;

using DATN_Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DATN.Data
{
    public class ApplicationDbContext :IdentityDbContext<User,Role,Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {

        }
        public DbSet<AddressShip> AddressShips { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<BillItems> BillItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItems> CartItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<ConsumerPoint> ConsumerPoints { get; set; }
        public DbSet<Formula> Formulas { get; set; }
        public DbSet<HistoryConsumerPoint> HistoryConsumerPoints { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<ProductItems> ProductItems { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<Reviews> Reviews { get; set; }
        public DbSet<Promotions> Promotions { get; set; }
        public DbSet<PromotionsProduct> PromotionsProducts { get; set; }
      
        public DbSet<Size> Sizes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<VoucherBill> VoucherBills { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Cart>().HasOne(u => u.User).WithOne(c => c.Cart).HasForeignKey<Cart>(c => c.UserId);
            builder.Entity<Bill>().HasOne(u=>u.Users).WithMany(b=>b.Bills).HasForeignKey(b=>b.UserId);
            builder.Entity<BillItems>().HasOne(b => b.Bills).WithMany(bi => bi.BillItems).HasForeignKey(bi => bi.BillId);
            builder.Entity<VoucherBill>().HasOne(u => u.Users).WithMany(vb => vb.VoucherBills).HasForeignKey(vb => vb.UserId);
            builder.Entity<VoucherBill>().HasOne(v => v.Voucher).WithMany(vb => vb.Voucher_Bills).HasForeignKey(vb => vb.VoucherId);
            builder.Entity<AddressShip>().HasOne(u=>u.Users).WithMany(a=>a.AddressShips).HasForeignKey(a=>a.UserId);
            builder.Entity<Reviews>().HasOne(u=>u.Users).WithMany(r=>r.Reviews).HasForeignKey(r => r.UserId);
            builder.Entity<Image>().HasOne(r => r.Reviews).WithMany(i => i.Images).HasForeignKey(i => i.ReviewId).OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Image>().HasOne(pi=>pi.ProductItems).WithMany(i=>i.Images).HasForeignKey(i => i.ProductItemId);
            builder.Entity<ProductItems>().HasOne(c => c.Colors).WithMany(pi => pi.ProductItems).HasForeignKey(pi => pi.ColorId);
            builder.Entity<ProductItems>().HasOne(s => s.Size).WithMany(pi => pi.ProductItems).HasForeignKey(pi => pi.SizeId);
            builder.Entity<PromotionsProduct>().HasOne(pi => pi.ProductItems).WithMany(pp => pp.PromotionsProducts).HasForeignKey(pp => pp.ProductItemsId);
            builder.Entity<PromotionsProduct>().HasOne(p=>p.Promotions).WithMany(pp=>pp.PromotionsProducts).HasForeignKey(pp=>pp.PromotionsId);
            builder.Entity<ProductItems>().HasOne(p=>p.Products).WithMany(pi=>pi.ProductItems).HasForeignKey(pi=>pi.ProductId);
            builder.Entity<Products>().HasOne(c => c.Categorys).WithMany(p => p.Products).HasForeignKey(p => p.CategoryId);
            builder.Entity<CartItems>().HasOne(pi => pi.ProductItems).WithMany(ci => ci.CartItems).HasForeignKey(ci => ci.ProductItemId);
            builder.Entity<CartItems>().HasOne(c => c.Cart).WithMany(ci => ci.CartItems).HasForeignKey(ci => ci.CartId);
            builder.Entity<BillItems>().HasOne(pi => pi.ProductItems).WithMany(bi => bi.BillItems).HasForeignKey(bi => bi.ProductItemsId);
            builder.Entity<BillItems>().HasOne(b => b.Bills).WithMany(bi => bi.BillItems).HasForeignKey(bi => bi.BillId);
            builder.Entity<Bill>().HasOne(h => h.HistoryConsumerPoints).WithMany(b => b.Bills).HasForeignKey(b => b.HistoryConsumerPointID);
            builder.Entity<HistoryConsumerPoint>().HasOne(f => f.Formulas).WithMany(h => h.HistoryConsumerPoints).HasForeignKey(h => h.FormulaId);
            builder.Entity<HistoryConsumerPoint>().HasOne(c=>c.ConsumerPoints).WithMany(h=>h.HistoryConsumerPoints).HasForeignKey(h=>h.ConsumerPointId);
            builder.Entity<ConsumerPoint>().HasOne(c => c.Users).WithOne(u => u.ConsumerPoint).HasForeignKey<ConsumerPoint>(u=>u.UserID).OnDelete(DeleteBehavior.NoAction);

            base.OnModelCreating(builder);
            builder.Entity<User>().ToTable("AspNetUsers");
            builder.Entity<Role>().ToTable("AspNetRoles");
            CreateRoles(builder);

        }
        protected void CreateRoles(ModelBuilder builder)
        {
            builder.Entity<Role>().HasData(
                    new Role() { Id = Guid.NewGuid(), Name = "Admin", NormalizedName = "ADMIN" },
                    new Role() { Id = Guid.NewGuid(), Name = "User", NormalizedName = "USER" }
                ); 
        }
    }
}
