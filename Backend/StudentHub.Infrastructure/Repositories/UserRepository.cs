using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentHub.Application.DTOs;
using StudentHub.Application.Entities;
using StudentHub.Application.Interfaces.Repositories;
using StudentHub.Infrastructure.Data;
using StudentHub.Infrastructure.Identity;

namespace StudentHub.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public UserRepository(AppDbContext db, UserManager<AppUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<User>> GetAllAsync(int page = 0, int pageSize = 0)
        {
            if (page == 0 && pageSize == 0) return await _db.Users.ToListAsync();
            return await _db.Users.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<(List<User> Users, int TotalCount)> SearchAsync(string? search, string? role, int page, int pageSize)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _db.Users.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToUpper();
                query = query.Where(u =>
                    u.Username.ToUpper().Contains(term) ||
                    u.FullName.ToUpper().Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(role))
            {
                var userIdsInRole = new List<Guid>();
                var identityUsers = await _userManager.GetUsersInRoleAsync(role);
                userIdsInRole = identityUsers.Select(u => u.Id).ToList();
                query = query.Where(u => userIdsInRole.Contains(u.Id));
            }

            var total = await query.CountAsync();
            var users = await query
                .OrderBy(u => u.Username)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (users, total);
        }

        public async Task<Result<User?>> GetByUsernameAsync(string username)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username.ToUpper() == username.ToUpper());
            if (user == null) return Result<User?>.Failure($"Пользователь {username} не найден", "username", ErrorType.NotFound);
            return Result<User?>.Success(user);
        }


        public async Task<Result<User?>> GetByIdAsync(Guid id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return Result<User?>.Failure($"Пользователь {id} не найден", "id", ErrorType.NotFound);
            return Result<User?>.Success(user);
        }


        public async Task<Result<User?>> AddAsync(User user, string password)
        {
            await _db.Users.AddAsync(user);

            var appUser = new AppUser { Id = user.Id, UserName = user.Username };
            var result = await _userManager.CreateAsync(appUser, password);

            if (result.Succeeded)
            {
                if (await _roleManager.RoleExistsAsync("User"))
                {
                    await _userManager.AddToRoleAsync(appUser, "User");
                }
                await _db.SaveChangesAsync();
                return Result<User>.Success(user);
            }

            // Check if the error is due to duplicate username
            var duplicateError = result.Errors.FirstOrDefault(e =>
                e.Code == "DuplicateUserName" ||
                e.Description.Contains("already", StringComparison.OrdinalIgnoreCase));

            if (duplicateError != null)
            {
                return Result<User?>.Failure(
                    $"Пользователь с именем '{user.Username}' уже существует",
                    "username",
                    ErrorType.Conflict);
            }

            return Result<User?>.Failure(result.Errors.Select(e => new Error
            {
                Field = "Password",
                Message = e.Description
            }).ToList());
        }

        public async Task<Result> CheckPasswordAsync(string userName, string password)
        {
            var appUser = await _userManager.FindByNameAsync(userName);
            if (appUser == null)
                return Result.Failure($"Пользователь {userName} не найден", "username", ErrorType.NotFound);

            var result = await _userManager.CheckPasswordAsync(appUser, password);
            return result ?
                 Result.Success() :
                 Result.Failure("Неверный пароль", "password", ErrorType.Unauthorized);
        }

        public async Task<Result> AddToRoleAsync(string username, string role)
        {
            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null)
                return Result.Failure($"Пользователь  {username}  не найден", "username", ErrorType.NotFound);

            var result = await _userManager.AddToRoleAsync(appUser, role);
            return result.Succeeded ?
                Result.Success() :
                Result.Failure(result.Errors.Select(e => new Error
                {
                    Message = e.Description
                }).ToList());
        }

        public async Task<Result> ReplaceAssignableRoleAsync(Guid userId, string role)
        {
            var allowed = new[] { "User", "Teacher" };
            if (!allowed.Contains(role))
                return Result.Failure("Role must be User or Teacher", "role", ErrorType.Validation);

            var appUser = await _userManager.FindByIdAsync(userId.ToString());
            if (appUser == null)
                return Result.Failure($"Пользователь {userId} не найден", "id", ErrorType.NotFound);

            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole<Guid>(role));

            var currentRoles = await _userManager.GetRolesAsync(appUser);
            var removable = currentRoles.Where(r => allowed.Contains(r)).ToList();
            if (removable.Count > 0)
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(appUser, removable);
                if (!removeResult.Succeeded)
                    return Result.Failure(removeResult.Errors.Select(e => new Error { Message = e.Description }).ToList());
            }

            if (!await _userManager.IsInRoleAsync(appUser, role))
            {
                var addResult = await _userManager.AddToRoleAsync(appUser, role);
                if (!addResult.Succeeded)
                    return Result.Failure(addResult.Errors.Select(e => new Error { Message = e.Description }).ToList());
            }

            return Result.Success();
        }

        public async Task<List<string>> GetRolesAsync(Guid userId)
        {
            var appUser = await _userManager.FindByIdAsync(userId.ToString());
            if (appUser == null) return new List<string>();
            return (await _userManager.GetRolesAsync(appUser)).ToList();
        }

        public async Task<Result> CreateRole(string roleName)
        {
            var role = new IdentityRole<Guid>
            {
                Id = Guid.NewGuid(),
                Name = roleName,
                NormalizedName = roleName.ToUpper()
            };

            var result = await _roleManager.CreateAsync(role);
            return result.Succeeded ?
                Result.Success() :
                Result.Failure(result.Errors.Select(e => new Error
                {
                    Message = e.Description
                }).ToList());
        }

        public async Task UpdateAsync(User user)
        {
            _db.Update<User>(user);
            await _db.SaveChangesAsync();
        }

        public async Task<Result> DeleteAsync(Guid id)
        {
            var user = await _db.Users
                .Include(u => u.Projects)
                    .ThenInclude(p => p.Attachments)
                .Include(u => u.Projects)
                    .ThenInclude(p => p.Ratings)
                .Include(u => u.Projects)
                    .ThenInclude(p => p.Comments)
                        .ThenInclude(c => c.Reports)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return Result.Failure($"Пользователь {id} не найден", "id", ErrorType.NotFound);

            var authoredComments = await _db.Comments.Where(c => c.AuthorId == id).ToListAsync();
            var authoredCommentIds = authoredComments.Select(c => c.Id).ToList();
            var projectIds = user.Projects.Select(p => p.Id).ToList();
            var projectComments = await _db.Comments
                .Include(c => c.Reports)
                .Where(c => projectIds.Contains(c.ProjectId) && c.AuthorId != id)
                .ToListAsync();
            var projectCommentIds = projectComments.Select(c => c.Id).ToList();
            var reports = await _db.CommentReports
                .Where(r => r.ReporterId == id || authoredCommentIds.Contains(r.CommentId) || projectCommentIds.Contains(r.CommentId))
                .ToListAsync();
            var ratings = await _db.Ratings.Where(r => r.AuthorId == id || projectIds.Contains(r.ProjectId)).ToListAsync();
            var attachments = user.Projects.SelectMany(p => p.Attachments).ToList();

            _db.CommentReports.RemoveRange(reports);
            _db.Comments.RemoveRange(projectComments);
            _db.Comments.RemoveRange(authoredComments);
            _db.Ratings.RemoveRange(ratings);
            _db.Attachments.RemoveRange(attachments);
            _db.Projects.RemoveRange(user.Projects);
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();

            var appUser = await _userManager.FindByIdAsync(id.ToString());
            if (appUser != null)
            {
                var deleteResult = await _userManager.DeleteAsync(appUser);
                if (!deleteResult.Succeeded)
                    return Result.Failure(deleteResult.Errors.Select(e => new Error { Message = e.Description }).ToList());
            }

            return Result.Success();
        }

        public async Task<Result<User?>> GetByExternalIdAsync(string externalId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.ExternalId == externalId);
            if (user == null) return Result<User?>.Failure($"Пользователь с externalId {externalId} не найден", "externalId", ErrorType.NotFound);
            return Result<User?>.Success(user);
        }

        public async Task<Result<User?>> AddOAuth2UserAsync(string externalId, string username, string fullName)
        {
            // Check if user with this externalId already exists
            var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.ExternalId == externalId);
            if (existingUser != null)
            {
                return Result<User?>.Failure($"Пользователь с externalId {externalId} уже существует", "externalId", ErrorType.Conflict);
            }

            // Check if user with this username already exists
            var existingUsername = await _db.Users.FirstOrDefaultAsync(u => u.Username.ToUpper() == username.ToUpper());
            if (existingUsername != null)
            {
                return Result<User?>.Failure($"Пользователь с именем '{username}' уже существует", "username", ErrorType.Conflict);
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = username,
                FullName = fullName,
                ExternalId = externalId
            };

            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();

            return Result<User?>.Success(user);
        }

        public async Task<Result<UserMute>> MuteUserAsync(Guid userId, Guid mutedByUserId, TimeSpan duration, string? reason)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null) return Result<UserMute>.Failure($"User {userId} not found", "userId", ErrorType.NotFound);

            // Deactivate any existing active mutes
            var existingMutes = await _db.UserMutes
                .Where(m => m.UserId == userId && m.MutedUntil > DateTime.UtcNow)
                .ToListAsync();
            
            // Mark all existing active mutes as expired by setting MutedUntil to now
            foreach (var mute in existingMutes)
            {
                mute.MutedUntil = DateTime.UtcNow;
            }

            var userMute = new UserMute
            {
                UserId = userId,
                MutedByUserId = mutedByUserId,
                Reason = reason ?? "No reason provided",
                MutedUntil = DateTime.UtcNow.Add(duration),
                CreatedAt = DateTime.UtcNow
            };

            await _db.UserMutes.AddAsync(userMute);
            await _db.SaveChangesAsync();

            return Result<UserMute>.Success(userMute);
        }

        public async Task<Result> UnmuteUserAsync(Guid userId)
        {
            var activeMute = await _db.UserMutes
                .Where(m => m.UserId == userId && m.MutedUntil > DateTime.UtcNow)
                .FirstOrDefaultAsync();

            if (activeMute == null) return Result.Success(); // No active mute, nothing to do

            activeMute.MutedUntil = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result<UserMute?>> GetActiveMuteAsync(Guid userId)
        {
            var mute = await _db.UserMutes
                .Include(m => m.MutedByUser)
                .Where(m => m.UserId == userId && m.MutedUntil > DateTime.UtcNow)
                .OrderByDescending(m => m.CreatedAt)
                .FirstOrDefaultAsync();

            return Result<UserMute?>.Success(mute);
        }

        public async Task<Result<List<UserMute>>> GetAllActiveMutesAsync(int page = 1, int pageSize = 20)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var mutes = await _db.UserMutes
                .Include(m => m.User)
                .Include(m => m.MutedByUser)
                .Where(m => m.MutedUntil > DateTime.UtcNow)
                .OrderByDescending(m => m.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Result<List<UserMute>>.Success(mutes);
        }

        public async Task<int> CountActiveMutesAsync()
        {
            return await _db.UserMutes.CountAsync(m => m.MutedUntil > DateTime.UtcNow);
        }
    }
}
