using QuanLyQuanCoffee.DAO;
using QuanLyQuanCoffee.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyQuanCoffee
{
    public partial class fTableManager : Form
    {
        private Account loginAccount;

        public Account LoginAccount
        {
            get => loginAccount;
            set
            {
                loginAccount = value;
                changeAccount(loginAccount.Type);
            }
        }
        public fTableManager(Account account)
        {
            InitializeComponent();
            this.LoginAccount = account;
            loadTable();
            loadCategory();
            loadComboboxTable(cbSwitchTable);
        }
        #region methods
        void changeAccount(int type)
        {
            adminToolStripMenuItem.Enabled = type == 1;
            accountInformationToolStripMenuItem.Text += " (" + LoginAccount.DisplayName + ")"; 
        }
        void loadCategory()
        {
            List<Category> listCategory = CategoryDAO.Instance.GetListCategory();
            cbCategory.DataSource = listCategory;
            cbCategory.DisplayMember = "Name";
        }
        void loadFoodListByCategoryID(int id)
        {
            List<Food> listFood = FoodDAO.Instance.GetListFoodByCategoryID(id);
            cbFood.DataSource = listFood;
            cbFood.DisplayMember = "Name";
        }
        void loadTable()
        {
            flpTable.Controls.Clear();
            List<Table> Tablelist = TableDAO.Instance.loadTableList();

            foreach (Table item in Tablelist)
            {
                Button btn = new Button() { Width = TableDAO.TableWidth, Height = TableDAO.TableHeight };
                btn.Text = item.Name + Environment.NewLine + (item.ReservationStatus == 1 ? "Đã có người đặt" : item.Status);
                btn.Tag = item; //  Gán cả đối tượng Table vào Tag của Button
                btn.Click += btn_Click; // Gán sự kiện Click

                // Cập nhật màu sắc theo trạng thái bàn
                if (item.ReservationStatus == 1) // Bàn đã đặt trước
                {
                    btn.BackColor = Color.Yellow;
                }
                else if (item.Status == "Trống")
                {
                    btn.BackColor = Color.Aqua;
                }
                else
                {
                    btn.BackColor = Color.LightBlue;
                }

                flpTable.Controls.Add(btn);

            }
        }
        void ShowBill(int id)
        {
            lsvBill.Items.Clear();
            List<DTO.Menu> listBillInfo = MenuDAO.Instance.GetListMenuByTable(id);
            float totalPrice = 0;
            foreach (DTO.Menu item in listBillInfo)
            {
                ListViewItem lsvItem = new ListViewItem(item.FoodName.ToString());
                lsvItem.SubItems.Add(item.Count.ToString());
                lsvItem.SubItems.Add(item.Price.ToString());
                lsvItem.SubItems.Add(item.TotalPrice.ToString());
                totalPrice += item.TotalPrice;
                lsvBill.Items.Add(lsvItem);
            }
            CultureInfo culture = new CultureInfo("vi-VN");
            //Thread.CurrentThread.CurrentCulture = culture;
            txbTotalPrice.Text = totalPrice.ToString("c", culture); 
        }
        void loadComboboxTable(ComboBox cb)
        {
            cb.DataSource = TableDAO.Instance.loadTableList();
            cb.DisplayMember = "Name";
        }
        #endregion methds

        #region events
        private void btn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;

            if (btn.Tag is Table table) // ✅ Kiểm tra nếu Tag chứa đối tượng Table hợp lệ
            {
                int tableID = table.ID;
                txbIDTable.Text = tableID.ToString(); // ✅ Hiển thị ID bàn lên TextBox
                lsvBill.Tag = table; // Lưu bàn vào ListView để dùng sau
                ShowBill(tableID);
            }
            else
            {
                MessageBox.Show("Lỗi: Không thể lấy ID bàn vì Tag không chứa đối tượng Table.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void logOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void personalInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fAccountProfile f = new fAccountProfile(LoginAccount);
            f.UpdateAccount += F_UpdateAccount;
            f.ShowDialog();
        }

        private void F_UpdateAccount(object sender, AccountEvent e)
        {
            personalInformationToolStripMenuItem.Text = "Personal Information (" + e.Acc.DisplayName  + ")";
        }

        private void adminToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fAdmin f = new fAdmin();
            f.loginAccount = LoginAccount;   
            f.InsertFood += F_InsertFood;
            f.UpdateFood += F_UpdateFood;
            f.DeleteFood += F_DeleteFood;
            f.ShowDialog();
        }

        private void F_DeleteFood(object sender, EventArgs e)
        {
            loadFoodListByCategoryID((cbCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as Table).ID);
            loadTable();

        }

        private void F_UpdateFood(object sender, EventArgs e)
        {
            loadFoodListByCategoryID((cbCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as Table).ID);
        }
            
        private void F_InsertFood(object sender, EventArgs e)
        {
            loadFoodListByCategoryID((cbCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as Table).ID);

        }

        private void cbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = 0;
            ComboBox cb = sender as ComboBox;
            if (cb.SelectedItem == null)
            {
                return;
            }
            Category selected = cb.SelectedItem as Category;
            id = selected.ID;
            loadFoodListByCategoryID(id);
        }
        private void btnAddFood_Click(object sender, EventArgs e)
        {
            Table table = lsvBill.Tag as Table;
            if (table == null)
            {
                MessageBox.Show("Please select a table to add food");
                return;
            }
            int idBill = BillDAO.Instance.GetUnCheckBillIDByTableID(table.ID);
            int foodID = (cbFood.SelectedItem as Food).ID;
            int count = (int)nmFoodCount.Value;
            if (idBill == -1)
            {
                BillDAO.Instance.InsertBill(table.ID);
                BillInfoDAO.Instance.InsertBillInfo(BillDAO.Instance.GetMaxIDBill(), foodID, count);
            }
            else
            {
                BillInfoDAO.Instance.InsertBillInfo(idBill, foodID, count);
            }
            ShowBill(table.ID);
            loadTable();
            #endregion events
        }

        private void btnCheckOut_Click(object sender, EventArgs e)
        {
            Table table = lsvBill.Tag as Table;
            int idBill = BillDAO.Instance.GetUnCheckBillIDByTableID(table.ID);
            int discount = (int)nmDiscount.Value;
            double totalPrice = double.Parse(txbTotalPrice.Text, NumberStyles.Currency, CultureInfo.GetCultureInfo("vi-VN"));
            double finalTotalPrice = totalPrice - (totalPrice / 100) * discount;
            if (idBill != -1)
            {
                if (MessageBox.Show(string.Format("Are you sure you want to check out the bill for {0}\n Total Price - (Total Price / 100) * Discount = {1} - ({1} / 100 ) * {2} = {3}", table.Name, totalPrice, discount, finalTotalPrice), "Notification", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                {
                    BillDAO.Instance.checkOut(idBill, discount,(float)finalTotalPrice);
                    ShowBill(table.ID);
                    loadTable();
                }
            }
        }
        private void btnSwitchTable_Click(object sender, EventArgs e)
        {
            int id1 = (lsvBill.Tag as Table).ID;
            int id2 = (cbSwitchTable.SelectedItem as Table).ID;
            if (MessageBox.Show(string.Format("Are you sure you want to switch table {0} to table {1}", (lsvBill.Tag as Table).Name, (cbSwitchTable.SelectedItem as Table).Name), "Notification", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                TableDAO.Instance.SwitchTable(id1, id2);
                loadTable();
            }

        }

        private void payToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnCheckOut_Click(this, new EventArgs());
        }

        private void addFoodToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            btnAddFood_Click(this, new EventArgs());
        }

        private void btnReservation_Click(object sender, EventArgs e)
        {
            // Lấy ID bàn từ TextBox
            if (string.IsNullOrEmpty(txbIDTable.Text))
            {
                MessageBox.Show("Vui lòng chọn bàn trước khi đặt!");
                return;
            }

            int idTable = int.Parse(txbIDTable.Text);
            string customerName = txbCustomerName.Text;
            string phone = txbPhoneNumber.Text;
            DateTime reservationTime = dtpkReservationTime.Value;

            if (TableDAO.Instance.BookTable(idTable, customerName, phone, reservationTime))
            {
                MessageBox.Show("Đặt bàn thành công!");
                loadTable(); // Cập nhật lại danh sách bàn
            }
            else
            {
                MessageBox.Show("Bàn đã có người đặt hoặc lỗi hệ thống!");
            }
        }
        private void btnTable_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button; // Lấy Button vừa được nhấn
            int idTable = (int)btn.Tag; // Lấy ID bàn từ Tag của Button

            // Hiển thị ID bàn lên TextBox
            txbIDTable.Text = idTable.ToString();
        }

        private void btnCancelReser_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txbIDTable.Text))
            {
                MessageBox.Show("Vui lòng chọn bàn cần hủy đặt trước!");
                return;
            }

            int idTable = int.Parse(txbIDTable.Text);

            // Xác nhận hủy đặt bàn
            DialogResult result = MessageBox.Show("Do you want cancel reservation?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                if (TableDAO.Instance.CancelReservation(idTable))
                {
                    MessageBox.Show("Cancel reservation successfully!");
                    loadTable(); // Cập nhật lại danh sách bàn
                }
                else
                {
                    MessageBox.Show("Lỗi: Không thể hủy đặt bàn!");
                }
            }
        }
    }
}
