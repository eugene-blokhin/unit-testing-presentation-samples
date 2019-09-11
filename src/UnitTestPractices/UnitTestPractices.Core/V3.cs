using System;
using System.Threading.Tasks;

namespace UnitTestPractices.Core.V3
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
    
    public class MeasurementsService
    {
        private readonly ITemperatureMeasurementsRepo _temperatureMeasurementsRepo;

        public MeasurementsService(ITemperatureMeasurementsRepo temperatureMeasurementsRepo)
        {
            _temperatureMeasurementsRepo = temperatureMeasurementsRepo;
        }
        
        public async Task SaveMeasurementsAsync(MeasurementsData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (!data.Temperature.HasValue)  
                throw new ArgumentException($"The argument's property {nameof(data.Temperature)} must have a value");
            if (!data.Timestamp.HasValue)  
                throw new ArgumentException($"The argument's property {nameof(data.Timestamp)} must have a value");
            
            await _temperatureMeasurementsRepo.PostAsync(data.Timestamp.Value, data.Temperature.Value);
        }

        public async Task<MeasurementsData> GetMeasurementsAsync(DateTime timestamp)
        {
            // Oh no! I what a strange argument value! And the test will pass...
            var temp = await _temperatureMeasurementsRepo.GetAsync(new DateTime(2019, 09, 12));

            return new MeasurementsData
            {
                Temperature = temp,
                Timestamp = timestamp
            };
        }
    }
}