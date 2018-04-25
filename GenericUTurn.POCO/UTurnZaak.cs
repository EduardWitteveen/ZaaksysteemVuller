using System;
using System.Data;

namespace GenericUTurn.POCO
{
    public class UTurnZaak : Zaak
    {
        internal UTurnZaak(DataRow row) : base(row)
        {
            if (System.DBNull.Value.Equals(this.Row["Timestamp"])) this.Row["Timestamp"] = System.DateTime.Now;
        }

        public void Update(Zaak backofficezaak)
        {
            foreach(DataColumn column in Row.Table.Columns)
            {
                // never change the zaakid!
                if (column.ColumnName.ToUpper() != "ZAAKID")
                {
                    var backofficevalue = backofficezaak.Row[column.ColumnName];
                    var uturnvalue = Row[column];
                    if (uturnvalue.ToString() != backofficevalue.ToString())
                    {
                        // should be the same type after all?
                        Row[column] = backofficevalue;
                    }
                }
            }
        }
    }
}