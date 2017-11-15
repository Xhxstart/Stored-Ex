namespace KokyakuRenkei.Common.Mapping.Transaction
{
    public class TranColumn
    {
        public string ColumnNm { get; set; }
        public object DefaultValue { get; set; }
        public string RefReportDataKey { get; set; }

        private object value;

        public object Value
        {
            get
            {
                if (this.value != null)
                {
                    return (object)this.value.ToString().Trim();
                }
                else
                {
                    return this.value;
                }
            }
            set
            {
                this.value = value;
            }
        }

        public bool Condition { get; set; }
    }
}