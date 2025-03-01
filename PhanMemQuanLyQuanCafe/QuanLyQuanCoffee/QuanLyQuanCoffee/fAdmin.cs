using QuanLyQuanCoffee.DAO;
using QuanLyQuanCoffee.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace QuanLyQuanCoffee
{
    public partial class fAdmin : Form
    {
        BindingSource foodList = new BindingSource();
        BindingSource accountList = new BindingSource();
        public Account loginAccount;
        public fAdmin()
        {
            InitializeComponent();
            load();
        }
        void load()
        {
            dtgvFood.DataSource = foodList;
            dtgvAccount.DataSource = accountList;
            loadDateTimePickerBill();
            loadListBillByDate(dtpkFromDate.Value, dtpkToDate.Value);
            loadListFood();
            loadAccountList();
            AddFoodBinding();
            AddAccountBinding();
            loadCategoryIntoCombobox(cbFoodCategory);
        }
        void AddAccountBinding()
        {
            txbUserName.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "UserName", true, DataSourceUpdateMode.Never));
            txbDisplayName.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "DisplayName", true, DataSourceUpdateMode.Never));
            nmAccountType.DataBindings.Add(new Binding("Value", dtgvAccount.DataSource, "Type", true, DataSourceUpdateMode.Never));

        }
        void loadAccountList()
        {
            accountList.DataSource = AccountDAO.Instance.GetListAccount();
        }
        void loadDateTimePickerBill()
        {
            DateTime today = DateTime.Now;
            dtpkFromDate.Value = new DateTime(today.Year, today.Month, 1);
            dtpkToDate.Value = dtpkFromDate.Value.AddMonths(1).AddDays(-1);
        }
        //void loadFoodList()
        //{
        //    string query = "SELECT * FROM Food";
        //    dtgvFood.DataSource = DataProvider.Instance.ExecuteQuery(query);
        //}
        //void loadAccountList()
        //{
        //    string query = "EXECUTE dbo.USP_GetAccountByUserName @username";

        //    dtgvAccount.DataSource = DataProvider.Instance.ExecuteQuery(query, new object[] {"staff"});

        //}
        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {
            
        }
        #region methods
        void loadListBillByDate(DateTime checkIn, DateTime checkOut)
        {
            dtgvBill.DataSource = BillDAO.Instance.GetBillListByDate(checkIn, checkOut);
        }
        void loadListFood()
        {
            foodList.DataSource = FoodDAO.Instance.GetListFood();
        }
        void AddFoodBinding()
        {
            txbFoodName.DataBindings.Add(new Binding("Text", dtgvFood.DataSource, "Name", true, DataSourceUpdateMode.Never));
            txbFoodID.DataBindings.Add(new Binding("Text", dtgvFood.DataSource, "ID", true, DataSourceUpdateMode.Never));
            nmFoodPrice.DataBindings.Add(new Binding("Value", dtgvFood.DataSource, "Price", true, DataSourceUpdateMode.Never));

        }
        void loadCategoryIntoCombobox(ComboBox cb)
        {
            cb.DataSource = CategoryDAO.Instance.GetListCategory();
            cb.DisplayMember = "Name";
        }
        #endregion

        #region events
        private void btnView_Click(object sender, EventArgs e)
        {
            loadListFood();
        }

        List<Food> SearchFoodByName(string name)
        {
            List<Food> listFood = FoodDAO.Instance.SearchFoodByName(name);
            return listFood;
        }
        #endregion
        private void btnViewBill_Click(object sender, EventArgs e)
        {
            loadListBillByDate(dtpkFromDate.Value, dtpkToDate.Value);
        }

        private event EventHandler insertFood;
        public event EventHandler InsertFood
        {
            add { insertFood += value; }
            remove { insertFood -= value; }
        }

        private event EventHandler updateFood;
        public event EventHandler UpdateFood
        {
            add { updateFood += value; }
            remove { updateFood -= value; }
        }

        private event EventHandler deleteFood;
        public event EventHandler DeleteFood
        {
            add { deleteFood += value; }
            remove { deleteFood -= value; }
        }

        private void btnViewCate_Click(object sender, EventArgs e)
        {

        }

        private void txbFoodID_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (dtgvFood.SelectedCells.Count > 0)
                {
                    var selectedCell = dtgvFood.SelectedCells[0];
                    if (selectedCell.OwningRow != null && selectedCell.OwningRow.Cells["idCategory"].Value != null)
                    {
                        int id = (int)selectedCell.OwningRow.Cells["idCategory"].Value;
                        Category category = CategoryDAO.Instance.GetCategoryByID(id);
                        cbFoodCategory.SelectedItem = category;
                        int index = -1;
                        int i = 0;
                        foreach (Category item in cbFoodCategory.Items)
                        {
                            if (item.ID == category.ID)
                            {
                                index = i;
                                break;
                            }
                            i++;
                        }
                        cbFoodCategory.SelectedIndex = index;
                    }
                }
            }
            catch
            {}
        }
        private void btnAddFood_Click(object sender, EventArgs e)
        {
            string name = txbFoodName.Text;
            int categoryID = (cbFoodCategory.SelectedItem as Category).ID;
            float price = (float)nmFoodPrice.Value;

            if(FoodDAO.Instance.InsertFood(name, categoryID, price))
            {
                MessageBox.Show("Add food successfully");
                loadListFood();
                if(insertFood != null)
                {
                    insertFood(this, new EventArgs());
                }
            }
            else
            {
                MessageBox.Show("Add food failed");
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            string name = txbFoodName.Text;
            int categoryID = (cbFoodCategory.SelectedItem as Category).ID;
            float price = (float)nmFoodPrice.Value;
            int id = Convert.ToInt32(txbFoodID.Text);

            if (FoodDAO.Instance.UpdateFood(id, name, categoryID, price))
            {
                MessageBox.Show("Update food successfully");
                loadListFood();
                if(updateFood != null)
                {
                    updateFood(this, new EventArgs());
                }
            }
            else
            {
                MessageBox.Show("Update food failed");
            }

        }

        private void btnDeleteFood_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(txbFoodID.Text);

            if (FoodDAO.Instance.DeleteFood(id))
            {
                MessageBox.Show("Delete food successfully");
                loadListFood();
                if(deleteFood != null)
                {
                    deleteFood(this, new EventArgs());
                }
            }
            else
            {
                MessageBox.Show("Delete food failed");
            }
        }

        private void btnSearchFood_Click(object sender, EventArgs e)
        {
            foodList.DataSource = SearchFoodByName(txbSearchFoodName.Text);
        }

        private void btnViewAcc_Click(object sender, EventArgs e)
        {
            loadAccountList();
        }
        void addAccount(string userName, string displayName, int type)
        {
            if (AccountDAO.Instance.InsertAccount(userName, displayName, type))
            {
                MessageBox.Show("Add account successfully");
                loadAccountList();
            }
            else
            {
                MessageBox.Show("Add account failed");
            }
            loadAccountList();
        }
        void updateAccount(string userName, string displayName, int type)
        {
            if (AccountDAO.Instance.UpdateAccount(userName, displayName, type))
            {
                MessageBox.Show("Update account successfully");
                loadAccountList();
            }
            else
            {
                MessageBox.Show("Update account failed");
            }
            loadAccountList();
        }
        void deleteAccount(string userName)
        {
            if (loginAccount.UserName.Equals(userName))
            {
                MessageBox.Show("You can not delete your own account");
                return;
            }
            if (AccountDAO.Instance.DeleteAccount(userName))
            {
                MessageBox.Show("Delete account successfully");
                loadAccountList();
            }
            else
            {
                MessageBox.Show("Delete account failed");
            }
            loadAccountList();
        }
        private void btnAddAcc_Click(object sender, EventArgs e)
        {
            string userName = txbUserName.Text;
            string displayName = txbDisplayName.Text;
            int type = (int)nmAccountType.Value;
            addAccount(userName, displayName, type);

        }

        private void btnDeleteAcc_Click(object sender, EventArgs e)
        {
            string userName = txbUserName.Text;
            deleteAccount(userName);

        }

        private void btnUpdateAcc_Click(object sender, EventArgs e)
        {
            string userName = txbUserName.Text;
            string displayName = txbDisplayName.Text;
            int type = (int)nmAccountType.Value;
           updateAccount(userName, displayName, type);
        }

        void ResetPassword(string userName)
        {
            if (AccountDAO.Instance.ResetPassword(userName))
            {
                MessageBox.Show("Reset password successfully");
            }
            else
            {
                MessageBox.Show("Reset password failed");
            }
        }
        private void btnResetPassword_Click(object sender, EventArgs e)
        {
            string userName = txbUserName.Text;
            ResetPassword(userName);
        }
    }
}
