using System;
using System.Collections.Generic;
using System.Text;

namespace ReplicationTransformCleaner.OrmLiteTables
{
    public class TxErrorsForInsert
    {
        public Guid TransactionId { get; set; }
        public bool HasBeenTransformed { get; set; }
    }
}