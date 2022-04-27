namespace CertUtil
{
    using System.Text;
    using CertUtil.Commons;
    using CertUtil.Commons.Controls;
    using CertUtil.Commons.Controls.Validation;
    using DisruptiveSoftware.Cryptography.Extensions;
    using DisruptiveSoftware.Cryptography.Utils;

    public partial class FormCertUtil : Form
    {
        private volatile bool _isClosing;

        public FormCertUtil()
        {
            InitializeComponent();

            Text = Text.Replace("{version}", AssemblyUtils.GetVersion()?.ToString(3));

            toolStripStatusLabel.Text = string.Empty;

            foreach (var cryptoObject in CertUtilConstants.CryptographicObjects)
                comboBoxObject.Items.Add(cryptoObject.Key);

            comboBoxObject.SelectedItem = comboBoxObject.Items[0];

            foreach (var pkcFormat in CertUtilConstants.PublicKeyCertificatesFormats)
                comboBoxFormat.Items.Add(pkcFormat.Key);

            comboBoxFormat.SelectedItem = comboBoxFormat.Items[0];

            // Handlers.
            comboBoxObject.SelectionChangeCommitted += ComboBoxObject_SelectionChangeCommitted;
            FormClosing += FormCertUtil_Closing;
        }

        private void AboutToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            using var form = new FormAbout();
            form.Text = form.Text.Replace("{title}", Application.ProductName);
            form.Update();

            form.ShowDialog(this);
        }

        private void ButtonBrowsePath_Click(object? sender, EventArgs e)
        {
            using var openFileDialog = new OpenFileDialog();
            openFileDialog.FileName = string.Empty;
            openFileDialog.Multiselect = false;
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            openFileDialog.Title = "Please choose a file that contains the private key...";

            openFileDialog.Filter =
                "PKCS #12 (*.p12)|*.p12|Personal Information Exchange (*.pfx)|*.pfx|All files (*.*)|*.*";

            if (DialogResult.OK == openFileDialog.ShowDialog(this))
            {
                textBoxPath.Text = openFileDialog.FileName;
                textBoxPath.Select(textBoxPath.Text.Length, 0);
            }
        }

        private void ButtonExport_Click(object? sender, EventArgs e)
        {
            ExportAsync();
        }

        private void ButtonToogleShowPassword_Click(object? sender, EventArgs e)
        {
            textBoxPassword.ToogleUseSystemPasswordChar();
        }

        private void CloseApplication()
        {
            _isClosing = true;

            Close();
        }

        private void ComboBoxObject_SelectionChangeCommitted(object? sender, EventArgs e)
        {
            comboBoxFormat.Items.Clear();

            var selectedItemObject = (string)comboBoxObject.SelectedItem;

            if (CertUtilConstants.CryptographicObjects[selectedItemObject] ==
                CryptographicObjectType.PublicKey)
                foreach (var pkcFormat in CertUtilConstants.PublicKeyCertificatesFormats)
                    comboBoxFormat.Items.Add(pkcFormat.Key);
            else if (CertUtilConstants.CryptographicObjects[selectedItemObject] ==
                     CryptographicObjectType.PrivateKey)
                foreach (var pkFormat in CertUtilConstants.PrivateKeyFormats)
                    comboBoxFormat.Items.Add(pkFormat.Key);

            comboBoxFormat.SelectedItem = comboBoxFormat.Items[0];
        }

        private void ExitToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            CloseApplication();
        }

        private async void ExportAsync()
        {
            try
            {
                if (!ValidateControls())
                {
                    UpdateStatusStrip("Please fill all required fields.");

                    return;
                }

                UpdateStatusStrip(string.Empty);

                ToogleControls(false);

                byte[]? cryptographicObjectContent = null;

                var saveFileDialogFilter = string.Empty;
                var saveFileDialogFileName = Path.GetFileNameWithoutExtension(textBoxPath.Text);

                var isSnkFile = string.Equals(
                    Path.GetExtension(textBoxPath.Text),
                    ".snk",
                    StringComparison.OrdinalIgnoreCase);

                var selectedItemObject = (string)comboBoxObject.SelectedItem;
                var selectedItemFormat = (string)comboBoxFormat.SelectedItem;

                var selectedCryptographicObjectType = CertUtilConstants.CryptographicObjects[selectedItemObject];

                await Task.Run(
                    () =>
                    {
                        switch (selectedCryptographicObjectType)
                        {
                            case CryptographicObjectType.PublicKey:
                            {
                                saveFileDialogFilter =
                                    $"{selectedItemFormat}|{CertUtilConstants.PublicKeyCertificatesFormats[selectedItemFormat]}";

                                saveFileDialogFileName += CertUtilConstants
                                    .PublicKeyCertificatesFormats[selectedItemFormat].Trim('*');

                                if (selectedItemFormat.Contains("Base64"))
                                {
                                    var
                                        cryptographicObjectText = isSnkFile
                                            ? CertificateUtils.ExportSnkPublicKeyToPEM(
                                                File.ReadAllBytes(textBoxPath.Text))
                                            : CertificateUtils.ExportPublicKeyCertificateToPEM(
                                                File.ReadAllBytes(textBoxPath.Text),
                                                textBoxPassword.Text.ToSecureString());

                                    cryptographicObjectContent = Encoding.ASCII.GetBytes(cryptographicObjectText);
                                }
                                else if (selectedItemFormat.Contains("Binary"))
                                {
                                    cryptographicObjectContent = isSnkFile
                                        ? CertificateUtils.ExportSnkPublicKeyCertificate(
                                            File.ReadAllBytes(textBoxPath.Text))
                                        : CertificateUtils.ExportPublicKeyCertificate(
                                            File.ReadAllBytes(textBoxPath.Text),
                                            textBoxPassword.Text.ToSecureString());
                                }
                                else
                                {
                                    throw new NotImplementedException();
                                }

                                break;
                            }
                            case CryptographicObjectType.PrivateKey:
                            {
                                saveFileDialogFilter =
                                    $"{selectedItemFormat}|{CertUtilConstants.PrivateKeyFormats[selectedItemFormat]}";

                                saveFileDialogFileName +=
                                    CertUtilConstants.PrivateKeyFormats[selectedItemFormat].Trim('*');

                                if (selectedItemFormat.Contains("Base64"))
                                {
                                    var cryptographicObjectText = isSnkFile
                                        ? CertificateUtils.ExportSnkPrivateKeyToPEM(File.ReadAllBytes(textBoxPath.Text))
                                        : CertificateUtils.ExportPrivateKeyToPEM(
                                            File.ReadAllBytes(textBoxPath.Text),
                                            textBoxPassword.Text.ToSecureString());

                                    if (cryptographicObjectText.IsNullOrEmpty())
                                        throw new Exception("Certificate does not have a private key.");

                                    cryptographicObjectContent = Encoding.ASCII.GetBytes(cryptographicObjectText!);
                                }
                                else if (selectedItemFormat.Contains("Binary"))
                                {
                                    cryptographicObjectContent = isSnkFile
                                        ? CertificateUtils.ExportSnkPrivateKey(File.ReadAllBytes(textBoxPath.Text))
                                        : CertificateUtils.ExportPrivateKey(
                                            File.ReadAllBytes(textBoxPath.Text),
                                            textBoxPassword.Text.ToSecureString());

                                    if (cryptographicObjectContent == null)
                                        throw new Exception("Certificate does not have a private key.");
                                }
                                else
                                {
                                    throw new NotImplementedException();
                                }

                                break;
                            }
                            default:
                                throw new NotImplementedException();
                        }
                    });

                using var saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Please choose a file name...";
                saveFileDialog.Filter = saveFileDialogFilter;
                saveFileDialog.FileName = saveFileDialogFileName;
                saveFileDialog.CheckFileExists = false;
                saveFileDialog.CheckPathExists = true;

                if (DialogResult.OK == saveFileDialog.ShowDialog(this))
                {
                    var fileName = saveFileDialog.FileName;

                    if (fileName.Length > 0)
                    {
                        await File.WriteAllBytesAsync(fileName, cryptographicObjectContent);

                        UpdateStatusStrip("File has been successfully saved.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            ToogleControls(true);
        }

        private void FormCertUtil_Closing(object? sender, FormClosingEventArgs e)
        {
            _isClosing = true;
        }

        private void ToogleControls(bool enabled)
        {
            textBoxPath.Enabled = enabled;
            buttonBrowsePath.Enabled = enabled;

            textBoxPassword.Enabled = enabled;
            buttonToogleShowPassword.Enabled = enabled;

            comboBoxObject.Enabled = enabled;
            comboBoxFormat.Enabled = enabled;

            buttonExport.Enabled = enabled;
        }

        private void UpdateStatusStrip(string text)
        {
            if (!_isClosing)
                statusStrip.InvokeIfRequired(
                    () =>
                    {
                        toolStripStatusLabel.Text = text;
                        statusStrip.Update();
                    });
        }

        private bool ValidateControls()
        {
            var result = true;

            result &= textBoxPath.IsValid(ValidationRules.FileExists);

            return result;
        }
    }
}