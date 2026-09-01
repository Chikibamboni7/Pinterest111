using System.Text.Json;
using Pinterest111.Models;

namespace Pinterest111.Services
{
    /// <summary>
    /// "База данных" постов (пинов) — обычный .txt файл, каждая строка это JSON-объект Pin.
    /// </summary>
    public class FilePinStore
    {
        private readonly string _filePath;
        private static readonly object _lock = new();

        public FilePinStore(IWebHostEnvironment env)
        {
            var dataDir = Path.Combine(env.ContentRootPath, "App_Data");
            Directory.CreateDirectory(dataDir);
            _filePath = Path.Combine(dataDir, "pins.txt");

            if (!File.Exists(_filePath))
                File.Create(_filePath).Dispose();
        }

        public List<Pin> GetAll()
        {
            lock (_lock)
            {
                var pins = new List<Pin>();
                foreach (var line in File.ReadAllLines(_filePath))
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var pin = JsonSerializer.Deserialize<Pin>(line);
                    if (pin != null) pins.Add(pin);
                }
                return pins;
            }
        }

        public List<Pin> GetAllNewestFirst() => GetAll().OrderByDescending(p => p.CreatedAt).ToList();

        public Pin? GetById(int id) => GetAll().FirstOrDefault(p => p.Id == id);

        public List<Pin> GetByAuthor(string username) =>
            GetAll().Where(p => string.Equals(p.Author, username, StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(p => p.CreatedAt)
                    .ToList();

        public Pin Add(Pin pin)
        {
            lock (_lock)
            {
                var all = GetAll();
                pin.Id = all.Count == 0 ? 1 : all.Max(p => p.Id) + 1;

                var json = JsonSerializer.Serialize(pin);
                File.AppendAllText(_filePath, json + Environment.NewLine);

                return pin;
            }
        }

        /// <summary>
        /// Удаляет пин по id. Возвращает true, если пин был найден и удалён.
        /// </summary>
        public bool Delete(int id)
        {
            lock (_lock)
            {
                var all = GetAll();
                var removed = all.RemoveAll(p => p.Id == id) > 0;

                if (removed)
                {
                    var lines = all.Select(p => JsonSerializer.Serialize(p));
                    File.WriteAllLines(_filePath, lines);
                }

                return removed;
            }
        }

        /// <summary>
        /// При смене username у пользователя переносим авторство его пинов на новое имя,
        /// иначе ссылки на профиль в старых постах будут вести на несуществующего пользователя.
        /// </summary>
        public void UpdateAuthorUsername(string oldUsername, string newUsername)
        {
            lock (_lock)
            {
                var all = GetAll();
                var changed = false;

                foreach (var pin in all)
                {
                    if (string.Equals(pin.Author, oldUsername, StringComparison.OrdinalIgnoreCase))
                    {
                        pin.Author = newUsername;
                        changed = true;
                    }
                }

                if (changed)
                {
                    var lines = all.Select(p => JsonSerializer.Serialize(p));
                    File.WriteAllLines(_filePath, lines);
                }
            }
        }
    }
}