using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rema.WebApi.Controllers;

namespace Prime.UnitTests.Services
{
    [TestClass]
    public class UserControllerTests
    {
        private readonly UsersController _userController;

        public UserControllerTests()
        {
            // _userController = new UsersController();
        }

        [TestMethod]
        public void IsPrime_InputIs1_ReturnFalse()
        {
            // var result = _primeService.IsPrime(1);

            // Assert.IsFalse(result, "1 should not be prime");
        }
    }
}
