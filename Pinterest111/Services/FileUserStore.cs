using System.Text.Json;
using Pinterest111.Models;

namespace Pinterest111.Services
{
    /// <summary>
    /// "База данных" пользователей — обычный .txt файл, где каждая строка это JSON-объект User.
    /// Это не настоящая БД, но подходит для учебного проекта.
    /// </summary>
    public class FileUserStore
    {
        private readonly string _filePath;
        private static readonly object _lock = new();

        public FileUserStore(IWebHostEnvironment env)
        {
            var dataDir = Path.Combine(env.ContentRootPath, "App_Data");
            Directory.CreateDirectory(dataDir);
            _filePath = Path.Combine(dataDir, "users.txt");

            if (!File.Exists(_filePath))
                File.Create(_filePath).Dispose();
        }

        public List<User> GetAll()
        {
            lock (_lock)
            {
                var users = new List<User>();
                foreach (var line in File.ReadAllLines(_filePath))
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var user = JsonSerializer.Deserialize<User>(line);
                    if (user != null) users.Add(user);
                }
                return users;
            }
        }

        public User? FindByUsernameOrEmail(string identifier)
        {
            identifier = identifier.Trim();
            return GetAll().FirstOrDefault(u =>
                string.Equals(u.Username, identifier, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(u.Email, identifier, StringComparison.OrdinalIgnoreCase));
        }

        public bool UsernameOrEmailExists(string username, string email)
        {
            var all = GetAll();
            return all.Any(u =>
                string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
        }

        public User? FindById(int id) => GetAll().FirstOrDefault(u => u.Id == id);

        public bool UsernameOrEmailTakenByAnotherUser(int currentUserId, string username, string email)
        {
            return GetAll().Any(u =>
                u.Id != currentUserId &&
                (string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase)));
        }

        public User Add(User user)
        {
            lock (_lock)
            {
                var all = GetAll();
                user.Id = all.Count == 0 ? 1 : all.Max(u => u.Id) + 1;

                var json = JsonSerializer.Serialize(user);
                File.AppendAllText(_filePath, json + Environment.NewLine);

                return user;
            }
        }

        /// <summary>
        /// Обновляет данные пользователя. Так как "БД" это .txt файл со строками JSON,
        /// обновление означает перезапись всего файла с изменённой записью.
        /// </summary>
        public void Update(User updatedUser)
        {
            lock (_lock)
            {
                var all = GetAll();
                var index = all.FindIndex(u => u.Id == updatedUser.Id);
                if (index == -1) return;

                all[index] = updatedUser;

                var lines = all.Select(u => JsonSerializer.Serialize(u));
                File.WriteAllLines(_filePath, lines);
            }
        }
    }
}