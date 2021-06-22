using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MorbositesBotApi.Services
{
    public class BotService : IBotService
    {
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly IMorbositeService _morbositeService;
        private readonly IDictionary<string, Func<Message, Task>> _commands;
        private readonly IDictionary<MessageType, Func<Message, Task>> _messageTypes;

        public BotService(ITelegramBotClient telegramBotClient, IMorbositeService userService)
        {
            _telegramBotClient = telegramBotClient;
            _morbositeService = userService;

            _messageTypes = new Dictionary<MessageType, Func<Message, Task>>()
            {
                { MessageType.Text, HandleTextAsync },
                { MessageType.ChatMembersAdded, HandleChatMembersAddedAsync },
                { MessageType.ChatMemberLeft, HandleChatMemberLeftAsync }
            };

            _commands = new Dictionary<string, Func<Message, Task>>()
            {
                { "/users", HandleUsersCommandAsync },
                { "/kickinactive", HandleKickInactiveCommandAsync },
                { "/help", HandleHelpCommandAsync }
            };
        }

        public async Task StartBotAsync()
        {
            var bot = await _telegramBotClient.GetMeAsync();

            _telegramBotClient.OnMessage += Bot_OnMessage;
            _telegramBotClient.StartReceiving();
        }

        public Task StopBotAsync()
        {
            _telegramBotClient.StopReceiving();
            return Task.CompletedTask;
        }

        private async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            try
            {
                if(e.Message.Chat.Id < 0)
                    if (_messageTypes.TryGetValue(e.Message.Type, out var function))
                        await function(e.Message);
                else
                    await _telegramBotClient.SendTextMessageAsync(
                        chatId: e.Message.Chat,
                        text: $"Este bot funciona solo con grupos"
                    );
            }
            catch (Exception ex)
            {
                await _telegramBotClient.SendTextMessageAsync(
                    chatId: e.Message.Chat,
                    text: $"Exception: {ex.Message}"
                );
            }

        }

        private async Task HandleTextAsync(Message message)
        {

            if (message.Text.StartsWith('/'))
                await HandleCommandAsync(message);
            else
                await _morbositeService.UpdateLasstMessageForUserAsync(message.Chat.Id, message.From);
        }

        private async Task HandleCommandAsync(Message message)
        {
            var command = message.Text.Split()[0];

            if (_commands.TryGetValue(command, out var function))
                await function(message);
            else
                await _telegramBotClient.SendTextMessageAsync(
                    chatId: message.Chat,
                    text: $"Comando {command} no encontrado!"
                );
        }

        private async Task HandleUsersCommandAsync(Message message)
        {
            var users = await _morbositeService.GetMorbositesAsync(message.Chat.Id);

            if (users.Any())
            {
                var neverActiveUsers = users.Where(u => u.LastMessageOn == default && DateTime.UtcNow.Subtract(u.JoinedOn).TotalDays > 1);

                if (neverActiveUsers.Any())
                {
                    await _telegramBotClient.SendTextMessageAsync(
                        chatId: message.Chat,
                        text: $"Los siguientes usuarios entraron y nunca hablaron:\n{string.Join('\n', neverActiveUsers.Select(u => u.Username))}"
                    );
                }

                var usersMessage = string.Join('\n', users.Select(u => $"USERNAME: {u.Username} | INACTIVE: {DateTime.UtcNow.Subtract(u.LastMessageOn ?? u.JoinedOn).TotalDays.ToString("0")} días."));
                await _telegramBotClient.SendTextMessageAsync(
                    chatId: message.Chat,
                    text: usersMessage
                );
            } else
            {
                await _telegramBotClient.SendTextMessageAsync(
                    chatId: message.Chat,
                    text: "No hay usuarios registrados en la base de datos para este grupo..."
                );
            }
            
        }

        private async Task HandleKickInactiveCommandAsync(Message message)
        {
            var inactiveMorbosites = await _morbositeService.GetInactiveMorbositesAsync(message.Chat.Id);

            foreach (var morbosite in inactiveMorbosites)
            {
                await _telegramBotClient.KickChatMemberAsync(
                   chatId: message.Chat,
                   userId: morbosite.UserId,
                   untilDate: DateTime.UtcNow.AddHours(1),
                   revokeMessages: true
               );

                await _telegramBotClient.SendTextMessageAsync(
                    chatId: message.Chat,
                    text: $"Usuario {morbosite.Username} eliminado"
                );
            }
        }

        private async Task HandleHelpCommandAsync(Message message)
        {
            var commands = string.Join('\n', _commands.Keys.OrderBy(k => k));

            await _telegramBotClient.SendTextMessageAsync(
                chatId: message.Chat,
                text: $"Comandos disponibles:\n{commands}"
            );
        }

        private async Task HandleChatMembersAddedAsync(Message message)
        {
            await _telegramBotClient.SendTextMessageAsync(
                chatId: message.Chat,
                text: $"{message.From.Username} se unió al grupo!"
            );

            await _morbositeService.AddMorbositeAsync(message.Chat.Id, message.From);
        }

        private async Task HandleChatMemberLeftAsync(Message message)
        {
            await _telegramBotClient.SendTextMessageAsync(
                chatId: message.Chat,
                text: $"{message.From.Username} abandonó el grupo!"
            );

            await _morbositeService.DeleteMorbositeAsync(message.Chat.Id, message.From);
        }
    }
}
