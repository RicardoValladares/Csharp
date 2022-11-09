using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Text.RegularExpressions;
using AForge.Video;
using System.Diagnostics;
using AForge.Video.DirectShow;
using System.Collections;
using System.IO;
using System.Drawing.Imaging;
using System.IO.Ports;
using System.Globalization;
using System.Net;

namespace Csharp_WebCam{
    public partial class Form1 : Form{
        /*objetos a usar*/
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoDevice;
        private VideoCapabilities[] snapshotCapabilities;
        private ArrayList listCamera = new ArrayList();
        public string pathFolder = Application.StartupPath + @"\ImageCapture\";
        private static bool needSnapshot = false;
        private static string _usbcamera;
        public string usbcamera { get { return _usbcamera; } set { _usbcamera = value; } }
        /*inicializamos formulario*/
        public Form1(){
            InitializeComponent();
            getListCameraUSB();
        }
        /*boton para abrir la camaara*/
        private void button1_Click(object sender, EventArgs e){
            OpenCamera();
        }
        /*boton para tomar foto*/
        private void button2_Click(object sender, EventArgs e){
            needSnapshot = true;
        }
        /*boton para cerrar la camaara*/
        private void button3_Click(object sender, EventArgs e){
            CloseCurrentVideoSource();
        }
        /*cerrar la camara al cerrar el formulario*/
        private void Form1_FormClosing(object sender, FormClosingEventArgs e){
            CloseCurrentVideoSource();
        }

        #region Buscar y Abrir Camara
        private void OpenCamera(){
            try{
                usbcamera = comboBox1.SelectedIndex.ToString();
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (videoDevices.Count != 0){
                    // enlistamos las camaras
                    foreach (FilterInfo device in videoDevices){
                        listCamera.Add(device.Name);
                    }
                }
                else{
                    MessageBox.Show("Cameras fuera de funcionamiento");
                }
                videoDevice = new VideoCaptureDevice(videoDevices[Convert.ToInt32(usbcamera)].MonikerString);
                snapshotCapabilities = videoDevice.SnapshotCapabilities;
                OpenVideoSource(videoDevice);
            }
            catch (Exception err){
                MessageBox.Show(err.ToString());
            }
        }
        #endregion


        //Delegamos metodo de captura de foto 
        public delegate void CaptureSnapshotManifast(Bitmap image);
        public void UpdateCaptureSnapshotManifast(Bitmap image){
            try{
                needSnapshot = false;
                pictureBox2.Image = image;
                pictureBox2.Update();
                string namaImage = "sampleImage";
                string nameCapture = namaImage + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".bmp";
                if (Directory.Exists(pathFolder)){
                    pictureBox2.Image.Save(pathFolder + nameCapture, ImageFormat.Bmp);
                }
                else{
                    Directory.CreateDirectory(pathFolder);
                    pictureBox2.Image.Save(pathFolder + nameCapture, ImageFormat.Bmp);
                }
            }catch { }
        }

        //Abrimos el visor de video
        public void OpenVideoSource(IVideoSource source){
            try{
                this.Cursor = Cursors.WaitCursor;
                CloseCurrentVideoSource();
                videoSourcePlayer1.VideoSource = source;
                videoSourcePlayer1.Start();
                this.Cursor = Cursors.Default;
            }catch { }
        }

        //Enlistamos las WebCams
        private void getListCameraUSB(){
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videoDevices.Count != 0){
                foreach (FilterInfo device in videoDevices){ comboBox1.Items.Add(device.Name);}
            }
            else{
                comboBox1.Items.Add("No Camaras");
            }
            comboBox1.SelectedIndex = 0;
        }

        //Cerramos el visor de video y la camara
        public void CloseCurrentVideoSource(){
            try{
                if (videoSourcePlayer1.VideoSource != null){
                    videoSourcePlayer1.SignalToStop();
                    for (int i = 0; i < 30; i++){
                        if (!videoSourcePlayer1.IsRunning)
                            break;
                        System.Threading.Thread.Sleep(100);
                    }
                    if (videoSourcePlayer1.IsRunning){
                        videoSourcePlayer1.Stop();
                    }
                    videoSourcePlayer1.VideoSource = null;
                }
            }catch { }
        }

        //mostramos fotograma a fotograma
        private void videoSourcePlayer1_NewFrame_1(object sender, ref Bitmap image){
            try{
                DateTime now = DateTime.Now;
                Graphics g = Graphics.FromImage(image);
                SolidBrush brush = new SolidBrush(Color.Red);
                g.DrawString(now.ToString(), this.Font, brush, new PointF(5, 5));
                brush.Dispose();
                if (needSnapshot){
                    this.Invoke(new CaptureSnapshotManifast(UpdateCaptureSnapshotManifast), image);
                }
                g.Dispose();
            }catch{ }
        }

        
    }
}
