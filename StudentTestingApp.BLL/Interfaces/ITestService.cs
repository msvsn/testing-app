using StudentTestingApp.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentTestingApp.BLL.Interfaces
{
    public interface ITestService
    {
        Task<TestViewDto?> CreateTestAsync(TestCreateDto testDto, Guid authorId);
        Task<TestViewDto?> GetTestForViewingAsync(Guid testId);
        Task<TestViewDto?> GetTestForEditingAsync(Guid testId, Guid authorId);
        Task<IEnumerable<TestViewDto>> GetTestsByAuthorAsync(Guid authorId);
        Task<IEnumerable<TestViewDto>> GetAllAvailableTestsAsync();
        Task<bool> UpdateTestAsync(Guid testId, TestCreateDto testDto, Guid authorId);
        Task<bool> DeleteTestAsync(Guid testId, Guid authorId);
        Task<TestResultDto?> SubmitTestAsync(TestSubmissionDto submissionDto, Guid studentId);
        Task<IEnumerable<TestResultDto>> GetResultsForStudentAsync(Guid studentId);
        Task<IEnumerable<TestResultDto>> GetResultsForTestAsync(Guid testId);
        Task<TestResultDto?> GetResultByIdAsync(Guid resultId);
        Task<bool> AssignTestToStudentAsync(Guid testId, Guid studentId);
        Task<IEnumerable<TestViewDto>> GetAssignedTestsForStudentAsync(Guid studentId);
    }
}
