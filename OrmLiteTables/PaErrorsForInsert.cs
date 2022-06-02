using System;
using System.Collections.Generic;
using System.Text;

namespace ReplicationTransformCleaner.OrmLiteTables
{
    public class PaErrorsForInsert
    {
        public Guid PaymentArrangementId { get; set; }
        public bool HasBeenTransformed { get; set; }
    }
}