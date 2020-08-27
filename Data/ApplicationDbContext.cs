using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TaskManager.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ListEmployee>()
                        .HasOne(m => m.Project)
                        .WithMany(t => t.ListEmployee)
                        .HasForeignKey(m => m.ProjectId)
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ListEmployee>()
                        .HasOne(m => m.ApplicationUser)
                        .WithMany(t => t.ListEmployee)
                        .HasForeignKey(m => m.ApplicationUserId)
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Task>()
                        .HasOne(m => m.Appendix)
                        .WithMany(t => t.Task)
                        .HasForeignKey(m => m.AppendixId);

            modelBuilder.Entity<Module>()
                        .HasOne(m => m.Project)
                        .WithMany(t => t.Module)
                        .HasForeignKey(m => m.ProjectId)
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Task>()
                        .HasOne(m => m.Module)
                        .WithMany(t => t.Task)
                        .HasForeignKey(m => m.ModuleId)
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Task>()
                        .HasOne(m => m.ApplicationUser)
                        .WithMany(t => t.Task)
                        .HasForeignKey(m => m.ApplicationUserId)
                        .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Task>()
                        .HasOne(m => m.Status)
                        .WithMany(t => t.Task)
                        .HasForeignKey(m => m.StatusId)
                        .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ApplicationUser>()
                        .HasOne(m => m.Position)
                        .WithMany(t => t.ApplicationUsers)
                        .HasForeignKey(m => m.PositionId)
                        .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ApplicationUser>()
                        .HasOne(m => m.QualificationLevel)
                        .WithMany(t => t.ApplicationUsers)
                        .HasForeignKey(m => m.QualificationLevelId)
                       .OnDelete(DeleteBehavior.Cascade);
            base.OnModelCreating(modelBuilder);

        }



        public virtual DbSet<TaskManager.Data.ApplicationUser> ApplicationUsers { get; set; }

        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<Position> Positions { get; set; }
        public virtual DbSet<Status> Statuses { get; set; }
        public virtual DbSet<QualificationLevel> QualificationLevels { get; set; }
        public virtual DbSet<Module> Modules { get; set; }
        public virtual DbSet<Task> Tasks { get; set; }
        public virtual DbSet<ListEmployee> ListEmployees { get; set; }
        public virtual DbSet<Appendix> Appendices { get; set; }
        public virtual DbSet<Utils> Utils { get; set; }
    }


    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<ListEmployee> ListEmployee { get; set; }
        public virtual ICollection<Task> Task { get; set; }


        public int? PositionId { get; set; }
        public virtual Position Position { get; set; }


        public int? QualificationLevelId { get; set; }
        public virtual QualificationLevel QualificationLevel { get; set; }

    }

    public class Project
    {

        public int Id { get; set; }

        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Дата завершения")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }

        public virtual ICollection<ListEmployee> ListEmployee { get; set; }

        public virtual ICollection<Module> Module { get; set; }

        [Display(Name = "Дата создания")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreateDate { get; set; }

        [Display(Name = "Дата обновления")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime UpdateDate { get; set; }


    }

    public class Utils
    {
        public int Id { get; set; }

        [Display(Name = "Главный бухгалтер")]
        public string firstParameter { get; set; }
        [Display(Name = "Координатор разработки программного обеспечения и тех. задания")]
        public string secondParameter { get; set; }
        [Display(Name = "Специалист по управлению проектной деятельностью")]
        public string thirdParameter { get; set; }

        [Display(Name = "Дата создания")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreateDate { get; set; }

        [Display(Name = "Дата обновления")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime UpdateDate { get; set; }

    }

    public class ListEmployee
    {
        public int Id { get; set; }
        public String ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }

        [Display(Name = "Дата создания")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreateDate { get; set; }

        [Display(Name = "Дата обновления")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime UpdateDate { get; set; }

    }

    public class Module
    {
        public int Id { get; set; }

        [Display(Name = "Название")]
        public string Name { get; set; }

        public int ProjectId { get; set; }

        public virtual Project Project { get; set; }

        [Display(Name = "Дата завершения")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "Дата создания")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreateDate { get; set; }

        [Display(Name = "Дата обновления")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime UpdateDate { get; set; }
        public virtual ICollection<Task> Task { get; set; }
    }


    public class Appendix
    {
        public int Id { get; set; }

        [Display(Name = "Описание")]
        public string Name { get; set; }


        [Display(Name = "Название файла")]
        public string FileName { get; set; }


        public virtual ICollection<Task> Task { get; set; }

    }

    public class Task
    {
        public int Id { get; set; }
        [Display(Name = "Описание")]
        public string Name { get; set; }
        [Display(Name = "Причина невыполнения")]
        public string Note { get; set; }
        public int ModuleId { get; set; }
        public virtual Module Module { get; set; }
        public String ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        [Display(Name = "Дата завершения")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }
        [Display(Name = "Дата создания")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreateDate { get; set; }

        public int? AppendixId { get; set; }

        public virtual Appendix Appendix { get; set; }

        [Display(Name = "Дата обновления")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime UpdateDate { get; set; }

        public int StatusId { get; set; }
        public virtual Status Status { get; set; }
    }

    public class Status
    {
        public int Id { get; set; }
        [Display(Name = "Статус")]
        public string Name { get; set; }
        public virtual ICollection<Task> Task { get; set; }

        [Display(Name = "Дата создания")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreateDate { get; set; }

        [Display(Name = "Дата обновления")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime UpdateDate { get; set; }
    }

    public class QualificationLevel
    {
        public int Id { get; set; }
        [Display(Name = "Название")]
        public string Name { get; set; }
        public virtual ICollection<Task> Task { get; set; }

        [Display(Name = "Дата создания")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreateDate { get; set; }

        [Display(Name = "Дата обновления")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime UpdateDate { get; set; }


        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }
    }

    public class Position
    {
        public int Id { get; set; }
        [Display(Name = "Название")]
        public string Name { get; set; }
        public virtual ICollection<Task> Task { get; set; }
        [Display(Name = "Дата создания")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreateDate { get; set; }
        [Display(Name = "Дата обновления")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime UpdateDate { get; set; }

        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }
    }



}
