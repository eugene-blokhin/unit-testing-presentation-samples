using System;
using System.Threading.Tasks;

namespace UnitTestPractices.Core.V4
{
    public class MeasurementsData
    {
        public float? Temperature { get; set; }
        public DateTime? Timestamp { get; set; }
    }

    public interface ITemperatureMeasurementsRepo
    {
        Task PostAsync(DateTime timestamp, float temperature);
        Task<float> GetAsync(DateTime timestamp);
    }
    
    public interface IBroadcastMeasurements
    {
        Task PublishAsync(DateTime timestamp, float temperature);
    }
    
    public class MeasurementsService
    {
        private readonly ITemperatureMeasurementsRepo _temperatureMeasurementsRepo;
        private readonly IBroadcastMeasurements _measurementsBroadcaster;

        public MeasurementsService(
            ITemperatureMeasurementsRepo temperatureMeasurementsRepo,
            IBroadcastMeasurements measurementsBroadcaster
        )
        {
            _temperatureMeasurementsRepo = temperatureMeasurementsRepo;
            _measurementsBroadcaster = measurementsBroadcaster;
        }
        
        public async Task SaveMeasurementsAsync(MeasurementsData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (!data.Temperature.HasValue)  
                throw new ArgumentException($"The argument's property {nameof(data.Temperature)} must have a value");
            if (!data.Timestamp.HasValue)  
                throw new ArgumentException($"The argument's property {nameof(data.Timestamp)} must have a value");

            await _measurementsBroadcaster.PublishAsync(data.Timestamp.Value, data.Temperature.Value);
            await _temperatureMeasurementsRepo.PostAsync(data.Timestamp.Value, data.Temperature.Value);
        }

        public async Task<MeasurementsData> GetMeasurementsAsync(DateTime timestamp)
        {
            var temp = await _temperatureMeasurementsRepo.GetAsync(timestamp);

            return new MeasurementsData
            {
                Temperature = temp,
                Timestamp = timestamp
            };
        }
    }
}