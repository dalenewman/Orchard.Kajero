namespace Kajero.Models {
    public class SaveResponse {
        public int Status { get; set; }
        public string Message { get; set; }

        public SaveResponse() {
            Status = 200;
            Message = "Ok";
        }

        public SaveResponse(int status, string message) {
            Status = status;
            Message = message;
        }
    }
}