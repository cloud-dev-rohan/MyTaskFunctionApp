using Microsoft.EntityFrameworkCore;
using MyTaskFunctionApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace MyTaskFunctionApp.Data
{
    public class TaskDbContext : DbContext
    {
        public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options) { }
        public DbSet<TaskItem> Tasks { get; set; }
    }
}
