using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCareJordan.Api.Data;
using PetCareJordan.Api.Dtos;
using PetCareJordan.Api.Models;

namespace PetCareJordan.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChatController(PetCareJordanContext context) : ControllerBase
{
    [HttpGet("vets")]
    [Authorize(Roles = "User")]
    public async Task<ActionResult<IEnumerable<ChatVetDto>>> GetVets()
    {
        var vets = await context.Users
            .Where(user => user.Role == UserRole.Vet)
            .OrderBy(user => user.FullName)
            .Select(user => new ChatVetDto(user.Id, user.FullName, user.City))
            .ToListAsync();

        return Ok(vets);
    }

    [HttpGet("conversations")]
    public async Task<ActionResult<IEnumerable<ChatConversationDto>>> GetMyConversations()
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
        {
            return Unauthorized();
        }

        var conversations = await context.ChatConversations
            .Include(conversation => conversation.User)
            .Include(conversation => conversation.Vet)
            .Include(conversation => conversation.Messages)
            .Where(conversation => conversation.UserId == currentUserId || conversation.VetId == currentUserId)
            .OrderByDescending(conversation => conversation.UpdatedAtUtc ?? conversation.CreatedAtUtc)
            .ToListAsync();

        var result = conversations.Select(conversation =>
        {
            var isOwner = conversation.UserId == currentUserId;
            var counterpart = isOwner ? conversation.Vet : conversation.User;
            var latestMessage = conversation.Messages.OrderByDescending(message => message.SentAtUtc).FirstOrDefault();

            return new ChatConversationDto(
                conversation.Id,
                conversation.UserId,
                conversation.VetId,
                counterpart?.Id ?? 0,
                counterpart?.FullName ?? "Unknown",
                counterpart?.Role ?? UserRole.User,
                latestMessage?.Message ?? string.Empty,
                latestMessage?.SentAtUtc,
                conversation.Messages.Count(message => message.SenderId != currentUserId && !message.IsReadByRecipient),
                conversation.CreatedAtUtc,
                conversation.UpdatedAtUtc);
        });

