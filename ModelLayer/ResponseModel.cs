namespace ModelLayer
{
    public class ResponseModel<T>
    {
        public bool success { get; set; } = true;
        public string message { get; set; } = "";
        public T? data { get; set; }

    }
}