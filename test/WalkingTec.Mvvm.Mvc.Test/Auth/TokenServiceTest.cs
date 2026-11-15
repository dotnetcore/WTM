using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Auth;
using WalkingTec.Mvvm.Mvc.Auth;

namespace WalkingTec.Mvvm.Mvc.Test.Auth
{
    [TestClass]
    public class TokenServiceTest
    {
        private TokenService _service;

        public TokenServiceTest()
        {
            var jwtOpts = new JwtOption
            {
                Issuer = "wtm-test",
                Audience = "wtm-test-audience",
                SecurityKey = "this-is-a-test-security-key-for-wtm-unit-tests-32chars!",
                Expires = 3600
            };
            var configs = new Configs { JwtOptions = jwtOpts };
            var monitor = Mock.Of<IOptionsMonitor<Configs>>(m => m.CurrentValue == configs);
            _service = new TokenService(monitor);
        }

        [TestMethod]
        public void IssueToken_ValidLoginUser_ReturnsToken()
        {
            var loginUser = new LoginUserInfo { ITCode = "admin", Name = "Administrator" };
            var token = _service.IssueTokenAsync(loginUser).Result;

            Assert.IsNotNull(token);
            Assert.IsFalse(string.IsNullOrEmpty(token.AccessToken));
            Assert.IsTrue(token.ExpiresIn > 0);
        }

        [TestMethod]
        public void IssueToken_NullLoginUser_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<System.AggregateException>(() =>
                _service.IssueTokenAsync(null!).Result);
        }

        [TestMethod]
        public void IssueToken_TokenContainsCorrectClaims()
        {
            var loginUser = new LoginUserInfo
            {
                ITCode = "testuser", Name = "Test User", TenantCode = "tenant1"
            };

            var token = _service.IssueTokenAsync(loginUser).Result;
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token.AccessToken);

            var subClaim = jwt.Claims.FirstOrDefault(c => c.Type == AuthConstants.JwtClaimTypes.Subject);
            Assert.IsNotNull(subClaim);
            Assert.AreEqual("testuser", subClaim.Value);

            var tenantClaim = jwt.Claims.FirstOrDefault(c => c.Type == AuthConstants.JwtClaimTypes.TenantCode);
            Assert.IsNotNull(tenantClaim);
            Assert.AreEqual("tenant1", tenantClaim.Value);
        }

        [TestMethod]
        public void IssueToken_DifferentUsers_ProduceDifferentTokens()
        {
            var t1 = _service.IssueTokenAsync(new LoginUserInfo { ITCode = "user1" }).Result;
            var t2 = _service.IssueTokenAsync(new LoginUserInfo { ITCode = "user2" }).Result;
            Assert.AreNotEqual(t1.AccessToken, t2.AccessToken);
        }

        [TestMethod]
        public void IssueToken_TokenCanBeValidated()
        {
            var token = _service.IssueTokenAsync(new LoginUserInfo { ITCode = "admin" }).Result;
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token.AccessToken);

            Assert.AreEqual("wtm-test", jwt.Issuer);
            Assert.IsTrue(jwt.ValidTo > System.DateTime.UtcNow);
        }

        [TestMethod]
        public void IssueToken_ShortSecurityKey_IsPaddedTo32()
        {
            var jwtOpts = new JwtOption { SecurityKey = "short" };
            Assert.IsTrue(jwtOpts.SecurityKey.Length >= 32);
            Assert.IsTrue(jwtOpts.SecurityKey.StartsWith("short"));
        }

        [TestMethod]
        public void IssueToken_RemoteToken_AddedAsClaim()
        {
            var loginUser = new LoginUserInfo { ITCode = "admin", RemoteToken = "remote-token-value" };
            var token = _service.IssueTokenAsync(loginUser).Result;
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token.AccessToken);

            var rtClaim = jwt.Claims.FirstOrDefault(c => c.Type == AuthConstants.JwtClaimTypes.RToken);
            Assert.IsNotNull(rtClaim);
            Assert.AreEqual("remote-token-value", rtClaim.Value);
        }
    }
}
