using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 홍보_상태_모니터_프로그램
{
    public delegate void EventHandler(string name);
    public partial class LoginForm : Form
    {
        public event EventHandler loginEventHandler;
        public LoginForm()
        {
            InitializeComponent();
        }

        private void OK_Click(object sender, EventArgs e)
        {
            LoginHandler loginHandler = new LoginHandler();
            if (ControlCheck())
            {
                if (loginHandler.LoginCheck(UsernameTextBox.Text, PasswordTextBox.Text))
                {
                    string userName = loginHandler.GetUserName();
                    loginEventHandler(userName);
                    DialogResult = DialogResult.OK;
                }
                else
                {
                    MessageBox.Show("로그인 정보가 정확하지 않습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    UsernameTextBox.Clear();
                    PasswordTextBox.Clear();
                }
            }
        }

        private bool ControlCheck()
        {
            if (String.IsNullOrEmpty(UsernameTextBox.Text))
            {
                MessageBox.Show("아이디와 비밀번호를 입력해 주세요", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                UsernameTextBox.Focus();
                return false;
            }
            else if (String.IsNullOrEmpty(PasswordTextBox.Text))
            {
                MessageBox.Show("비밀번호를 입력해 주세요", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                PasswordTextBox.Focus();
                return false;
            }
            return true;
        }
    }
}
