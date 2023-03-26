using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace laba1энтропияДСВ
{
    public partial class MainForm : Form
    {
        private int n;
        private List<double> probabilites = new List<double>();
        private DataSet ds = new DataSet();
        private DataTable dt = new DataTable();
        private bool isUnique;
        private string prevValue;
        private bool isSumCorrect = false;
        //add comments  
        //++++ввели количество элементов
        //ввели вероятности - надо проверить что каждое значение от 0 до 1
        // сумма не больше 1,
        // при вычислении не меньше единицы + все значения введены
        // сразу сообщение
        //отметка сортировки
        //кнопки
        public MainForm()
        {
            InitializeComponent();
            ds.Tables.Add(dt);
            DataColumn eventX = new DataColumn("Событие", Type.GetType("System.String"));
            ds.Tables[0].Columns.Add(eventX);
            DataColumn probability = new DataColumn("Вероятность", Type.GetType("System.String"));
            ds.Tables[0].Columns.Add(probability);
            dataGridView1.DataSource = ds.Tables[0];
            dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.Automatic;
            CreateRow(1);
            //checkedListBox1.SetItemChecked(0, true);
        }
        private void CreateRow(int i)
        {
            DataRow row = dt.NewRow();
            for (int j = 0; j <= i; j++)
            {
                if (CheckUniqueness((j + 1).ToString(), i))
                {
                    row["Событие"] = (j + 1).ToString();
                    ds.Tables[0].Rows.Add(row);
                    break;
                }
            }
            //проверка на пристствующие числа, если какого-то нет то и=оно
        }
        private bool CheckUniqueness (string title, int index)
        {
            isUnique = true;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if ((ds.Tables[0].Rows[i].ItemArray[0].ToString() == title) && (i != index))
                {
                    isUnique = false;
                    break;
                }
            }
            return isUnique;
        }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            n = Convert.ToInt32(numericUpDown1.Value);
            if (n > ds.Tables[0].Rows.Count)
            {
                for (int i = ds.Tables[0].Rows.Count; i < n; i++)
                {
                    CreateRow(i);
                }
            }
            else
            {
                while (ds.Tables[0].Rows.Count != n)
                {
                    ds.Tables[0].Rows.RemoveAt(ds.Tables[0].Rows.Count - 1);
                }
            }
            labelResoult.Text = "";
        }
        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            prevValue = dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString();
        }
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (Convert.ToInt32(e.ColumnIndex) == 0)
            {
                if (!CheckUniqueness(dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString(), e.RowIndex))
                {
                    dataGridView1[e.ColumnIndex, e.RowIndex].Value = prevValue;
                    MessageBox.Show("Значения событий должны быть уникальными!");
                }
            }
            if (Convert.ToInt32(e.ColumnIndex) == 1)
            {
                
                if (dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString().Any(c => c == '.'))
                {
                    MessageBox.Show("Введите значение через запятую");
                    dataGridView1[e.ColumnIndex, e.RowIndex].Value = "";
                }
                else
                {
                    try
                    {
                        if (dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString().Any(c => char.IsLetter(c)) ||
                        Convert.ToDouble(dataGridView1[e.ColumnIndex, e.RowIndex].Value) > 1 ||
                        Convert.ToDouble(dataGridView1[e.ColumnIndex, e.RowIndex].Value) < 0)
                        {
                            dataGridView1[e.ColumnIndex, e.RowIndex].Value = "";
                            MessageBox.Show("Значение вероятности должно принадлежать промежутку [0;1]");
                        }
                        else
                        {
                            if (!SumOfProbabilites())
                                dataGridView1[e.ColumnIndex, e.RowIndex].Value = "";
                        }
                    }
                    catch (FormatException) {}
                }
                labelResoult.Text = "";
            }
        }
        private bool SumOfProbabilites()
        {
            double probability = 0;
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].Cells[1].Value.ToString() != "")
                    probability += Convert.ToDouble(dataGridView1.Rows[i].Cells[1].Value);
            }
            probability = Math.Round(probability, 3);
            if (probability > 1)
            {
                MessageBox.Show("Сумма вероятностей должна быть не больше единицы");
                return false;
            }
            if (probability < 1) isSumCorrect = false;
            else isSumCorrect = true;
            return true;
        }

        private void buttonResoult_Click(object sender, EventArgs e)
        {
            bool allValues = true;
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].Cells[1].Value.ToString() == "")
                {
                    MessageBox.Show($"Введите значение в {i+1} строчке (событие \"{dataGridView1.Rows[i].Cells[0].Value}\")");
                    allValues = false;
                    break;
                }
            }
            if (allValues)
            {
                if (isSumCorrect)
                {
                    double res = Shennon();
                    labelResoult.Text = res.ToString();
                }
                else MessageBox.Show("Сумма вероятностей должна быть равна единице");
            }
            
        }
        private double Shennon()
        {
            double sum = 0.0;
            for (int i = 0; i < n; i++)
            {
                if (Convert.ToDouble(dataGridView1.Rows[i].Cells[1].Value) != 0.0)
                {
                    double x = Convert.ToDouble(dataGridView1.Rows[i].Cells[1].Value);
                    sum -= x * Math.Log(x, 2);
                }
            }
            return Math.Round(sum, 3);
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                ds.Tables[0].Rows.RemoveAt(dataGridView1.CurrentRow.Index);
                if (numericUpDown1.Value == 1)
                {
                    ds.Tables[0].Clear();
                    CreateRow(1);
                }
                else numericUpDown1.Value -= 1;
                dataGridView1.CurrentCell = null;
                SumOfProbabilites();
            }
            else MessageBox.Show("Выберите ячейку, ряд которой нужно удалить");
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            ds.Tables[0].Clear();
            if (numericUpDown1.Value == 1)
                CreateRow(1);
            else numericUpDown1.Value = 1;
        }

        //private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        //{
        //    for (int i = 0; i < checkedListBox1.Items.Count; i++)
        //    {
        //        if (i != e.Index)
        //            checkedListBox1.SetItemChecked(i, false);
        //    }
        //    if (e.Index == 1)
        //        dataGridView1.Sort()
        //    if (e.Index == 2) 
        //        BubbleSort(1);
        //}
        //private void BubbleSort(int index)
        //{
        //    for (int i = 0; i < n; i++)
        //    {
        //        for (int j = i + 1; j < n; j++)
        //        {
        //            if (ds.Tables[0].Rows[i].ItemArray[index].ToString().CompareTo(ds.Tables[0].Rows[j].ItemArray[index].ToString()) > 0)
        //            {
        //                //string str = ds.Tables[0].Rows[i].ItemArray[0].ToString();
        //                //string str2 = ds.Tables[0].Rows[i].ItemArray[1].ToString();
        //                //ds.Tables[0].Rows[i].ItemArray[0] = "";
        //                //ds.Tables[0].Rows[i].ItemArray[1] = "";
        //                //string str11 = ds.Tables[0].Rows[j].ItemArray[0].ToString();
        //                //string str12 = ds.Tables[0].Rows[j].ItemArray[1].ToString();
        //                //ds.Tables[0].Rows[j].ItemArray[0] = str;
        //                //ds.Tables[0].Rows[j].ItemArray[1] = str2;
        //                //ds.Tables[0].Rows[i].ItemArray[0] = str11;
        //                //ds.Tables[0].Rows[i].ItemArray[1] = str12;
                         
        //                //DataRow row1 = ds.Tables[0].Rows[i];
        //                //DataRow row2 = ds.Tables[0].Rows[j];
        //                //ds.Tables[0].Rows.RemoveAt(j);
        //                //ds.Tables[0].Rows.RemoveAt(i);
        //                //ds.Tables[0].Rows.InsertAt(row2, i);
        //                //ds.Tables[0].Rows.InsertAt(row1, j);
        //            }
        //        }
        //    }
        //}
    }
}
