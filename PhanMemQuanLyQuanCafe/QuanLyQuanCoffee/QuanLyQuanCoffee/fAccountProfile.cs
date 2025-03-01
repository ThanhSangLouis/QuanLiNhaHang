using QuanLyQuanCoffee.DAO;
using QuanLyQuanCoffee.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyQuanCoffee
{
    public partial class fAccountProfile : Form
    {
        private Account loginAccount;

        public Account LoginAccount
        {
            get => loginAccount;
            set
            {
                loginAccount = value;
                changeAccount(loginAccount);
            }
        }
        public fAccountProfile(Account account)
        {
            InitializeComponent();
            this.LoginAccount = account;
        }
        void changeAccount(Account account)
        {
            txbUserName.Text = LoginAccount.UserName;
            txbDisplayName.Text = LoginAccount.DisplayName;
        }
        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        void updateAccount()
        {
            string displayName = txbDisplayName.Text;
            string password = txbPassword.Text;
            string newpass = txbNewPass.Text;
            string reenterPass = txbReEnterNewPass.Text;
            string userName = txbUserName.Text;

            if (!newpass.Equals(reenterPass))
            {
                MessageBox.Show("Please re-enter the correct password with the new password!");
            }
            else
            {
                if (AccountDAO.Instance.updateAccount(userName, displayName, password, newpass))
                {
                    MessageBox.Show("Update successful!");
                    if (updateAccount1 != null)
                    {
                        updateAccount1(this, new AccountEvent(AccountDAO.Instance.GetAccountByUserName(userName)));
                    }
                }
                else
                {
                    MessageBox.Show("Please enter the correct password!");
                }
            }
        }
        private event EventHandler<AccountEvent> updateAccount1;
        public event EventHandler<AccountEvent> UpdateAccount
        {
            add { updateAccount1 += value; }
            remove { updateAccount1 -= value; }
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            updateAccount();
        }
    }
    public class AccountEvent : EventArgs
    {
        private Account acc;
        public Account Acc { get => acc; set => acc = value; }
        public AccountEvent(Account acc)
        {
            this.Acc = acc;
        }
    }
}
