using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace impresora{
    public partial class Form1 : Form{
        public Form1(){
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e){
            Ticket ticket = new Ticket();
            ticket.HeaderImage = picturebox1.Image;
            ticket.AddHeaderLine("Super Mercado");
            ticket.AddSubHeaderLine("Venta 1");
            ticket.AddItem("5","Peras","1.99");
            ticket.AddItem("1", "Aguacate", "1.00");
            ticket.AddTotal("TOTAL","2.99");
            ticket.AddFooterLine("Gracias por su preferencia...");
            //ticket.PrintTicket("80mm Series Printer");
            ticket.PrintTicket();
        }
    }
}
