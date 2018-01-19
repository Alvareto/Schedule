using System;
using Microsoft.EntityFrameworkCore;

namespace ScheduleApp.Model
{
    public partial class ScheduleContext : DbContext
    {
        public virtual DbSet<DatePreference> DatePreference { get; set; }
        public virtual DbSet<History> History { get; set; }
        public virtual DbSet<HistoryShift> HistoryShift { get; set; }
        public virtual DbSet<Schedule> Schedule { get; set; }
        public virtual DbSet<Shift> Shift { get; set; }
        public virtual DbSet<SwitchRequest> SwitchRequest { get; set; }
        public virtual DbSet<SwitchShift> SwitchShift { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<PendingSwitch> PendingSwitch { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseNpgsql(@"Host=95.85.27.10;Database=schedule;Username=postgres;Password=rassuslab");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DatePreference>(entity =>
            {
                entity.ToTable("datepreference");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.IsPreffered).HasColumnName("is_preffered").HasDefaultValue(false);

                entity.Property(e => e.ShiftId).HasColumnName("shift_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.DatePreference)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("datepreference_user_id_fkey");

                entity.HasOne(d => d.Shift)
                    .WithMany(p => p.DatePreferences)
                    .HasForeignKey(d => d.ShiftId)
                    .HasConstraintName("datepreference_shift_id_fkey");
            });

            modelBuilder.Entity<History>(entity =>
            {
                entity.ToTable("history");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.HistoryShiftId).HasColumnName("history_shift_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.HistoryShift)
                    .WithMany(p => p.History)
                    .HasForeignKey(d => d.HistoryShiftId)
                    .HasConstraintName("history_history_shift_id_fkey");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.History)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("history_user_id_fkey");
            });

            modelBuilder.Entity<HistoryShift>(entity =>
            {
                entity.ToTable("historyshift");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Shiftdate).HasColumnName("shiftdate");
            });

            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.ToTable("schedule");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ShiftId).HasColumnName("shift_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Shift)
                    .WithMany(p => p.Templates)
                    .HasForeignKey(d => d.ShiftId)
                    .HasConstraintName("schedule_shift_id_fkey");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Schedule)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("schedule_user_id_fkey");
            });

            modelBuilder.Entity<Shift>(entity =>
            {
                entity.ToTable("shift");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IsShorterDay).HasColumnName("is_shorter_day").HasDefaultValue(false);

                entity.Property(e => e.ShiftDate).HasColumnName("shiftdate");
            });

            modelBuilder.Entity<PendingSwitch>(entity =>
            {
                entity.ToTable("pendingswitches");

                entity.Property(o => o.Id).HasColumnName("id");

                entity.Property(o => o.Status).HasColumnName("status");

                entity.Property(o => o.Date).HasColumnName("pending_switch");
                entity.Property(o => o.SwitchRequestId).HasColumnName("switch_request_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.PendingRequests)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("pendingswitches_user_id_fkey");

                entity.HasOne(d => d.SwitchRequest)
                    .WithMany(p => p.PendingSwitch)
                    .HasForeignKey(d => d.SwitchRequestId)
                    .HasConstraintName("pendingswitches_switch_request_id_fkey");

            });


            modelBuilder.Entity<SwitchRequest>(entity =>
            {
                entity.ToTable("switchrequest");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CurrentShiftId).HasColumnName("current_shift_id");

                entity.Property(e => e.HasBeenSwitched).HasColumnName("has_been_switched");

                entity.Property(e => e.IsBroadcast).HasColumnName("is_broadcast");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.RequestCreatedDate).HasColumnName("request_created_time");

                entity.Property(e => e.WishShiftId).HasColumnName("wish_shift_id");

                entity.Property(e => e.UserWishShiftId).HasColumnName("user_wish_shift_id");

                entity.HasOne(d => d.CurrentShift)
                    .WithMany(p => p.SwitchRequestCurrentShift)
                    .HasForeignKey(d => d.CurrentShiftId)
                    .HasConstraintName("switchrequest_current_shift_id_fkey");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.SwitchRequest)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("switchrequest_user_id_fkey");

                entity.HasOne(d => d.UserWishShift)
                    .WithMany(p => p.WishShiftRequests)
                    .HasForeignKey(d => d.UserWishShiftId)
                    .HasConstraintName("switchrequest_user_wish_shift_id_fkey");

                entity.HasOne(d => d.WishShift)
                    .WithMany(p => p.SwitchRequestWishShift)
                    .HasForeignKey(d => d.WishShiftId)
                    .HasConstraintName("switchrequest_wish_shift_id_fkey");
            });

            modelBuilder.Entity<SwitchShift>(entity =>
            {
                entity.ToTable("switchshift");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.NewUserDate).HasColumnName("new_user_date");

                entity.Property(e => e.NewUserId).HasColumnName("new_user_id");

                entity.Property(e => e.PrevUserDate).HasColumnName("prev_user_date");

                entity.Property(e => e.PrevUserId).HasColumnName("prev_user_id");

                entity.HasOne(d => d.NewUser)
                    .WithMany(p => p.SwitchshiftNewUser)
                    .HasForeignKey(d => d.NewUserId)
                    .HasConstraintName("switchshift_new_user_id_fkey");

                entity.HasOne(d => d.PrevUser)
                    .WithMany(p => p.SwitchshiftPrevUser)
                    .HasForeignKey(d => d.PrevUserId)
                    .HasConstraintName("switchshift_prev_user_id_fkey");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.HasIndex(e => new { e.Username, e.Email })
                    .HasName("userandmailconstraint")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Email).HasColumnName("email");

                entity.Property(e => e.FirstName).HasColumnName("firstname");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.LastName).HasColumnName("lastname");

                entity.Property(e => e.Password).HasColumnName("password");

                entity.Property(e => e.Role).HasColumnName("role");

                entity.Property(e => e.Username).HasColumnName("username");
            });
        }
    }
}
