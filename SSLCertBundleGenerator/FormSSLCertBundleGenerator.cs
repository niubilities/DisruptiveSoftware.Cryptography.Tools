using X509Constants = DisruptiveSoftware.Cryptography.X509.Constants;

namespace SSLCertBundleGenerator
{
    using DisruptiveSoftware.Cryptography.BouncyCastle.Extensions;
    using DisruptiveSoftware.Cryptography.Extensions;
    using DisruptiveSoftware.Cryptography.X509;
    using SSLCertBundleGenerator.Commons;
    using SSLCertBundleGenerator.Commons.Controls;
    using SSLCertBundleGenerator.Commons.Controls.Validation;
    using SSLCertBundleGenerator.Extensions;

    public partial class FormSslCertBundleGenerator : Form
    {
        private volatile bool _isClosing;

        public FormSslCertBundleGenerator()
        {
            InitializeComponent();

            Text = Text.Replace("{version}", AssemblyUtils.GetVersion().ToString(3));

            toolStripStatusLabel.Text = string.Empty;

            comboBoxKeySize.Items.Add(X509Constants.RsaKeySize.KeySize1024);
            comboBoxKeySize.Items.Add(X509Constants.RsaKeySize.KeySize2048);
            comboBoxKeySize.Items.Add(X509Constants.RsaKeySize.KeySize4096);

            comboBoxValidity.Items.AddRange(Enumerable.Range(1, 24).Cast<object>().ToArray());

            // Default values.
            comboBoxKeySize.SelectedItem = comboBoxKeySize.Items[1];
            comboBoxValidity.SelectedItem = comboBoxValidity.Items[11];

            numericUpDownSerialNumber.Maximum = long.MaxValue;
            numericUpDownSerialNumber.Minimum = 2;
            numericUpDownSerialNumber.Value = 2;

            // SSL Certificate Enhanced Key Usage property must contain Server Authentication (1.3.6.1.5.5.7.3.1).
            checkBoxServerAuthentication.Checked = true;
            checkBoxServerAuthentication.Enabled = false;

            pictureBoxInfo.SetToolTip(
                "Multiple Subject Alternative Names (SANs) can be specified separated by semicolons.");

            // Handlers.
            FormClosing += FormSSLCertBundleGenerator_Closing;
        }

        public sealed override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var form = new FormAbout();
            form.Text = form.Text.Replace("{title}", Application.ProductName);
            form.Update();

            form.ShowDialog(this);
        }

        private void ButtonBrowse_Click(object sender, EventArgs e)
        {
            using var folderBrowserDialog = new FolderBrowserDialog();
            var applicationPathDirectory = Path.GetDirectoryName(AssemblyUtils.GetApplicationPath());

            folderBrowserDialog.SelectedPath =
                textBoxSavePath.Text.IsNullOrEmpty() ? applicationPathDirectory : textBoxSavePath.Text;

            folderBrowserDialog.ShowNewFolderButton = true;
            folderBrowserDialog.Description = "Please choose a directory where to save certificates.";

            if (DialogResult.OK == folderBrowserDialog.ShowDialog())
                textBoxSavePath.Text = folderBrowserDialog.SelectedPath;
        }

        private void ButtonGenerate_Click(object sender, EventArgs e)
        {
            GenerateCertificatesAsync();
        }

        private void ButtonShowPassword_Click(object sender, EventArgs e)
        {
            textBoxPassword.ToogleUseSystemPasswordChar();
        }

