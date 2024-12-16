using DE01.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Entity;

namespace DE01
{
    public partial class Form1 : Form
    {
        private Model1 _context;
        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _context = new Model1();
            var lops = _context.Lops.ToList();
            cboLop.DataSource = lops;
            cboLop.DisplayMember = "TenLop";
            cboLop.ValueMember = "MaLop";
            LoadListView();
        }
        private void LoadListView()
        {
            lvSinhvien.Items.Clear();
            var Sinhviens = _context.SinhViens.Include(s => s.Lop).ToList();
            foreach (var sv in Sinhviens)
            {
                var item = new ListViewItem(sv.MaSV);
                item.SubItems.Add(sv.HotenSV);
                item.SubItems.Add(sv.Ngaysinh.ToString("dd/MM/yyyy"));
                item.SubItems.Add(sv.Lop.TenLop);
                lvSinhvien.Items.Add(item);
            }
        }

        private void btThem_Click(object sender, EventArgs e)
        {
            var sv = new SinhVien
            {
                MaSV = txtMaSV.Text,
                HotenSV = txtHotenSV.Text,
                Ngaysinh = dtNgaysinh.Value,
                MaLop = cboLop.SelectedValue.ToString()
            };

            _context.SinhViens.Add(sv);
            _context.SaveChanges();

            MessageBox.Show("Thêm sinh viên thành công!");
            LoadListView();
        }

        private void btSua_Click(object sender, EventArgs e)
        {
            try
            {
                var maSV = txtMaSV.Text;
                var sinhvien = _context.SinhViens.FirstOrDefault(s => s.MaSV == maSV);

                if (sinhvien == null)
                {
                    MessageBox.Show("Không tìm thấy sinh viên cần sửa!");
                    return;
                }

                sinhvien.HotenSV = txtHotenSV.Text; // Sửa lại tên textbox
                sinhvien.Ngaysinh = dtNgaysinh.Value;
                sinhvien.MaLop = cboLop.SelectedValue.ToString();

                _context.SaveChanges();

                MessageBox.Show("Cập nhật thông tin sinh viên thành công!");
                LoadListView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
        }

        private void btXoa_Click(object sender, EventArgs e)
        {
            try
            {
                var maSV = txtMaSV.Text;
                var sv = _context.SinhViens.FirstOrDefault(s => s.MaSV == maSV);

                if (sv == null)
                {
                    MessageBox.Show("Không tìm thấy sinh viên!");
                    return;
                }

                var confirm = MessageBox.Show("Bạn có chắc chắn muốn xóa?", "Xác nhận", MessageBoxButtons.YesNo);
                if (confirm == DialogResult.Yes)
                {
                    _context.SinhViens.Remove(sv);
                    _context.SaveChanges();

                    MessageBox.Show("Xóa sinh viên thành công!");
                    LoadListView();
                    ClearForm(); // Xóa dữ liệu trên Form
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
        }
            private void ClearForm()
        {
            txtMaSV.Clear();
            txtHotenSV.Clear();
            dtNgaysinh.Value = DateTime.Now;
            cboLop.SelectedIndex = -1; // Bỏ chọn ComboBox
        }

        private void lvSinhvien_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvSinhvien.SelectedItems.Count > 0)
            {
                var selectedItem = lvSinhvien.SelectedItems[0];
                txtMaSV.Text = selectedItem.SubItems[0].Text; // MaSV
                txtHotenSV.Text = selectedItem.SubItems[1].Text; // HotenSV
                dtNgaysinh.Value = DateTime.ParseExact(selectedItem.SubItems[2].Text, "dd/MM/yyyy", null); // NgaySinh
                cboLop.Text = selectedItem.SubItems[3].Text; // TenLop
            }
        }

        private void btLuu_Click(object sender, EventArgs e)
        {

        }

        private void btTim_Click(object sender, EventArgs e)
        {
            var keyword = txtTim.Text.Trim();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                LoadListView();
                return;
            }

            var Sinhviens = _context.SinhViens
                .Include(s => s.Lop)
                .Where(s => s.HotenSV.Contains(keyword))
                .ToList();

            lvSinhvien.Items.Clear();
            if (Sinhviens.Count == 0)
            {
                MessageBox.Show("Không tìm thấy sinh viên nào có tên phù hợp!");
                return;
            }

            foreach (var sv in Sinhviens)
            {
                var item = new ListViewItem(sv.MaSV);
                item.SubItems.Add(sv.HotenSV);
                item.SubItems.Add(sv.Ngaysinh.ToString("dd/MM/yyyy hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture));
                item.SubItems.Add(sv.Lop.TenLop);
                lvSinhvien.Items.Add(item);
            }
        }
    }
}
