using AutoMapper;
using StudentTestingApp.BLL.DTOs;
using StudentTestingApp.BLL.Interfaces;
using StudentTestingApp.Core.Models;
using StudentTestingApp.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using StudentTestingApp.BLL.Exceptions;

namespace StudentTestingApp.BLL.Services
{
    public class TestService : ITestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TestService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<TestViewDto?> CreateTestAsync(TestCreateDto testDto, Guid authorId)
        {
            var author = await _unitOfWork.Users.GetByIdAsync(authorId);
            if (author == null || author.Role != UserRole.Teacher)
            {
                throw new AuthorizationException("Тільки викладачі можуть створювати тести.");
            }
            if (!testDto.Questions.Any())
            {
                throw new ValidationException("Тест повинен містити хоча б одну запитання.");
            }
            if (string.IsNullOrWhiteSpace(testDto.Title))
            {
                throw new ValidationException("Назва тесту не може бути порожньою.");
            }
            ValidateQuestions(testDto.Questions);

            var newTest = _mapper.Map<Test>(testDto);
            newTest.AuthorId = authorId;
            newTest.Id = Guid.NewGuid();

            await _unitOfWork.Tests.AddAsync(newTest);
            var changes = await _unitOfWork.SaveChangesAsync();
            if (changes <= 0) throw new DataAccessException("Не вдалося зберегти новий тест.");

            var testViewDto = _mapper.Map<TestViewDto>(newTest);
            testViewDto.AuthorName = author.Name;
            return testViewDto;
        }

        public async Task<IEnumerable<TestViewDto>> GetAllAvailableTestsAsync()
        {
            Debug.WriteLine("[TestService] Entering GetAllAvailableTestsAsync...");
            var tests = await _unitOfWork.Tests.GetAllAsync();
            Debug.WriteLine($"[TestService] Retrieved {tests?.Count() ?? 0} tests from repository."); 

            if (tests == null || !tests.Any())
            {
                Debug.WriteLine("[TestService] No tests found or repository returned null. Returning empty list.");
                return Enumerable.Empty<TestViewDto>();
            }

            var testViewDtos = new List<TestViewDto>();

            var authorIds = tests.Select(t => t.AuthorId).Distinct();
            var authors = (await _unitOfWork.Users.FindAsync(u => authorIds.Contains(u.Id)))
                            .ToDictionary(a => a.Id);

            foreach (var test in tests)
            {
                var authorName = authors.TryGetValue(test.AuthorId, out var author) ? author.Username : "Невідомий автор";
                var dto = _mapper.Map<TestViewDto>(test);
                dto.AuthorName = authorName;
                testViewDtos.Add(dto);
            }
            return testViewDtos.OrderBy(t => t.Title);
        }

        public async Task<IEnumerable<TestViewDto>> GetTestsByAuthorAsync(Guid authorId)
        {
            var author = await _unitOfWork.Users.GetByIdAsync(authorId);
            if (author == null || author.Role != UserRole.Teacher)
            {
                return Enumerable.Empty<TestViewDto>();
            }

            var tests = await _unitOfWork.Tests.FindAsync(t => t.AuthorId == authorId);
            var dtos = _mapper.Map<IEnumerable<TestViewDto>>(tests);

            foreach (var dto in dtos)
            {
                 dto.AuthorName = author.Name;
            }
            return dtos.OrderBy(t => t.Title);
        }

        public async Task<TestViewDto?> GetTestForViewingAsync(Guid testId)
        {
            var test = await _unitOfWork.Tests.GetByIdAsync(testId);
            if (test == null) throw new NotFoundException($"Тест з ID {testId} не знайдений.");

            var author = await _unitOfWork.Users.GetByIdAsync(test.AuthorId);
            var authorName = author?.Name ?? "Невідомий автор";

            var dto = _mapper.Map<TestViewDto>(test);
            dto.AuthorName = authorName;
            return dto;
        }

        public async Task<TestViewDto?> GetTestForEditingAsync(Guid testId, Guid authorId)
        {
            var test = await _unitOfWork.Tests.GetByIdAsync(testId);
            if (test == null) throw new NotFoundException($"Тест з ID {testId} не знайдений.");
            if (test.AuthorId != authorId)
            {
                throw new AuthorizationException("Ви не маєте доступу до редагування цього тесту.");
            }

            var author = await _unitOfWork.Users.GetByIdAsync(authorId);
            var authorName = author?.Name ?? "Невідомий автор";

            var dto = _mapper.Map<TestViewDto>(test);
            dto.AuthorName = authorName;
            return dto;
        }

