namespace Pinterest111.Services
{
    using System.Text.Json;
    using Pinterest111.Models;

    /// <summary>
    /// Хранение досок в файле App_Data/boards.txt (каждая строка — JSON Board)
    /// </summary>
    public class FileBoardStore
    {
        private readonly string _filePath;
        private static readonly object _lock = new();

        public FileBoardStore(IWebHostEnvironment env)
        {
            var dataDir = Path.Combine(env.ContentRootPath, "App_Data");
            Directory.CreateDirectory(dataDir);
            _filePath = Path.Combine(dataDir, "boards.txt");

            if (!File.Exists(_filePath))
                File.Create(_filePath).Dispose();
        }

        public List<Board> GetAll()
        {
            lock (_lock)
            {
                var list = new List<Board>();
                foreach (var line in File.ReadAllLines(_filePath))
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var b = JsonSerializer.Deserialize<Board>(line);
                    if (b != null) list.Add(b);
                }
                return list;
            }
        }

        public List<Board> GetAllNewestFirst() => GetAll().OrderByDescending(b => b.CreatedAt).ToList();

        public Board? GetById(int id) => GetAll().FirstOrDefault(b => b.Id == id);

        public List<Board> GetByAuthor(string username) =>
            GetAll()
               .Where(b => string.Equals(b.Author, username, StringComparison.OrdinalIgnoreCase))
               .OrderByDescending(b => b.CreatedAt)
               .ToList();

        public Board Add(Board board)
        {
            lock (_lock)
            {
                var all = GetAll();
                board.Id = all.Count == 0 ? 1 : all.Max(b => b.Id) + 1;
                var json = JsonSerializer.Serialize(board);
                File.AppendAllText(_filePath, json + Environment.NewLine);
                return board;
            }
        }

        public bool Delete(int id)
        {
            lock (_lock)
            {
                var all = GetAll();
                var removed = all.RemoveAll(b => b.Id == id) > 0;
                if (removed)
                {
                    var lines = all.Select(b => JsonSerializer.Serialize(b));
                    File.WriteAllLines(_filePath, lines);
                }
                return removed;
            }
        }

        public bool AddPinToBoard(int boardId, int pinId)
        {
            lock (_lock)
            {
                var all = GetAll();
                var board = all.FirstOrDefault(b => b.Id == boardId);
                if (board == null) return false;
                if (!board.PinIds.Contains(pinId))
                {
                    board.PinIds.Add(pinId);
                    var lines = all.Select(b => JsonSerializer.Serialize(b));
                    File.WriteAllLines(_filePath, lines);
                }
                return true;
            }
        }

        public bool RemovePinFromBoard(int boardId, int pinId)
        {
            lock (_lock)
            {
                var all = GetAll();
                var board = all.FirstOrDefault(b => b.Id == boardId);
                if (board == null) return false;
                var removed = board.PinIds.RemoveAll(id => id == pinId) > 0;
                if (removed)
                {
                    var lines = all.Select(b => JsonSerializer.Serialize(b));
                    File.WriteAllLines(_filePath, lines);
                }
                return removed;
            }
        }

        public void UpdateAuthorUsername(string oldUsername, string newUsername)
        {
            lock (_lock)
            {
                var all = GetAll();
                var changed = false;
                foreach (var b in all)
                {
                    if (string.Equals(b.Author, oldUsername, StringComparison.OrdinalIgnoreCase))
                    {
                        b.Author = newUsername;
                        changed = true;
                    }
                }
                if (changed)
                {
                    var lines = all.Select(b => JsonSerializer.Serialize(b));
                    File.WriteAllLines(_filePath, lines);
                }
            }
        }
    }
}
