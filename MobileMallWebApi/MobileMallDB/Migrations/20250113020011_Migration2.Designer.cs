﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MobileMallDB;

#nullable disable

namespace MobileMallDB.Migrations
{
    [DbContext(typeof(EFDBContext))]
    [Migration("20250113020011_Migration2")]
    partial class Migration2
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.20")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("MobileMallDB.Models.Captcha", b =>
                {
                    b.Property<string>("key")
                        .HasMaxLength(256)
                        .HasColumnType("VARCHAR(256)");

                    b.Property<string>("base64")
                        .IsRequired()
                        .HasColumnType("LONGTEXT");

                    b.Property<string>("md5")
                        .HasMaxLength(256)
                        .HasColumnType("VARCHAR(256)");

                    b.HasKey("key");

                    b.ToTable("Captchas");
                });

            modelBuilder.Entity("MobileMallDB.Models.Comment", b =>
                {
                    b.Property<int>("comment_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("content")
                        .IsRequired()
                        .HasColumnType("LONGTEXT");

                    b.Property<DateTime>("create_time")
                        .HasColumnType("DATETIME");

                    b.Property<int>("goods_id")
                        .HasColumnType("int");

                    b.Property<int>("score")
                        .HasColumnType("int");

                    b.Property<int>("user_id")
                        .HasColumnType("int");

                    b.HasKey("comment_id");

                    b.HasIndex("goods_id");

                    b.HasIndex("user_id");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("MobileMallDB.Models.Goods", b =>
                {
                    b.Property<int>("goods_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("category_id")
                        .HasColumnType("int");

                    b.Property<string>("content")
                        .HasColumnType("LONGTEXT");

                    b.Property<string>("goods_image")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("VARCHAR(256)");

                    b.Property<string>("goods_image_name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("VARCHAR(256)");

                    b.Property<string>("goods_name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("VARCHAR(256)");

                    b.Property<decimal>("goods_price_max")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<decimal>("goods_price_min")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<int>("status")
                        .HasColumnType("int");

                    b.HasKey("goods_id");

                    b.HasIndex("category_id");

                    b.HasIndex("goods_name")
                        .HasDatabaseName("Index_GoodsName");

                    b.ToTable("Goods");
                });

            modelBuilder.Entity("MobileMallDB.Models.Goods_Category", b =>
                {
                    b.Property<int>("category_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("imgUrl")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("VARCHAR(256)");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("VARCHAR(256)");

                    b.HasKey("category_id");

                    b.ToTable("Goods_Categorys");
                });

            modelBuilder.Entity("MobileMallDB.Models.Goods_File", b =>
                {
                    b.Property<int>("file_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("external_url")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("VARCHAR(256)");

                    b.Property<string>("file_ext")
                        .HasMaxLength(20)
                        .HasColumnType("VARCHAR(20)");

                    b.Property<int?>("file_size")
                        .HasColumnType("int");

                    b.Property<int>("file_type")
                        .HasColumnType("int");

                    b.Property<int>("goods_id")
                        .HasColumnType("int");

                    b.Property<string>("image_name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("VARCHAR(256)");

                    b.Property<DateTime>("update_time")
                        .HasColumnType("DATETIME");

                    b.HasKey("file_id");

                    b.HasIndex("goods_id");

                    b.ToTable("Goods_Files");
                });

            modelBuilder.Entity("MobileMallDB.Models.User", b =>
                {
                    b.Property<int>("user_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("avatar_url")
                        .HasMaxLength(256)
                        .HasColumnType("VARCHAR(256)");

                    b.Property<decimal>("balance")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<string>("city")
                        .HasMaxLength(256)
                        .HasColumnType("VARCHAR(256)");

                    b.Property<string>("district")
                        .HasMaxLength(256)
                        .HasColumnType("VARCHAR(256)");

                    b.Property<int>("gender")
                        .HasColumnType("int");

                    b.Property<DateTime>("last_login_time")
                        .HasColumnType("DATETIME");

                    b.Property<string>("mobile")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("VARCHAR(20)");

                    b.Property<string>("nick_name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("VARCHAR(256)");

                    b.Property<decimal>("pay_money")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<string>("province")
                        .HasMaxLength(256)
                        .HasColumnType("VARCHAR(256)");

                    b.HasKey("user_id");

                    b.HasIndex("nick_name")
                        .HasDatabaseName("Index_Nickname");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("MobileMallDB.Models.Comment", b =>
                {
                    b.HasOne("MobileMallDB.Models.Goods", "Goods")
                        .WithMany("Comments")
                        .HasForeignKey("goods_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MobileMallDB.Models.User", "Users")
                        .WithMany("Comments")
                        .HasForeignKey("user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Goods");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("MobileMallDB.Models.Goods", b =>
                {
                    b.HasOne("MobileMallDB.Models.Goods_Category", "Goods_Categorys")
                        .WithMany("Goods")
                        .HasForeignKey("category_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Goods_Categorys");
                });

            modelBuilder.Entity("MobileMallDB.Models.Goods_File", b =>
                {
                    b.HasOne("MobileMallDB.Models.Goods", "Goods")
                        .WithMany("Goods_Files")
                        .HasForeignKey("goods_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Goods");
                });

            modelBuilder.Entity("MobileMallDB.Models.Goods", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Goods_Files");
                });

            modelBuilder.Entity("MobileMallDB.Models.Goods_Category", b =>
                {
                    b.Navigation("Goods");
                });

            modelBuilder.Entity("MobileMallDB.Models.User", b =>
                {
                    b.Navigation("Comments");
                });
#pragma warning restore 612, 618
        }
    }
}
