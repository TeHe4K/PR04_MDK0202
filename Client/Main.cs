using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Main : Form
    {
        private const string TagUp = "up";
        private const string TagFolder = "folder";
        private const string TagFile = "file";

        private readonly FtpClient client = new FtpClient();
        // Путь относительно корневой папки пользователя, например ["Docs", "2026"]
        private readonly List<string> path = new List<string>();

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            ImageList icons = new ImageList();
            icons.ColorDepth = ColorDepth.Depth32Bit;
            icons.ImageSize = new Size(16, 16);
            icons.Images.Add(TagFolder, DrawFolderIcon());
            icons.Images.Add(TagFile, DrawFileIcon());
            listFiles.SmallImageList = icons;
            UpdateButtons();
        }

        // ---------- Подключение ----------

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            IPAddress ip;
            int port;
            if (!IPAddress.TryParse(txtIp.Text.Trim(), out ip))
            {
                ShowError("Неверный IP-адрес");
                return;
            }
            if (!int.TryParse(txtPort.Text.Trim(), out port) || port < 1 || port > 65535)
            {
                ShowError("Неверный порт");
                return;
            }
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Text;
            if (login == "" || password == "" || login.Contains(" ") || password.Contains(" "))
            {
                ShowError("Введите логин и пароль (без пробелов)");
                return;
            }

            client.Ip = ip;
            client.Port = port;
            SetBusy(true, "Подключение...");
            try
            {
                await Task.Run(() => client.Connect(login, password));
                List<string> items = await Task.Run(() => client.Cd(null));
                path.Clear();
                ShowItems(items);
                SetStatus("Подключено как " + login);
            }
            catch (Exception ex)
            {
                ShowError("Не удалось подключиться: " + ex.Message);
            }
            finally
            {
                SetBusy(false, null);
            }
        }

        // ---------- Навигация ----------

        private async Task Navigate(string folder)
        {
            SetBusy(true, "Загрузка списка...");
            try
            {
                List<string> items = await Task.Run(() => client.Cd(folder));
                if (folder == "..")
                {
                    if (path.Count > 0)
                        path.RemoveAt(path.Count - 1);
                }
                else if (folder != ".")
                {
                    path.Add(folder);
                }
                ShowItems(items);
                SetStatus("Готово");
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
            finally
            {
                SetBusy(false, null);
            }
        }

        private void ShowItems(List<string> items)
        {
            listFiles.BeginUpdate();
            listFiles.Items.Clear();
            if (path.Count > 0)
            {
                ListViewItem up = new ListViewItem("..", TagFolder);
                up.SubItems.Add("На уровень выше");
                up.Tag = TagUp;
                listFiles.Items.Add(up);
            }
            foreach (string name in items)
            {
                bool isFolder = name.EndsWith("/");
                ListViewItem item = new ListViewItem(isFolder ? name.TrimEnd('/') : name, isFolder ? TagFolder : TagFile);
                item.SubItems.Add(isFolder ? "Папка" : "Файл");
                item.Tag = isFolder ? TagFolder : TagFile;
                listFiles.Items.Add(item);
            }
            listFiles.EndUpdate();
            lblPath.Text = "Путь: /" + string.Join("/", path);
            UpdateButtons();
        }

        // Двойной щелчок: папка — открыть, ".." — назад, файл — скачать
        private async void listFiles_DoubleClick(object sender, EventArgs e)
        {
            if (listFiles.SelectedItems.Count == 0)
                return;
            ListViewItem item = listFiles.SelectedItems[0];
            string tag = (string)item.Tag;
            if (tag == TagUp)
                await Navigate("..");
            else if (tag == TagFolder)
                await Navigate(item.Text);
            else
                await DownloadFile(item.Text);
        }

        private async void btnUp_Click(object sender, EventArgs e)
        {
            await Navigate("..");
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await Navigate(".");
        }

        // ---------- Скачивание и загрузка ----------

        private async void btnDownload_Click(object sender, EventArgs e)
        {
            if (listFiles.SelectedItems.Count == 0 || (string)listFiles.SelectedItems[0].Tag != TagFile)
            {
                ShowError("Выберите файл для скачивания");
                return;
            }
            await DownloadFile(listFiles.SelectedItems[0].Text);
        }

        private async Task DownloadFile(string fileName)
        {
            string savePath;
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.FileName = fileName;
                dialog.Filter = "Все файлы (*.*)|*.*";
                if (dialog.ShowDialog(this) != DialogResult.OK)
                    return;
                savePath = dialog.FileName;
            }

            SetBusy(true, "Скачивание " + fileName + "...");
            try
            {
                byte[] data = await Task.Run(() => client.Get(fileName));
                File.WriteAllBytes(savePath, data);
                SetStatus("Файл сохранён: " + savePath);
            }
            catch (Exception ex)
            {
                ShowError("Ошибка скачивания: " + ex.Message);
            }
            finally
            {
                SetBusy(false, null);
            }
        }

        private async void btnUpload_Click(object sender, EventArgs e)
        {
            string localPath;
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "Все файлы (*.*)|*.*";
                if (dialog.ShowDialog(this) != DialogResult.OK)
                    return;
                localPath = dialog.FileName;
            }

            SetBusy(true, "Загрузка " + Path.GetFileName(localPath) + "...");
            string result;
            try
            {
                result = await Task.Run(() => client.Upload(localPath));
            }
            catch (Exception ex)
            {
                ShowError("Ошибка загрузки: " + ex.Message);
                return;
            }
            finally
            {
                SetBusy(false, null);
            }
            await Navigate(".");
            SetStatus(result);
        }

        // ---------- Вспомогательное ----------

        private void SetBusy(bool busy, string status)
        {
            UseWaitCursor = busy;
            groupConnect.Enabled = !busy;
            panelNav.Enabled = !busy;
            panelButtons.Enabled = !busy;
            listFiles.Enabled = !busy;
            if (status != null)
                SetStatus(status);
            if (!busy)
                UpdateButtons();
        }

        private void UpdateButtons()
        {
            bool connected = client.IsConnected;
            btnUp.Enabled = connected && path.Count > 0;
            btnRefresh.Enabled = connected;
            btnDownload.Enabled = connected;
            btnUpload.Enabled = connected;
        }

        private void SetStatus(string text)
        {
            lblStatus.Text = text;
        }

        private void ShowError(string text)
        {
            SetStatus(text);
            MessageBox.Show(this, text, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static Bitmap DrawFolderIcon()
        {
            Bitmap bmp = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(bmp))
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(240, 190, 60)))
            using (Pen pen = new Pen(Color.FromArgb(170, 125, 20)))
            {
                g.FillRectangle(brush, 1, 3, 6, 3);
                g.FillRectangle(brush, 1, 5, 14, 9);
                g.DrawRectangle(pen, 1, 5, 13, 8);
            }
            return bmp;
        }

        private static Bitmap DrawFileIcon()
        {
            Bitmap bmp = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(bmp))
            using (Pen pen = new Pen(Color.Gray))
            {
                g.FillRectangle(Brushes.White, 3, 1, 10, 14);
                g.DrawRectangle(pen, 3, 1, 9, 13);
                g.DrawLine(pen, 5, 5, 10, 5);
                g.DrawLine(pen, 5, 8, 10, 8);
                g.DrawLine(pen, 5, 11, 10, 11);
            }
            return bmp;
        }
    }
}
