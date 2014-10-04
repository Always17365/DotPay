using System;
using Xunit;

namespace DotPay.Common.Test
{
    public class UtilitiesTest
    {
        [Fact]
        public void TestGenerateOTP()
        {
            var otp = Utilities.GenerateGoogleAuthOTP("DPI45HCEBCJK6HG7");

            Assert.Equal(otp.Length, 6);
        }
    }
}
