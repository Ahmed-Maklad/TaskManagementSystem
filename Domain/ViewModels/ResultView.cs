namespace Domain.ViewModels
{
    public class ResultView<T>
    {
        public T? Data { get; set; }
        public bool IsSuccess { get; set; }
        public string? Msg { get; set; }
    }

}
