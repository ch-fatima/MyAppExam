using Application.User.Query.GetUser;
using Core.Models.Dtos;
using Infrastructure.Cryptography;
using Infrastructure.UnitOfWork;
using Moq;
using Xunit;

namespace AllTests
{
    public class GetUserQueryTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly GetUserQueryHandler _handler;
        private readonly Mock<ICryptographyService> _cryptoService;


        public GetUserQueryTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _cryptoService = new Mock<ICryptographyService>();
            _handler = new GetUserQueryHandler(_unitOfWorkMock.Object, _cryptoService.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsUserList()
        {
            // Arrange
            var expectedUsers = new List<UserDto>
            {
                new UserDto { 
                    Id = Guid.NewGuid(), 
                    FirstName = "John",
                    LastName = "Doe", 
                    Email = "john@example.com" 
                },
                new UserDto {
                    Id = Guid.NewGuid(),
                    FirstName = "Jane", 
                    LastName = "Smith", 
                    Email = "jane@example.com" 
                }
            };

            var query = new GetUserQuery
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                PhoneNo = "09123456789",
                UserName = "johndoe",
                IsActive = true
            };

            _unitOfWorkMock.Setup(x => x.UserRepository.GetUsers(It.IsAny<GetUserDto>()))
                .ReturnsAsync(expectedUsers);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(result.Count, expectedUsers.Count);
        }

        [Fact]
        public async Task Handle_WithEmptyFilters_ReturnsAllUsers()
        {
            // Arrange
            var allUsers = new List<UserDto>
            {
                new UserDto { Id = Guid.NewGuid(), FirstName = "Ali", LastName = "Rezaei" },
                new UserDto { Id = Guid.NewGuid(), FirstName = "Mohammad", LastName = "Karimi" },
                new UserDto { Id = Guid.NewGuid(), FirstName = "Sara", LastName = "Ahmadi" }
            };

            var query = new GetUserQuery(); 

            _unitOfWorkMock.Setup(x => x.UserRepository.GetUsers(It.Is<GetUserDto>(dto =>
                dto.FirstName == null &&
                dto.LastName == null &&
                dto.Email == null)))
                .ReturnsAsync(allUsers);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(result.Count, allUsers.Count);
        }

        [Fact]
        public async Task Handle_WithPartialFilters_ReturnsFilteredUsers()
        {
            // Arrange
            var expectedUsers = new List<UserDto>
            {
                new UserDto { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" }
            };

            var query = new GetUserQuery
            {
                FirstName = "John",
                LastName = "Doe"
            };

            _unitOfWorkMock.Setup(x => x.UserRepository.GetUsers(It.Is<GetUserDto>(dto =>
                dto.FirstName == "John" &&
                dto.LastName == "Doe")))
                .ReturnsAsync(expectedUsers);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(result.Count, expectedUsers.Count);
        }

        [Fact]
        public async Task Handle_RepositoryThrowsException_PropagatesException()
        {
            // Arrange
            var query = new GetUserQuery { FirstName = "Test" };
            var expectedException = new InvalidOperationException("Database connection failed");

            _unitOfWorkMock.Setup(x => x.UserRepository.GetUsers(It.IsAny<GetUserDto>()))
                .ThrowsAsync(expectedException);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(query, CancellationToken.None));

            Assert.Equal("Database connection failed", exception.Message);
        }

        [Fact]
        public async Task Handle_WithCancellation_ShouldHandleCancellation()
        {
            // Arrange
            var query = new GetUserQuery { FirstName = "Test" };
            var cancellationToken = new CancellationToken(true); 

            _unitOfWorkMock.Setup(x => x.UserRepository.GetUsers(It.IsAny<GetUserDto>()))
                .ThrowsAsync(new OperationCanceledException());

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                _handler.Handle(query, cancellationToken));
        }

    }
}
