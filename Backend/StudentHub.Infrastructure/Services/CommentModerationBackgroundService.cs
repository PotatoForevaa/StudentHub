using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StudentHub.Application.DTOs;
using StudentHub.Application.DTOs.Responses;
using StudentHub.Application.Entities;
using StudentHub.Application.Interfaces.Repositories;
using StudentHub.Application.Interfaces.Services;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StudentHub.Infrastructure.Services
{
    public class CommentModerationBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<CommentModerationBackgroundService> _logger;
        private readonly int _pollingIntervalSeconds;

        public CommentModerationBackgroundService(IServiceScopeFactory scopeFactory, ILogger<CommentModerationBackgroundService> logger, CommentSettings commentSettings)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _pollingIntervalSeconds = commentSettings.ModerationPollingIntervalSeconds > 0 ? commentSettings.ModerationPollingIntervalSeconds : 5;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var repository = scope.ServiceProvider.GetRequiredService<IProjectRepository>();
                    var toxicService = scope.ServiceProvider.GetRequiredService<IToxicFilterService>();

                    var pendingCommentsResult = await repository.GetCommentsByModerationStatusAsync(CommentModerationStatus.Pending);
                    if (!pendingCommentsResult.IsSuccess)
                    {
                        _logger.LogWarning("Unable to load pending comments for moderation: {Errors}", string.Join("; ", pendingCommentsResult.Errors.Select(e => e.Message)));
                    }
                    else
                    {
                        foreach (var comment in pendingCommentsResult.Value.Where(c => !string.IsNullOrWhiteSpace(c.ToxicFilterTaskId)))
                        {
                            if (string.IsNullOrWhiteSpace(comment.ToxicFilterTaskId))
                                continue;

                            var taskResult = await toxicService.GetTaskResultAsync(comment.ToxicFilterTaskId!);
                            if (!taskResult.IsSuccess)
                            {
                                _logger.LogWarning("Failed to fetch toxin task result for comment {CommentId}: {Errors}", comment.Id, string.Join("; ", taskResult.Errors.Select(e => e.Message)));
                                continue;
                            }

                            var statusResult = taskResult.Value!;
                            if (statusResult.Status == "pending")
                                continue;

                            if (statusResult.Status == "failure")
                            {
                                _logger.LogWarning("Toxic filter task {TaskId} failed for comment {CommentId}: {Error}", comment.ToxicFilterTaskId, comment.Id, statusResult.ErrorMessage);
                                continue;
                            }

                            comment.ModerationStatus = statusResult.IsToxic
                                ? CommentModerationStatus.Toxic
                                : CommentModerationStatus.Approved;
                            comment.ModeratedBy = CommentModerationOrigin.AI;
                            await repository.UpdateCommentAsync(comment);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while processing comment moderation background service.");
                }

                await Task.Delay(TimeSpan.FromSeconds(_pollingIntervalSeconds), stoppingToken);
            }
        }
    }
}
