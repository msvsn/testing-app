using StudentTestingApp.Core.Models;

namespace StudentTestingApp.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<User> Users { get; }
        IRepository<Test> Tests { get; }
        IRepository<TestResult> TestResults { get; }
        Task<int> SaveChangesAsync();
    }
}
