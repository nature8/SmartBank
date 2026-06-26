//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace CustomerService.Tests.Services
//{
//    internal class CustomerBusinessTests
//    {
//    }
//}


using Xunit;

namespace CustomerService.Tests.Services
{
    public class CustomerBusinessTests
    {
        [Fact]
        public void SampleTest_ShouldPass()
        {
            // Arrange
            int a = 5;
            int b = 5;

            // Act
            int result = a + b;

            // Assert
            Assert.Equal(10, result);
        }
    }
}