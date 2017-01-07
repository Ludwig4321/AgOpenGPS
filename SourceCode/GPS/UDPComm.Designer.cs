﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;


namespace AgOpenGPS
{
    public partial class FormGPS
    {
        // Server socket
        private Socket serverSocket;

        //endpoint of the reply from the zero MKR1000
        IPEndPoint epZero;

        // Data stream
        private byte[] buffer = new byte[1024];

        // Status delegate
        private delegate void UpdateStatusDelegate(string status);
        private UpdateStatusDelegate updateStatusDelegate = null;

        private void SendUDPMessage(string message)
        {
            try
            {
                // Get packet as byte array
                byte[] byteData = Encoding.ASCII.GetBytes(message);

                if (byteData.Length != 0)

                    // Send packet to the zero
                    serverSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, epZero, new AsyncCallback(SendData), null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Send Error: " + ex.Message, "UDP Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

 
        public void SendData(IAsyncResult asyncResult)
        {
            try
            {
                serverSocket.EndSend(asyncResult);
            }
            catch (Exception ex)
            {
                MessageBox.Show("SendData Error: " + ex.Message, "UDP Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReceiveData(IAsyncResult asyncResult)
        {
            try
            {
                // Initialise the IPEndPoint for the client
                EndPoint epSender = new IPEndPoint(IPAddress.Any, 0);
            
                // Receive all data
                int msgLen = serverSocket.EndReceiveFrom(asyncResult, ref epSender);

                byte[] localMsg = new byte[msgLen];
                Array.Copy(buffer, localMsg, msgLen);

                // Listen for more connections again...
                serverSocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epSender, new AsyncCallback(ReceiveData), epSender);

                string text = Encoding.ASCII.GetString(localMsg);

                // Update status through a delegate
                Invoke(updateStatusDelegate, new object[] { text });
            }
            catch (Exception ex)
            {
                MessageBox.Show("ReceiveData Error: " + ex.Message, "UDP Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void UpdateStatus(string status)
        {
            //rtxtStatus.AppendText(status);
            recvSentence.Append(status);
            pn.rawBuffer += status;
            textBox1.Text = status;

            //rtxtStatus.Text = (status);
        }

        
        //int book = 0;
        //private void timer1_Tick(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        // Get packet as byte array
        //        byte[] byteData = Encoding.ASCII.GetBytes(book.ToString());
        //        book++;

        //        if (byteData.Length != 0)

        //            // Send packet to the server
        //            serverSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, epZero, new AsyncCallback(SendData), null);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Send Error: " + ex.Message, "UDP Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}



    }
}