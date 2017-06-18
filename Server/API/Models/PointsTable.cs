using Microsoft.SqlServer.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models {
    /*
     * Utilizada para passar uma tabela de pontos ao Stored Procedure
     * Mais informações ver: https://msdn.microsoft.com/en-us/library/bb675163.aspx 
     */

    public class PointsTable : IEnumerable<SqlDataRecord> {
        private static readonly SqlMetaData[] metaData;
        private readonly List<SqlDataRecord> rows = new List<SqlDataRecord>();

        public string TypeName {
            get {
                return "dbo.Points";
            }
        }

        static PointsTable() {
            metaData = new SqlMetaData[]
            {
                new SqlMetaData("x", SqlDbType.Int),
                new SqlMetaData("y", SqlDbType.Int),
                new SqlMetaData("idx", SqlDbType.Int),
                new SqlMetaData("pointStyleId", SqlDbType.BigInt),
            };
        }

        public PointsTable() { }

        public PointsTable(params LinePoint[] points) {
            Add(points);
        }

        public PointsTable(IEnumerable<LinePoint> points) {
            Add(points);
        }

        public void Add(params LinePoint[] points) {
            Add((IEnumerable<LinePoint>)points);
        }

        public void Add(IEnumerable<LinePoint> points) {
            foreach(LinePoint point in points) {
                Add(point);
            }
        }

        public void Add(LinePoint point) {
            var row = new SqlDataRecord(metaData);

            row.SetInt32(0, point.X.Value);
            row.SetInt32(1, point.Y.Value);
            row.SetInt32(2, point.Idx.Value);
            row.SetInt64(3, point.Style.Id.Value);

            rows.Add(row);
        }

        public IEnumerator<SqlDataRecord> GetEnumerator() {
            return rows.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return rows.GetEnumerator();
        }
    }
}
