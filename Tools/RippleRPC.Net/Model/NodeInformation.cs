using System.Dynamic;

namespace RippleRPC.Net.Model
{
    public class NodeInformation
    {
        public ExpandoObject FinalFields { get; set; }
        public string LedgerEntryType { get; set; }
        public string LedgerIndex { get; set; }
        public ExpandoObject PreviousFields { get; set; }
        public string PreviousTxnID { get; set; }
        public int PreviousTxnLgrSeq { get; set; }

    }
}
