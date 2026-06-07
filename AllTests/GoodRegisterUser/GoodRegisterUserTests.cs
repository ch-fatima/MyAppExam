using Application.User.Command.Create;
using Core.Models;
using Core.Models.Entities;
using Infrastructure.Cryptography;
using Infrastructure.Logger;
using Infrastructure.UnitOfWork;
using Moq;
using Xunit;

namespace AllTests.GoodRegisterUser
{
    public class GoodRegisterUserTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILoggerManager> _loggerManagerMock;
        private readonly Mock<ICryptographyService> _cryptoService;
        private readonly InitSetting _initSetting;
        private readonly RegisterUserCommandHandler _handler;

        public GoodRegisterUserTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerManagerMock = new Mock<ILoggerManager>();
            _cryptoService = new Mock<ICryptographyService>();
            _initSetting = new InitSetting
            {
                DefaultuserRoleId = Guid.NewGuid().ToString()
            };

            _handler = new RegisterUserCommandHandler(
                _unitOfWorkMock.Object,
                _loggerManagerMock.Object,
                _initSetting, _cryptoService.Object);
        }

        #region --Success Tests--
        [Fact]
        public async Task Handle_ValidRequest_ShouldRegisterUserSuccessfully()
        {
            // Arrange
            var command = CreateValidCommand();

            _unitOfWorkMock.Setup(x => x.UserRepository.IsExistUser(It.IsAny<User>()))
                .ReturnsAsync(false);

            _unitOfWorkMock.Setup(x => x.UserRepository.InsertAsync(It.IsAny<User>()))
                .ReturnsAsync(CreateValidUser);

            _unitOfWorkMock.Setup(x => x.AuthRepository.InsertAsync(It.IsAny<Auth>()))
                .ReturnsAsync(CreateValidAuth);

            _unitOfWorkMock.Setup(x => x.UserRoleRepository.InsertAsync(It.IsAny<UserRole>()))
                .ReturnsAsync(CreateValidUserRole);

            _unitOfWorkMock.Setup(x => x.BeginTransaction());
            _unitOfWorkMock.Setup(x => x.Commit());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(command.FirstName, result.FirstName);
            Assert.Equal(command.LastName, result.LastName);
            Assert.Equal(command.Email, result.Email);
            Assert.Equal(command.UserName, result.UserName);

            _unitOfWorkMock.Verify(x => x.BeginTransaction(), Times.Once);
            _unitOfWorkMock.Verify(x => x.Commit(), Times.Once);
            _unitOfWorkMock.Verify(x => x.UserRepository.InsertAsync(It.IsAny<Core.Models.Entities.User>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.AuthRepository.InsertAsync(It.IsAny<Auth>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.UserRoleRepository.InsertAsync(It.IsAny<UserRole>()), Times.Once);
        }

        [Fact]
        public async Task Handle_UserWithRoleId_ShouldUseProvidedRoleId()
        {
            // Arrange
            var roleId = Guid.NewGuid();
            var command = CreateValidCommand();
            command.RoleId = roleId;

            _unitOfWorkMock.Setup(x => x.UserRepository.IsExistUser(It.IsAny<User>()))
                .ReturnsAsync(false);

            _unitOfWorkMock.Setup(x => x.UserRepository.InsertAsync(It.IsAny<User>()))
                .ReturnsAsync(CreateValidUser);

            _unitOfWorkMock.Setup(x => x.AuthRepository.InsertAsync(It.IsAny<Auth>()))
                .ReturnsAsync(CreateValidAuth);

            UserRole capturedUserRole = null;
            _unitOfWorkMock.Setup(x => x.UserRoleRepository.InsertAsync(It.IsAny<UserRole>()))
                .Callback<UserRole>(ur => capturedUserRole = ur)
                .ReturnsAsync(CreateValidUserRole);

            _unitOfWorkMock.Setup(x => x.BeginTransaction());
            _unitOfWorkMock.Setup(x => x.Commit());

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(capturedUserRole);
            Assert.Equal(roleId, capturedUserRole.RoleId);
        }

        [Fact]
        public async Task Handle_PasswordIsNull_ShouldSetAuthPasswordAndSaltToNull()
        {
            // Arrange
            var command = CreateValidCommand();
            command.Password = null;

            _unitOfWorkMock.Setup(x => x.UserRepository.IsExistUser(It.IsAny<User>()))
                .ReturnsAsync(false);

            _unitOfWorkMock.Setup(x => x.UserRepository.InsertAsync(It.IsAny<User>()))
                .ReturnsAsync(CreateValidUser);

            Auth capturedAuth = null;
            _unitOfWorkMock.Setup(x => x.AuthRepository.InsertAsync(It.IsAny<Auth>()))
                .Callback<Auth>(auth => capturedAuth = auth)
                .ReturnsAsync(CreateValidAuth);

            _unitOfWorkMock.Setup(x => x.UserRoleRepository.InsertAsync(It.IsAny<UserRole>()))
                .ReturnsAsync(CreateValidUserRole);

            _unitOfWorkMock.Setup(x => x.BeginTransaction());
            _unitOfWorkMock.Setup(x => x.Commit());

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(capturedAuth);
            Assert.Null(capturedAuth.Password);
            Assert.Null(capturedAuth.Salt);
        }

        [Fact]
        public async Task Handle_ShouldSetLoginCodeExpirationToOneMinuteFromNow()
        {
            // Arrange
            var command = CreateValidCommand();
            var beforeExecution = DateTime.Now;

            _unitOfWorkMock.Setup(x => x.UserRepository.IsExistUser(It.IsAny<User>()))
                .ReturnsAsync(false);

            _unitOfWorkMock.Setup(x => x.UserRepository.InsertAsync(It.IsAny<User>()))
                .ReturnsAsync(CreateValidUser);

            Auth capturedAuth = null;
            _unitOfWorkMock.Setup(x => x.AuthRepository.InsertAsync(It.IsAny<Auth>()))
                .Callback<Auth>(auth => capturedAuth = auth)
                .ReturnsAsync(CreateValidAuth);

            _unitOfWorkMock.Setup(x => x.UserRoleRepository.InsertAsync(It.IsAny<UserRole>()))
                .ReturnsAsync(CreateValidUserRole);

            _unitOfWorkMock.Setup(x => x.BeginTransaction());
            _unitOfWorkMock.Setup(x => x.Commit());

            // Act
            await _handler.Handle(command, CancellationToken.None);
            var afterExecution = DateTime.Now;

            // Assert
            Assert.NotNull(capturedAuth);
            Assert.NotNull(capturedAuth.LoginCode);
            Assert.Equal(0, capturedAuth.LoginCodeTryCount);
            Assert.True(capturedAuth.LoginCodeExpirationDate >= beforeExecution.AddMinutes(0.9));
            Assert.True(capturedAuth.LoginCodeExpirationDate <= afterExecution.AddMinutes(1.1));
        }

        #endregion

        #region --Fail Tests--
        #endregion

        private RegisterUserCommand CreateValidCommand()
        {
            return new RegisterUserCommand
            {
                FirstName = "fateme",
                LastName = "chourli",
                Email = "chourli@gmail.com",
                UserName = "chourli",
                Password = "ValidPass123!",
                PhoneNo = "09999999999",
                RoleId = Guid.NewGuid(),
                CreateDate = "2024/12/25"
            };
        }

        private User CreateValidUser()
        {
            return new User
            {
                FirstName = "fateme",
                LastName = "chourli",
                Email = "chourli@gmail.com",
                UserName = "chourli",
                PhoneNo = "09999999999"
            };
        }
        private Auth CreateValidAuth()
        {
            return new Auth
            {
                PhoneNo = "09999999999"
            };
        }
        private UserRole CreateValidUserRole()
        {
            return new UserRole
            {
                UserId = Guid.NewGuid(),
                Id = Guid.NewGuid(),
                RoleId = Guid.NewGuid()
            };
        }
    }
}
