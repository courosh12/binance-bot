﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Trading.Bot.Data;

namespace Trading.Bot.Data.Migrations
{
    [DbContext(typeof(TradeContext))]
    partial class TradeContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.11");

            modelBuilder.Entity("Trading.Bot.Data.BotEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("BotIdentifier")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Bots");
                });

            modelBuilder.Entity("Trading.Bot.Data.TradesEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("BotId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("ExecutionTime")
                        .HasColumnType("TEXT");

                    b.Property<int>("OrderType")
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("Price")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Quantity")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("BotId");

                    b.ToTable("Trades");
                });

            modelBuilder.Entity("Trading.Bot.Data.TradesEntity", b =>
                {
                    b.HasOne("Trading.Bot.Data.BotEntity", "Bot")
                        .WithMany("Trades")
                        .HasForeignKey("BotId");

                    b.Navigation("Bot");
                });

            modelBuilder.Entity("Trading.Bot.Data.BotEntity", b =>
                {
                    b.Navigation("Trades");
                });
#pragma warning restore 612, 618
        }
    }
}
