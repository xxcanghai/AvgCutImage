using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace AvgCutImage
{
    public partial class FmMain : Form
    {
        string SavePath = Path.Combine(Environment.CurrentDirectory, "AvgCutImage");
        public FmMain()
        {
            InitializeComponent();
        }

        //选择图片
        private void btSelectImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog opfd = new OpenFileDialog();
            opfd.Filter = "图片文件(*.jpg,*.jpeg,*.bmp,*.png)|*.jpg;*.jpeg;*.bmp;*.png";
            opfd.InitialDirectory = Environment.CurrentDirectory;
            opfd.Multiselect = false;
            opfd.Title = "请选择要切割的图片";
            if (opfd.ShowDialog(this) != System.Windows.Forms.DialogResult.OK) return;
            tbImagePath.Text = opfd.FileName;
            pictureBox1.ImageLocation = opfd.FileName;
        }

        private void tbImagePath_Click(object sender, EventArgs e)
        {
            btSelectImage_Click("", null);
        }

        //开始图片切割
        private void btCut_Click(object sender, EventArgs e)
        {
            if (!File.Exists(tbImagePath.Text)) { MessageBox.Show("请先把要切割的图片拖拽到程序上", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            if (!Directory.Exists(SavePath))
                Directory.CreateDirectory(SavePath);
            FileInfo fileImg = new FileInfo(tbImagePath.Text);
            using (Bitmap b = new Bitmap(fileImg.FullName))
            {
                List<Rectangle> lstRect = CreateAvgCutRectangle(b.Width, b.Height, (int)numCutRows.Value, (int)numCutCells.Value);
                for (int i = 0; i < lstRect.Count; i++)
                {
                    CutImage(tbImagePath.Text, lstRect[i].X, lstRect[i].Y, lstRect[i].Width, lstRect[i].Height,
                        string.Format("{0}_{1}.bmp", fileImg.Name.Remove(fileImg.Name.LastIndexOf(".")),
                            NumToNostring(i)));
                }
            }
            System.Diagnostics.Process.Start("explorer.exe", SavePath);
        }

        /// <summary>
        /// 把原始图片按照指定的分块切割并保存到目标路径中
        /// </summary>
        /// <param name="srcImagePath">原始图片即要切割的图片的路径</param>
        /// <param name="srcX">切割左上角点X坐标</param>
        /// <param name="srcY">切割左上角点Y坐标</param>
        /// <param name="srcWidth">切割宽度</param>
        /// <param name="srcHeight">切割高度</param>
        /// <param name="dstImagePath">要保存到的目标路径</param>
        private void CutImage(string srcImagePath, int srcX, int srcY, int srcWidth, int srcHeight, string dstImagePath)
        {
            int dstWidth = srcWidth, dstHeight = srcHeight;
            using (Bitmap bitmap = new Bitmap(dstWidth, dstHeight))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    using (Image img = Image.FromFile(srcImagePath))
                    {
                        g.DrawImage(img, new Rectangle(0, 0, dstWidth, dstHeight),
                            new Rectangle(srcX, srcY, srcWidth, srcHeight), GraphicsUnit.Pixel);
                        g.Save();
                        bitmap.Save(Path.Combine(SavePath, dstImagePath));
                    }
                }
            }
        }

        /// <summary>
        /// 根据原始图像大小和要切割的数量创建切割分块合集
        /// </summary>
        /// <param name="imgWidth">要切割的图片的宽度</param>
        /// <param name="imgHeight">要切割的图片的高度</param>
        /// <param name="cutRowsNum">要切割的行数</param>
        /// <param name="cutCellsNum">要切割的列数</param>
        /// <returns></returns>
        private List<Rectangle> CreateAvgCutRectangle(int imgWidth, int imgHeight, int cutRowsNum, int cutCellsNum)
        {
            List<Rectangle> lstRect = new List<Rectangle>();
            int avgWidth = imgWidth / cutCellsNum;//每块的平均宽度
            int avgHeight = imgHeight / cutRowsNum;//每块的平均高度
            int offsetX = 0, offsetY = 0;
            int cutHeight = avgHeight;
            int cutWidth = avgWidth;
            while (true)
            {
                //列分块
                if (imgHeight - offsetY < avgHeight * 2)
                    cutHeight = imgHeight - offsetY;

                offsetX = 0;
                cutWidth = avgWidth;
                while (true)
                {
                    //行分块
                    if (imgWidth - offsetX < avgWidth * 2)
                        cutWidth = imgWidth - offsetX;
                    lstRect.Add(new Rectangle(offsetX, offsetY, cutWidth, cutHeight));
                    offsetX += cutWidth;
                    if (offsetX >= imgWidth) break;
                }
                offsetY += cutHeight;
                if (offsetY >= imgHeight) break;
            }

            return lstRect;
        }

        /// <summary>
        /// 把数值转换成等长字符串，前加0
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private string NumToNostring(int num)
        {
            string zero = string.Empty;
            if (num < 10) zero = "00";
            else if (num < 100) zero = "0";
            else zero = "";
            return zero + num.ToString();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            FmAbout fma = new FmAbout();
            fma.ShowDialog(this);
        }

        //正在拖拽中
        private void FmMain_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        //拖拽完成时
        private void FmMain_DragDrop(object sender, DragEventArgs e)
        {
            //检测是否是windows格式
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                //获取拖放的文件列表
                string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);
                FileInfo f = new FileInfo(paths[0]);
                //拖放的第一个文件的不能是目录并且扩展名必须是.PP 
                if (f.Attributes != FileAttributes.Directory && "*.jpg;*.jpeg;*.bmp;*.png".Contains(f.Extension))
                {
                    tbImagePath.Text = paths[0];
                    pictureBox1.ImageLocation = paths[0];
                }
                else { MessageBox.Show("不是图片文件或不支持的图片格式！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }
    }
}
