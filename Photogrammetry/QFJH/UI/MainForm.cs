using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QFJH.Properties;

namespace QFJH.UI
{
    /// <summary>
    /// 主界面-基础代码放这里
    /// </summary>
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            this.Text = Resources.Prog_Name;

            //鼠标滚动            
            pictureRef.MouseWheel += new MouseEventHandler(pictureBox1_MouseWheel);     //左侧像片
            pictureRef.SizeMode = PictureBoxSizeMode.Zoom;
            pictureMatch.MouseWheel += new MouseEventHandler(pictureMatch_MouseWheel);     //右侧像片
            pictureMatch.SizeMode = PictureBoxSizeMode.Zoom;

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show(Resources.MainForm_ClosingTip, Resources.Prog_Name,
                MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// 鼠标滚轮事件--左侧像片的PictureBox：影像大小（其实是picturebox大小）随滚轮转动而改变大小，且相片的焦点不变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            double step = 1.2;//缩放倍率
            if (e.Delta > 0)
            {
                if (pictureRef.Height >= Screen.PrimaryScreen.Bounds.Height * 10)
                    return;
                pictureRef.Height = (int)(pictureRef.Height * step);
                pictureRef.Width = (int)(pictureRef.Width * step);

                int px = Cursor.Position.X - pictureRef.Location.X;
                int py = Cursor.Position.Y - pictureRef.Location.Y;
                int px_add = (int)(px * (step - 1.0));
                int py_add = (int)(py * (step - 1.0));
                pictureRef.Location = new Point(pictureRef.Location.X - px_add, pictureRef.Location.Y - py_add);
                Application.DoEvents();
            }
            else
            {
                if (pictureRef.Height <= Screen.PrimaryScreen.Bounds.Height)
                    return;
                pictureRef.Height = (int)(pictureRef.Height / step);
                pictureRef.Width = (int)(pictureRef.Width / step);

                int px = Cursor.Position.X - pictureRef.Location.X;
                int py = Cursor.Position.Y - pictureRef.Location.Y;
                int px_add = (int)(px * (1.0 - 1.0 / step));
                int py_add = (int)(py * (1.0 - 1.0 / step));
                pictureRef.Location = new Point(pictureRef.Location.X + px_add, pictureRef.Location.Y + py_add);
                Application.DoEvents();
            }
        }

        /// <summary>
        /// 鼠标滚轮事件--右侧像片的PictureBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void pictureMatch_MouseWheel(object sender, MouseEventArgs e)
        {
            double step = 1.2;//缩放倍率
            if (e.Delta > 0)
            {
                if (pictureMatch.Height >= Screen.PrimaryScreen.Bounds.Height * 10)
                    return;
                pictureMatch.Height = (int)(pictureMatch.Height * step);
                pictureMatch.Width = (int)(pictureMatch.Width * step);

                int px = Cursor.Position.X - pictureMatch.Location.X;
                int py = Cursor.Position.Y - pictureMatch.Location.Y;
                int px_add = (int)(px * (step - 1.0));
                int py_add = (int)(py * (step - 1.0));
                pictureMatch.Location = new Point(pictureMatch.Location.X - px_add, pictureMatch.Location.Y - py_add);
                Application.DoEvents();
            }
            else
            {
                if (pictureMatch.Height <= Screen.PrimaryScreen.Bounds.Height)
                    return;
                pictureMatch.Height = (int)(pictureMatch.Height / step);
                pictureMatch.Width = (int)(pictureMatch.Width / step);

                int px = Cursor.Position.X - pictureMatch.Location.X;
                int py = Cursor.Position.Y - pictureMatch.Location.Y;
                int px_add = (int)(px * (1.0 - 1.0 / step));
                int py_add = (int)(py * (1.0 - 1.0 / step));
                pictureMatch.Location = new Point(pictureMatch.Location.X + px_add, pictureMatch.Location.Y + py_add);
                Application.DoEvents();
            }
        }

