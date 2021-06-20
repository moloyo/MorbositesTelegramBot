using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace MorbositesBotApi.Services
{
    public class BotService : IBotService
    {
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly IMorbositeService _morbositeService;
        private readonly ILogger<BotService> _logger;

        public BotService(ITelegramBotClient telegramBotClient, IMorbositeService userService, ILogger<BotService> logger)
        {
            _telegramBotClient = telegramBotClient;
            _morbositeService = userService;
            _logger = logger;
        }

        public async Task StartBotAsync()
        {
            var bot = await _telegramBotClient.GetMeAsync();

            _logger.LogInformation($"Hello {bot.Id} - {bot.Username}");

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
                _logger.LogInformation($"Type: {e.Message.Type}");
                switch (e.Message.Type)
                {
                    case Telegram.Bot.Types.Enums.MessageType.Unknown:
                        break;
                    case Telegram.Bot.Types.Enums.MessageType.Text:
                        await HandleTextAsync(e.Message);
                        break;
                    case Telegram.Bot.Types.Enums.MessageType.ChatMembersAdded:
                        await HandleChatMembersAddedAsync(e.Message);
                        break;
                    case Telegram.Bot.Types.Enums.MessageType.ChatMemberLeft:
                        await HandleChatMemberLeftAsync(e.Message);
                        break;
                    #region Unsupported Message Types
                    //case Telegram.Bot.Types.Enums.MessageType.Photo:
                    //    break;
                    //case Telegram.Bot.Types.Enums.MessageType.Audio:
                    //    break;
                    //case Telegram.Bot.Types.Enums.MessageType.Video:
                    //    break;
                    //case Telegram.Bot.Types.Enums.MessageType.Voice:
                    //    break;
                    //case Telegram.Bot.Types.Enums.MessageType.Document:
                    //    break;
                    //case Telegram.Bot.Types.Enums.MessageType.Sticker:
                    //    break;
                    //case Telegram.Bot.Types.Enums.MessageType.Location:
                    //    break;
                    //case Telegram.Bot.Types.Enums.MessageType.Contact:
                    //    break;
                    //case Telegram.Bot.Types.Enums.MessageType.Venue:
                    //    break;
                    //case Telegram.Bot.Types.Enums.MessageType.Game:
                    //    break;
                    //case Telegram.Bot.Types.Enums.MessageType.VideoNote:
                    //    break;
                    //case Telegram.Bot.Types.Enums.MessageType.Invoice:
                    //    break;
                    //case Telegram.Bot.Types.Enums.MessageType.SuccessfulPayment:
                    //    break;
                    //case Telegram.Bot.Types.Enums.MessageType.WebsiteConnected:
                    //    break;
                    //case Telegram.Bot.Types.Enums.MessageType.ChatTitleChanged:
                    //    break;
                    //case Telegram.Bot.Types.Enums.MessageType.ChatPhotoChanged:
                    //    break;
                    //case Telegram.Bot.Types.Enums.MessageType.MessagePinned:
                    //    break;
                    //case Telegram.Bot.Types.Enums.MessageType.ChatPhotoDeleted:
                    //    break;
                    //case Telegram.Bot.Types.Enums.MessageType.GroupCreated:
                    //    break;
                    //case Telegram.Bot.Types.Enums.MessageType.SupergroupCreated:
                    //    break;
                    //case Telegram.Bot.Types.Enums.MessageType.ChannelCreated:
                    //    break;
                    //case Telegram.Bot.Types.Enums.MessageType.MigratedToSupergroup:
                    //    break;
                    //case Telegram.Bot.Types.Enums.MessageType.MigratedFromGroup:
                    //    break;
                    //case Telegram.Bot.Types.Enums.MessageType.Poll:
                    //    break;
                    //case Telegram.Bot.Types.Enums.MessageType.Dice:
                    //    break;
                    //case Telegram.Bot.Types.Enums.MessageType.MessageAutoDeleteTimerChanged:
                    //    break;
                    //case Telegram.Bot.Types.Enums.MessageType.ProximityAlertTriggered:
                    //    break;
                    //case Telegram.Bot.Types.Enums.MessageType.VoiceChatScheduled:
                    //    break;
                    //case Telegram.Bot.Types.Enums.MessageType.VoiceChatStarted:
                    //    break;
                    //case Telegram.Bot.Types.Enums.MessageType.VoiceChatEnded:
                    //    break;
                    //case Telegram.Bot.Types.Enums.MessageType.VoiceChatParticipantsInvited:
                    //    break;
                    #endregion
                    default:
                        break;
                }
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
            {
                await HandleCommandAsync(message);
            } else
            {
                _logger.LogInformation(
                    $"Message: {message.Text}" +
                    Environment.NewLine +
                    $"From: {message.From.Username}.");

                await _morbositeService.UpdateLasstMessageForUserAsync(message.From);
            }
        }

        private async Task HandleCommandAsync(Message message)
        {
            var command = message.Text.Split()[0];

            switch (command)
            {
                case "/users":
                    await HandleUsersCommandAsync(message);
                    break;
                default:
                    await _telegramBotClient.SendTextMessageAsync(
                       chatId: message.Chat,
                       text: $"Command {command} not found!"
                   );
                    break;
            }
        }

        private async Task HandleUsersCommandAsync(Message message)
        {
            var users = await _morbositeService.GetMorbositesAsync();

            var usersMessage = string.Join('\n', users.Select(u => $"USERNAME: {u.Username} | INACTIVE: {DateTime.UtcNow.Subtract(u.LastMessageOn ?? u.JoinedOn).Minutes} minutos."));

            await _telegramBotClient.SendTextMessageAsync(
                chatId: message.Chat,
                text: usersMessage
            );
        }

        private async Task HandleChatMembersAddedAsync(Message message)
        {
            await _telegramBotClient.SendTextMessageAsync(
                chatId: message.Chat,
                text: $"{message.From.Username} abandonó el grupo!"
            );

            await _morbositeService.DeleteMorbositeAsync(message.From);
        }

        private async Task HandleChatMemberLeftAsync(Message message)
        {
            await _telegramBotClient.SendTextMessageAsync(
                chatId: message.Chat,
                text: $"{message.From.Username} se unió al grupo!"
            );

            await _morbositeService.AddMorbositeAsync(message.From);
        }
    }
}
