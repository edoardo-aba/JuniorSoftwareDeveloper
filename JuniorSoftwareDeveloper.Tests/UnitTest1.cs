using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Xunit;
using TaskManagerApi.Models;

namespace JuniorSoftwareDeveloper.Tests
{
    public class UnitTest1
    {
        [Fact]
        //! basic testing for the correctness of the TaskItem
        public void TaskItem_DefaultValues_AreCorrect()
        {
            var t = new TaskItem();

            Assert.Equal(Guid.Empty, t.Id);
            Assert.Equal(string.Empty, t.Title);
            Assert.False(t.IsCompleted);
            Assert.Equal(string.Empty, t.TenantId);
        }

        [Fact]
        //! testing for the correct creation of a task
        public void AddTask_ToList_AssignsIdAndAdds()
        {
            var tasks = new List<TaskItem>();
            var newTask = new TaskItem { Title = "Hello", IsCompleted = false, TenantId = "T1" };

            newTask.Id = Guid.NewGuid();
            tasks.Add(newTask);

            Assert.Single(tasks);
            Assert.NotEqual(Guid.Empty, tasks[0].Id);
            Assert.False(tasks[0].IsCompleted);
            Assert.Equal("Hello", tasks[0].Title);
            Assert.Equal("T1", tasks[0].TenantId);
        }

        [Fact]
        //! testing for the filtering on TenantId
        public void FilterTasks_ByTenantId_ReturnsOnlyMatching()
        {
            var list = new List<TaskItem>
            {
                new TaskItem { TenantId = "A" },
                new TaskItem { TenantId = "B" },
                new TaskItem { TenantId = "A" },
            };

            var filtered = list.Where(t => t.TenantId == "A").ToList();

            Assert.Equal(2, filtered.Count);
            Assert.All(filtered, t => Assert.Equal("A", t.TenantId));
        }

        [Fact]
        //! testing the update functionality when the Id is correct
        public void UpdateTask_WhenIdMatches_UpdatesProperties()
        {
            // Arrange
            var id = Guid.NewGuid();
            var list = new List<TaskItem>
            {
                new TaskItem { Id = id, Title = "Old", IsCompleted = false, TenantId = "X" }
            };
            var updated = new TaskItem { Id = id, Title = "New", IsCompleted = true, TenantId = "Y" };

            // Act
            var existing = list.FirstOrDefault(t => t.Id == updated.Id);
            if (existing != null)
            {
                existing.Title = updated.Title;
                existing.IsCompleted = updated.IsCompleted;
                existing.TenantId = updated.TenantId;
            }

            Assert.Equal("New", list[0].Title);
            Assert.True(list[0].IsCompleted);
            Assert.Equal("Y", list[0].TenantId);
        }

        [Fact]
        //! Testing the update functionality when the Id is incorrect
        public void UpdateTask_WhenIdNotFound_ReturnsNull()
        {
            var list = new List<TaskItem>();
            var missingId = Guid.NewGuid();

            var existing = list.FirstOrDefault(t => t.Id == missingId);

            Assert.Null(existing);
        }
    }
}
