using PetCareJordan.Api.Models;

namespace PetCareJordan.Api.Dtos;

public record ChatVetDto(
    int Id,
    string FullName,
    string City);

public record ChatConversationDto(
    int Id,
    int UserId,
    int VetId,
    int CounterpartId,
    string CounterpartName,
    UserRole CounterpartRole,
    string LastMessage,
    DateTime? LastMessageAt,
    int UnreadIncomingCount,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);

public record ChatMessageDto(
    int Id,
    int ConversationId,
    int SenderId,
    string SenderName,
    UserRole SenderRole,
    string Message,
    DateTime SentAtUtc);

public record CreateConversationRequest(int? ParticipantId, int? VetId, string? OpeningMessage);

public record SendChatMessageRequest(string Message);