        public async Task<bool> UpdateTestAsync(Guid testId, TestCreateDto testDto, Guid authorId)
        {
             if (testDto == null) throw new ArgumentNullException(nameof(testDto));

            var test = await _unitOfWork.Tests.GetByIdAsync(testId);
            if (test == null) throw new NotFoundException($"Тест з ID {testId} не знайдений.");
            if (test.AuthorId != authorId)
            {
                throw new AuthorizationException("Ви не маєте доступу до редагування цього тесту.");
            }
            if (!testDto.Questions.Any())
            {
                throw new ValidationException("Тест повинен містити хоча б одну запитання.");
            }
            if (string.IsNullOrWhiteSpace(testDto.Title))
            {
                throw new ValidationException("Назва тесту не може бути порожньою.");
            }
            ValidateQuestions(testDto.Questions);

            _mapper.Map(testDto, test);
            test.AuthorId = authorId;

            await _unitOfWork.Tests.UpdateAsync(test);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteTestAsync(Guid testId, Guid authorId)
        {
            var test = await _unitOfWork.Tests.GetByIdAsync(testId);
            if (test == null)
            {
                return true;
            }

            if (test.AuthorId != authorId)
            {
                throw new AuthorizationException("Ви не маєте доступу до видалення цього тесту.");
            }

            var resultsToDelete = await _unitOfWork.TestResults.FindAsync(r => r.TestId == testId);
            foreach (var result in resultsToDelete)
            {
                try
                {
                    await _unitOfWork.TestResults.DeleteAsync(result.Id);
                }
                catch (KeyNotFoundException)
                {
                }
            }

            await _unitOfWork.Tests.DeleteAsync(testId);

            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<TestResultDto?> SubmitTestAsync(TestSubmissionDto submissionDto, Guid studentId)
        {
            if (submissionDto == null) throw new ArgumentNullException(nameof(submissionDto));

            var student = await _unitOfWork.Users.GetByIdAsync(studentId);
            var test = await _unitOfWork.Tests.GetByIdAsync(submissionDto.TestId);

            if (student == null) throw new NotFoundException($"Студент з ID {studentId} не знайдений.");
            if (student.Role != UserRole.Student) throw new AuthorizationException("Ви не маєте доступу до подачі тесту.");
            if (test == null) throw new NotFoundException($"Тест з ID {submissionDto.TestId} не знайдений.");
            if (submissionDto.Answers == null) throw new ValidationException("Підача повинна містити відповіді.");

            var existingResult = (await _unitOfWork.TestResults.FindAsync(r => r.StudentId == studentId && r.TestId == test.Id)).FirstOrDefault();
            if (existingResult != null) {
                throw new ValidationException("Ви вже пройшли цей тест.");
            }

            int score = 0;
            var questionMap = test.Questions.ToDictionary(q => q.Id);
            int totalQuestionsInTest = test.Questions.Count;

            foreach (var studentAnswer in submissionDto.Answers)
            {
                if (questionMap.TryGetValue(studentAnswer.QuestionId, out var question))
                {
                    var correctAnswers = question.Answers.Where(a => a.IsCorrect).Select(a => a.Id).ToList();
                    
                    if (correctAnswers.Any() && correctAnswers.Contains(studentAnswer.SelectedAnswerId))
                    {
                        score++;
                    }
                }
            }

            double percentage = totalQuestionsInTest > 0 ? (double)score / totalQuestionsInTest * 100 : 0;

            var newResult = new TestResult
            {
                Id = Guid.NewGuid(),
                StudentId = studentId,
                TestId = test.Id,
                Timestamp = DateTime.UtcNow,
                Score = score,
                TotalQuestions = totalQuestionsInTest
            };

            await _unitOfWork.TestResults.AddAsync(newResult);
            var changes = await _unitOfWork.SaveChangesAsync();
            if (changes <= 0) throw new DataAccessException("Не вдалося зберегти результат тесту.");

            var resultDto = _mapper.Map<TestResultDto>(newResult);
            resultDto.StudentName = student.Name;
            resultDto.TestTitle = test.Title;

            return resultDto;
        }

        public async Task<IEnumerable<TestResultDto>> GetResultsForStudentAsync(Guid studentId)
        {
            var results = await _unitOfWork.TestResults.FindAsync(r => r.StudentId == studentId);
            var resultDtos = new List<TestResultDto>();

            var testIds = results.Select(r => r.TestId).Distinct();
            var tests = (await _unitOfWork.Tests.FindAsync(t => testIds.Contains(t.Id))).ToDictionary(t => t.Id);
            var student = await _unitOfWork.Users.GetByIdAsync(studentId);
            var studentName = student?.Name ?? "Невідомий студент";

            foreach (var result in results)
            {
                var testTitle = tests.TryGetValue(result.TestId, out var test) ? test.Title : "Невідомий тест";
                var dto = _mapper.Map<TestResultDto>(result);
                dto.StudentName = studentName;
                dto.TestTitle = testTitle;
                resultDtos.Add(dto);
            }
            return resultDtos.OrderByDescending(r => r.Timestamp);
        }

        public async Task<IEnumerable<TestResultDto>> GetResultsForTestAsync(Guid testId)
        {
            var results = await _unitOfWork.TestResults.FindAsync(r => r.TestId == testId);
            var resultDtos = new List<TestResultDto>();

            var studentIds = results.Select(r => r.StudentId).Distinct();
            var students = (await _unitOfWork.Users.FindAsync(u => studentIds.Contains(u.Id))).ToDictionary(s => s.Id);
            var test = await _unitOfWork.Tests.GetByIdAsync(testId);
            var testTitle = test?.Title ?? "Невідомий тест";

            foreach (var result in results)
            {
                var studentName = students.TryGetValue(result.StudentId, out var student) ? student.Name : "Невідомий студент";
                var dto = _mapper.Map<TestResultDto>(result);
                dto.StudentName = studentName;
                dto.TestTitle = testTitle;
                resultDtos.Add(dto);
            }
            return resultDtos.OrderBy(r => r.StudentName).ThenByDescending(r => r.Timestamp);
        }

        public async Task<TestResultDto?> GetResultByIdAsync(Guid resultId)
        {
            var result = await _unitOfWork.TestResults.GetByIdAsync(resultId);
            if (result == null) throw new NotFoundException($"Test result with ID {resultId} not found.");

            var student = await _unitOfWork.Users.GetByIdAsync(result.StudentId);
            var test = await _unitOfWork.Tests.GetByIdAsync(result.TestId);

            var dto = _mapper.Map<TestResultDto>(result);
            dto.StudentName = student?.Name ?? "Невідомий студент";
            dto.TestTitle = test?.Title ?? "Невідомий тест";

            return dto;
        }

        public async Task<bool> AssignTestToStudentAsync(Guid testId, Guid studentId)
        {
            var student = await _unitOfWork.Users.GetByIdAsync(studentId);
            if (student == null || student.Role != StudentTestingApp.Core.Models.UserRole.Student)
            {
                return false; 
            }

            var test = await _unitOfWork.Tests.GetByIdAsync(testId);
            if (test == null)
            {
                return false;
            }
            if (student.AssignedTestIds == null)
            {
                student.AssignedTestIds = new List<Guid>();
            }

            if (student.AssignedTestIds.Contains(testId))
            {
                return true; 
            }

            student.AssignedTestIds.Add(testId);

            try
            {
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving test assignment: {ex.Message}");
                return false; 
            }
        }

        public async Task<IEnumerable<TestViewDto>> GetAssignedTestsForStudentAsync(Guid studentId)
        {
            var student = await _unitOfWork.Users.GetByIdAsync(studentId);
            if (student == null || student.Role != StudentTestingApp.Core.Models.UserRole.Student)
            {
                return Enumerable.Empty<TestViewDto>();
            }

            if (student.AssignedTestIds == null || !student.AssignedTestIds.Any())
            {
                return Enumerable.Empty<TestViewDto>();
            }
            var assignedTests = await _unitOfWork.Tests.FindAsync(t => student.AssignedTestIds.Contains(t.Id));
             if (assignedTests == null)
             {
                 return Enumerable.Empty<TestViewDto>();
             }

            var assignedTestDtos = _mapper.Map<IEnumerable<TestViewDto>>(assignedTests);

            foreach (var dto in assignedTestDtos)
            {
                 var test = assignedTests.FirstOrDefault(t => t.Id == dto.Id);
                 if (test != null)
                 {
                     var author = await _unitOfWork.Users.GetByIdAsync(test.AuthorId);
                     dto.AuthorName = author?.Name ?? "Невідомий автор";
                 }
            }

            return assignedTestDtos;
        }

        private void ValidateQuestions(List<QuestionDto> questions)
        {
            if (questions == null) throw new ValidationException("Список запитань не може бути null.");

            for (int i = 0; i < questions.Count; i++)
            {
                var qDto = questions[i];
                if (string.IsNullOrWhiteSpace(qDto.Text)) throw new ValidationException($"Запитання {i + 1} не може бути пустим.");
                if (qDto.Answers == null || qDto.Answers.Count < 2) throw new ValidationException($"Запитання {i + 1} повинно містити принаймні 2 відповіді.");
                if (qDto.Answers.Any(a => string.IsNullOrWhiteSpace(a.Text))) throw new ValidationException($"Відповідь не може бути пустою в запитанні {i + 1}.");
                if (!qDto.Answers.Any(a => a.IsCorrect))
                {
                    throw new ValidationException($"Запитання {i + 1} повинно мати принаймні одну правильну відповідь.");
                }
            }
        }
    }
}
