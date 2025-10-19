namespace Mediarq.Tests.Core.Common.User
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Mediarq.Core.Common.User;
    using Xunit;

    public class DefaultUserContextTests
    {
        private readonly DefaultUserContext _testClass;
        private string _userId;
        private string _userName;
        private IEnumerable<string> _roles;

        public DefaultUserContextTests()
        {
            _userId = "TestValue2040072159";
            _userName = "TestValue124943459";
            _roles = new[] { "TestValue1479215693", "TestValue1462738427", "TestValue580002003" };
            _testClass = new DefaultUserContext(_userId, _userName, _roles);
        }

        [Fact]
        public void CanConstruct()
        {
            // Act
            var instance = new DefaultUserContext(_userId, _userName, _roles);

            // Assert
            instance.Should().NotBeNull();
        }

        [Fact]
        public void ImplementsIEquatable_DefaultUserContext()
        {
            // Arrange
            var same = new DefaultUserContext(_userId, _userName, _roles);
            var different = new DefaultUserContext("TestValue105060810", "TestValue2091252392", new[] { "TestValue820090765", "TestValue296905130", "TestValue1925101478" });

            // Assert
            _testClass.Equals(default(object)).Should().BeFalse();
            _testClass.Equals(new object()).Should().BeFalse();
            _testClass.Equals((object)same).Should().BeTrue();
            _testClass.Equals((object)different).Should().BeFalse();
            _testClass.Equals(same).Should().BeTrue();
            _testClass.Equals(different).Should().BeFalse();
            _testClass.GetHashCode().Should().Be(same.GetHashCode());
            _testClass.GetHashCode().Should().NotBe(different.GetHashCode());
            (_testClass == same).Should().BeTrue();
            (_testClass == different).Should().BeFalse();
            (_testClass != same).Should().BeFalse();
            (_testClass != different).Should().BeTrue();
        }

        [Fact]
        public void UserIdIsInitializedCorrectly()
        {
            _testClass.UserId.Should().Be(_userId);
        }

        [Fact]
        public void UserNameIsInitializedCorrectly()
        {
            _testClass.UserName.Should().Be(_userName);
        }

        [Fact]
        public void RolesIsInitializedCorrectly()
        {
            _testClass.Roles.Should().BeSameAs(_roles);
        }
    }
}