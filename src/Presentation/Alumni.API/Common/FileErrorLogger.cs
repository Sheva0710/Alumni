using System.Text;

namespace Alumni.API.Common
{
    /// <summary>
    /// Hata loglarını 'logs/error.logs' dosyasına thread-safe (eşzamanlı kilit korumalı)
    /// ve asenkron olarak kaydeden merkezi dosya loglayıcı.
    /// </summary>
    public static class FileErrorLogger
    {
        private static readonly SemaphoreSlim _semaphore = new(1, 1);

        /// <summary>
        /// logs/error.logs dosyasına formatlanmış hata kaydı ekler.
        /// </summary>
        public static async Task LogErrorAsync(
            string level,
            string method,
            string route,
            int statusCode,
            string reason,
            string? details = null,
            string? ip = null)
        {
            await _semaphore.WaitAsync();
            try
            {
                var logsDir = GetLogsDirectory();
                var filePath = Path.Combine(logsDir, "error.logs");

                var sb = new StringBuilder();
                sb.AppendLine("================================================================================");
                sb.AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level.ToUpperInvariant()} {statusCode}]");
                sb.AppendLine($"Method: {method.ToUpperInvariant()} | Route: {route}{(string.IsNullOrEmpty(ip) ? string.Empty : $" | IP: {ip}")}");
                sb.AppendLine($"Reason: {reason}");
                if (!string.IsNullOrWhiteSpace(details))
                {
                    sb.AppendLine($"Details: {details}");
                }
                sb.AppendLine("================================================================================");
                sb.AppendLine();

                await File.AppendAllTextAsync(filePath, sb.ToString(), Encoding.UTF8);
            }
            catch
            {
                // Dosya loglama hatası web uygulamasının ana akışını kesintiye uğratmamalıdır
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Proje ana dizinini tespit edip 'logs' klasörünün yolunu döner.
        /// </summary>
        private static string GetLogsDirectory()
        {
            var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
            var current = dir;

            while (current != null)
            {
                if (File.Exists(Path.Combine(current.FullName, "Alumni.slnx")) ||
                    Directory.Exists(Path.Combine(current.FullName, ".git")) ||
                    File.Exists(Path.Combine(current.FullName, "docker-compose.yml")))
                {
                    var target = Path.Combine(current.FullName, "logs");
                    if (!Directory.Exists(target))
                    {
                        Directory.CreateDirectory(target);
                    }
                    return target;
                }
                current = current.Parent;
            }

            var fallback = Path.Combine(Directory.GetCurrentDirectory(), "logs");
            if (!Directory.Exists(fallback))
            {
                Directory.CreateDirectory(fallback);
            }
            return fallback;
        }
    }
}

