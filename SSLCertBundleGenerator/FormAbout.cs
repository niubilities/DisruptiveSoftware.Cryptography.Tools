namespace SSLCertBundleGenerator
{
  using System.Diagnostics;
  using SSLCertBundleGenerator.Commons;
  using SSLCertBundleGenerator.Properties;

  public partial class FormAbout : Form
  {
    public FormAbout()
    {
      InitializeComponent();

      var version = AssemblyUtils.GetProductVersion();

      labelVersion.Text = labelVersion.Text.Replace("{version}", version);

      richTextBoxCopyright.Text = richTextBoxCopyright.Text.Replace(
        "{bouncyCastleCryptoLibVersion}",
        AssemblyUtils.GetVersion("BouncyCastle.Crypto")?.ToString());

      richTextBoxCopyright.Text = richTextBoxCopyright.Text.Replace(
        "{disruptiveSoftwareCryptographyLibVersion}",
        AssemblyUtils.GetVersion("DisruptiveSoftware.Cryptography")?.ToString());

      KeyDown += FormAbout_KeyPress;

      richTextBoxCopyright.LinkClicked += RichTextBoxCopyright_LinkClicked;
      linkLabelContact.LinkClicked += LinkLabelContact_LinkClicked;
      linkLabelSource.LinkClicked += LinkLabelSource_LinkClicked;
    }

    private void FormAbout_KeyPress(object? sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Escape) Close();
    }

    private void LinkLabelContact_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
    {
      Process.Start(
        $"mailto:{linkLabelContact.Text}?subject=About {Resources.Title} v{AssemblyUtils.GetVersion()}");
    }

    private void LinkLabelSource_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
    {
      Process.Start("https://github.com/cristianst85/DisruptiveSoftware.Cryptography.Tools");
    }

    private void RichTextBoxCopyright_LinkClicked(object? sender, LinkClickedEventArgs e)
    {
      if (e.LinkText != null) Process.Start(e.LinkText);
    }
  }
}