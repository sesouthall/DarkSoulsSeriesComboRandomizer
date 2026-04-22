namespace DarkSoulsSeriesComboRandomizer
{
    public class SuccessOrError
    {
        public bool Succeeded { get; private set; }
        public List<string> Errors { get; private set; }

        public SuccessOrError()
        {
            this.Succeeded = true;
            this.Errors = new List<string>();
        }

        public void AddError(string errorMessage)
        {
            this.Succeeded = false;
            this.Errors.Add(errorMessage);
        }
    }
}