namespace DesktopShorctutBackupForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //string backupFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "desktop_shortcuts_backup.json");
        private string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        private string backupDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DesktopFullBackup");
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        static void BackupDesktopShortcuts(string desktopPath, string exe, string filename)
        {
            string backupDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);
            Directory.CreateDirectory(backupDirectory);

            List<string> shortcutPaths = Directory.GetFiles(desktopPath, "*" + exe).ToList();

            foreach (string shortcutPath in shortcutPaths)
            {
                string fileName = Path.GetFileName(shortcutPath);
                string newShortcutPath = Path.Combine(backupDirectory, fileName);
                if (!File.Exists(newShortcutPath))
                {
                    File.Copy(shortcutPath, newShortcutPath, true);
                    Console.WriteLine($"File {fileName} Backed Up!");
                }
            }

        }
        static void RestoreMissingDesktopShortcuts(string desktopPath, string exe, string filename)
        {
            string backupDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

            if (!Directory.Exists(backupDirectory))
            {
                Console.WriteLine($"No backup directory found at {backupDirectory}");
                return;
            }

            List<string> shortcutPaths = Directory.GetFiles(backupDirectory, "*" + exe).ToList();

            foreach (string shortcutPath in shortcutPaths)
            {
                string fileName = Path.GetFileName(shortcutPath);
                string newShortcutPath = Path.Combine(desktopPath, fileName);

                if (!File.Exists(newShortcutPath))
                {
                    Console.WriteLine($"Missing desktop shortcuts restored from {backupDirectory}");
                    File.Copy(shortcutPath, newShortcutPath, true);
                }
            }

        }
        static void BackupDirectory(string sourceDirectory, string backupDirectory)
        {
            Directory.CreateDirectory(backupDirectory);

            // Backup files
            foreach (string filePath in Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories))
            {
                string relativePath = filePath.Substring(sourceDirectory.Length + 1);
                string backupFilePath = Path.Combine(backupDirectory, relativePath);

                Directory.CreateDirectory(Path.GetDirectoryName(backupFilePath));

                if (!File.Exists(backupFilePath))
                {
                    File.Copy(filePath, backupFilePath);
                    Console.WriteLine($"File {relativePath} backed up!");
                }
            }

            // Backup directories
            foreach (string directoryPath in Directory.GetDirectories(sourceDirectory, "*", SearchOption.AllDirectories))
            {
                string relativePath = directoryPath.Substring(sourceDirectory.Length + 1);
                string backupDirectoryPath = Path.Combine(backupDirectory, relativePath);

                Directory.CreateDirectory(backupDirectoryPath);

                Console.WriteLine($"Directory {relativePath} backed up!");
            }
        }
        static void RestoreDirFromBackup(string sourceDirectory, string backupDirectory)
        {
            if (!Directory.Exists(backupDirectory))
            {
                Console.WriteLine($"No backup directory found at {backupDirectory}");
                return;
            }

            // Loop through all files and directories in the backup directory
            foreach (string backupPath in Directory.GetFileSystemEntries(backupDirectory, "*", SearchOption.AllDirectories))
            {
                string relativePath = backupPath.Substring(backupDirectory.Length + 1);
                string sourcePath = Path.Combine(sourceDirectory, relativePath);

                // Check if the file or directory exists in the source directory
                if (!File.Exists(sourcePath) && !Directory.Exists(sourcePath))
                {
                    // If it's a file, create the directory structure in the source directory and copy the file
                    if (File.Exists(backupPath))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(sourcePath));
                        File.Copy(backupPath, sourcePath);
                        Console.WriteLine($"File {relativePath} restored from backup!");
                    }
                    // If it's a directory, create the directory in the source directory
                    else if (Directory.Exists(backupPath))
                    {
                        Directory.CreateDirectory(sourcePath);
                        Console.WriteLine($"Directory {relativePath} restored from backup!");
                    }
                }
            }
        }

        //backup shortcuts
        private void button1_Click(object sender, EventArgs e)
        {
            BackupDesktopShortcuts(desktopPath, ".lnk", "DesktopShortcutsBackup");
            BackupDesktopShortcuts(desktopPath, ".url", "DesktopShortcutsBackup");
        }
        //restore shortcuts
        private void button2_Click(object sender, EventArgs e)
        {
            RestoreMissingDesktopShortcuts(desktopPath, ".lnk", "DesktopShortcutsBackup");
            RestoreMissingDesktopShortcuts(desktopPath, ".url", "DesktopShortcutsBackup");
        }
        //backup full dir
        private void button4_Click(object sender, EventArgs e)
        {
            BackupDirectory(desktopPath, backupDirectory);
        }
        //restore full dir
        private void button3_Click(object sender, EventArgs e)
        {
            RestoreDirFromBackup(desktopPath, backupDirectory);
        }
    }
}