        private void CheckBoxRandomSerialNumber_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownSerialNumber.Enabled = !checkBoxRandomSerialNumber.Checked;
        }

        private void CloseApplication()
        {
            _isClosing = true;

            Close();
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseApplication();
        }

        private void FormSSLCertBundleGenerator_Closing(object sender, FormClosingEventArgs e)
        {
            _isClosing = true;
        }

        private async void GenerateCertificatesAsync()
        {
            try
            {
                if (!ValidateControls())
                {
                    UpdateStatusStrip("Please fill all required fields.");

                    return;
                }

                var savePath = textBoxSavePath.Text;

                if (!Directory.Exists(savePath)) throw new Exception("Destination directory does not exist.");

                if (Directory.GetFiles(savePath).Length > 0)
                    throw new Exception("Destination directory must be empty.");

                UpdateStatusStrip("Generating Certificate files...");

                ToogleControls(false);

                var now = DateTime.UtcNow;
                var keySize = Convert.ToUInt32(comboBoxKeySize.SelectedItem);
                var validityInMonths = Convert.ToInt32(comboBoxValidity.SelectedItem);
                var serialNumber = Convert.ToInt64(numericUpDownSerialNumber.Value);

                await Task.Run(
                    () =>
                    {
                        var certificateBuilderResult = new CaCertificateBuilder()
                            .WithSerialNumberConfiguration(checkBoxRandomSerialNumber.Checked, serialNumber - 1)
                            .SetKeySize(keySize)
                            .SetSubjectDn(textBoxCN.Text + " CA", textBoxOU.Text, textBoxO.Text, null, textBoxC.Text)
                            .SetNotBefore(now)
                            .SetNotAfter(now.AddMonths(validityInMonths))
                            .Build();

                        var pkcs12Data =
                            certificateBuilderResult.ExportCertificate(textBoxPassword.Text.ToSecureString());

                        var sslCertificateBuilder = new SslCertificateBuilder()
                            .WithSerialNumberConfiguration(checkBoxRandomSerialNumber.Checked, serialNumber)
                            .SetKeySize(keySize)
                            .SetSubjectDn(textBoxCN.Text, textBoxOU.Text, textBoxO.Text, null, textBoxC.Text)
                            .SetNotBefore(now)
                            .SetNotAfter(now.AddMonths(validityInMonths))
                            .SetIssuerCertificate(pkcs12Data, textBoxPassword.Text.ToSecureString());

                        if (checkBoxClientAuthentication.Checked)
                            sslCertificateBuilder = sslCertificateBuilder.SetClientAuthKeyUsage();

                        if (checkBoxServerAuthentication.Checked)
                            sslCertificateBuilder = sslCertificateBuilder.SetServerAuthKeyUsage();

                        if (!textBoxSAN.Text.IsNullOrEmpty())
                        {
                            var sans = textBoxSAN.Text.Split(';').Select(x => x.Trim()).ToList();
                            sslCertificateBuilder = sslCertificateBuilder.SetSubjectAlternativeNames(sans);
                        }

                        var sslCertificateBuilderResult = sslCertificateBuilder.Build();
                        File.WriteAllBytes(Path.Combine(savePath, "caCertificate.p12"), pkcs12Data);

                        if (checkBoxCertificateExportCrt.Checked)
                        {
                            var certData = certificateBuilderResult.Certificate.ExportPublicKeyCertificate();
                            File.WriteAllBytes(Path.Combine(savePath, "caCertificate.crt"), certData);
                        }

                        var sslPkcs12Data =
                            sslCertificateBuilderResult.ExportCertificate(textBoxPassword.Text.ToSecureString());

                        File.WriteAllBytes(Path.Combine(savePath, "sslCertificate.p12"), sslPkcs12Data);

                        if (checkBoxCertificateExportCrt.Checked)
                        {
                            var sslCertData = sslCertificateBuilderResult.Certificate.ExportPublicKeyCertificate();
                            File.WriteAllBytes(Path.Combine(savePath, "sslCertificate.crt"), sslCertData);
                        }
                    });

                UpdateStatusStrip("Certificates generated successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                UpdateStatusStrip(string.Empty);
            }

            ToogleControls(true);
        }

        private void ToogleControls(bool enabled)
        {
            textBoxCN.Enabled = enabled;
            textBoxO.Enabled = enabled;
            textBoxOU.Enabled = enabled;
            textBoxC.Enabled = enabled;

            numericUpDownSerialNumber.Enabled = !checkBoxRandomSerialNumber.Checked;
            checkBoxRandomSerialNumber.Enabled = enabled;
            comboBoxKeySize.Enabled = enabled;
            comboBoxValidity.Enabled = enabled;

            textBoxSAN.Enabled = enabled;

            checkBoxClientAuthentication.Enabled = enabled;

            textBoxPassword.Enabled = enabled;
            buttonToogleShowPassword.Enabled = enabled;

            checkBoxCertificateExportCrt.Enabled = enabled;

            textBoxSavePath.Enabled = enabled;
            buttonBrowseSavePath.Enabled = enabled;

            buttonGenerate.Enabled = enabled;
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

            result &= textBoxCN.IsValid(ValidationRules.Required);
            result &= textBoxOU.IsValid(ValidationRules.Required);
            result &= textBoxO.IsValid(ValidationRules.Required);
            result &= textBoxC.IsValid(ValidationRules.Required);
            result &= numericUpDownSerialNumber.IsValid(ValidationRules.Required);
            result &= textBoxPassword.IsValid(ValidationRules.Required);
            result &= textBoxSavePath.IsValid(ValidationRules.Required);

            return result;
        }
    }
}