        #region 左侧PictureBox的鼠标移动： 可实现用鼠标光标来移动照片位置
        int xPos;
        int yPos;
        bool MoveFlag;
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            MoveFlag = true;//已经按下.
            xPos = e.X;//当前x坐标.
            yPos = e.Y;//当前y坐标.
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            MoveFlag = false;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (MoveFlag)
            {
                pictureRef.Left += Convert.ToInt16(e.X - xPos);//设置x坐标.
                pictureRef.Top += Convert.ToInt16(e.Y - yPos);//设置y坐标.
            }
            #region 显示像方坐标（图片坐标系）
            if (this.pictureRef.Image != null) {

                int originalWidth = this.pictureRef.Image.Width;
                int originalHeight = this.pictureRef.Image.Height;

                PropertyInfo rectangleProperty = this.pictureRef.GetType().GetProperty("ImageRectangle", BindingFlags.Instance | BindingFlags.NonPublic);
                Rectangle rectangle = (Rectangle)rectangleProperty.GetValue(this.pictureRef, null);

                int currentWidth = rectangle.Width;
                int currentHeight = rectangle.Height;

                double rate = (double)currentHeight / (double)originalHeight;

                int black_left_width = (currentWidth == this.pictureRef.Width) ? 0 : (this.pictureRef.Width - currentWidth) / 2;
                int black_top_height = (currentHeight == this.pictureRef.Height) ? 0 : (this.pictureRef.Height - currentHeight) / 2;

                int zoom_x = e.X - black_left_width;
                int zoom_y = e.Y - black_top_height;

                double original_x = (double)zoom_x / rate;
                double original_y = (double)zoom_y / rate;

                this.Coordinate.Text = "像方坐标（左）：（"+ original_x + ", " + original_y + ");    ";
            }
            #endregion
        }
        #endregion

        #region 右侧PictureBox的鼠标移动
        int xPosR;
        int yPosR;
        bool MoveFlagR;
        private void pictureMatch_MouseDown(object sender, MouseEventArgs e)
        {
            MoveFlagR = true;//已经按下.
            xPosR = e.X;//当前x坐标.
            yPosR = e.Y;//当前y坐标.
        }

        private void pictureMatch_MouseUp(object sender, MouseEventArgs e)
        {
            MoveFlagR = false;
        }

        private void pictureMatch_MouseMove(object sender, MouseEventArgs e)
        {
            if (MoveFlagR)
            {
                pictureMatch.Left += Convert.ToInt16(e.X - xPosR);//设置x坐标.
                pictureMatch.Top += Convert.ToInt16(e.Y - yPosR);//设置y坐标.
            }

            #region 显示像方坐标（图片坐标系）
            if (this.pictureMatch.Image != null)
            {

                int originalWidth = this.pictureMatch.Image.Width;
                int originalHeight = this.pictureMatch.Image.Height;

                PropertyInfo rectangleProperty = this.pictureMatch.GetType().GetProperty("ImageRectangle", BindingFlags.Instance | BindingFlags.NonPublic);
                Rectangle rectangle = (Rectangle)rectangleProperty.GetValue(this.pictureMatch, null);

                int currentWidth = rectangle.Width;
                int currentHeight = rectangle.Height;

                double rate = (double)currentHeight / (double)originalHeight;

                int black_left_width = (currentWidth == this.pictureMatch.Width) ? 0 : (this.pictureMatch.Width - currentWidth) / 2;
                int black_top_height = (currentHeight == this.pictureMatch.Height) ? 0 : (this.pictureMatch.Height - currentHeight) / 2;

                int zoom_x = e.X - black_left_width;
                int zoom_y = e.Y - black_top_height;

                double original_x = (double)zoom_x / rate;
                double original_y = (double)zoom_y / rate;

                this.CoordinateR.Text = "像方坐标（右）：（" + original_x + ", " + original_y + ")";
            }
            #endregion
        }
        #endregion

        #region 图像上画十字
        private void DrawPicMarkSearch()
        {
            foreach (var t in _targetData)
            {
                _serImg?.DrawMark((int)t.RightColNumber, (int)t.RightRowNumber, t.PointNumber);
            }
            foreach (var t in _existData)
            {
                _serImg?.DrawMark((int)t.RightColNumber, (int)t.RightRowNumber, t.PointNumber);
            }
            pictureMatch.Image = _serImg?.ImgData;
        }

        private void DrawPicMarkBase()
        {
            foreach (var t in _existData)
            {
                _baseImg?.DrawMark((int)t.LeftColNumber, (int)t.LeftRowNumber, t.PointNumber);
            }
            pictureRef.Image = _baseImg?.ImgData;
        }
        #endregion

        private bool CheckOpen(bool a = false)
        {
            bool isOk = true;

            if (_existData.Count == 0)
                isOk = false;
            else if (_camPara == null)
                isOk = false;
            else if (a)
            {
                if (_targetData.Count == 0)
                    isOk = false;
            }

            if (isOk == false)
            {
                MessageBox.Show("请打开相应数据", Resources.Prog_Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return isOk;
        }

        private OpenFileDialog MyOpenFileDialog(string def, string filter)
        {
            var ofd = new OpenFileDialog
            {
                ValidateNames = true,
                CheckFileExists = true,
                CheckPathExists = true,
                InitialDirectory = @"C:\Users\Administrator\Desktop\PhotogrammetryData",
                Multiselect = false,
                Title = def + " - " + Resources.Prog_Name,
                Filter = filter
            };

            return ofd.ShowDialog() == DialogResult.OK ? ofd : null;
        }

        private void 退出XToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void 关于AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog();
        }

        /// <summary>
        /// 切换参数信息显示
        /// </summary>
        private void treeViewImg_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var parent = e.Node.Parent;
            if (parent == null)
            {
                textBox8.Text = "";
                return;
            }

            // 0为基准，1为搜索
            switch (parent.Index)
            {
                case 0:
                    textBox8.Text = "基准影像：" + e.Node.Text;
                    break;

                case 1:
                    textBox8.Text = "搜索影像：" + e.Node.Text;
                    break;
            }
        }

        #region 滚动条跳转
        private void dataExistPoint_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex,
                col = e.ColumnIndex;
            if (row < 0) return;
            if (pictureRef.Image == null | _existData.Count == 0) return;

            try
            {
                panelLU.VerticalScroll.Value = (int)_existData[row].LeftRowNumber;
                panelLU.HorizontalScroll.Value = (int)_existData[row].LeftColNumber;
            }
            catch (Exception)
            {
                return;
            }
        }

        private void dataTargetPoint_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex,
                col = e.ColumnIndex;
            if (row < 0) return;
            if (pictureMatch.Image == null | _targetData.Count == 0) return;
            try
            {
                panelRU.VerticalScroll.Value = (int)_targetData[row].RightRowNumber;
                panelRU.HorizontalScroll.Value = (int)_targetData[row].RightColNumber;
            }
            catch (Exception)
            {
                return;
            }
        }
        #endregion

        /// <summary>
        /// 保存所有点的数据（临时添加）
        /// </summary>
        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_fr == null | _left == null | _right == null | _fr?.HasProcessed == false |
                _left?.HasProcessed == false | _right?.HasProcessed == false)
            {
                MessageBox.Show(Resources.NotProcessed, Resources.Prog_Name, MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog
            {
                Title = "保存结果",
                CheckPathExists = true,
                InitialDirectory = @"C:\Users\Administrator\Desktop\PhotogrammetryData",
                Filter = "CSV文件(*.csv)|*.csv"
            };
            if (sfd.ShowDialog() != DialogResult.OK) return;

            var c = _fr.GetDictForSave();
            var a = _left.GetDictForSave();
            var b = _right.GetDictForSave();

            try
            {
                using (StreamWriter sw = new StreamWriter(sfd.OpenFile(), Encoding.Default))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("点ID,左影像行号,左影像列号,右影像行号,右影像列号,左像平面X,左像平面Y,右像平面X,右像平面Y");

                    for (int i = 0; i < a.Count; i++)
                    {
                        sb.AppendLine(a[i]["ID"].ToString("####") + "," + a[i]["row"] + "," + a[i]["col"] + "," +
                                      b[i]["row"] + "," + b[i]["col"] + "," +
                                      a[i]["xa"] + "," + a[i]["ya"] + "," + b[i]["xa"] + "," + b[i]["ya"]);
                    }

                    foreach (var t in c)
                    {
                        sb.AppendLine(t["ID"].ToString("####") + "," + t["lRow"] + "," + t["lCol"] + "," +
                                      t["rRow"] + "," + t["rCol"] + "," +
                                      t["lXa"] + "," + t["lYa"] + "," + t["rXa"] + "," + t["rYa"]);
                    }

                    sw.WriteLine(sb.ToString());
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Resources.Prog_Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

    }
}
