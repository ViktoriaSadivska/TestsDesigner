﻿using DataLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Windows;

namespace TestClient
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        bool Open;
        TcpClient Client { get; set; }
        public LoginWindow()
        {
            InitializeComponent();
            InitializeConnection();
        }

        //initializer
        private void InitializeConnection()
        {
            Client = new TcpClient();
            Client.Connect(IPAddress.Parse("127.0.0.1"), 12400);
            Client.GetStream().ReadTimeout = -1;
            Client.GetStream().WriteTimeout = -1;
        }

        //server interaction
        private void Listen()
        {
            NetworkStream stream = Client.GetStream();
            byte[] buffer = new byte[2024];
            DataPart dataPart;

            stream.Read(buffer, 0, buffer.Length);
            using (var ms = new MemoryStream(buffer))
            {
                ms.Position = 0;
                dataPart = (DataPart)new BinaryFormatter().Deserialize(ms);
            }
            byte[] data = dataPart.Buffer;
            ChooseAction(data);
        }
        private void SendMsg(string msg)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(msg);
            byte[][] bufferArray = DataPart.BufferSplit(bytes, 1024);
            string id = DataPart.GenerateId();
            DataPart dataPart;
            byte[] dataPartArr;

            for (int i = 0; i < bufferArray.Length; ++i)
            {
                dataPart = new DataPart()
                {
                    Id = id,
                    PartCount = bufferArray.Length,
                    PartNum = i,
                    Buffer = bufferArray[i]
                };
                using (MemoryStream ms = new MemoryStream())
                {
                    new BinaryFormatter().Serialize(ms, dataPart);
                    dataPartArr = ms.ToArray();
                }
                NetworkStream stream = Client.GetStream();
                stream.Write(dataPartArr, 0, dataPartArr.Length);
            }
            Listen();
        }
        private void ChooseAction(byte[] data)
        {
            string serverMessage = Encoding.UTF8.GetString(data);
            if (serverMessage == "true") 
            {
                Open = true;
                Dispatcher.Invoke(() => { Close(); }); 
            }
            else if (serverMessage == "false")
            {
                ErrorLabel.Dispatcher.Invoke(() => { ErrorLabel.Content = "wrong login or password"; });
            }
        }

        //button clicks
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SendMsg($"login|{LoginTextBox.Text}|password|{PasswordTextBox.Password}");
            }
            catch (Exception Ex) 
            {
                Client.GetStream().Close();
                Client.Close();
            }
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Open = false;
            Client.GetStream().Close();
            Client.Close();
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Open)
            {
                MainWindow window = new MainWindow(Client);
                window.Show();
            }
        }
    }
}
