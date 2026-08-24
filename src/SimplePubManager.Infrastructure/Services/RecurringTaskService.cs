using SimplePubManager.Domain.Entities;
using SimplePubManager.Domain.Entities.Models;
using SimplePubManager.Domain.Enums;
using SimplePubManager.Infrastructure.Data.Repositories;
using DomainTask = SimplePubManager.Domain.Entities.Models.Task;
using DomainTaskStatus = SimplePubManager.Domain.Enums.TaskStatus;

namespace SimplePubManager.Infrastructure.Services
{
    /// <summary>
    /// Service for managing recurring task generation from templates.
    /// </summary>
    public class RecurringTaskService
    {
        private readonly TaskRepository _taskRepository;
        private readonly RecurringTaskTemplateRepository _recurringTaskTemplateRepository;

        /// <summary>
        /// Initializes a new instance of the RecurringTaskService class.
        /// </summary>
        /// <param name="taskRepository">The task repository</param>
        /// <param name="recurringTaskTemplateRepository">The recurring task template repository</param>
        public RecurringTaskService(
            TaskRepository taskRepository,
            RecurringTaskTemplateRepository recurringTaskTemplateRepository)
        {
            _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
            _recurringTaskTemplateRepository = recurringTaskTemplateRepository ?? throw new ArgumentNullException(nameof(recurringTaskTemplateRepository));
        }

        /// <summary>
        /// Generates new tasks from all active recurring task templates that are due.
        /// </summary>
        /// <param name="organizationId">The organization ID</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public async System.Threading.Tasks.Task GenerateDailyTasksAsync(Guid organizationId)
        {
            // Get all templates that are due for task generation
            var dueTemplates = await _recurringTaskTemplateRepository.GetDueTemplatesAsync(organizationId);

            foreach (var template in dueTemplates)
            {
                // Generate task from template
                var task = await GenerateTaskFromTemplateAsync(template);

                if (task != null)
                {
                    // Update the template's next occurrence date
                    template.NextOccurrenceDate = CalculateNextOccurrence(template.NextOccurrenceDate, template.RecurrencePattern);
                    await _recurringTaskTemplateRepository.UpdateAsync(template);
                }
            }
        }

        /// <summary>
        /// Generates a new task from a recurring task template.
        /// </summary>
        /// <param name="template">The recurring task template</param>
        /// <returns>The created task if successful; otherwise null</returns>
        public async System.Threading.Tasks.Task<DomainTask?> GenerateTaskFromTemplateAsync(RecurringTaskTemplate template)
        {
            if (template == null)
            {
                return null;
            }

            // Create new task from template
            var newTask = new DomainTask
            {
                Id = Guid.NewGuid(),
                OrganizationId = template.OrganizationId,
                Title = template.Title,
                Description = template.Description,
                AssignedToUserId = template.AssignedToUserId,
                AssignedToAreaId = template.AssignedToAreaId,
                DueDate = DateTime.UtcNow.AddDays(1), // Due tomorrow by default
                Status = DomainTaskStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            // Add the task
            var createdTask = await _taskRepository.AddAsync(newTask);
            return createdTask;
        }

        /// <summary>
        /// Calculates the next occurrence date based on the recurrence pattern.
        /// </summary>
        /// <param name="currentDate">The current next occurrence date</param>
        /// <param name="pattern">The recurrence pattern</param>
        /// <returns>The next occurrence date</returns>
        private DateTime CalculateNextOccurrence(DateTime currentDate, RecurrencePattern pattern)
        {
            return pattern switch
            {
                RecurrencePattern.Daily => currentDate.AddDays(1),
                RecurrencePattern.Weekly => currentDate.AddDays(7),
                RecurrencePattern.Monthly => currentDate.AddMonths(1),
                _ => currentDate.AddDays(1)
            };
        }
    }
}
