using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Tamir.SharpSsh;
using Renci.SshNet;

namespace sFtpTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            DialogResult dr = openFileDialog1.ShowDialog();
            txtFileLocation.Text = openFileDialog1.FileName;
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            string hostName = string.Empty;
            string userName = string.Empty;
            string password = string.Empty;
            string putFilePath = string.Empty;
            string toDir = string.Empty;

            hostName = txtSite.Text;
            userName = txtUserId.Text;
            password = txtPassword.Text;

            // Directory as grabbed from FileZilla ftp client
            // sftp://FTP-TXP-Calibrus@dr.ftp.ista-billing.com/ftpusers/FTP-TXP-Calibrus
            toDir = "/ftpusers/" + txtUserId.Text; //this is only used when doing the UploadFileTamirSharpSSH method
            //toDir =  txtUserId.Text;
            putFilePath = txtFileLocation.Text;

            try
            {

                //Renci.sshNet
                UploadFileRenciSshNet(hostName, userName, password, putFilePath);



                ////Tamir.SharpSSH
                //UploadFileTamirSharpSSH(hostName, userName, password, putFilePath, toDir);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }



        }
        /// <summary>
        /// This sample will upload a file on your local machine to the remote system. 
        /// Using the Tamir.SharpSSH dll found on \\Tmpdev2\Production\CalibrusFramework\2012\SharpSSH\SharpSSH-1.1.1.13.bin which is written in .net 2.0 
        /// http://www.tamirgal.com/blog/page/SharpSSH.aspx
        /// http://www.codeproject.com/Articles/11966/sharpSsh-A-Secure-Shell-SSH-library-for-NET
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="putFilePath"></param>
        /// <param name="toDir"></param>
        private static void UploadFileTamirSharpSSH(string hostName, string userName, string password, string putFilePath, string toDir)
        {

            string remoteFileName = System.IO.Path.GetFileName(putFilePath);
            toDir +="/" + remoteFileName;
            SshTransferProtocolBase sshCp;
            sshCp = new Sftp(hostName, userName, password);
            
            sshCp.Connect();
            sshCp.Put(putFilePath, toDir);
            sshCp.Close();
        }

        /// <summary>
        /// This sample will upload a file on your local machine to the remote system. 
        /// Using the Renci.SshNet dll found on \\Tmpdev2\Production\CalibrusFramework\2012\RenciSshNet which is written in .net 4.0 and is a rewrite of the Tamir.SharpSSH
        /// http://sshnet.codeplex.com/wikipage?title=Draft%20for%20Documentation%20page
        /// </summary>
        private static void UploadFileRenciSshNet(string host, string username, string password, string localFileName)
        {
            //string host = "";
            //string username = "";
            //string password = "";
            //string localFileName = "";
            string remoteFileName = System.IO.Path.GetFileName(localFileName);

            using (var sftp = new SftpClient(host, username, password))
            {
                sftp.Connect();

                using (Stream file = File.OpenRead(localFileName))
                {
                    sftp.UploadFile(file, remoteFileName);
                }

                sftp.Disconnect();
            }
        }
    }
}
