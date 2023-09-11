using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Linq.Mapping;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LatihanDataGrid
{
    public partial class Form1 : Form
    {
        private BindingList<SiswaModel> listData = new BindingList<SiswaModel>();
        private DbContextDataContext dbContext;

        public Form1()
        {
            InitializeComponent();

            dbContext = new DbContextDataContext();

            string nama = "yudis";
            string[] nama2 = new string[] { "yudis", "agus", "budi" };

            var dataYudis = new SiswaModel
            {
                Id = "A1",
                Nama = "Yudis",
                Alamat = "Yogya",
                Nilai = 81
            };
            var dataAgus = new SiswaModel
            {
                Id = "A2",
                Nama = "Agus",
                Alamat = "Klaten",
                Nilai = 76
            };


            listData.Add(dataYudis);
            listData.Add(dataAgus);
            var binding = new BindingSource();
            binding.DataSource = listData;
            dataGridView1.DataSource = binding;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(listData[0].Nama);

            MessageBox.Show(dataGridView1.Rows[0].Cells["Nama"].Value.ToString());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var data1 = new SiswaModel
            {
                Id = "A3",
                Nama = "Budi",
                Alamat = "Klaten",
                Nilai = 76
            };
            listData.Add(data1);
            dataGridView1.Refresh();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var binding = new BindingSource();
            binding.DataSource = listData.OrderBy(x => x.Nilai);

            dataGridView1.DataSource = binding;
            dataGridView1.Refresh();

        }

        private void button4_Click(object sender, EventArgs e)
        {
            var removedData = listData.FirstOrDefault(x => x.Nama == "Budi");
            listData.Remove(removedData);
            dataGridView1.Refresh();
        }

        private void SetLayout()
        {
            dataGridView1.Columns["Id"].DefaultCellStyle.BackColor = Color.HotPink;
            dataGridView1.Columns["Id"].DefaultCellStyle.Font = new Font("Courier New", 12f);

        }

        private void button5_Click(object sender, EventArgs e)
        {
            SetLayout();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var customer = (
                from c in dbContext.BTR_Customers
                join d in dbContext.BTR_Wilayahs on c.WilayahId equals d.WilayahId
                select new CustomerModel
                {
                    CustomerId = c.CustomerId,
                    CustomerName = c.CustomerName,
                    WilayahName = d.WilayahName
                }).ToList();
            customer = customer.OrderBy(x => x.CustomerName).ToList();

            dataGridView1.DataSource = customer;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            var customer = dbContext.BTR_Customers.Where(x => x.CustomerId == "CS0001").FirstOrDefault();
            customer.CustomerName = "4 Putra";
            dbContext.SubmitChanges();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            var customerPerWilayah = (
                from c in dbContext.BTR_Customers
                join d in dbContext.BTR_Wilayahs on c.WilayahId equals d.WilayahId
                group c by d.WilayahName into g
                select new 
                {
                    Name = g.Key,
                    Jumlah = g.Count()
                }).ToList();
            var top5 = customerPerWilayah.OrderByDescending(x => x.Jumlah).Take(5).ToList();

            chart1.DataSource = top5;
            chart1.Series[0].XValueMember = "Name";
            chart1.Series[0].YValueMembers = "Jumlah";
            chart1.DataBind();
        }
        public static DataTable ConvertListToDataTable<T>(List<T> list)
        {
            DataTable table = new DataTable();

            // Get the type of the objects in the list
            Type type = typeof(T);

            // Create the table columns using the object's properties
            foreach (var propertyInfo in type.GetProperties())
            {
                table.Columns.Add(propertyInfo.Name, propertyInfo.PropertyType);
            }

            // Populate the table with data from the list
            foreach (T item in list)
            {
                DataRow row = table.NewRow();
                foreach (var propertyInfo in type.GetProperties())
                {
                    row[propertyInfo.Name] = propertyInfo.GetValue(item);
                }
                table.Rows.Add(row);
            }

            return table;
        }
    }

    public class Graph1
    {
        public string WilayahName { get; set; }
        public int Jumlah { get; set; }
    }
    public class CustomerModel
    {
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string WilayahName { get; set; }
    }

    public class SiswaModel
    {
        public string Id { get; set; }
        public string Nama { get; set; }
        public string Alamat { get; set; }
        public decimal Nilai { get; set; }
    }
}
