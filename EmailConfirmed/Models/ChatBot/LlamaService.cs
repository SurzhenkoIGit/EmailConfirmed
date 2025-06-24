using LLama;
using LLama.Abstractions;
using LLama.Common;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using static LLama.Common.ChatHistory;

namespace EmailConfirmed.Models.ChatBot
{
    public class LlamaService : IDisposable
    {
        private readonly LLamaContext _context;
        private readonly InteractiveExecutor _executor;
        private readonly ChatSession _session;
        private readonly LLamaWeights _weights;
        private readonly InferenceParams _inferenceParams;
        private readonly ChatHistory _history;
        private bool _isDisposed;
        public LlamaService(IWebHostEnvironment environment)
        {
            try
            {
                var modelPath = Path.Combine(environment.ContentRootPath, "Models", "ChatBot", "suzume-llama-3-8B-multilingual-orpo-borda-top25.Q5_K_M.gguf");
                if (!File.Exists(modelPath))
                {
                    throw new FileNotFoundException($"LLaMA model not found at path: {modelPath}");
                }

                var modelParams = new ModelParams(modelPath)
                {
                    ContextSize = 1024,
                    GpuLayerCount = 0
                };
                _weights = LLamaWeights.LoadFromFile(modelParams);
                _context = _weights.CreateContext(modelParams);

                var executor = new InteractiveExecutor(_context);

                _history = new ChatHistory();
                _history.AddMessage(AuthorRole.System, @"Ты - ассистент для веб-сайта по имени Иишка. Вот функционал проекта:
                                                        1. Регистрация пользователей:
                                                          - Создание аккаунта с email и паролем
                                                          - Отправка письма для подтверждения email
                                                          - Подтверждение email по ссылке
                                                        2. Двухфакторная аутентификация (2FA):
                                                          - Настройка 2FA через приложение-аутентификатор
                                                          - Подтверждение входа с помощью кода из приложения
                                                        3. Управление аккаунтом:
                                                          - Смена пароля
                                                          - Включение/отключение 2FA
                                                          - Генерация резервных кодов для 2FA
                                                        4. Безопасность:
                                                          - Хранение паролей в хешированном виде
                                                          - Защита от подбора паролей
                                                          - Сессии и токены безопасности
                                                         Ты должен:
                                                         1. Отвечать на русском языке
                                                         2. Давать краткие и точные ответы как по функционалу проекта, так и на различные темы
                                                         3. Помогать пользователям разобраться с регистрацией и безопасностью
                                                         4. Объяснять процессы подтверждения email и настройки 2FA
                                                         5. Предоставлять рекомендации по безопасности.");

                _session = new(executor, _history);

                _inferenceParams = new InferenceParams()
                {
                    MaxTokens = 1024,
                    AntiPrompts = new List<string> { "User:" }
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to initialize LLaMA service: {ex.Message}", ex);
            }
        }
        public async Task<string> GetResponseAsync(string message)
        {
            if (_session == null)
            {
                throw new InvalidOperationException("LLaMA session is not initialized");
            }
            try
            {
                var response = new StringBuilder();

                await foreach (var text in _session.ChatAsync(new Message(AuthorRole.User, message), _inferenceParams))
                {
                    response.Append(text);
                }
                var cleanedResponse = CleanResponse(response.ToString());
                return cleanedResponse.Trim();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error generating response: {ex.Message}", ex);
            }
        }
        private string CleanResponse(string response)
        {
            // Удаляем слова "Assistant:" и "User:" с учетом регистра и пробелов
            var cleaned = Regex.Replace(response, @"(?i)(assistant:|user:)\s*", "", RegexOptions.Multiline);

            // Удаляем возможные пустые строки в начале и конце
            cleaned = Regex.Replace(cleaned, @"^\s+|\s+$", "", RegexOptions.Multiline);

            // Удаляем множественные пустые строки
            cleaned = Regex.Replace(cleaned, @"\n\s*\n", "\n");

            return cleaned;
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                try
                {
                    _context?.Dispose();
                    _weights?.Dispose();
                    _isDisposed = true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error disposing LLaMA model: {ex.Message}");
                }
            }
        }
    }
}
