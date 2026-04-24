using PetCareJordan.Api.Models;

namespace PetCareJordan.Api.Dtos;

public record LoginRequest(string Email, string Password);

public record RegisterRequest(string FullName, string Email, string Password, string PhoneNumber, string City, UserRole Role);

public record AuthResponse(int Id, string FullName, string Email, string City, string PhoneNumber, UserRole Role, string Token);
