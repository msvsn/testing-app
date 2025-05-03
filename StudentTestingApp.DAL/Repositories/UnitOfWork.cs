using StudentTestingApp.Core.Models;
using StudentTestingApp.DAL.Interfaces;
using System.IO;
using System;
using System.Threading.Tasks;

namespace StudentTestingApp.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly string _dataDirectoryPath;
        private JsonRepository<User>? _userRepository;
        private JsonRepository<Test>? _testRepository;
        private JsonRepository<TestResult>? _testResultRepository;
        private bool _disposed = false;

        public UnitOfWork(string dataDirectoryPath = "Data")
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            _dataDirectoryPath = Path.GetFullPath(Path.Combine(basePath, dataDirectoryPath));

            if (!Directory.Exists(_dataDirectoryPath))
            {
                Directory.CreateDirectory(_dataDirectoryPath);
            }
        }

        public IRepository<User> Users => _userRepository ??= new JsonRepository<User>(_dataDirectoryPath);
        public IRepository<Test> Tests => _testRepository ??= new JsonRepository<Test>(_dataDirectoryPath);
        public IRepository<TestResult> TestResults => _testResultRepository ??= new JsonRepository<TestResult>(_dataDirectoryPath);

        public Task<int> SaveChangesAsync()
        {
            int changes = 0;
            try
            {
                if (_userRepository != null)
                {
                    _userRepository.SaveData();
                    changes = 1;
                }
                 if (_testRepository != null)
                 {
                     _testRepository.SaveData();
                     changes = 1;
                 }
                  if (_testResultRepository != null)
                 {
                     _testResultRepository.SaveData();
                     changes = 1;
                 }
                 return Task.FromResult(changes > 0 ? 1 : 0);
            }
            catch (Exception ex)
            {
                 Console.WriteLine($"Помилка збереження змін: {ex.Message}");
                 return Task.FromException<int>(ex);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {                    
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
