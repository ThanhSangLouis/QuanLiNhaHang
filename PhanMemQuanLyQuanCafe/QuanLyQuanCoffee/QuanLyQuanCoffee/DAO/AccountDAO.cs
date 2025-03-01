using QuanLyQuanCoffee.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCoffee.DAO
{
    internal class AccountDAO
    {
        private static AccountDAO instance;

        public static AccountDAO Instance
        {
            get { if (instance == null) instance = new AccountDAO(); return instance; }
            private set { instance = value; }

        }
        private AccountDAO() { }
        public bool Login(string userName, string passWord)
        {
            byte[] temp = ASCIIEncoding.ASCII.GetBytes(passWord); //Lấy ra mảng byte từ chuỗi passWord
            byte[] hasData = new MD5CryptoServiceProvider().ComputeHash(temp); //Mã hóa mảng byte thành mảng byte khác
            string hasPass = "";
            foreach (byte item in hasData)
            {
                hasPass += item;
            }
            //var list = hasData.ToString();
            //list.Reverse();

            string query = "USP_Login @userName , @passWord";
            DataTable result = DataProvider.Instance.ExecuteQuery(query, new object[] {userName, hasPass});

            return result.Rows.Count > 0;
        }
        public Account GetAccountByUserName(string userName)
        {
            DataTable data = DataProvider.Instance.ExecuteQuery("SELECT * FROM Account WHERE userName = '" + userName + "'");
            foreach (DataRow item in data.Rows)
            {
                return new Account(item);
            }
            return null;
        }
        public bool updateAccount(string userName, string displayName, string pass, string newPass)
        {
           int result = DataProvider.Instance.ExecuteNonQuery("EXEC USP_UpdateAccount @userName , @displayName , @password , @newPassword", new object[] { userName, displayName, pass, newPass });
           return result > 0;
        }
        public DataTable GetListAccount()
        {
            return DataProvider.Instance.ExecuteQuery("SELECT userName, displayName, type FROM Account");
        }
        public bool InsertAccount(string userName, string displayName, int type)
        {
            string query = string.Format("INSERT dbo.Account ( userName, displayName, type, passWord )VALUES  ( N'{0}', N'{1}', {2}, N'{3}')", userName, displayName, type, "20720532132149213101239102231223249249135100218");
            int result = DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }   

        public bool UpdateAccount(string userName, string displayName, int type)
        {
            string query = string.Format("UPDATE dbo.Account SET displayName = N'{1}', type = {2} WHERE userName = N'{0}'", userName, displayName, type);
            int result = DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }
        public bool DeleteAccount(string name )
        {

            string query = string.Format("Delete Account where userName = N'{0}'",name);
            int result = DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }
        public bool ResetPassword(string userName)
        {
            string query = string.Format("UPDATE dbo.Account SET passWord = N'20720532132149213101239102231223249249135100218' WHERE userName = N'{0}'", userName);
            int result = DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }
    }
}
