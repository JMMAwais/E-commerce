using E_commerce.Data;
using E_commerce.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.Services
{
    public class TokenDbService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public TokenDbService(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<ApplicationUser> GetRefreshTokenAsync(string token)
        {
           var tok= await _userManager.Users.Include(a => a.RefreshTokens).FirstOrDefaultAsync(a => a.RefreshTokens.Any(w => w.Token == token));
            var refreshToken =await _userManager.Users.Include(u => u.RefreshTokens)
             .SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));
            return tok;
        }
        public async Task AddRefreshTokenAsync(RefreshToken refreshToken, string userId)
        {
            refreshToken.UserId = userId;
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateRefreshTokenAsync(RefreshToken token)
        {
            _context.RefreshTokens.Update(token);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
                return false;

            var user = await _context.Users
        .Include(u => u.RefreshTokens)
        .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refreshToken));
            if (user == null)
            {
                return false;
            }
            var token = user.RefreshTokens.FirstOrDefault(t => t.Token == refreshToken);
            if (token != null)
            {
               user.RefreshTokens.Remove(token); // or mark it as revoked
                await _context.SaveChangesAsync();
                return true;
            }
            else {
                return false;
            }
               
        }
    }
}
