using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCoffee.DTO
{
    public class Table
    {
        public Table(int id, string name, string status) 
        {
            this.ID = id;
            this.Name = name;
            this.Status = status;
            this.ReservationStatus = reservationStatus;
        }
        public Table(DataRow row)
        {
            this.ID = (int)row["id"];
            this.Name = row["name"].ToString();
            this.Status = row["status"].ToString();
            this.ReservationStatus = row["reservationStatus"] != DBNull.Value ? (int)row["reservationStatus"] : 0; // Kiểm tra NULL
        }
        private string status;
        private string name;
        private int iD;
        private int reservationStatus;
        public int ID { get => iD; set => iD = value; }
        public string Name { get => name; set => name = value; }
        public string Status { get => status; set => status = value; }
        public int ReservationStatus { get => reservationStatus; set => reservationStatus = value; }
    }
}
