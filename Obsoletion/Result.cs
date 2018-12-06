namespace Obsoletion
{
    public class Result
    {
        public string Message { get; set; }
        public string ObsoletionDate { get; set; }
        public string Symbol { get; set; }
        public string Recommendation { get; set; }
        public string Type { get; set; }


        public override string ToString()
        {
            return string.Join(",", Symbol, Type, Recommendation, ObsoletionDate, Message);
        }
    }
}