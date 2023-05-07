using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskBoardAPI.Class;
using TaskBoardAPI.Utils;

namespace TaskBoardAPI.Controllers.Common
{
    public class GetLoginToken
    {
        public static TokenProviderOptions GetOptions()
        {
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("DharmanandanDiampndsPvtLtd"));

            return new TokenProviderOptions
            {
                Path = "/api/token",
                Audience = "DDPLCoreAudience",
                Issuer = "DDPLCoreIssuer",
                Expiration = TimeSpan.FromMinutes(Convert.ToInt32("720")),
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
            };
        }

        public static LoginResponseData Execute(DataTable tblData, string client_id)
        {
            var options = GetOptions();
            var now = DateTime.UtcNow;

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUniversalTime().ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim(ClaimTypes.Role,tblData.Rows[0]["role"].ToString() )
            };

            var jwt = new JwtSecurityToken(
               issuer: options.Issuer,
               audience: options.Audience,
               claims: claims.ToArray(),
               notBefore: now,
               expires: now.Add(options.Expiration),
               signingCredentials: options.SigningCredentials);
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new LoginResponseData
            {
                Access_token = encodedJwt,
                Expires_in = (int)options.Expiration.TotalSeconds,
                Client_id = client_id,
                Token_type = "Bearer",
                Issued = now,
                Expires = now.Add(options.Expiration),
                firstName = tblData.Rows[0]["firstName"].ToString(),
                lastName = tblData.Rows[0]["lastName"].ToString(),
                profileImg = (byte[])tblData.Rows[0]["profileImg"],
                role = tblData.Rows[0]["role"].ToString(),
                userId = pub.GetInt(tblData.Rows[0]["userId"]),
                userName = tblData.Rows[0]["userName"].ToString()
            };
            return response;
        }
    }
}
