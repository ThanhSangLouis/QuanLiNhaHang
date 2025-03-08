using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuanLyQuanCoffee.DTO;
using System.Threading.Tasks;
using System.Data;

namespace QuanLyQuanCoffee.DAO
{
    public class TableDAO
    {
        private static TableDAO instance;
        public static TableDAO Instance
        {
            get { if (instance == null) instance = new TableDAO(); return TableDAO.instance; }
            private set { TableDAO.instance = value; }
        }
        public static int TableWidth = 100;
        public static int TableHeight = 100;
        private TableDAO() { }
        public void SwitchTable(int id1, int id2)
        {
            DataProvider.Instance.ExecuteQuery("USP_SwitchTable @idTable1 , @idTable2", new object[] { id1, id2 });
        }
        public List<Table> loadTableList()
        {
            List<Table> tableList = new List<Table>();
            DataTable data = DataProvider.Instance.ExecuteQuery("SELECT * from dbo.TableFood");
            foreach (DataRow item in data.Rows)
            {
                Table table = new Table(item);  
                tableList.Add(table);
            }
            return tableList;
        }
        // Thêm tính năng đặt bàn và huỷ đặt bàn
        public bool BookTable(int idTable, string customerName, string phone, DateTime reservationTime)
        {
            string query = "EXEC USP_BookTable @idTable , @customerName , @phone , @reservationTime";
            int result = DataProvider.Instance.ExecuteNonQuery(query, new object[] { idTable, customerName, phone, reservationTime });

            //Sau khi đặt bàn, cập nhật trạng thái bàn thành "Có Người"
            string updateQuery = "UPDATE TableFood SET status = N'Có người' WHERE id = @idTable";
            DataProvider.Instance.ExecuteNonQuery(updateQuery, new object[] { idTable });

            return result > 0;
        }

        public bool CancelReservation(int idTable)
        {
            string query = "EXEC USP_CancelReservation @idTable";
            int result = DataProvider.Instance.ExecuteNonQuery(query, new object[] { idTable });
            return result > 0;
        }

    }
}
