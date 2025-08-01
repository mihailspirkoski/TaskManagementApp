﻿using System.Security.Claims;
using TaskService.Application.DTOs;

namespace TaskService.Application
{
    public interface IUserApplicationService
    {
        Task<string> RegisterAsync(RegisterUserDto dto);
        Task<string> LoginAsync(LoginUserDto dto);
        int GetCurrentUserId(ClaimsPrincipal user);
    }
}
