using System;
using System.Threading.Tasks;

namespace UnitTestPractices.Core.V2
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
            
            // The call below will not be caught! The test will be false positive.
            await _temperatureMeasurementsRepo.PostAsync(default, default); 
            
            await _temperatureMeasurementsRepo.PostAsync(data.Timestamp.Value, data.Temperature.Value);
        }
        
        
        public async Task<MeasurementsData> GetMeasurementsAsync(DateTime timestamp)
        {
            // This will cause a false negative test
            // This extra call is not needed here, but it has nothing
            // to do with the output of this method. It still works correctly.
            var _ = await _temperatureMeasurementsRepo.GetAsync(DateTime.UtcNow);
            
            var temp = await _temperatureMeasurementsRepo.GetAsync(timestamp);

            return new MeasurementsData
            {
                Temperature = temp,
                Timestamp = timestamp
            };
        }
    }
}