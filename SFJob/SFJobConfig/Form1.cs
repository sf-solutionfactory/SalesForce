using SharedSettings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SFJobConfig
{
    public partial class Form1 : Form
    {
        Settings settings = new Settings();
        public Form1()
        {
            InitializeComponent();
            dgvErrores.Columns.Add("error", "Error");
            dgvErrores.Columns[0].Width = 100;
            txtClientId.Focus();
            CargarConf();
            Habilitar();
        }
        private void Guardar()
        {
            List<string> errores = new List<string>();
            btnGuardar.Enabled = false;
            if (ValidarTXT(this))
            {
                btnGuardar.Enabled = true;
                MessageBox.Show("Favor de llenar todos los campos.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            settings.clientId = txtClientId.Text;
            settings.clientSecret = txtClientSecret.Text;
            settings.username = txtUsername.Text;
            settings.password = txtPassword.Text;
            settings.token = txtToken.Text;
            settings.url_login = txtURLLogin.Text;
            settings.url_clientes = txtURLClientes.Text;
            settings.url_facturas = txtURLFacturas.Text;
            settings.url_viajes = txtURLViajes.Text;
            settings.tiempo = Convert.ToDouble(txtTiempo.Text);
            settings.job_intervalo = Convert.ToDouble(txtInterval.Text);
            settings.editTiempo = chcEditHora.Checked;
            settings.hora_ejec = dateHoraEjec.Value;
            if (txtLectura.Text.Substring(txtLectura.Text.Length - 1) != "\\")
            { settings.carpeta_lectura = txtLectura.Text + "\\"; }
            else
            { settings.carpeta_lectura = txtLectura.Text; }
            if (txtLOG.Text.Substring(txtLOG.Text.Length - 1) != "\\")
            { settings.carpeta_log = txtLOG.Text + "\\"; }
            else
            { settings.carpeta_log = txtLOG.Text; }
            if (txtCorrectos.Text.Substring(txtCorrectos.Text.Length - 1) != "\\")
            { settings.carpeta_correctos = txtCorrectos.Text + "\\"; }
            else
            { settings.carpeta_correctos = txtCorrectos.Text; }
            if (!Revisar_email(settings.username))
            {
                MessageBox.Show("Usuario no valido", "¡Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnGuardar.Enabled = true;
                return;
            }
            foreach(DataGridViewRow row in dgvErrores.Rows)
            {
                errores.Add(row.Cells[0].Value.ToString());
            }
            settings.errores = errores;
            AppSettings.SettConfig(settings);
            btnGuardar.Enabled = true;
            MessageBox.Show("Configuración guardada", "¡Correcto!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void CargarConf()
        {
            settings = AppSettings.GetConfig(settings);
            txtClientId.Text = settings.clientId;
            txtClientSecret.Text = settings.clientSecret;
            txtUsername.Text = settings.username;
            txtPassword.Text = settings.password;
            txtToken.Text = settings.token;
            txtURLLogin.Text = settings.url_login;
            txtURLClientes.Text = settings.url_clientes;
            txtURLFacturas.Text = settings.url_facturas;
            txtURLViajes.Text = settings.url_viajes;
            txtTiempo.Text = settings.tiempo.ToString();
            txtInterval.Text = settings.job_intervalo.ToString();
            chcEditHora.Checked = settings.editTiempo;
            dateHoraEjec.Value = settings.hora_ejec;
            txtLectura.Text = settings.carpeta_lectura;
            txtLOG.Text = settings.carpeta_log;
            txtCorrectos.Text = settings.carpeta_correctos;
            foreach(string error in settings.errores)
            {
                dgvErrores.Rows.Add(error);
            }
        }
        private void SoloNum(ref KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar) || Char.IsControl(e.KeyChar) || e.KeyChar == (char)46)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }
        private Boolean Revisar_email(String email)
        {
            String expresion;
            expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(email, expresion))
            {
                if (Regex.Replace(email, expresion, String.Empty).Length == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        private void Habilitar()
        {
            if (chcEditHora.Checked == true)
            {
                settings = AppSettings.GetConfig(settings);
                label17.Enabled = true;
                dateHoraEjec.Enabled = true;
                dateHoraEjec.Value = settings.hora_ejec;
            }
            else
            {
                label17.Enabled = false;
                dateHoraEjec.Enabled = false;
            }
        }
        private string ExaminarRuta(string dir)
        {
            string res = dir;
            folderBrowserDialog1.SelectedPath = dir;
            DialogResult result = folderBrowserDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                res = folderBrowserDialog1.SelectedPath;
            }

            return res;
        }
        private void AgregarError()
        {
            dgvErrores.Rows.Add(txtNError.Text);
            txtNError.Clear();
            txtNError.Focus();
        }
        private bool ValidarTXT(Form formulario)
        {
            bool vacio = false;
            foreach (Control Controls in formulario.Controls)
            {
                if (Controls is TextBox & Controls.Text == String.Empty)
                {
                    vacio = true;
                    return vacio;
                }
                if (Controls is TabControl)
                {
                    foreach (Control control in Controls.Controls)
                    {
                        if (control is TextBox & control.Text == String.Empty)
                        {
                            vacio = true;
                            return vacio;
                        }
                        if (control is TabPage)
                        {
                            foreach (Control control1 in control.Controls)
                            {
                                if (control1 is TextBox & control1.Text == String.Empty)
                                {
                                    vacio = true;
                                    return vacio;
                                }
                                if (control1 is GroupBox)
                                {
                                    foreach (Control control2 in control1.Controls)
                                    {
                                        if (control2 is TextBox & control2.Text == String.Empty && control2.Name != "txtNError")
                                        {
                                            vacio = true;
                                            return vacio;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return vacio;
        }
        private void Guardar_Click(object sender, EventArgs e)
        {
            Guardar();
        }
        private void Cancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void EditHora_Check(object sender, EventArgs e)
        {
            Habilitar();
        }
        private void Num_KeyPress(object sender, KeyPressEventArgs e)
        {
            SoloNum(ref e);
        }
        private void Lectura_Click(object sender, EventArgs e)
        {
            this.txtLectura.Text = ExaminarRuta(txtLectura.Text);
        }
        private void LOG_Click(object sender, EventArgs e)
        {
            this.txtLOG.Text = ExaminarRuta(txtLOG.Text);
        }
        private void Correctos_Click(object sender, EventArgs e)
        {
            this.txtCorrectos.Text = ExaminarRuta(txtCorrectos.Text);
        }
        private void BtnNError_Click(object sender, EventArgs e)
        {
            AgregarError();
        }
    }
}
