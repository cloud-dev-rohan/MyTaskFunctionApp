using Microsoft.EntityFrameworkCore;
using MyTaskFunctionApp.Data;
using MyTaskFunctionApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTaskFunctionApp
{
    public interface ITaskService
    {
        Task<List<TaskItem>> GetAllAsync();
        Task<TaskItem>  GetByIdAsync(Guid id);
        Task<TaskItem> CreateAsync(TaskItem task);
        Task<TaskItem> UpdateAsync(Guid id, TaskItem item);
        Task<bool> DeleteAsync(Guid id);
    }
    public class TaskService:ITaskService
    {
        private readonly TaskDbContext _context;

        public TaskService(TaskDbContext context)
        {
            _context = context;
        }

        public async Task<List<TaskItem>> GetAllAsync() =>
            await _context.Tasks.ToListAsync();

        public async Task<TaskItem> GetByIdAsync(Guid id) =>
            await _context.Tasks.FindAsync(id);

        public async Task<TaskItem> CreateAsync(TaskItem task)
        {


            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<TaskItem> UpdateAsync(Guid id, TaskItem item)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return null;
            task.Title = item.Title;
            task.Description = item.Description;
            task.DueDate = item.DueDate;
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return false;

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }
    }

}
