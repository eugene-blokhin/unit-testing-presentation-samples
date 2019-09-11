using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using NUnit.Framework;
using UnitTestPractices.Core.V4;

namespace UnitTestPractices.Tests
{
    [TestFixture]
    class V4
    {
        /*TODO - remove comments
        public class UserServiceTests_DONTs
        {
            [Test]
            public async Task SaveMeasurementsAsync_ShouldSaveTemperature()
            {
                // Arrange
                var data = new MeasurementsData
                {
                    Timestamp = DateTime.Now,
                    Temperature = 23.3f
                };

                float? savedTemp = null;
                DateTime? savedTimestamp = null;

                var temperatureRepo = new Mock<ITemperatureMeasurementsRepo>();
                temperatureRepo
                    .Setup(r => r.PostAsync(
                        It.IsAny<DateTime>(),
                        It.IsAny<float>()
                    ))
                    .Returns((DateTime timestamp, float temp) =>
                    {
                        (savedTimestamp, savedTemp) = (timestamp, temp);
                        return Task.CompletedTask;
                    });

                var sut = new MeasurementsService(temperatureRepo.Object);

                // Act
                await sut.SaveMeasurementsAsync(data);

                // Assert
                Assert.That(savedTemp, Is.EqualTo(data.Temperature));
                Assert.That(savedTimestamp, Is.EqualTo(data.Timestamp));
            }

            [Test]
            public async Task GetMeasurements_ShouldReturnMeasurements()
            {
                // Arrange
                var temperature = 23.4f;
                var timestamp = DateTime.Now;

                var repoMock = new Mock<ITemperatureMeasurementsRepo>(MockBehavior.Strict);
                repoMock.Setup(r => r.GetAsync(It.IsAny<DateTime>()))
                    .ReturnsAsync(temperature);

                var sut = new MeasurementsService(repoMock.Object);

                // Act
                var result = await sut.GetMeasurementsAsync(timestamp);

                // Assert
                Assert.That(result.Timestamp, Is.EqualTo(timestamp));
                Assert.That(result.Temperature, Is.EqualTo(temperature));
            }
        }
        */

        public class UserServiceTests_DOs
        {
            private IFixture _fixture;

            [SetUp]
            public void BeforeTest()
            {
                _fixture = new Fixture().Customize(new AutoMoqCustomization());
            }
            
            [Test]
            public async Task SaveMeasurementsAsync_ShouldSaveTemperature()
            {
                var data = _fixture.Create<MeasurementsData>();

                var tempRepo = new Mock<ITemperatureMeasurementsRepo>(MockBehavior.Strict);
                tempRepo.Setup(r => r.PostAsync(data.Timestamp.Value, data.Temperature.Value))
                    .Returns(Task.CompletedTask)
                    .Verifiable();

                _fixture.Inject(tempRepo.Object);

                var sut = _fixture.Create<MeasurementsService>();

                await sut.SaveMeasurementsAsync(data);

                tempRepo.Verify();
            }
            
            // We just need to add new test method
            [Test]
            public async Task SaveMeasurementsAsync_ShouldBroadcastMeasurements()
            {
                var data = _fixture.Create<MeasurementsData>();

                var tempRepo = new Mock<IBroadcastMeasurements>(MockBehavior.Strict);
                tempRepo.Setup(r => r.PublishAsync(data.Timestamp.Value, data.Temperature.Value))
                    .Returns(Task.CompletedTask)
                    .Verifiable();

                _fixture.Inject(tempRepo.Object);

                var sut = _fixture.Create<MeasurementsService>();

                await sut.SaveMeasurementsAsync(data);

                tempRepo.Verify();
            }

            [Test]
            public async Task GetMeasurements_ShouldReturnTemperature()
            {
                // Arrange
                var temperature = _fixture.Create<float>();
                var timestamp = _fixture.Create<DateTime>();

                var repoMock = new Mock<ITemperatureMeasurementsRepo>();
                repoMock.Setup(r => r.GetAsync(timestamp))
                    .ReturnsAsync(temperature);

                _fixture.Inject(repoMock.Object);

                var sut = _fixture.Create<MeasurementsService>();

                // Act
                var result = await sut.GetMeasurementsAsync(timestamp);

                // Assert
                Assert.That(result.Timestamp, Is.EqualTo(timestamp));
                Assert.That(result.Temperature, Is.EqualTo(temperature));
            }
        }
    }
}