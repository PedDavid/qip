using QIP.Domain;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlTypes;

namespace QIP.Repositories.Model {
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
                new SqlMetaData("pointStyleWidth", SqlDbType.Int)
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

            row.SetInt32(0, point.X);
            row.SetInt32(1, point.Y);
            row.SetInt32(2, point.Idx);
            row.SetSqlInt32(3, point.Style.Width??SqlInt32.Null);

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
