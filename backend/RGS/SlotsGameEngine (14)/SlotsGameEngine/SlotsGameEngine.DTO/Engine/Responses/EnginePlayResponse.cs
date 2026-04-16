namespace SlotsGameEngine.DTO.Engine.Responses
{
    public class EnginePlayResponse
    {
        public int StatusCode { get; set; } = 200;
        public string Message { get; set; } = "OK";

        public decimal Win { get; set; }
        public int FreeSpins { get; set; }

        // arbitrary results payload from the engine
        public object? Results { get; set; }

        public Feature? Feature { get; set; }
    }

    public class Feature
    {
        public string? Type { get; set; }
        public string? Name { get; set; }

        // 1 if this is the closing spin of a feature, else 0
        public int IsClosure { get; set; }
    }
}

