﻿using BukBot.Services;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace BukBot
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly IServiceProvider _services;

        public CommandHandler(DiscordSocketClient client, CommandService commandService, IServiceProvider services)
        {
            _client = client;
            _commandService = commandService;
            _services = services;
        }

        public async Task InitializeAsync()
        {
            await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
            _client.MessageReceived += HandleMessageAsync;

        }

        private async Task HandleMessageAsync(SocketMessage socketMessage)
        {
            var argPos = 0;
            if (socketMessage.Author.IsBot) return;

            var userMessage = socketMessage as SocketUserMessage;
            if (userMessage is null) return;

            if (!(userMessage.HasCharPrefix('$', ref argPos) ||
                userMessage.Author.IsBot) ||
                userMessage.Channel.Name != "boter-czanyl")
                return;

            var context = new SocketCommandContext(_client, userMessage);
            if (await _commandService.ExecuteAsync(context, argPos, _services) is var result && !result.IsSuccess)
                await context.Channel.SendMessageAsync(result.ErrorReason);
        }
    }
}