        return Ok(result);
    }

    [HttpPost("conversations")]
    [Authorize(Roles = "User,Vet")]
    public async Task<ActionResult<ChatConversationDto>> CreateOrOpenConversation(CreateConversationRequest request)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
        {
            return Unauthorized();
        }

        var participantId = request.ParticipantId ?? request.VetId;
        if (participantId is null || participantId == currentUserId.Value)
        {
            return BadRequest("Choose another account to start a chat.");
        }

        var participant = await context.Users.FirstOrDefaultAsync(user =>
            user.Id == participantId.Value &&
            (user.Role == UserRole.User || user.Role == UserRole.Vet));
        if (participant is null)
        {
            return BadRequest("Selected account does not exist.");
        }

        var firstParticipantId = Math.Min(currentUserId.Value, participant.Id);
        var secondParticipantId = Math.Max(currentUserId.Value, participant.Id);

        var conversation = await context.ChatConversations
            .Include(item => item.User)
            .Include(item => item.Vet)
            .Include(item => item.Messages)
            .FirstOrDefaultAsync(item => item.UserId == firstParticipantId && item.VetId == secondParticipantId);

        if (conversation is null)
        {
            conversation = new ChatConversation
            {
                UserId = firstParticipantId,
                VetId = secondParticipantId,
                CreatedAtUtc = DateTime.UtcNow
            };

            context.ChatConversations.Add(conversation);
            await context.SaveChangesAsync();

            conversation = await context.ChatConversations
                .Include(item => item.User)
                .Include(item => item.Vet)
                .Include(item => item.Messages)
                .FirstAsync(item => item.Id == conversation.Id);
        }

        var openingText = request.OpeningMessage?.Trim() ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(openingText) &&
            !conversation.Messages.Any(message =>
                message.SenderId == currentUserId.Value &&
                message.Message == openingText))
        {
            var message = new ChatMessage
            {
                ConversationId = conversation.Id,
                SenderId = currentUserId.Value,
                Message = openingText,
                SentAtUtc = DateTime.UtcNow,
                IsReadByRecipient = false
            };

            context.ChatMessages.Add(message);
            conversation.UpdatedAtUtc = message.SentAtUtc;
            await context.SaveChangesAsync();
            conversation.Messages.Add(message);
        }

        var counterpart = conversation.UserId == currentUserId.Value
            ? conversation.Vet
            : conversation.User;
        var latestMessage = conversation.Messages.OrderByDescending(message => message.SentAtUtc).FirstOrDefault();

        var dto = new ChatConversationDto(
            conversation.Id,
            conversation.UserId,
            conversation.VetId,
            counterpart?.Id ?? participant.Id,
            counterpart?.FullName ?? participant.FullName,
            counterpart?.Role ?? participant.Role,
            latestMessage?.Message ?? string.Empty,
            latestMessage?.SentAtUtc,
            0,
            conversation.CreatedAtUtc,
            conversation.UpdatedAtUtc);

        return Ok(dto);
    }

    [HttpGet("conversations/{conversationId:int}/messages")]
    public async Task<ActionResult<IEnumerable<ChatMessageDto>>> GetConversationMessages(int conversationId)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
        {
            return Unauthorized();
        }

        var conversation = await context.ChatConversations
            .FirstOrDefaultAsync(item => item.Id == conversationId);

        if (conversation is null)
        {
            return NotFound();
        }

        if (!IsParticipant(conversation, currentUserId.Value))
        {
            return Forbid();
        }

        var unreadIncoming = await context.ChatMessages
            .Where(message =>
                message.ConversationId == conversationId &&
                message.SenderId != currentUserId.Value &&
                !message.IsReadByRecipient)
            .ToListAsync();

        if (unreadIncoming.Count > 0)
        {
            var readTime = DateTime.UtcNow;
            foreach (var message in unreadIncoming)
            {
                message.IsReadByRecipient = true;
                message.ReadAtUtc = readTime;
            }

            await context.SaveChangesAsync();
        }

        var messages = await context.ChatMessages
            .Where(message => message.ConversationId == conversationId)
            .Include(message => message.Sender)
            .OrderBy(message => message.SentAtUtc)
            .Select(message => new ChatMessageDto(
                message.Id,
                message.ConversationId,
                message.SenderId,
                message.Sender!.FullName,
                message.Sender.Role,
                message.Message,
                message.SentAtUtc))
            .ToListAsync();

        return Ok(messages);
    }

    [HttpPost("conversations/{conversationId:int}/messages")]
    public async Task<ActionResult<ChatMessageDto>> SendMessage(int conversationId, SendChatMessageRequest request)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
        {
            return Unauthorized();
        }

        var conversation = await context.ChatConversations
            .FirstOrDefaultAsync(item => item.Id == conversationId);

        if (conversation is null)
        {
            return NotFound();
        }

        if (!IsParticipant(conversation, currentUserId.Value))
        {
            return Forbid();
        }

        var text = request.Message?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(text))
        {
            return BadRequest("Message cannot be empty.");
        }

        var sender = await context.Users.FirstOrDefaultAsync(user => user.Id == currentUserId);
        if (sender is null)
        {
            return Unauthorized();
        }

        var message = new ChatMessage
        {
            ConversationId = conversationId,
            SenderId = currentUserId.Value,
            Message = text,
            SentAtUtc = DateTime.UtcNow,
            IsReadByRecipient = false
        };

        context.ChatMessages.Add(message);
        conversation.UpdatedAtUtc = message.SentAtUtc;
        await context.SaveChangesAsync();

        return Ok(new ChatMessageDto(
            message.Id,
            message.ConversationId,
            message.SenderId,
            sender.FullName,
            sender.Role,
            message.Message,
            message.SentAtUtc));
    }

    [HttpDelete("conversations/{conversationId:int}")]
    public async Task<IActionResult> DeleteConversation(int conversationId)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
        {
            return Unauthorized();
        }

        var conversation = await context.ChatConversations
            .Include(item => item.Messages)
            .FirstOrDefaultAsync(item => item.Id == conversationId);

        if (conversation is null)
        {
            return NotFound();
        }

        if (!IsParticipant(conversation, currentUserId.Value))
        {
            return Forbid();
        }

        context.ChatConversations.Remove(conversation);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private int? GetCurrentUserId()
    {
        return int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)
            ? userId
            : null;
    }

    private static bool IsParticipant(ChatConversation conversation, int userId) =>
        conversation.UserId == userId || conversation.VetId == userId;
}
