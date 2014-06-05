﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace YiTao.Modules.Bll.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class YiTaoContext : DbContext
    {
        public YiTaoContext()
            : base("name=YiTaoContext")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<AppImage> AppImages { get; set; }
        public virtual DbSet<Area> Areas { get; set; }
        public virtual DbSet<ChouJiangItem> ChouJiangItems { get; set; }
        public virtual DbSet<Collection> Collections { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<DaLei> DaLeis { get; set; }
        public virtual DbSet<DingDan> DingDans { get; set; }
        public virtual DbSet<DuiHuanItem> DuiHuanItems { get; set; }
        public virtual DbSet<HaoDian> HaoDians { get; set; }
        public virtual DbSet<JiFenLiShi> JiFenLiShis { get; set; }
        public virtual DbSet<JuZheKouItem> JuZheKouItems { get; set; }
        public virtual DbSet<LiuYan> LiuYans { get; set; }
        public virtual DbSet<LiuYanComment> LiuYanComments { get; set; }
        public virtual DbSet<LunBo> LunBoes { get; set; }
        public virtual DbSet<Management> Managements { get; set; }
        public virtual DbSet<OtherUrl> OtherUrls { get; set; }
        public virtual DbSet<ShangPin> ShangPins { get; set; }
        public virtual DbSet<ShangPin2Areas> ShangPin2Areas { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<ThreeLei> ThreeLeis { get; set; }
        public virtual DbSet<TowLei> TowLeis { get; set; }
        public virtual DbSet<TuiSong> TuiSongs { get; set; }
        public virtual DbSet<UserAddress> UserAddresses { get; set; }
        public virtual DbSet<VoteAccount> VoteAccounts { get; set; }
        public virtual DbSet<VoteItem> VoteItems { get; set; }
        public virtual DbSet<VoteTopic> VoteTopics { get; set; }
        public virtual DbSet<ZhuanTi> ZhuanTis { get; set; }
        public virtual DbSet<ZhuanTiItem> ZhuanTiItems { get; set; }
    }
}
