using StudentTestingApp.Core.Models;
using StudentTestingApp.DAL.Interfaces;
using System.Linq.Expressions;
using System.Text.Json;
using System.IO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;

namespace StudentTestingApp.DAL.Repositories
{
    public class JsonRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly string _filePath;
        private List<T> _items;
        private static readonly object _fileLock = new object();

        public JsonRepository(string dataDirectoryPath)
        {
            _filePath = Path.Combine(dataDirectoryPath, $"{typeof(T).Name.ToLowerInvariant()}s.json");
            _items = LoadData();
        }

        private List<T> LoadData()
        {
            lock (_fileLock)
            {
                if (!File.Exists(_filePath))
                {
                    var directory = Path.GetDirectoryName(_filePath);
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    File.WriteAllText(_filePath, "[]");
                    return new List<T>();
                }
                try
                {
                    var jsonData = File.ReadAllText(_filePath);
                    if (string.IsNullOrWhiteSpace(jsonData) || jsonData.Trim() == "[]")
                    {
                        return new List<T>();
                    }
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<List<T>>(jsonData, options) ?? new List<T>();
                }
                catch (JsonException ex)
                {
                    Debug.WriteLine($"ПОМИЛКА десеріалізації JSON у файлі '{_filePath}'. Перевірте вміст файлу. Деталі: {ex.Message}");
                    throw new InvalidOperationException($"Помилка завантаження даних з {_filePath}. Некоректний формат JSON.", ex);
                }
                catch (IOException ioEx)
                {
                    Debug.WriteLine($"ПОМИЛКА ВВОДУ/ВИВОДУ при доступі до файлу '{_filePath}'. Перевірте права доступу. Деталі: {ioEx.Message}");
                    throw new InvalidOperationException($"Помилка доступу до файлу даних {_filePath}.", ioEx);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"НЕОЧІКУВАНА ПОМИЛКА при завантаженні даних з файлу '{_filePath}'. Деталі: {ex.ToString()}");
                    throw new InvalidOperationException($"Неочікувана помилка під час завантаження даних з {_filePath}.", ex);
                }
            }
        }

        public void SaveData()
        {
             lock (_fileLock)
            {
                 try
                 {
                     var options = new JsonSerializerOptions { WriteIndented = true };
                     var jsonData = JsonSerializer.Serialize(_items, options);
                     File.WriteAllText(_filePath, jsonData);
                 }
                 catch (Exception ex)
                 {
                     Debug.WriteLine($"ПОМИЛКА збереження даних до файлу '{_filePath}': {ex.ToString()}");
                 }
             }
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<T>>(_items.ToList());
        }

        public Task<T?> GetByIdAsync(Guid id)
        {
            return Task.FromResult(_items.FirstOrDefault(item => item.Id == id));
        }

        public Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            var compiledPredicate = predicate.Compile();
            return Task.FromResult(_items.Where(compiledPredicate).ToList().AsEnumerable());
        }

        public Task AddAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (_items.Any(item => item.Id == entity.Id))
            {
                throw new InvalidOperationException($"Сутність з id {entity.Id} вже існує.");
            }
            _items.Add(entity);
            return Task.CompletedTask;
        }

        public async Task UpdateAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var existingEntity = await GetByIdAsync(entity.Id);
            if (existingEntity != null)
            {
                _items.Remove(existingEntity);
                _items.Add(entity);
            }
            else
            {
                throw new KeyNotFoundException($"Сутність з id {entity.Id} не знайдена для оновлення.");
            }
            await Task.CompletedTask;
        }


        public async Task DeleteAsync(Guid id)
        {
            var entityToDelete = await GetByIdAsync(id);
            if (entityToDelete != null)
            {
                _items.Remove(entityToDelete);
            }
            else
            {
                throw new KeyNotFoundException($"Сутність з id {id} не знайдена для видалення.");
            }
            await Task.CompletedTask;
        }
    }
}
